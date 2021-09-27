using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace VRUsability
{
    public class MetricData : MonoBehaviour
    {
        // The transforms which should always be tracked
        public bool autoFindDevices = false;
        public Transform leftController, rightController;
        public Transform headset;
        public GameObject dose;
        public GameObject timer;

        public static MetricData instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = FindObjectOfType<MetricData>();
                }

                return _instance;
            }
        }
        private static MetricData _instance = null;

        /// <summary>
        /// The frequency at which the metrics are gathered (in seconds).
        /// </summary>
        [Range(1, 20)]
        public int logsPerSecond = 10;

        /// <summary>
        /// Where the output will be 
        /// </summary>
        public const string csvOutputFileLocation = "Output";

        /// <summary>
        /// The transforms we need to track for logging data
        /// </summary>
        public List<Transform> transformsToTrack;
        public List<TrackedObject> trackedObjects = new List<TrackedObject>();

        /// <summary>
        /// The current room the user is in
        /// </summary>
        public Room currentRoom = Room.TutorialRoom;

        /// <summary>
        /// The class that measures the arm length of the user
        /// </summary>
        public ArmLengthMeasurer measurer = new ArmLengthMeasurer();

        /// <summary>
        /// A list of interactions to be printed
        /// </summary>
        public Dictionary<int, string> grabInteractions = new Dictionary<int, string>();

        /// <summary>
        /// Instructions for the tutorial
        /// </summary>
        public TMPro.TextMeshProUGUI instructionsAsText;
        public GameObject[] imageObjectsInOrder;

        // Global counter for log
        int numberOfLogs = 0;

        /// <summary>
        /// The rooms we want to record data in, so we can separate it
        /// </summary>
        public enum Room
        {
            // If this is the current room, tracking is paused
            NO_TRACKING = -1,

            TutorialRoom = 0,
            ControlRoom,
            ForceRoom,
            ExtendRoom,
            Count,
        }

        /// <summary>
        /// Parent for the cubes that the user interacts with
        /// </summary>
        public GameObject dynamicObjects;

        public enum Mode
        {
            TUTORIAL,
            CONTROL,
            FORCE,
            EXTEND
        }

        public Mode mode
        {
            get
            {
                return _mode;
            }
            set
            {
                _mode = value;
            }
        }
        private Mode _mode;

        /// <summary>
        /// Tracked object class organizes the information we want to print. Could be used in the future
        /// for restoring a play session and visualizing
        /// </summary>
        public class TrackedObject
        {
            public Transform transform;

            public string name;
            public int hashcode;

            public List<Vector3> positionAtFrames = new List<Vector3>();
            public List<Vector3> orientationAtFrames = new List<Vector3>();
        }

        public void Start()
        {
            StartTracking(); // Begin logging data
            Debug.Log("Dose: " + dose.gameObject.GetComponent<Controller>().totalPlayerDosage);
            Debug.Log("Time: " + timer.gameObject.GetComponent<Event_Mananger_Script>().trainingTimer);
            TutorialFunction(0); // Reset the tutorial
        }

        /// <summary>
        /// Track objects with the Trackable tag
        /// </summary>
        public void AddTaggedObjects()
        {
            // Find game objects with tags
            var tagged = GameObject.FindGameObjectsWithTag("Trackable");

            if (transformsToTrack == null) transformsToTrack = new List<Transform>(); 

            // Remove duplicates from the list of transforms
            foreach (var t in transformsToTrack)
                if (t == null) transformsToTrack.Remove(t);

            // Add the gameobjects ot the list of transforms
            foreach (var go in tagged)
            {
                if (!transformsToTrack.Contains(go.transform))
                {
                    transformsToTrack.Add(go.transform);
                }
            }
        }

        public void StartTracking()
        {
            List<Transform> extraTrackables = new List<Transform>();

            // Add headset and controllers to track
            if (!extraTrackables.Contains(headset))         extraTrackables.Add(headset);
            if (!extraTrackables.Contains(leftController))  extraTrackables.Add(leftController);
            if (!extraTrackables.Contains(rightController)) extraTrackables.Add(rightController);

            extraTrackables.AddRange(transformsToTrack);

            // Create the list if it's null
            if (trackedObjects == null) trackedObjects = new List<TrackedObject>();

            // create a unique identifier
            foreach (var objectTransform in extraTrackables)
            {
                string gameObjectName = objectTransform.gameObject.name;
                int uniqueID = objectTransform.gameObject.GetHashCode();

                // Add it to the list of objects to track
                trackedObjects.Add(new TrackedObject() { name = gameObjectName, hashcode = uniqueID, transform = objectTransform });
            }

            // Start the function which repeats logsPerSecond each frame
            InvokeRepeating("TrackObjects", 0.0f, 1.0f / (float)logsPerSecond);
        }

        /// <summary>
        /// The repeating function which happens frequently (logsPerSecond), and tracks the positions of the objects
        /// </summary>
        public void TrackObjects()
        {
            // Create the list if it's null
           if (trackedObjects == null) trackedObjects = new List<TrackedObject>();

            // Increment the count of logs we have
            numberOfLogs++;

            // Using the trackedObjects class, we add to the positions and orientations in this function
            foreach (var trackedObject in trackedObjects)
            {
                Vector3 position = Vector3.zero;
                Vector3 orientation = Vector3.zero;

                if (trackedObject.transform.gameObject.activeInHierarchy)
                {
                    position = trackedObject.transform.position;
                    orientation = trackedObject.transform.rotation.eulerAngles;
                }

                trackedObject.positionAtFrames.Add(position);
                trackedObject.orientationAtFrames.Add(orientation);
            }
        }

        /// <summary>
        /// Abort the tracking completely
        /// </summary>
        public void StopTracking()
        {
            CancelInvoke();
        }

        private void OnApplicationQuit()
        {
            StopTracking();
            PrintToCSV();
        }

        /// <summary>
        /// Add a message to be CSV logged
        /// </summary>
        /// <param name="interactionMessage">ie: Grabbed Cube w Right Hand</param>
        public void AddInteraction(string interactionMessage)
        {
            if (grabInteractions.ContainsKey(numberOfLogs) || grabInteractions.ContainsValue(interactionMessage)) return;
            grabInteractions.Add(numberOfLogs, interactionMessage);
            Debug.Log(interactionMessage);
        }

        /// <summary>
        /// Print the results to a CSV
        /// </summary>
        public void PrintToCSV()
        {
            string dir = DateTime.Now.ToString("yyyy-dd-M--HH-mm-ss");

            // Create a timestamp directory for the participants.
            System.IO.Directory.CreateDirectory(string.Format("{0}/../{1}/{2}", Application.dataPath, csvOutputFileLocation, dir));

            // The strings to be printed
            List<string> outputLines;

            // Gather and print the arm length information, from the ArmMeasurer class,
            {
                var m = measurer;

                try
                {
                    outputLines = new List<string>(
                        new string[3]
                        {
                            ",Avg Length,StDev,Up Arm Length,Down Arm Length,Forward Arm Length,Outward Arm Length",
                            string.Format("Left Arm,{0},{1},{2},{3},{4},{5}", m.leftArmLength, m.leftArmSTDev, m.leftArmLengths[0], m.leftArmLengths[1], m.leftArmLengths[2], m.leftArmLengths[3]),
                            string.Format("Right Arm,{0},{1},{2},{3},{4},{5}", m.leftArmLength, m.leftArmSTDev, m.leftArmLengths[0], m.leftArmLengths[1], m.leftArmLengths[2], m.leftArmLengths[3]),
                        }
                    );

                    System.IO.File.WriteAllLines(string.Format("{0}/../{1}/{2}/armLengths.csv", Application.dataPath, csvOutputFileLocation, dir), outputLines.ToArray());
                }
                catch
                {
                    //Debug.LogWarning("Couldn't Write File. Possible because it's in the editor. Ignore this");
                }
            }

            // Create a CSV for the formatted data that we're interested in
            //{
            //    outputLines = new List<string>();
            //    {
            //        string outputLine = "Metrics Timestamp,Interaction,Attempt,Mode,Region,ID,Distance";
            //        outputLines.Add(outputLine);
            //    }
            //
            //    foreach (var kV in grabInteractions)
            //    {
            //        float secs = (float)kV.Key * (1.0f / logsPerSecond);
            //        TimeSpan t = TimeSpan.FromSeconds(secs);
            //
            //        string outputLine = string.Format("{0:D2}m:{1:D2}s:{2:D2}ms,",
            //            t.Minutes, t.Seconds, t.Milliseconds) + kV.Value;
            //
            //        outputLines.Add(outputLine);
            //    }
            //
            //    System.IO.File.WriteAllLines(string.Format("{0}/../{1}/{2}/grabInteractions.csv", Application.dataPath, csvOutputFileLocation, dir), outputLines.ToArray());
            //}

            // Gather and print locations and rotations of trackable objects
            {
                outputLines = new List<string>();
                {
                    string outputLine = "Metrics Timestamp,";

                    foreach (var trackedObject in trackedObjects)
                    {
                        outputLine += string.Format("{0} - Position X,{0} - Position Y,{0} - Position Z,{0} - Rotation X,{0} - Rotation Y,{0} - Rotation Z,", trackedObject.name.ToString());
                    }

                    outputLines.Add(outputLine);
                }

                for (int i = 0; i < numberOfLogs; i++)
                {
                    float secs = (float)i * (1.0f / logsPerSecond);
                    TimeSpan t = TimeSpan.FromSeconds(secs);

                    string outputLine = string.Format("{0:D2}m:{1:D2}s:{2:D2}ms,",
                        t.Minutes, t.Seconds, t.Milliseconds);

                    // For each of the tracked objects
                    foreach (var to in trackedObjects)
                    {
                        outputLine +=
                            string.Format("{0},{1},{2},", to.positionAtFrames[i].x.ToString("F3"), to.positionAtFrames[i].y.ToString("F3"), to.positionAtFrames[i].z.ToString("F3")) +
                            string.Format("{0},{1},{2},", to.orientationAtFrames[i].x.ToString("F3"), to.orientationAtFrames[i].y.ToString("F3"), to.orientationAtFrames[i].z.ToString("F3"));
                    }

                    outputLines.Add(outputLine);
                }
                
                System.IO.File.WriteAllLines(string.Format("{0}/../{1}/{2}/positions.csv", Application.dataPath, csvOutputFileLocation, dir), outputLines.ToArray());
            }

            //Creat a CSV for time and accumulateted dose
            {
                try
                {
                    outputLines = new List<string>(
                        new string[3]
                        {
                            "Dose time,Accumulated Dose, Time Taken",
                            string.Format("Amount of total dose user obtained while searching for the source (severts),{0}", dose.GetComponent<Controller>().totalPlayerDosage.ToString("F6")),
                            string.Format("The time it took the user to complete the training (Secs),{0}",timer.GetComponent<Event_Mananger_Script>().trainingTimer.ToString("F3")),
                        }
                    );
                    
                    

                    System.IO.File.WriteAllLines(string.Format("{0}/../{1}/{2}/DoseAndTimer.csv", Application.dataPath, csvOutputFileLocation, dir), outputLines.ToArray());
                }
                catch
                {
                    Debug.LogWarning("Couldn't Write File. Possible because it's in the editor. Ignore this");
                }
            }
        }

        /// <summary>
        /// Set the image and text on the tutorial poster.
        /// </summary>
        /// <param name="poseState">From 0 to 3, what pose will the user be measured in? What image appears?</param>
        public void TutorialFunction(int poseState)
        {
            /// Uncomment this if you want an instruction poster.
            
            // foreach (var go in imageObjectsInOrder) go.SetActive(false);
            // switch(poseState)
            // {
            //     case 0:
            //         instructionsAsText.text = "Raise your arms up.";
            //         imageObjectsInOrder[0].SetActive(true);
            //         break;
            //     case 1:
            //         instructionsAsText.text = "Lower your arms.";
            //         imageObjectsInOrder[1].SetActive(true);
            //         break;
            //     case 2:
            //         instructionsAsText.text = "Reach your arms forward.";
            //         imageObjectsInOrder[2].SetActive(true);
            //         break;
            //     case 3:
            //         instructionsAsText.text = "Stretch your arms outward.";
            //         imageObjectsInOrder[3].SetActive(true);
            //         break;
            //     case 4:
            //         instructionsAsText.text = "Thank you.";
            //         break;
            // }
        }

        public enum Hand
        {
            LEFT,
            RIGHT,
        }

        private GameObject leftShoulder;
        private GameObject rightShoulder;

        /// <summary>
        /// Figure out where the shoulder is and parent it.
        /// </summary>
        public void CreateShoulderApproximations()
        {
            TutorialFunction(4);

            leftShoulder = new GameObject("Left Shoulder");
            rightShoulder = new GameObject("Right Shoulder");

            leftShoulder.transform.SetParent(headset);
            rightShoulder.transform.SetParent(headset);

            leftShoulder.transform.position = measurer.leftShoulder;
            rightShoulder.transform.position = measurer.rightShoulder;
        }

        /// <summary>
        /// How far is the hand from the shoulder?
        /// </summary>
        /// <param name="hand">Which hand to detect</param>
        /// <returns>Return the distance</returns>
        public float GetArmReach(Hand hand)
        {
            if (leftShoulder == null || rightShoulder == null) return 0.0f;

            switch (hand)
            {
                case Hand.LEFT:
                    return Vector3.Distance(leftShoulder.transform.position, leftController.transform.position);
                case Hand.RIGHT:
                    return Vector3.Distance(rightShoulder.transform.position, rightController.transform.position);
                default:
                    break;
            }

            return 0.0f;
        }

        public float GetArmLength(Hand hand)
        {
            switch (hand)
            {
                case Hand.LEFT:
                    return measurer.leftArmLength;
                case Hand.RIGHT:
                    return measurer.rightArmLength;
                default:
                    break;
            }

            return 0.0f;
        }

        public class ArmLengthMeasurer
        {
            /// <summary>
            /// The following are used to determine which parts of the user's arm have been measured
            /// </summary>
            public bool armsRaisedMeasured = false;
            public bool armsLoweredMeasured = false;
            public bool armsForwardMeasured = false;
            public bool armsOutwardMeasured = false;

            public float leftArmLength = -1.0f; // Init to -1
            public float rightArmLength = -1.0f; // Init to -1

            public float leftArmSTDev = 0.0f; // Init to -1
            public float rightArmSTDev = 0.0f; // Init to -1

            /// <summary>
            /// The following are used when calculating the user's arm length
            /// </summary>
            public Vector3[] armsRaisedPosition;
            public Vector3[] armsLoweredPosition;
            public Vector3[] armsForwardPosition;
            public Vector3[] armsOutwardPosition;

            public List<float> leftArmLengths;
            public List<float> rightArmLengths;

            public Vector3 leftShoulder;
            public Vector3 rightShoulder;

            public void CalculateArmLength()
            {
                leftShoulder.y = (armsForwardPosition[0].y + armsOutwardPosition[0].y) * 0.5f; // Average should work here
                rightShoulder.y = (armsForwardPosition[1].y + armsOutwardPosition[1].y) * 0.5f; // Average should work here

                // Use the up and down arm positions to approximate the X coordinate of the shoulder
                leftShoulder.x = (armsLoweredPosition[0].x);
                rightShoulder.x = (armsLoweredPosition[1].x);

                // Use the up and down arm positions to approximate the Z coordinate of the shoulder
                leftShoulder.z = (armsLoweredPosition[0].z);
                rightShoulder.z = (armsLoweredPosition[1].z);

                leftArmLengths = new List<float>(new float[]{
                    Vector3.Distance(leftShoulder, armsRaisedPosition[0]),
                    Vector3.Distance(leftShoulder, armsLoweredPosition[0]),
                    Vector3.Distance(leftShoulder, armsForwardPosition[0]),
                    Vector3.Distance(leftShoulder, armsOutwardPosition[0])
                });

                rightArmLengths = new List<float>(new float[]{
                    Vector3.Distance(rightShoulder, armsRaisedPosition[1]),
                    Vector3.Distance(rightShoulder, armsLoweredPosition[1]),
                    Vector3.Distance(rightShoulder, armsForwardPosition[1]),
                    Vector3.Distance(rightShoulder, armsOutwardPosition[1])
                });

                leftArmSTDev = (float)leftArmLengths.StdDev();
                rightArmSTDev = (float)rightArmLengths.StdDev();

                leftArmLength = rightArmLength = 0.0f;

                foreach (float length in leftArmLengths)    leftArmLength += length * 0.25f;
                foreach (float length in rightArmLengths)   rightArmLength += length * 0.25f;

                FindObjectOfType<MetricData>().CreateShoulderApproximations();
            }

            /// <summary>
            /// Grab the user's arms position when they are raised.
            /// </summary>
            /// <param name="md">The MetricData class</param>
            public void MeasureRaised(MetricData md)
            {
                armsRaisedPosition = new Vector3[2]
                {
                    md.leftController.position,
                    md.rightController.position,
                };

                armsRaisedMeasured = true;
            }

            /// <summary>
            /// Grab the user's arms position when they are lowered.
            /// </summary>
            /// <param name="md">The MetricData class</param>
            public void MeasureLowered(MetricData md)
            {
                armsLoweredPosition = new Vector3[2]
                {
                    md.leftController.position,
                    md.rightController.position,
                };

                armsLoweredMeasured = true;
            }

            /// <summary>
            /// Grab the user's arms position when they are forward.
            /// </summary>
            /// <param name="md">The MetricData class</param>
            public void MeasureForward(MetricData md)
            {
                armsForwardPosition = new Vector3[2]
                {
                    md.leftController.position,
                    md.rightController.position,
                };

                armsForwardMeasured = true;
            }

            /// <summary>
            /// Grab the user's arms position when they are outward.
            /// </summary>
            /// <param name="md">The MetricData class</param>
            public void MeasureOutward(MetricData md)
            {
                armsOutwardPosition = new Vector3[2]
                {
                    md.leftController.position,
                    md.rightController.position,
                };

                armsOutwardMeasured = true;
            }
        }
    }

    /// <summary>
    /// Calculate standard deviation for a list using LINQ
    /// </summary>
    public static class Extensions
    {
        public static double StdDev(this IEnumerable<float> values)
        {
            double ret = 0;
            int count = values.Count();
            if (count > 1)
            {
                //Compute the Average
                double avg = values.Average();

                //Perform the Sum of (value-avg)^2
                double sum = values.Sum(d => (d - avg) * (d - avg));

                //Put it all together
                ret = Math.Sqrt(sum / count);
            }
            return ret;
        }
    }
}