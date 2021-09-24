using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GeigerSound : MonoBehaviour {

    private AudioSource audio;
    
    private GeigerController geigerController;


    // Start is called before the first frame update
    void Start() {

      
        audio = GetComponents<AudioSource>()[ 1 ];

        geigerController = GetComponent<GeigerController>();
        StartCoroutine( SoundCoroutine() );

    }

    // Update is called once per frame
    void Update() { 
    

        
    }

    IEnumerator SoundCoroutine() {

        while ( true ) {

            if ( geigerController != null ) {
                
                if ( geigerController.getIntensity() > 0 && geigerController.getActive() ) {


                    audio.Play();

                    //Want it so at 100% tick is every 0.5seconds
                    float minTime = 0.1f;
                    float maxTime = 1f;

                    //y=mx+b
                    float m = -( maxTime - minTime ) / ( 100 );
                    float b = maxTime;

                    yield return new WaitForSeconds( ( m * geigerController.getIntensity() ) + b );


                }
                else {

                    yield return new WaitForSeconds( 1f );

                }
                
            }

        }

    }


}
