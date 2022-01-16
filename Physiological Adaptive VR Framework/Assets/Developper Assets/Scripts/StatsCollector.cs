﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatsCollector : MonoBehaviour
{
    //This is script is going to collect all the stats that needs to be output and keep track of them

    //--- Public Variables ---
    public bool isComplete = false; // Checks to see if all the tasks have been complete 
    bool isWritten; //Makes sure to write it once, prevents errors and or writing more than needed


    //--- Private Variables ---
    private List<float> timeToFindEachSource = new List<float>();
    private List<int> orderSourcesFound = new List<int>();


    GameTimeScript timer;
    OutputResults printer;


    // Start is called before the first frame update
    void Start()
    {
        timer = FindObjectOfType<GameTimeScript>();
        printer = FindObjectOfType<OutputResults>();

        //Just to make sure that it is set to false when the simulation starts
        isWritten = false;
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnApplicationQuit()
    {
        isComplete = !isComplete;
        printer.OutputCSV("Time to complete: ",timer.timeOfCompletion);
        for(int x = 0; x < timeToFindEachSource.Count; x++)
        {
            //Debug.Log("Printing...");
            //printer.OutputCSV("Task "+(x+1)+" time to complete: ",timer.taskTimes[x]);
            printer.OutputCSV("Source:" + orderSourcesFound[x], timeToFindEachSource[x]);
        }
        printer.OutputClose();
        isWritten = true; 
    }

    public void RecordSourceFoundTime(float time, int index)
    {
        timeToFindEachSource.Add(time);
        orderSourcesFound.Add(index);
    }

}
