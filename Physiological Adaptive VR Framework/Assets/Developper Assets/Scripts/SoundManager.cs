﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    //Instance held through scenes
    private static SoundManager _instance;
    public static SoundManager Instance { get { return _instance; } }


    //---Private Variables---
    DifficultyCalculator Calculator;

    //Place holder variable
    public enum Difficulty
    {
        EASY = 1,
        MEDIUM = 2,
        HARD = 3
    };
    public Difficulty diff;


    // Start is called before the first frame update
    void Awake()
    {
        //Determine if it is a the only one in the scene. 
        //If it is not the only one in the scene when loading from another scene. Destroy the non persisting instance of object

        if (_instance == null)
        {
            Destroy(gameObject);
        }
        else
        {
            _instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void ChangeSound()
    {
        switch(diff)
        {
            case Difficulty.EASY:
                //This will be easy,
                //Helpful audio: Loud
                //Not Helpful: Off
                break;
            case Difficulty.MEDIUM:
                //This will be medium,
                //Helpful audio: Middle, LessFrequent
                //Not Helpful: Middle, Slower Pace
                break;
            case Difficulty.HARD:
                //This will be Hard,
                //Helpful audio: Off
                //Not Helpful: Loud, Faster
                break;
            default:
                break;
        }
    }
}
