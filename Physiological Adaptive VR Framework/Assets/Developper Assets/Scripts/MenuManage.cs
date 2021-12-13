using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuManage : MonoBehaviour
{
    // Variables for first 30s data collecting
    public GameObject readyForCollecting;
    public GameObject collectingIntro;
    public Text collectingTime;
    public GameObject dataToMenu;
    [Tooltip("The amount of time the system will record for baseline mesurements. In seconds")]
    public float baseLineRecordTime = 30.0f;

    float collectTime = 0;
    public bool countCollectTime = false;


    // Main Menu
    public GameObject menu;
    public GameObject gamePlay;
    public GameObject collection;
    public GameObject developers;


    //// Controllers Hand Tracking
    //public GameObject leftHand;
    //public GameObject rightHand;
    //public GameObject leftController;
    //public GameObject rightController;
    //public GameObject pointer;

    //Activate the sensors
    float baseTime = 30;
    float totalTime = 0;
    static public bool activeSensor = false;
    static public bool stopSensor = false;
    static public bool isRecordBaseLineDone = false;

    static public int stressTimeEMG = 0;
    static public int stressTimeECG = 0;
    static public int stressTimeEDA = 0;
    static public int stressLevel = 0;


    // Start is called before the first frame update
    void Start()
    {
        //readyForCollecting.SetActive(true); 
        //collectingIntro.SetActive(false);
        //collectingTime.gameObject.SetActive(false);
        //dataToMenu.SetActive(false);
        //
        //menu.SetActive(false);
        //gamePlay.SetActive(false);
        //collection.SetActive(false);
        //developers.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        // Collecting data time update
        if (countCollectTime)
        {
            collectTime += Time.deltaTime;
            //collectingTime.text = Mathf.Round(collectTime) + "";

            if(collectTime >= baseLineRecordTime)
            {
                countCollectTime = false;
                isRecordBaseLineDone = true;
                //stopSensor = true;
            }
        }

        //// Set controller unenable when using hands tracking
        //if(leftHand.GetComponent<SkinnedMeshRenderer>().enabled == true || rightHand.GetComponent<SkinnedMeshRenderer>().enabled == true){
        //    leftController.SetActive(false);
        //    rightController.SetActive(false);
        //    pointer.SetActive(false);
        //}
        //else
        //{
        //    leftController.SetActive(true);
        //    rightController.SetActive(true);
        //    pointer.SetActive(true);
        //}

        
    }

    // Functions for different button
    public void CollectingDataStart()
    {
        readyForCollecting.SetActive(false);
        collectingIntro.SetActive(true);
        collectingTime.gameObject.SetActive(true);

        activeSensor = true;
        countCollectTime = true;
    }

    public void DataToMenu()
    {
        collectingIntro.SetActive(false);
        collectingTime.gameObject.SetActive(false);
        dataToMenu.SetActive(false);

        menu.SetActive(true);
    }

    public void MenuStart()
    {
        menu.SetActive(false);
        gamePlay.SetActive(true);
    }

    public void MenuCollection()
    {
        menu.SetActive(false);
        collection.SetActive(true);
    }

    public void MenuDeveloper()
    {
        menu.SetActive(false);
        developers.SetActive(true);
    }

    public void MenuExit()
    {
        Application.Quit();
    }

    public void CollectionBack()
    {

        menu.SetActive(true);
        collection.SetActive(false);
    }

    public void DeveloperBack()
    {
        menu.SetActive(true);
        developers.SetActive(false);
    }

    public void ReadyBack()
    {
        menu.SetActive(true);
        gamePlay.SetActive(false);
    }

    public void SelectTestStageGoNext()
    {
        activeSensor = true;
    }

 
    public void ScoreBoardGoMenuButton()
    {
        menu.SetActive(true);
    }
    
    // Update the stress level
   
}
