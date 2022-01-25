using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Thesis.HUD;
using Thesis.Sound;
using UnityEngine.XR;

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
    [HideInInspector]public bool isConnected = false;
    [HideInInspector]public bool isStarted = false;
    public bool recordBaseline = false;
    public bool recordMetrics = false;
    public bool isFirstRecording = true;
    
    private float timeTillRecord = 0.0f;
    [SerializeField]private float timeToRecord = 60.0f;

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

    private StateMachine.Difficulty m_difficulty = StateMachine.Difficulty.MEDIUM;

    private bool m_FinishedTwoRuns = false;
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

                    if (lastTask != Task.BASELINE)
                    {
                        //Make Baseline start
                        //Make it so that the the player is in a place where they cant do anything that might affect the 
                        recordBaseline = true;
                        isFirstRecording = true;
                        lastTask = Task.BASELINE;
                    }


                    if (isStarted)
                    {
                        HUDController.hudText.SetHUDText("Please stay as still as possible and relax");
                        if(DirectionalLight[0].intensity >= 0.0f)
                        {
                            for(int l = 0; l < DirectionalLight.Length-1; l++)
                            {
                                DirectionalLight[l].intensity -= dimRate *Time.deltaTime;
                            }
                        }
                        //When baseline is done recording change the task
                        if(!recordBaseline)
                        {
                            currentTask++;
                        }

                    }

                    break;
                }
            case Task.TUTORIAL:
                {
                    if (lastTask != Task.TUTORIAL)
                    {
                        //Start tutorial
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
                    //All initial variables that need to be set when first switching to the stage
                    //That way it doesnt do any unnecessary computations
                    tutorialSource.gameObject.SetActive(false);
                    if(lastTask != Task.TASK)
                    {
                        isFirstRecording = false;
                        //HUDController.hudText.SetHUDText("There is a radioactive source in the lab. Find and scan it. *Careful* there are fakes.");
                        //Activate all sources
                        if (isFirstRun)
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
                        SoundManager.instance.PlaySound();
                        lastTask = Task.TASK;
                    }

                    //Reset counter to count if all sources have been found
                    SourcesScanned=0;
                    for (int x = 0; x < sources.Count-1; x++)
                    {
                        if(sources[x].GetComponent<SourceTrigger>().DoneDetecting)
                        {
                            SourcesScanned++;
                        }
                    }


                    if (SourcesScanned == 5)
                    {
                        for (int x = 0; x < sources.Count; x++)
                        {
                            sources[x].SetActive(false);
                        }
                        sources.Clear();
                        //They have all source
                        currentTask++;
                    }
                    if(timeTillRecord >= timeToRecord && !recordMetrics)
                    {
                        recordMetrics = true;
                        timeTillRecord = 0.0f;
                    }
                    else if (!recordMetrics)
                    {
                        timeTillRecord += Time.deltaTime;
                    }


                    m_difficulty = this.GetComponent<StateMachine>().difficulty;


                    if (m_difficulty == StateMachine.Difficulty.EASY)
                    {
                        SoundManager.instance.ChangeSound(m_difficulty);
                    }
                    else if(m_difficulty == StateMachine.Difficulty.MEDIUM)
                    {
                        SoundManager.instance.ChangeSound(m_difficulty);
                    }
                    else if (m_difficulty == StateMachine.Difficulty.HARD)
                    {
                        SoundManager.instance.ChangeSound(m_difficulty);
                    }

                    //Only if looking for a specific source
                    //if (sources[2].GetComponent<SourceTrigger>().DoneDetecting)
                    //{
                    //    //They have found the right source
                    //    currentTask++;
                    //}
                    //

                    

                    HUDController.hudText.SetHUDText("Please find all the sources.");
                    HUDController.hudText.SetCounterText("Sources Found: " + SourcesScanned + "/5");

                    break;
                }


            case Task.END:
                {
                    SoundManager.instance.StopSound();
                    //Put up some sort of UI to tell them that they have completed the simulation and they can remove them headset to complete the survey
                    HUDController.hudText.SetHUDText("Congratulations, you have found all the source. Please take off the headset and answer the survey questions");
                    if (OVRInput.Get(OVRInput.Button.PrimaryIndexTrigger) || OVRInput.Get(OVRInput.Button.SecondaryIndexTrigger) || Input.GetKeyDown(KeyCode.R))
                    {
                        if(!m_FinishedTwoRuns)
                        {
                            ResetSimulation();
                            m_FinishedTwoRuns = true;
                        }
                        else
                        {
                            //HUDController.hudText.SetHUDText("Congratulations, you have found all the source. Please take off the headset and answer the survey questions");
                        }
                    }
                    break;

                }

        }

       
    }
    private void ResetSimulation()
    {
        //Reset everything
        isFirstRun = !isFirstRun;
        
        tutorialSource.gameObject.SetActive(true);

        recordMetrics = false;
        timeTillRecord = 0.0f;

        currentTask = Task.TUTORIAL;

    }



}
