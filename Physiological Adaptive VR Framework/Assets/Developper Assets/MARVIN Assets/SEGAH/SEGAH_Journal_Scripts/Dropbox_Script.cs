using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dropbox_Script : MonoBehaviour
{
    public GameObject source;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (source.GetComponent<Source_Searched_Found>().isDroppedOff && this.gameObject.transform.localPosition.z <2.06 )
        {
                this.gameObject.transform.localPosition = new Vector3(this.gameObject.transform.localPosition.x, this.gameObject.transform.localPosition.y , this.gameObject.transform.localPosition.z +0.03f);
            
        }
    }
}
