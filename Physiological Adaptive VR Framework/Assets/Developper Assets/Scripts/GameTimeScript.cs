using System.Collections;
using System.Collections.Generic;
using System.IO;
using System;
using UnityEngine;

public class GameTimeScript : MonoBehaviour
{
    //--- Public Variables ---
    public float timeOfCompletion = 0.0f;
    public float[] taskTimes;

    //--- Private Variables ---
    float taskTimer = 0.0f;
    bool timerOn = false;
    int taskOn = 0;
    bool isPause = false;

    // Start is called before the first frame update
    void Start()
    {


    }

    // Update is called once per frame
    void Update()
    {
        //This will check to see if all the tasks are complete
        //If there are stop tracking stats and print the results on a .csv
        if (!this.GetComponent<StatsCollector>().isComplete)
        {
            if (!isPause)
            {
                timeOfCompletion += Time.deltaTime;
                if (timerOn)
                {
                    UpdateTaskTimer();
                }
            }


            if (Input.GetKeyDown(KeyCode.T))
            {
                if (!isPause)
                {
                    if (timerOn)
                    {
                        Debug.Log("Task in progress. Please wait until task is complete.");
                    }
                    else
                    {
                        StartTaskTimer();
                    }
                }
            }
            if (Input.GetKeyDown(KeyCode.Y))
            {
                if (!isPause)
                {
                    if (timerOn)
                    {
                        StopTaskTimer(taskOn);
                    }
                    else
                    {
                        Debug.Log("Task has not be start. Please start task first.");
                    }
                }
            }
            if (Input.GetKeyDown(KeyCode.P))
            {
                PauseTimers();
            }
        }
    }



    public void StartTaskTimer()
    {
        taskTimer = 0.0f;
        timerOn = true;
        taskOn++;
    }

    public void UpdateTaskTimer()
    {
        taskTimer += Time.deltaTime;
    }
    public void StopTaskTimer(int taskNum)
    {
        timerOn = false;
        taskTimes[taskNum - 1] = taskTimer;
        if(taskNum == taskTimes.Length)
        {
            this.GetComponent<StatsCollector>().isComplete = true;
        }
    }
    public void PauseTimers()
    {
        isPause = !isPause;
    }

}
