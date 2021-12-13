using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Event_Mananger_Script : MonoBehaviour
{


    public float trainingTimer = 0.0f;
    public bool startTraining = false;



    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

        //Time to keep track of how long it takes the player to find and drop off the source
        //If the player has completed the last sheilding test then start the timer
        if(startTraining)
        {
            trainingTimer += Time.deltaTime;
        }

    }



}
