using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SourceTrigger : MonoBehaviour
{

    public float timeTriggering = 0.0f;
    public bool doneDetecting = false;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
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
