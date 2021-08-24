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
    [Tooltip("This is the threshhold for the difficulties. Once the PhysioScore reaches one of these thresholds, the difficulty will switch accordingly")]
    public float[] difficultyThresholds;



    //--- Private Variables ---
    public Assets.Scripts.PluxUnityInterface Bitalino;
    int activeChannels;
    List<List<int>> readings;
    public List<int> MultiThreadSubList = null;


    //--- Calculation Variables ---
    float baseHR = 0.0f;
    float currentHR = 0.0f;
    public float weightHR = 0.5f;

    float baseEDA = 0.0f;
    float currentEDA = 0.0f;
    public float weightEDA = 0.5f;

    //Framework
    public List<float> weightPerChannel = null;
    List<float> averagePerChannel = null;
    List<float> averagePerChannelLast = null;

    //physioScore will be the "score" calculated with the two physiological metrics to help determine which difficulty the user should be on
    float physioScore = 0.0f; 


    // Start is called before the first frame update
    float physioScoreLast = 0.0f;

    public enum Difficulty
    {
        EASY = 1,
        MEDIUM = 2,
        HARD = 3
    };
    //Refference
    //public Difficulty diff = Difficulty.EASY;



    void Awake()
    {
        Bitalino = FindObjectOfType<Assets.Scripts.PluxUnityInterface>();
        
    }

    // Update is called once per frame
    void Update()
    {
        activeChannels = Bitalino.ActiveChannels.Count;

        if(activeChannels > 0)
        {
            readings = Bitalino.MultiThreadSubListPerChannel;
            MultiThreadSubList = readings[0];
            //physioScore = CalculatePhysioScore(0);
            Debug.Log(physioScore);
        }
    }

    float CalculatePhysioScore(int channel)
    {

        float average = 0.0f;
        for (int y = 0; y < readings[channel].Count; y++)
        {
            average += readings[channel][y];
        }
        return average / readings.Count;

        //Framework
        /*
         *  float average = 0.0f;
         *  for (int y = 0; y < readings[channel].Count; y++)
         *  {
         *      average += readings[channel][y];
         *  }
         *  averagePerChannel[y] = average / readings.Count;
         *  return  averagePerChannel[y];
         *
         * 
         * 
        */

        //Each Metric listed is the average of all the samples in that aquisition
        //General
        //Score = ((edanow - edalast)*edaweight) + ((hrnow - hrlast)*hrweight)
        
        //Framework
        /*
         * 
         *
         * for (int x = 0; x< Bitalino.ActiveChannels.Count; x++)
         * {
         *  Score += (averagePerChannel[y] - averagePerChannelLast[y]) * weightPerChannel[x]
         * }
         * 
         * averagePerChannelLast = averagePerChannel;
        */

    }
    private void AdaptDifficulty(float physioScoreCurrrent)
    {
        //This function will calculate the difference in scores





        //Compare last score to this one
        /* if (Score <= scoreLast-10)
         * {
         *  difficulty--;
         * }
         * elseif (score > scorelast - 10 && score < scorelast + 10)
         * {
         *  //Nothing changes
         *  break;
         * }
         * elseif (score >= scorelast + 10)
         * {
         *  difficulty ++;
         * }
         * scoreLast = Score
         * 
         */
    }

}
