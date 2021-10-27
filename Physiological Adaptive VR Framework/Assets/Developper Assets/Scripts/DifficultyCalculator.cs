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


    //--- Data Sets ---
    [HideInInspector]
    public List<double> baseLineEMG = new List<double>() { }; // For completion
    [HideInInspector]
    public List<double> baseLineEDA = new List<double>() { };
    [HideInInspector]
    public List<double> baseLineECG = new List<double>() { };

    [HideInInspector]
    public List<double> currentEMG = new List<double>() { }; // For completion
    [HideInInspector]
    public List<double> currentEDA= new List<double>() { };
    [HideInInspector]
    public List<double> currentECG = new List<double>() { };

    private float averageEMG = 0.0f;
    private float averageEDA = 0.0f;
    private float averageECG = 0.0f;





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

               //readings = Bitalino.MultiThreadSubListPerChannel2;
               //MultiThreadSubList = readings[0];
            //Testing
            //MultiThreadSubList = readings[0];
        }
        //CalculateAdaptScore();
    }



    private void CalculateDataAverage()
    {
        averageEMG = 0.0f;
        averageEDA = 0.0f;
        averageECG = 0.0f;

        //Keep them in seperate loops incase the legnth of the list are different so it wont cause any errors.
        for (int r = 0; r < currentEMG.Count; r++)
        {
            averageEMG += r;
        }
        for (int r = 0; r < currentEDA.Count; r++)
        {
            averageEDA += r;
        }
        for (int r = 0; r < currentECG.Count; r++)
        {
            averageECG += r;
        }

        averageEMG /= currentEMG.Count;
        averageEDA /= currentEDA.Count;
        averageECG /= currentECG.Count;
    }


    private void CalculateAdaptScore()
    {
        //Most if you want one specific channel but this will get you all the channels
        //for(int x=0; x< activeChannels; x++)
        //{
        //    CalculateAveragePerChannel(x);
        //}

        int adaptScore = 0;

        CalculateAdaptScore();

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
        //------Truth Table-------
        //HR = 5 bpm
        //EDA = 5-7 mS
        //What will be done in each state?
        //Easy state Visual cues, audio cues, flashing light will be off, siren off
        //Medium little- no visual or audio cues, flashing is slow, siren quiet
        //hard no visual or audio cues, flashing light is faster, siren louder
        //https://www.ncbi.nlm.nih.gov/pmc/articles/PMC8482411/pdf/nihms-1741060.pdf help1

        //Call the statemachines adapt function
        stateMachine.AdaptSimulation(physioScoreCurrrent);

    }



    private void CalculateAdaptScoreTrueTable()
    {

        int adaptScore = 0;

        CalculateAdaptScore();


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
