using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RedLight : MonoBehaviour
{
    public Light pointLight;
    public Light spotLight1;
    public Light spotLight2;

    public float flashRate = 10.0f;
    public float spinRate = 10.0f;
    public bool isFlashing = true;
    public bool isOn = true;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //this.transform.Rotate(this.transform.up * (spinRate * Time.deltaTime));
        //this.transform.Rotate(this.transform.up, (spinRate * Time.deltaTime));
        this.transform.Rotate(0,(spinRate * Time.deltaTime),0, Space.Self);



        if (isFlashing)
        {
            if (isOn)
            {
                spotLight1.intensity -= flashRate * Time.deltaTime;
                spotLight2.intensity -= flashRate * Time.deltaTime;
                pointLight.intensity -= flashRate * Time.deltaTime;
                if (pointLight.intensity <= 0.1f)
                {
                    isOn = false;
                }

            }
            else
            {
                spotLight1.intensity += flashRate * Time.deltaTime;
                spotLight2.intensity += flashRate * Time.deltaTime;
                pointLight.intensity += flashRate * Time.deltaTime;
                if (pointLight.intensity >= 1.0f)
                {
                    isOn = true;
                }
            }
        }
        else
        {
            if (pointLight.intensity <= 1.0f)
            {
                spotLight1.intensity += flashRate * Time.deltaTime;
                spotLight2.intensity += flashRate * Time.deltaTime;
                pointLight.intensity += flashRate * Time.deltaTime;
            }
        }
    }

    public void Adapt ()
    {

    }
}
