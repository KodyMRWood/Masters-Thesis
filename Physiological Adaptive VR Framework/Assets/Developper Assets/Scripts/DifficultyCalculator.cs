/*.
 * This script is to calculate the difficulty that the game/simulation will be running
 * It will take both the heart rate (HR) and electrodermal activity (EDA) and determine a suitable difficulty with both
 * 
 * Keep in mind that "difficulty" is a gerenal term that will be used to say how the simulation will adapt to the user, by making it easier or making it harder
 * 
 * */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DifficultyCalculator : MonoBehaviour
{
    //--- Public Variables ---

    public Assets.Scripts.PluxUnityInterface Bitalino;
    public List<int> MultiThreadSubList = null;


    //--- Private Variables ---
    private int activeChannels = 0;
    public List<List<int>> readings;
    private StateMachine stateMachine;
    private int adaptScore = 0;

    //Framework
    public List<float> weightPerChannel = null;
    List<float> averagePerChannel = null;
    List<float> averagePerChannelLast = null;




    void Awake()
    {
        Bitalino = FindObjectOfType<Assets.Scripts.PluxUnityInterface>();
        stateMachine = FindObjectOfType<StateMachine>();
        
    }

    // Update is called once per frame
    void Update()
    {
        //Change this into a different function that will be called at certain points
        activeChannels = Bitalino.ActiveChannels.Count;

        if(activeChannels > 0)
        {
                if(Input.GetKeyDown(KeyCode.A))
            {
                readings = Bitalino.MultiThreadSubListPerChannel2;
                MultiThreadSubList = readings[0];
            }

            //Testing
            //MultiThreadSubList = readings[0];
        }
        //CalculateAdaptScore();
    }

    //Return the average of a specific channel
    private void CalculateAveragePerChannel(int channel)
    {

        //Framework
        
         float average = 0.0f;
         for (int y = 0; y < readings[channel].Count; y++)
         {
             average += readings[channel][y];
         }
         averagePerChannel[channel] = average / readings.Count;
    }


    //Return the average of a all the  channels
    private void CalculateAverageAllChannels()
    {

        //Framewor
        float average = 0.0f;
        for (int channel = 0; channel < activeChannels; channel++)
        {

            for (int sample = 0; sample < readings[channel].Count; sample++)
            {
                average += readings[channel][sample];
                averagePerChannel[channel] = average / readings.Count;
            }
        }
    }


    private void CalculateAdaptScore()
    {
        //Most if you want one specific channel but this will get you all the channels
        //for(int x=0; x< activeChannels; x++)
        //{
        //    CalculateAveragePerChannel(x);
        //}

        
        CalculateAverageAllChannels();

        for(int x = 0; x < averagePerChannel.Count; x++)
        {
            if (averagePerChannel[x] < averagePerChannelLast[x])
            {
                adaptScore++;
            }
            else if (averagePerChannel[x] == averagePerChannelLast[x])
            {
                //Doesnt change the score
            }

            else if(averagePerChannel[x] > averagePerChannelLast[x])
            {
                adaptScore--;
            }

            averagePerChannelLast = averagePerChannel;

        }

        AdaptDifficulty(adaptScore);

    }
        private void AdaptDifficulty(float physioScoreCurrrent)
    {
        //Call the statemachines adapt function
        stateMachine.AdaptSimulation(physioScoreCurrrent);

    }

}
