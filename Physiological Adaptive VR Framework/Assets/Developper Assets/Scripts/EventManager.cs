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
        TASK1 = 3,
        TASK2 = 4,
        TASK3 = 5,
    }
    public Task currentTask = Task.TUTORIAL;

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
                        //Activate the sources

                        lastTask = Task.BASELINE;
                    }
                    break;
                }
            case Task.TUTORIAL:
                {
                    Debug.Log("Tutorial");
                    if (lastTask != Task.TUTORIAL)
                    {
                        //Activate the sources

                        lastTask = Task.TUTORIAL;
                    }
                    break;
                }
            case Task.TASK1:
                {
                    Debug.Log("TASK1");
                    if(lastTask != Task.TASK1)
                    {
                        //Activate the sources

                        lastTask = Task.TASK1;
                    }
                    //Find Source number one
                    break;
                }
            case Task.TASK2:
                {
                    Debug.Log("TASK2");
                    if (lastTask != Task.TASK2)
                    {
                        //Activate the sources

                        lastTask = Task.TASK2;
                    }
                    break;
                }
            case Task.TASK3:
                {
                    Debug.Log("TASK3");
                    if (lastTask != Task.TASK3)
                    {
                        //Activate the sources

                        lastTask = Task.TASK3;
                    }
                    break;
                }

        }
    }


}
