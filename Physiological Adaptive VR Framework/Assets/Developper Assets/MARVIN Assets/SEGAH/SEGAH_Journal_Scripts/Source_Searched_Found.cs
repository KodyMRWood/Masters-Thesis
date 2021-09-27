using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Source_Searched_Found: MonoBehaviour
{
    public bool isFound = false;
    public bool isDroppedOff = false;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(this.transform.parent != null)
        {
            isFound = true;
        }

        if(isFound)
        {
            isFound = true;
        }

        
    }

    private void OnTriggerEnter(Collider dropOff)
    {
        if (dropOff.gameObject.tag == "drop")
        {
            isDroppedOff = true;
        }
    }
}
