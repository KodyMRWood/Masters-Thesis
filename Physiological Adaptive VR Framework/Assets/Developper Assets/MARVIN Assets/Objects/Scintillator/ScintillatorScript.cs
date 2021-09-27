using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;


public class ScintillatorScript : MonoBehaviour
{
    [HideInInspector] public bool isOpen = true;

    public AudioClip openClip;
    public AudioClip closeClip;
    public AudioClip launchClip;

    private Animator a;
    private bool playerWithinTrigger = false;
    private DoseController doseController;
    private GameObject drawer;

    private bool hasAttached = false;

    private Source attachedSource;
    private TextMeshPro textMesh;


    private bool send = false; //Are you silly bud? I'm still gonna send it
   
    private float sentTime = 0f;
    private float lastSentTime = 0f;

    private AudioSource audioSource;
    
    // Start is called before the first frame update
    void Start() {
    
        a = this.GetComponent<Animator>();
        //doseController = GameObject.Find( "Dose controller" ).GetComponent<DoseController>();

        drawer = GameObject.Find( "LiquidScintillator_Drawer" ); //Dont spawn two scintillators, pls

        textMesh = GameObject.Find( "ScintillatorText" ).GetComponent<TextMeshPro>();
        textMesh.text = "";

        audioSource = GetComponent<AudioSource>();

    }

    public void addSource( Source source ) {

        hasAttached = true;
        attachedSource = source;

    }

    public bool hasAttachedSource() {

        return hasAttached;

    }

    private void Update() {

        if ( send ) {

            if ( Time.time - sentTime > 3.5f && hasAttached ) {

                send = false;
                hasAttached = false;

                audioSource.clip = launchClip;
                audioSource.Play();

                attachedSource.unfreeze();
                attachedSource.GetComponent<Rigidbody>().velocity = new Vector3( 0 , 3 , 3 );

                lastSentTime = Time.time;

                attachedSource = null;


            }

        }

        if ( isOpen && Time.time - lastSentTime > 8f ) {

            if ( !hasAttached ) {

                List<Source> sources = doseController.getSources();

                foreach ( Source source in sources ) {

                    //Interactable inta = source.GetComponent<Interactable>();

                    //if ( inta != null ) {
                    //
                    //   bool attached = inta.attachedToHand;
                    //
                    //   float distance = ( source.transform.position - this.transform.position ).magnitude; //MAGNITUDE POP POP https://www.youtube.com/watch?v=Lj9KOMEIylo
                    //   
                    //   if ( distance < 1  && !attached ) {
                    //
                    //       //BRING HER IN
                    //       source.freeze();
                    //       
                    //       addSource( source );
                    //
                    //   }
                    //
                    //}

                }

            }

        }

        if ( hasAttached ) {

            attachedSource.transform.rotation = drawer.transform.rotation * Quaternion.Euler( -90 , 0 , 0 );
            attachedSource.transform.position = drawer.transform.position + new Vector3( 0 , 0.04f , -0.15f );

        }

        if ( !isOpen && hasAttached ) {

            Dictionary<string,float> isotopes = attachedSource.getNewComposistion();

            string newText = "";

            foreach ( string isotope in isotopes.Keys ) {

                newText += isotope + ":" + ( int )( isotopes[ isotope ] * 100 ) + "%\n";

            }

            textMesh.text = newText;

        }
        else {

            textMesh.text = "";


        }

    }

    public void open() {
    
        if ( !isOpen ) {

            a.Play( "Scintillator_DrawerOpen" );
            isOpen = true;
            
            if ( hasAttached ) {

                send = true;
                sentTime = Time.time;

            }

            audioSource.clip = openClip;
            audioSource.Play();

        }
    
    }

    public void close()
    {
        switch (isOpen)
        {
            case true:
                a.Play("Scintillator_DrawerClose");
                isOpen = false;

                audioSource.clip = closeClip;
                audioSource.Play();

                break;



        }
    }

    public void toggle() {

        if ( isOpen ) {

            close();

        }
        else {

            open();

        }

    }

    /**
     *"At times the world may seem an unfriendly and sinister place, but believe that there is much more good in it than bad. 
     * All you have to do is look hard enough. 
     * and what might seem to be a series of unfortunate events may in fact be the first steps of a journey"
     **/

}
