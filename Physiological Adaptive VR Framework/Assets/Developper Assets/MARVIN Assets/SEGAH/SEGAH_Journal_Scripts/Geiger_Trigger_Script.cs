using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Geiger_Trigger_Script : MonoBehaviour
{
    public string triggering = null;
    public bool isTesting = false;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnTriggerEnter(Collider dropOff)
    {
        if (dropOff.gameObject.tag == "Test")
        {
            triggering = dropOff.gameObject.name;
            isTesting = true;
        }
    }

    private void OnTriggerExit(Collider dropOff)
    {
        if (dropOff.gameObject.tag == "Test")
        {
            triggering = null;
            isTesting = false;
        }
    }
}

