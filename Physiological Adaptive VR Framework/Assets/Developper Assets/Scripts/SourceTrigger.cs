using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SourceTrigger : MonoBehaviour
{

    public float timeTriggering = 0.0f;
    

    //--- Properties ---
    private bool isRealSource = false;
    public bool IsRealSource
    {
        get => isRealSource;
        set => isRealSource = value;
    }

    private bool doneDetecting = false;
    public bool DoneDetecting
    {
        get => doneDetecting;
        set => doneDetecting = value;
    }

    private void OnTriggerStay(Collider other)
    {
        if(other.gameObject.tag == "Geiger")
        {
            timeTriggering += Time.deltaTime;
            if (timeTriggering >= 5.0f)
            {
                doneDetecting = true;
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
