using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class PSOnTrigger : MonoBehaviour {

    private DoseController doseController;
    private Transform ceTransform;
    public ParticleSystem ps;

    public List<ParticleSystem.Particle> enter = new List<ParticleSystem.Particle>();
    

    void OnEnable() {

        //Finds the dose controller
        //We need the dose controller to find the shields
        GameObject[] allObjects = UnityEngine.Object.FindObjectsOfType<GameObject>();

        foreach ( GameObject gameObject in allObjects ) {

            if ( gameObject.GetComponent<DoseController>() != null ) {

                doseController = gameObject.GetComponent<DoseController>();
                break;

            }

        }

        ceTransform = GetComponentInChildren<Transform>(); 
        ps = GetComponent<ParticleSystem>();

        setColliders();

    }

    int frameCount = 0;

    void Update() {

        if ( frameCount == 90 ) {

            setColliders();
            frameCount = 0;

        }
        frameCount++;
   

    }

    //She a thicc
    
    private void setColliders() {

        List<Shield> shields = doseController.getShields(); //Gets all shields on the map from the dose controller
        shields = doseController.sortShields( shields , ceTransform.position ); //Sorts the shields closest to the source

        int max = 5;
        int count = shields.Count < max ? shields.Count : max; //If the number of shields is less than the max then iterate through all shields, else go up to 6 of the closest
        
        for ( int i = 0 ; i < count ; i++ ) {

            ps.trigger.SetCollider( i , shields[ i ].GetComponent<Collider>() );
      
            //Set to these colliders

        }

    }

    //Called when a particle collides with a shield
    private void OnParticleTrigger() {
    
        
        int enterParticleNumber = ps.GetTriggerParticles(ParticleSystemTriggerEventType.Enter, enter);

        Vector3 exitpoint = new Vector3( 0 ,0 , 0 );
        Vector3 entrypoint = new Vector3( 0, 0 , 0 );
        
        //Interates over particles that have collided
        for ( int i=0 ; i < enterParticleNumber ; i++ ) { 
        
            ParticleSystem.Particle p = enter[ i ]; //Gets the particle

            Ray ray = new Ray(p.position+p.velocity, -p.velocity.normalized);
            Ray namehit = new Ray(p.position, p.velocity.normalized);
            RaycastHit[] hits = Physics.RaycastAll(ray,p.velocity.magnitude);
            RaycastHit getName;
            string colliderhit = "N/A";

            if (Physics.Raycast (namehit,out getName , 0.1F ) ) { 
            
                colliderhit = getName.collider.name;
         

            }
            
            entrypoint = p.position;

           
            foreach (RaycastHit hit in hits ) { 
            
                if (hit.collider.gameObject.name == colliderhit){
                 
                    exitpoint = hit.point;
                 
                    break;

                }

            }

            if (Random.Range(0, 1000) < 1000 * ( 1 - Mathf.Exp( -1.2F * ( exitpoint - entrypoint ).magnitude ) ) ) {
                
                p.velocity = new Vector3(0, 0, 0);
                p.remainingLifetime = 1;      
                
            }

            enter[i] = p;


        }


        ps.SetTriggerParticles(ParticleSystemTriggerEventType.Enter, enter);


        //for (int i = 0; i < exitParticleNumber; i++)
        //{
        //    ParticleSystem.Particle p = exit[i];
        //    p.velocity = new Vector3(0, 0, 0);
        //    p.startColor = new Color32(255, 0, 0, 255);
        //    exit[i] = p;

        //}



    }

    void Start () {

        

    }

    

}
