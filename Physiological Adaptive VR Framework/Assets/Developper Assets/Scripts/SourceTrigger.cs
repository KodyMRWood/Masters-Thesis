using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SourceTrigger : MonoBehaviour
{
    private StatsCollector statsCollector = default;
    private float timeTriggering = 0.0f;
    private float timeToBeFound = 0.0f;

    [SerializeField] private int indexOfSource = 0;

    

    //--- Properties ---
    private bool isRealSource = false;
    public bool IsRealSource
    {
        get => isRealSource;
        set => isRealSource = value;
    }

    [SerializeField]private bool doneDetecting = false;
    public bool DoneDetecting
    {
        get => doneDetecting;
        set => doneDetecting = value;
    }

    private void OnEnable()
    {
        statsCollector = FindObjectOfType<StatsCollector>();
    }
    //private void Awake()
    //{
    //    statsCollector = FindObjectOfType<StatsCollector>();
    //}
    private void Update()
    {
        if(!DoneDetecting)
        {
            timeToBeFound += Time.deltaTime;
        }

        //Testing
        if(DoneDetecting)
        {
            statsCollector.RecordSourceFoundTime(timeToBeFound, indexOfSource);
            DoneDetecting = false;
        }
    }
    private void OnTriggerStay(Collider other)
    {
        if(other.gameObject.tag == "Geiger")
        {
            timeTriggering += Time.deltaTime;
            if (timeTriggering >= 5.0f && !DoneDetecting)
            {
                DoneDetecting = true;
                statsCollector.RecordSourceFoundTime(timeToBeFound,indexOfSource);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Geiger")
        {
            timeTriggering = 0.0f;
        }
    }   
}