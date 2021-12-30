﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Thesis.HUD;

public class EventManager : MonoBehaviour
{
    //-------- Public variables--------
    public int task = 0;
    public GameObject geiger;

    public enum Task
    {
        //Before doing anything where you can freely roam around the world
        FREEROAM = 0,
        BASELINE = 1,
        TUTORIAL = 2,
        TASK = 3,
        END = 4,
    }
    public Task currentTask = Task.TUTORIAL;
    public bool isConnected = false;
    public bool isStarted = false;
    public bool recordBaseline = false;
    public bool recordMetrics = false;
    public bool isFirstRecording = true;

    //Properties
    private bool doneBaseline = false;
    public bool DoneBaseline { get => doneBaseline; set => doneBaseline = value; }

    //Counter for all the source that have been found
    private int sourcesScanned = 0;
    public int SourcesScanned { get => sourcesScanned; set => sourcesScanned = value; }



    //Radioactive Variables
    public bool isFirstRun = true;
    public GameObject sourceSet1;
    public GameObject sourceSet2;
    public List<GameObject> sources;
    public GameObject tutorialSource;

    //Lights Variable
    public Light[] DirectionalLight;
    private float dimRate = 1.0f;


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
        if (isFirstRun && Input.GetKeyDown(KeyCode.Equals))
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
                        //Does not have to do anything

                        lastTask = Task.FREEROAM;
                    }
                    if(isConnected)
                    {
                        currentTask++;
                    }

                    break;
                }
            case Task.BASELINE:
                {
                    Debug.Log("Base line Recording");

                    if (lastTask != Task.BASELINE)
                    {
                        //Make Baseline start
                        //Make it so that the the player is in a place where they cant do anything that might affect the 
                        recordBaseline = true;
                        lastTask = Task.BASELINE;
                    }


                    if (isStarted)
                    {
                        HUDController.hudText.SetHUDText("Please stay as still as possible and relax");
                        //recordBaseline = true;
                        if(DirectionalLight[0].intensity >= 0.0f)
                        {
                            for(int l = 0; l < DirectionalLight.Length-1; l++)
                            {
                                DirectionalLight[l].intensity -= dimRate *Time.deltaTime;
                            }
                        }
                        //When baseline is done recording change the task
                        if(DoneBaseline)
                        {
                            currentTask++;
                        }

                    }

                    break;
                }
            case Task.TUTORIAL:
                {
                    Debug.Log("Tutorial");
                    if (lastTask != Task.TUTORIAL)
                    {
                        //Start tutorial
                        recordBaseline = false;
                        lastTask = Task.TUTORIAL;
                    }
                    if (DirectionalLight[0].intensity <= 1.0f)
                    {
                        for (int l = 0; l < DirectionalLight.Length - 1; l++)
                        {
                            DirectionalLight[l].intensity += dimRate * Time.deltaTime;
                        }
                    }
                    //Check to see if all the required tasks are complete. If they, switch to the next task
                    //The tasks that need to be completed is have the geiger counter picked up and the testing of the source. The testing will be holding the gieger close to the source for 10 seconds or so\
                    if (geiger.GetComponent<GeigerController>().pickedUp){

                        HUDController.hudText.SetHUDText("Scan the radioactive source and hold the Geiger close to it for 5 seconds");
                    }
                    else
                    {
                        HUDController.hudText.SetHUDText("Find the Geiger Counter");
                    }
                    if (geiger.GetComponent<GeigerController>().pickedUp && tutorialSource.GetComponent<SourceTrigger>().DoneDetecting)
                    {
                        currentTask++;
                    }
                    break;
                }
            case Task.TASK:
                {
                    Debug.Log("TASK1");
                    //All initial variables that need to be set when first switching to the stage
                    //That way it doesnt do any unnecessary computations
                    if(lastTask != Task.TASK)
                    { 
                        //HUDController.hudText.SetHUDText("There is a radioactive source in the lab. Find and scan it. *Careful* there are fakes.");
                        //Activate all sources
                        if(isFirstRun)
                        {
                            //Takes all the sources in the set and makes a list to use them easily
                            for (int x = 0; x < sourceSet1.transform.childCount; x++)
                            {
                                sources.Add(sourceSet1.transform.GetChild(x).gameObject);
                                sources[x].SetActive(true); 
                            }

                            //Random Source as "Real" Source (NOT USING THIS FOR CONSISTENT DATA)
                            //int index = Random.Range(0,sources.Count);
                            //sources[index].SetActive(true);


                            //Make the 3rd source the "Real" source
                            //sources[2].GetComponent<SourceTrigger>().IsRealSource = true;
                        }
                        else if (!isFirstRun)
                        {
                            //Takes all the sources in the set and makes a list to use them easily
                            for (int x = 0; x < sourceSet2.transform.childCount; x++)
                            {
                                sources.Add(sourceSet2.transform.GetChild(x).gameObject);
                                sources[x].SetActive(true);
                            }

                            //Random Source as "Real" Source (NOT USING THIS FOR CONSISTENT DATA)
                            //int index = Random.Range(0, sources.Count);
                            //sources[index].SetActive(true);

                            //Make the 3rd source the "Real" source
                            //sources[2].GetComponent<SourceTrigger>().IsRealSource = true;
                        }
                        lastTask = Task.TASK;
                    }


                    if(this.GetComponent<StateMachine>().difficulty == StateMachine.Difficulty.EASY)
                    {

                    }
                    else if(this.GetComponent<StateMachine>().difficulty == StateMachine.Difficulty.MEDIUM)
                    {

                    }
                    else if (this.GetComponent<StateMachine>().difficulty == StateMachine.Difficulty.HARD)
                    {

                    }

                    //Only if looking for a specific source
                    //if (sources[2].GetComponent<SourceTrigger>().DoneDetecting)
                    //{
                    //    //They have found the right source
                    //    currentTask++;
                    //}
                    //

                    if (SourcesScanned == 5)
                    {
                        //They have all source
                        currentTask++;
                    }
                    
                    if(recordMetrics)
                    {
                        StartCoroutine(TimerToNextRecording());
                    }

                    HUDController.hudText.SetHUDText("Please find all the sources.");
                    HUDController.hudText.SetCounterText("Sources Found: " + SourcesScanned + "/5");

                    break;
                }


            case Task.END:
                {
                    //Put up some sort of UI to tell them that they have completed the simulation and they can remove them headset to complete the survey
                    HUDController.hudText.SetHUDText("Congratulations, you have found all the source. Please pick which on your think is the real one.");
                    break;

                }

        }

        IEnumerator TimerToNextRecording()
        {
            //How often we sould sample the physiological metrics
            yield return new WaitForSeconds(60.0f);
            recordMetrics = true;
        }
    }


}
