using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class GeigerController : DoseBody {

    public bool pickedUp = false;
    public bool active = false;
    private Transform toolTransform;
    //private Interactable interactable;
    private Rigidbody toolBody;


    private Collider collider;

    private Transform handTransform;
    //private Hand hand;
    
    private GameObject handRenderPrefab;
    public Canvas geigerUI;

    private List<DoseReceptor> doseReceptors = new List<DoseReceptor>();
    
    //Components
    TextMesh doseTextMesh;
    AudioSource audioSourceGeigerClick;
    AudioSource audioSourceEffects;
    public MeshRenderer meshRendererStatusSphere;

    //Settings
    private string[] modes = { "CPS" , "CPM" , "Sv/hr" };
    private float[] scales = { 0.001f , 1f , 1000f , 10e6f };
    private string[] prefixes = { "m" , "" , "k" , "M" };

    int mode = 1;
    int scale = 2;

    private GameObject rightHand;
    private GameObject leftHand;

    bool triggerDown = false;

    public override void secondaryStart() {

        toolTransform = GetComponent<Transform>();
        //interactable = GetComponent<Interactable>();
        toolBody = GetComponent<Rigidbody>();

        //handTransform = rightHand.GetComponent<Transform>();
        //hand = rightHand.GetComponent<Hand>();

        doseReceptors.Add(new DoseReceptor( 1 , 0.02f , getTransform()) );
        efficiency = 0.025f;

        //components
        doseTextMesh = GetComponentInChildren<TextMesh>();
        audioSourceGeigerClick = GetComponents<AudioSource>()[ 1 ];
        audioSourceEffects = GetComponents<AudioSource>()[ 0 ];

        doseTextMesh.text = "";

        collider = GetComponent<BoxCollider>();
        

    }

    public bool getActive() {

        return active;

    }


    //On a scale of 1 to 100
    public float getIntensity() {

        float value = getValue();

        if ( value > 1000 ) {

            value = 1000;

        }

        return value / 10;

    }

    public float getValue() {

        float value;

        if ( mode == 0 || mode == 1 ) { //Counts

            value = getCountRate();

            if ( mode == 1 ) {

                value = value / 60; //CPS to CPM

            }

        }
        else { //Dose

            value = getDoseRate() / 1000;
            
        }

        value = value / scales[ scale ];
        
        return value;

    }

    

    // Update is called once per frame
    void Update() {
       
        if (this.GetComponent<OVRGrabbable>().isGrabbed)
        {
            pickedUp = true;
            active = true;
            //Play active sound, change color, turn on text mesh
            meshRendererStatusSphere.material.SetColor("_Color", Color.green);
            //collider.enabled = false; //Disabling the colliders since most people will put the geiger counter close to the source, and it'll launch it in the air

        }
        else
        {
            pickedUp = false;
            active = false;
            meshRendererStatusSphere.material.SetColor("_Color", Color.black);
            //collider.enabled = true;

        }

        if ( pickedUp ) {


            toolTransform.position = this.GetComponent<OVRGrabbable>().grabbedBy.transform.position;
            toolTransform.rotation = this.GetComponent<OVRGrabbable>().grabbedBy.transform.rotation * Quaternion.Euler(0, -90, 25);
            //Turn Canvas on an off
            geigerUI.enabled = false;
            
        }
        else
        {
            //geigerUI.enabled = true;
        }
        
        if ( active ) {


            if ( getValue() >= 0 ) {

                doseTextMesh.text = ( ( int ) getValue() ) + "\n" + prefixes[ scale ] + modes[ mode ]; //Cast to int so we dont get long decimals

            }
            else if ( getValue() < -10 ) {

                doseTextMesh.text = ( "LUDICROUS\n" + prefixes[ scale ] + modes[ mode ] ); //Prepare ship, ..., for LUDICROUS speed. What's the matter colonel Sanders, chicken?
                //https://youtu.be/mk7VWcuVOf0?t=46

            }
            else {

                doseTextMesh.text = ( 0 ) + "\n" + prefixes[ scale ] + modes[ mode ]; //Cast to int so we dont get long decimals

            }

        }
        else {

            doseTextMesh.text = "";

        }

        checkInputs();

    }

    private float totalTime = 0f;

    private void checkInputs() {



        //Change Mode
        if (this.GetComponent<OVRGrabbable>().isGrabbed && OVRInput.GetDown(OVRInput.Button.One, this.GetComponent<OVRGrabbable>().grabbedBy.GetController()))
        {
            audioSourceEffects.Play();

            //Change modes
            if ((mode + 1) >= modes.Length)
            {

                mode = 0;

            }
            else
            {

                mode++;

            }

        }
        //Change Scale
        if (this.GetComponent<OVRGrabbable>().isGrabbed && OVRInput.GetDown(OVRInput.Button.Two, this.GetComponent<OVRGrabbable>().grabbedBy.GetController()))
        {
            //Change scale
            if ((scale + 1) >= scales.Length)
            {

                scale = 0;

            }
            else
            {

                scale++;

            }

        }
    }

    private void updateActive() {

        if ( this.GetComponent<OVRGrabbable>().isGrabbed) {

            rightHand = GameObject.Find( "RightHandAnchor" );
            leftHand  = GameObject.Find("LeftHandAnchor");
            

            if ( ( rightHand.GetComponent<Transform>().position - toolTransform.position ).magnitude < ( leftHand.GetComponent<Transform>().position - toolTransform.position ).magnitude ) {

                //Right hand is closest, so that must've picked it up
                handTransform = rightHand.GetComponent<Transform>();
                //hand = rightHand.GetComponent<Hand>();

            }
            else {

                handTransform = leftHand.GetComponent<Transform>();
                //hand = leftHand.GetComponent<Hand>();

            }

            //Play active sound, change color, turn on text mesh
            meshRendererStatusSphere.material.SetColor( "_Color" , Color.green);
            collider.enabled = false; //Disabling the colliders since most people will put the geiger counter close to the source, and it'll launch it in the air

        }
        else {

            meshRendererStatusSphere.material.SetColor("_Color" , Color.black);
            collider.enabled = true;

        }

    }

    private void updateTransform() {

        if ( pickedUp ) {
            
            toolBody.useGravity = false;
            //interactable.highlightOnHover = false;
            toolTransform.SetParent(handTransform);
            
            //handRenderPrefab = hand.renderModelPrefab;
       
            //handRenderPrefab.GetComponent<MeshRenderer>().enabled = false;
            //hand.SetRenderModel(handRenderPrefab);
            


        }
        else {

            toolTransform.SetParent(null);

            toolBody.useGravity = true;
            //interactable.highlightOnHover = true;
            toolTransform.position = handTransform.position;

            //hand.SetRenderModel(handRenderPrefab);

        }


    }



    void SendMessage( string message ) {

        if ( message == "Pickup" ) {

            if ( pickedUp == false ) {

                active = true;
                updateActive();

            }
     
        }
        else if ( message == "Drop" ) {

            if ( pickedUp ) {

                active = false;
                updateActive();

            }

            pickedUp = !pickedUp;
            updateTransform();

        }

    }

    public override List<DoseReceptor> getDoseReceptors() {

        return doseReceptors;

    }

}
