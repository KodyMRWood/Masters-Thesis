using System.Collections;
using System.Collections.Generic;
using System;
using System.IO;
using UnityEngine;

public class OutputResults : MonoBehaviour
{

    //--- Public Variables ---



    //--- Private Variables ---
    string filePath = "";
    StreamWriter writer;

    // Start is called before the first frame update
    void Start()
    {
        filePath = GetPath();
        if (File.Exists(filePath))
        {
            Debug.Log("It exists");
            writer = new StreamWriter(filePath);
        }
        else
        {
            Debug.Log("It does not exist");
            writer = System.IO.File.CreateText(filePath);
        }
    }

    public void OutputCSV(string stat, float timeOfCompletion)
    {
        //writer.WriteLine("Participant Number,System,Time Completion");
        writer.WriteLine(stat + timeOfCompletion);
        writer.Flush();
    }

    public void OutputClose()
    {
        writer.Close();
    }



    private string GetPath()
    {
#if UNITY_EDITOR
        return Application.dataPath + "/CSV/" + "Results " + DateTime.Now.ToString("yyyy-mm-dd-hh-mm-ss") + ".csv";
#elif UNITY_ANDROID
        return Application.persistentDataPath + "Results" + DateTime.Now.ToString("yyyy-mm-dd-hh-mm-ss") + ".csv";
#elif UNITY_IPHONE
        return Application.persistentDataPath + "Results" + DateTime.Now.ToString("yyyy-mm-dd-hh-mm-ss") + ".csv";
#else
        return Application.dataPath+"/"+ "Results" + DateTime.Now.ToString("yyyy-mm-dd-hh-mm-ss") + ".csv";
#endif
    }
}
