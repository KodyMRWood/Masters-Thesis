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
    float baseHR = 0.0f;
    float currentHR = 0.0f;

    float baseEDA = 0.0f;
    float currentEDA = 0.0f;

    float phsioScore = 0.0f; //physioScore will be the "score" calculated with the two physiological metrics to help determine which difficulty the user should be on
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        CalculatePhysioScore();
    }

    float CalculatePhysioScore()
    {
        return 0.0f;
    }
}
