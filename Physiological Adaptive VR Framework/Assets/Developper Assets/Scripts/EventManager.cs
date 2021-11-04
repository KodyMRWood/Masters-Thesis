using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventManager : MonoBehaviour
{
    //-------- Public variables--------
    public int task = 0;

    public enum Task
    {
        //Before doing anything where you can freely roam around the world
        FREEROAM = 0,
        BASELINE = 1,
        TUTORIAL = 2,
        TASK = 3,
    }
    public Task currentTask = Task.TUTORIAL;

    public int sourcesFound = 0;
    public bool isFirstRun = true;
    //public GameObject[] sourcesSet1 = new GameObject[10];
    //public GameObject[] sourcesSet2 = new GameObject[10];


    //-------- Private variables--------



    //Variable to know if the task just switched
    private Task lastTask = Task.FREEROAM;
    //-------- Other variables--------
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //Checks to see if this is the participants first run.
        //Changed manually by pressing the = key
        //Depending on the run, the sources will spawn in different locations
        if(isFirstRun && Input.GetKeyDown(KeyCode.Equals))
        {
            isFirstRun = false;
        }
        //states
        switch(currentTask)
        {
            case Task.FREEROAM:
                {
                    Debug.Log("Free roam");
                    if (lastTask != Task.FREEROAM)
                    {
                        //Activate the sources

                        lastTask = Task.FREEROAM;
                    }
                    break;
                }
            case Task.BASELINE:
                {
                    Debug.Log("Base line Recording");

                    if (lastTask != Task.BASELINE)
                    {
                        //Make Baseline start

                        lastTask = Task.BASELINE;
                    }
                    break;
                }
            case Task.TUTORIAL:
                {
                    Debug.Log("Tutorial");
                    if (lastTask != Task.TUTORIAL)
                    {
                        //Start tutorial

                        lastTask = Task.TUTORIAL;
                    }
                    break;
                }
            case Task.TASK:
                {
                    Debug.Log("TASK1");
                    //All initial variables that need to be set when first switching to the stage
                    //That way it doesnt do any unnecessary computations
                    //if(lastTask != Task.TASK)
                    //{ 
                    //    //Activate all sources
                    //    if(isFirstRun)
                    //    {
                    //        for(int source = 0; source < sourcesSet1.Length; source++ )
                    //        {
                    //            sourcesSet1[source].gameObject.SetActive(true);
                    //        }
                    //    }
                    //    lastTask = Task.TASK;
                    //}

                    //Find Source number one
                    break;
                }

        }
    }


}
