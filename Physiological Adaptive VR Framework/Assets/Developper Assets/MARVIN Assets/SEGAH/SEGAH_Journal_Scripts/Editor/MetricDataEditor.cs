using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace VRUsability
{
    [CustomEditor(typeof(MetricData))]
    public class MetricDataEditor : Editor
    {
        MetricData metricData;

        GUIContent headsetContent = new GUIContent("Headset", "The VR Headset to track.");
        GUIContent leftControllerContent = new GUIContent("Left Controller", "The VR Controller for the left hand, to track.");
        GUIContent rightControllerContent = new GUIContent("Right Controller", "The VR Controller for the right hand, to track.");
        GUIContent dynamicObjectsContent = new GUIContent("Dynamic Objects", "Parent for the dynamic objects to be reset.");
        GUIContent eventManagerContent = new GUIContent("Event Manager", "The event manager in the scene.");
        GUIContent gameManagerContent = new GUIContent("Game Manager", "The game manager in the scene.");

        SerializedProperty poseImageArray;
        SerializedProperty dynamicObjects;
        SerializedProperty transformsToTrackList;

        bool isMetricsCollapsed = true;

        private void OnEnable()
        {
            metricData = target as MetricData;
            
            poseImageArray = serializedObject.FindProperty("imageObjectsInOrder");
            transformsToTrackList = serializedObject.FindProperty("transformsToTrack");
            dynamicObjects = serializedObject.FindProperty("dynamicObjects");
        }

        public override void OnInspectorGUI()
        {
            using (new EditorGUILayout.VerticalScope("Box"))
            {
                EditorGUILayout.LabelField("Metric GameObjects", EditorStyles.boldLabel);
                EditorGUI.indentLevel++;

                metricData.autoFindDevices = GUILayout.Toggle(metricData.autoFindDevices, "Auto Find Devices");

                EditorGUI.BeginDisabledGroup(metricData.autoFindDevices);
                {
                    // Inspector for the headset and controllers.
                    metricData.headset = EditorGUILayout.ObjectField(headsetContent, metricData.headset, typeof(Transform), true) as Transform;
                    metricData.leftController = EditorGUILayout.ObjectField(leftControllerContent, metricData.leftController, typeof(Transform), true) as Transform;
                    metricData.rightController = EditorGUILayout.ObjectField(rightControllerContent, metricData.rightController, typeof(Transform), true) as Transform;
                    metricData.timer = EditorGUILayout.ObjectField(eventManagerContent, metricData.timer, typeof(GameObject), true) as GameObject; 
                    metricData.dose = EditorGUILayout.ObjectField(gameManagerContent, metricData.dose, typeof(GameObject), true) as GameObject ;
                }
                EditorGUI.EndDisabledGroup();

                EditorGUI.indentLevel--;
            }

            using (new EditorGUILayout.VerticalScope("Box"))
            {
                EditorGUILayout.LabelField("Tutorial Manager", EditorStyles.boldLabel);
                EditorGUI.indentLevel++;

                EditorGUILayout.HelpBox("Ask the participant to keep their arms stretched as straight as possible, " +
                    "and to avoid moving their torso. If they move, their limbs can be re-measured", MessageType.Warning);

                EditorGUILayout.LabelField(string.Format("{0}Measure Arms Raised", metricData.measurer.armsRaisedMeasured ? "☑" : "☐"));
                EditorGUILayout.LabelField(string.Format("{0}Measure Arms Lowered", metricData.measurer.armsLoweredMeasured ? "☑" : "☐"));
                EditorGUILayout.LabelField(string.Format("{0}Measure Arms Forward", metricData.measurer.armsForwardMeasured ? "☑" : "☐"));
                EditorGUILayout.LabelField(string.Format("{0}Measure Arms Outward", metricData.measurer.armsOutwardMeasured ? "☑" : "☐"));

                if (metricData.transformsToTrack == null) metricData.transformsToTrack = new List<Transform>();
                if (metricData.transformsToTrack.Count == 0)
                {
                    EditorGUILayout.HelpBox("No transforms are being tracked. Make sure they are enabled in the scene," + 
                        "and then click on the \"Add Tagged Objects\" Button", MessageType.Error);
                }

                if (GUILayout.Button("Measure Arms Raised")) { metricData.measurer.MeasureRaised(metricData); metricData.TutorialFunction(0); }
                EditorGUI.BeginDisabledGroup(!metricData.measurer.armsRaisedMeasured);
                {
                    if (GUILayout.Button("Measure Arms Lowered")) { metricData.measurer.MeasureLowered(metricData); metricData.TutorialFunction(1); }
                    EditorGUI.BeginDisabledGroup(!metricData.measurer.armsLoweredMeasured);
                    {
                        if (GUILayout.Button("Measure Arms Forward")) { metricData.measurer.MeasureForward(metricData); metricData.TutorialFunction(2); }
                        EditorGUI.BeginDisabledGroup(!metricData.measurer.armsForwardMeasured);
                        {
                            if (GUILayout.Button("Measure Arms Outward")) { metricData.measurer.MeasureOutward(metricData); metricData.TutorialFunction(3); }
                        }
                        EditorGUI.EndDisabledGroup();
                    }
                    EditorGUI.EndDisabledGroup();
                }
                EditorGUI.EndDisabledGroup();
                
                EditorGUI.BeginDisabledGroup(!(
                    metricData.measurer.armsRaisedMeasured && metricData.measurer.armsLoweredMeasured &&
                    metricData.measurer.armsForwardMeasured && metricData.measurer.armsOutwardMeasured));
                {
                    if (GUILayout.Button("Calculate Arm Lengths"))
                    {
                        metricData.measurer.CalculateArmLength();
                    }
                }
                EditorGUI.EndDisabledGroup();

                if (metricData.measurer.leftArmLength > 0.0f || metricData.measurer.rightArmLength > 0.0f)
                {
                    EditorGUILayout.LabelField(
                        string.Format("Left Arm Length : \t{0}m \t", metricData.measurer.leftArmLength.ToString("F3")) +
                        string.Format("STDev : {0}", metricData.measurer.leftArmSTDev.ToString("F3")));
                    EditorGUILayout.LabelField(
                        string.Format("Right Arm Length : \t{0}m \t", metricData.measurer.rightArmLength.ToString("F3")) +
                        string.Format("STDev : {0}", metricData.measurer.rightArmSTDev.ToString("F3")));
                }

                EditorGUI.indentLevel--;

                // EditorGUILayout.LabelField("Instructions for the User on the Poster", EditorStyles.boldLabel);
                // 
                // EditorGUI.indentLevel++;
                // EditorGUI.BeginChangeCheck();
                // 
                // metricData.instructionsAsText = EditorGUILayout.ObjectField("Text Field", metricData.instructionsAsText, typeof(TMPro.TextMeshProUGUI), true) as TMPro.TextMeshProUGUI;
                // EditorGUILayout.PropertyField(poseImageArray, new GUIContent() { text = "Image Array of Poses" }, true);
                // 
                // EditorGUI.indentLevel--;
                // 
                // EditorGUILayout.LabelField("Study Controls", EditorStyles.boldLabel);
                // EditorGUI.indentLevel++;
                // 
                // EditorGUILayout.PropertyField(dynamicObjects, dynamicObjectsContent);
                // 
                // if (EditorGUI.EndChangeCheck()) // Has the array been modified? It's the only property here.
                // {
                //     serializedObject.ApplyModifiedProperties();
                // }
                // 
                // EditorGUI.indentLevel--;
            }

            using (new EditorGUILayout.VerticalScope("Box"))
            {
                EditorGUILayout.LabelField("Trackable Manager", EditorStyles.boldLabel);
                EditorGUI.indentLevel++;

                EditorGUI.BeginChangeCheck();

                EditorGUILayout.HelpBox("This button adds all the objects with the tag 'Trackable'", MessageType.Info);
                if (GUILayout.Button("Add Tagged Objects"))
                {
                    metricData.AddTaggedObjects();
                }

                EditorGUILayout.PropertyField(transformsToTrackList, new GUIContent() { text = "Transforms To Track" }, true);
                if (EditorGUI.EndChangeCheck()) serializedObject.ApplyModifiedProperties();

                EditorGUI.indentLevel--;
            }
        }
    }
}