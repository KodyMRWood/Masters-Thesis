using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Event_Mananger_Script : MonoBehaviour
{
    public GameObject Geiger;
    public GameObject TestSource;
    public GameObject SearchSource;
    public float trainingTimer = 0.0f;
    public string objective = "No objective";
    public bool startTraining = false;
    //water, lead glass, concrete, wood, Alara
    public GameObject[] teachingUI = { };



    static private string []triggers = { "Testing_Trigger_1",
                                         "Testing_Trigger_2",
                                         "Testing_Trigger_3",
                                         "Testing_Trigger_4",
                                         "Testing_Trigger_5" };

    public bool gamified = true;
    private int State = 0;
    private int tempState = 0;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        switch(State)
        {
            case 0:
                {
                    objective = "Go to the 'testing area' and test the concrete.";
                    //Testing Concrete
                    //If (the geiger is triggering Testing_Trigger_1 *AND* the Source_Testing is trigger Testing_Trigger_2) 
                    //OR (the Source_Testing is triggering Testing_Trigger_1 *AND* the Geiger is trigger Testing_Trigger_2)
                    if (TestSource.GetComponent<Source_Test_Script>().triggering== triggers[0] && Geiger.GetComponent<Geiger_Trigger_Script>().triggering==triggers[1]
                        || Geiger.GetComponent<Geiger_Trigger_Script>().triggering == triggers[0] && TestSource.GetComponent<Source_Test_Script>().triggering == triggers[1])
                    {
                        if (gamified)
                        {
                            teachingUI[2].SetActive(true);
                        }
                        State++;
                    }
                        
                }
                break;

            case 1:
                {
                    objective = "Test 1 Complete! Now Test the Wood.";
                    //Testing Wood
                    //If (the geiger is triggering Testing_Trigger_2 *AND* the Source_Testing is trigger Testing_Trigger_3) 
                    //OR (the Source_Testing is triggering Testing_Trigger_2 *AND* the Geiger is trigger Testing_Trigger_3)
                    if (TestSource.GetComponent<Source_Test_Script>().triggering == triggers[1] && Geiger.GetComponent<Geiger_Trigger_Script>().triggering == triggers[2]
                        || Geiger.GetComponent<Geiger_Trigger_Script>().triggering == triggers[1] && TestSource.GetComponent<Source_Test_Script>().triggering == triggers[2])
                    {
                        if (gamified)
                        {
                            teachingUI[3].SetActive(true);
                        }
                        State++;
                    }
                }
                break;

            case 2:
                {
                    objective = "Test 2 Complete! Now Test the Water.";
                    //Testing Water
                    //If (the geiger is triggering Testing_Trigger_3 *AND* the Source_Testing is trigger Testing_Trigger_4) 
                    //OR (the Source_Testing is triggering Testing_Trigger_3 *AND* the Geiger is trigger Testing_Trigger_4)
                    if (TestSource.GetComponent<Source_Test_Script>().triggering == triggers[2] && Geiger.GetComponent<Geiger_Trigger_Script>().triggering == triggers[3]
                        || Geiger.GetComponent<Geiger_Trigger_Script>().triggering == triggers[2] && TestSource.GetComponent<Source_Test_Script>().triggering == triggers[3])
                    {
                        if (gamified)
                        {
                            teachingUI[0].SetActive(true);
                        }
                        State++;
                    }
                }
                break;

            case 3:
                {

                    objective = "Test 3 Complete! Now Test the Lead Glass.";
                    //Testing Lead Glass
                    //If (the geiger is triggering Testing_Trigger_4 *AND* the Source_Testing is trigger Testing_Trigger_5) 
                    //OR (the Source_Testing is triggering Testing_Trigger_4 *AND* the Geiger is trigger Testing_Trigger_5)
                    if (TestSource.GetComponent<Source_Test_Script>().triggering == triggers[3] && Geiger.GetComponent<Geiger_Trigger_Script>().triggering == triggers[4]
                        || Geiger.GetComponent<Geiger_Trigger_Script>().triggering == triggers[3] && TestSource.GetComponent<Source_Test_Script>().triggering == triggers[4])
                    {
                        if (gamified)
                        {
                            teachingUI[1].SetActive(true);
                        }
                        Destroy(TestSource);
                        //Boolean to start a timer
                        startTraining = true;
                        State++;
                        
                    }
                }
                break;

            case 4:
                {
                    objective = "Test 4 Complete! Find the radioactive source somewhere in the Lab.";
                    //Searching for the source
                    //If the source is picked up
                    if (SearchSource.GetComponent<Source_Searched_Found>().isFound)
                    {
                        
                        State++;
                    }
                }
                break;

            case 5:
                {
                    objective = "Good Job you found it! Now drop it off back at the container.";
                    //Dropping of the source
                    //If the source has been dorpped off

                    if (SearchSource.GetComponent<Source_Searched_Found>().isDroppedOff)
                    {
                        //Stop the timer and the dose from going up
                        startTraining = false;

                        objective = "Great Job you have finished the training!";
                        if (gamified)
                        {
                            teachingUI[4].SetActive(true);
                        }
                    }
                }
                break;
            case 6:
                {
                    objective = "Pick up the Geiger.";
                    //If the player does not have the Geiger
                    if (Geiger.GetComponent<GeigerController>().pickedUp)
                    {
                        State = tempState;   
                    }
                }
                break;
        }

        //Time to keep track of how long it takes the player to find and drop off the source
        //If the player has completed the last sheilding test then start the timer
        if(startTraining)
        {
            trainingTimer += Time.deltaTime;
        }

        //Check if Geiger is picked up
        if (!Geiger.GetComponent<GeigerController>().pickedUp && State!=6)
        {
            tempState = State;
            State = 6;
        }

    }



}
