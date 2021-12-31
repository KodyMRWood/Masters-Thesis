using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class UI_HUD_Scripts : MonoBehaviour
{
 
    public Text Timer;
    public Text Dose;
    public Text Objective;

    public GameObject eventManager;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    //We are <color=green>green</color> with envy


    void Update()
    {


        //if (eventManager.gameObject.GetComponent<Event_Mananger_Script>().trainingTimer != 0.0f)
        //{
        //    Timer.text = "Timer: " + eventManager.gameObject.GetComponent<Event_Mananger_Script>().trainingTimer.ToString();
        //
        //
        //}
        //else
        //{
        //    Timer.text = "Timer: Training not started.";
        //}
        Dose.text = "Dose: " + this.gameObject.GetComponent<Controller>().totalPlayerDosage.ToString();
        //Objective.text = "Objective: " + eventManager.gameObject.GetComponent<Event_Mananger_Script>().objective;
    }
}
