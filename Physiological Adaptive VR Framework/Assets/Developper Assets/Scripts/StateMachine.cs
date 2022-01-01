///
/// Responsible for changing the difficulty of the simulation. 
///
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateMachine : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public enum Difficulty
    {
        EASY = 1,
        MEDIUM = 2,
        HARD = 3,
        DEFAULT= 0,
    }
    public Difficulty difficulty = Difficulty.MEDIUM;

    public void AdaptSimulation (float stateScore)
    {
        if (stateScore <= -1 && difficulty != Difficulty.HARD)
        {
            //If the score is negative than the majority of the physiological metrics decreased, thus the simulation must become more difficult 
            difficulty++;
        }
        else if (stateScore == 0)
        {
            //Difficulty stays the same. Only here for completion
        }
        else if (stateScore >= 1 && difficulty != Difficulty.EASY)
        {
            //If the score is negative than the majority of the physiological metrics decreased, thus the simulation must become easier 
            difficulty--;
        }

        Debug.Log(difficulty + " " + stateScore);
    }
}
