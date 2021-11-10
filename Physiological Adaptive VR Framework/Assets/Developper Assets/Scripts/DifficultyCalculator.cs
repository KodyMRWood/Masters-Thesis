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
    public float sigDiffEDA = 0.0f;
    public float sigDiffECG = 0.0f;
    public int sigDiffHR = 5;



    //--- Private Variables ---
    private int activeChannels = 0;
    public List<List<int>> readings;
    private List<float> baselineAverage;
    //Framework
    public List<float> weightPerChannel = null;
    List<float> averagePerChannel = null;
    List<float> averagePerChannelLast = null;


    //--- Data Sets ---
    [HideInInspector]
    public List<double> baseLineEMG = new List<double>() { }; // For completion
    //[HideInInspector]
    public List<double> baseLineEDA = new List<double>() { };
    //[HideInInspector]
    public List<double> baseLineECG = new List<double>() { };

    public int baseHeartRate = 0;
    private double baseAverageEMG = 0.0;
    private double baseAverageEDA = 0.0;
    private double baseAverageECG = 0.0;


    [HideInInspector]
    public List<double> currentEMG = new List<double>() { }; // For completion
   [HideInInspector]
    public List<double> currentEDA= new List<double>() { };
    [HideInInspector]
    public List<double> currentECG = new List<double>() { };

    public int heartRate = 0;
    private double averageEMG = 0.0;
    private double averageEDA = 0.0;
    private double averageECG = 0.0;


    public void CalculateBaseDataAverage()
    {

        //averageEMG = 0.0;
        baseAverageEDA = 0.0;
        baseAverageECG = 0.0;

        //Keep them in seperate loops incase the legnth of the list are different so it wont cause any errors.
        //for (int r = 0; r < currentEMG.Count; r++)
        //{
        //    averageEMG += r;
        //}
        for (int x = 0; x < baseLineEDA.Count; x++)
        {
            baseAverageEDA += baseLineEDA[x];
        }
        for (int y = 0; y < baseLineECG.Count; y++)
        {
            baseAverageECG += baseLineECG[y];
        }


        //averageEMG /= currentEMG.Count;
        baseAverageEDA = baseAverageEDA / (double)baseLineEDA.Count;
        baseAverageECG = baseAverageECG / (double)baseLineECG.Count;
    }

    public void CalculateDataAverage()
    {

        //averageEMG = 0.0;
        averageEDA = 0.0;
        averageECG = 0.0;

        //Keep them in seperate loops incase the legnth of the list are different so it wont cause any errors.
        //for (int r = 0; r < currentEMG.Count; r++)
        //{
        //    averageEMG += r;
        //}
        for (int x = 0; x < currentEDA.Count; x++)
        {
            averageEDA += currentEDA[x];
        }
        for (int y = 0; y < currentECG.Count; y++)
        {
            averageECG += currentECG[y];
        }


        //averageEMG /= currentEMG.Count;
        averageEDA = averageEDA / (double)currentEDA.Count;
        averageECG = averageECG / (double)currentECG.Count;


        CalculateAdaptScoreTrueTable();
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
        this.GetComponent<StateMachine>().AdaptSimulation(physioScoreCurrrent);

    }



    private void CalculateAdaptScoreTrueTable()
    {

        int adaptScore = 0;


        // If the average of a measurement is higher than the baseline of the same measurement the score will get higher 
        // visa versa, if avearage is lower than baseline score will be lower
        // If the score is lower, it means that the measurements were lower than the baseline,  therefore the difficulty needs to get harderr
        // If the score is higher, it means the measurmenets werrer higher than the baseline, therefore diffculty needs to get easier
        
       
        //EDA

        if (averageEDA < baseAverageEDA + sigDiffEDA)
        {
            adaptScore--;
        }
        else if (averageEDA >= baseAverageEDA + sigDiffEDA && averageEDA <= baseAverageEDA + (sigDiffEDA*2))
        {
            //Doesnt change the score
        }

        else if (averageEDA > baseAverageEDA + (sigDiffEDA * 2))
        {
            adaptScore++;
        }

        //ECG
        if (averageECG < baseAverageECG + sigDiffECG)
        {
            adaptScore--;
        }
        else if (averageECG >= baseAverageECG + sigDiffECG && averageECG <= baseAverageECG + (sigDiffECG * 2))
        {
            //Doesnt change the score
        }

        else if (averageECG > baseAverageECG + (sigDiffECG * 2))
        {
            adaptScore++;
        }


        //HR
        if (heartRate < baseHeartRate + sigDiffHR)
        {
            adaptScore--;
        }
        else if (heartRate >= baseHeartRate + sigDiffHR && heartRate <= baseHeartRate + (sigDiffHR * 2))
        {
            //Doesnt change the score
        }

        else if (heartRate > baseHeartRate + (sigDiffHR * 2))
        {
            adaptScore++;
        }

        AdaptDifficulty(adaptScore);

    }

}
