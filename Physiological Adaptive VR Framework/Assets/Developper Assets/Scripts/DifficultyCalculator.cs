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
    
    // This array will be set in the inspector. It will determine what the rresearcher considers a significant change in the metric
    // ie. HR + 5bpm is significantly higher
    public float[] significantChange; 



    //--- Private Variables ---
    private int activeChannels = 0;
    public List<List<int>> readings;
    private StateMachine stateMachine;
    private List<float> baselineAverage;
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

        //Framework
        for (int channel = 0; channel < activeChannels; channel++)
        {

            float average = 0.0f;
            for (int sample = 0; sample < readings[channel].Count; sample++)
            {
                average += readings[channel][sample];
                averagePerChannel[channel] = average / readings.Count;
            }
        }
    }

    //This function will be used to calculate the average measurements for each channel to get the baseline of the user whmen they are in a  relaxed state. It will be stored in the baseLineAverage list which will be used when determining the difficulty.

    private void CalculateAverageAllChannelsBaseline()
    {
        //Framework
        for (int channel = 0; channel < activeChannels; channel++)
        {
            float average = 0.0f;
            for (int sample = 0; sample < readings[channel].Count; sample++)
            {
                average += readings[channel][sample];
                baselineAverage[channel] = average / readings.Count;
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

        int adaptScore = 0;

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



    private void CalculateAdaptScoreTrueTable()
    {

        int adaptScore = 0;

        CalculateAverageAllChannels();


        // If the average of a measurement is higher than the baseline of the same measurement the score will get higher 
        // visa versa, if avearage is lower than baseline score will be lower
        // If the score is lower, it means that the measurements were lower than the baseline,  therefore the difficulty needs to get harderr
        // If the score is higher, it means the measurmenets werrer higher than the baseline, therefore diffculty needs to get easier
        
        //Change it so that is baseline + significant Change in metric (i.e hr baseline+5bpm)
        for (int x = 0; x < averagePerChannel.Count; x++)
        {
            if (averagePerChannel[x] < baselineAverage[x] + significantChange[x])
            {
                adaptScore--;
            }
            else if (averagePerChannel[x] == baselineAverage[x] + significantChange[x])
            {
                //Doesnt change the score
            }

            else if (averagePerChannel[x] > baselineAverage[x] + significantChange[x])
            {
                adaptScore++;
            }
        }

        AdaptDifficulty(adaptScore);

    }

}
