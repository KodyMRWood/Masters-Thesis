using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Source : MonoBehaviour {

    
    public float initialActivity;
    public Units unit;
    public bool firstUse = true;
    public string originalDate = "";

    public List<Isotope> isotopes = new List<Isotope>();

    private Dictionary<string , float> activities = new Dictionary<string , float>();

    private Transform sourceTransform;
    
    private double startTime;

    private BoxCollider boxColldier;
    private Rigidbody rigidbody;

    //Used to contain information
    void Start() {

        this.sourceTransform = GetComponent<Transform>();
        this.startTime = new System.TimeSpan(System.DateTime.Now.Ticks).TotalMilliseconds;
        

        boxColldier = GetComponent<BoxCollider>();

        rigidbody = GetComponent<Rigidbody>();

    }

    public Dictionary<string , float> getNewComposistion() {

        Dictionary<string , float> newComposistion = new Dictionary<string , float>();

        float totalActivity = 0f;

        foreach ( string nom in activities.Keys ) {

            totalActivity += activities[ nom ];

        }

        foreach ( string nom in activities.Keys ) {

            newComposistion[ nom ] = activities[ nom ] / totalActivity;

        }

        return newComposistion;

    }

    private List<Isotope> getFullDecayChain() {

        List<Isotope> decayChain = new List<Isotope>();

        foreach ( Isotope isotope in getIsotopes() ) {

            recurseDecayChain( ref decayChain , isotope , 0 );

        }

        return decayChain;

    }

    private List<Isotope> recurseDecayChain( ref List<Isotope> decayChain , Isotope parent , int round ) {

        if ( round > 3 ) {

            Debug.Log( "ITS TIME TO STOP" );
            Debug.Break();

        }
        else {
            
            decayChain.Add( parent );

            if ( parent.getDecayProducts().Count != 0 ) {
                
                foreach ( Isotope daughter in parent.getDecayProducts() ) {
                    
                    recurseDecayChain( ref decayChain , daughter , round + 1 );

                }

            }

        }

        return decayChain;

    }

    private void recurseActivity( Isotope mother , Isotope daughter , float time , float initialAcitivity , bool first ) {

        //First calculate own activity
        float decayedActivity = 0;
        
        if ( first ) {

            float lambda = Mathf.Log( 2 ) / daughter.getHalfLife();
            decayedActivity = initialActivity * Mathf.Exp( ( time / 1000 ) * -lambda ) * daughter.getConcentration();
            
            activities[ daughter.getIsotopeName() ] = decayedActivity;

        }
        else {

            //Daughter parent
  
            if ( !daughter.isStable() ) {

                float lambdaM = Mathf.Log( 2 ) / mother.getHalfLife();
                float lambdaD = Mathf.Log( 2 ) / daughter.getHalfLife();

                decayedActivity = initialAcitivity * ( lambdaD / ( lambdaD - lambdaM ) ) * ( Mathf.Exp( -lambdaM * time ) - Mathf.Exp( -lambdaD * time ) ) * daughter.getConcentration();

                activities[ daughter.getIsotopeName() ] = decayedActivity;

            }
            else {

                activities[ daughter.getIsotopeName() ] = 0;

            }

        }

        foreach ( Isotope grandDaughter in daughter.getDecayProducts() ) {

            recurseActivity( daughter , grandDaughter , time , decayedActivity , false );

        }

    }

    public float getDecayedActivity( string isotopeName ) {

        double time = new System.TimeSpan(System.DateTime.Now.Ticks).TotalMilliseconds - startTime; //Milliseconds

        if ( !firstUse && originalDate.Length > 2 ) {

            time = new System.TimeSpan(System.DateTime.Now.Ticks).TotalMilliseconds - new System.TimeSpan( System.Convert.ToDateTime(originalDate).Ticks ).TotalMilliseconds;
           
        }

        float timef = ( float ) time;

        foreach ( Isotope mother in isotopes ) {
            
            recurseActivity( null , mother , timef , initialActivity , true );

        }

        return activities[ isotopeName ];

    }

    public Vector3 getPosistion() {

        if ( this.sourceTransform == null ){

            return new Vector3(0 , 0 , 0);

        }
        

        return sourceTransform.position;

    }

    //Returns Bq
    public float getActivity( string isotopeName ) {

        int setting = ( int ) unit; //mCi,Ci,Bq,TBq
        float activity = getDecayedActivity( isotopeName );

        if ( setting == 0 ) { //mCi

            return activity * 37000000;

        }
        else if ( setting == 1 ) { //Ci

            return activity * 37000000000;

        }
        else if ( setting == 2 ) { //Bq

            return activity;

        }
        else if ( setting == 3 ) { //TBq

            return activity * 1000000000000;

        }


        return activity;

    }

    public float getActivity( string unit , string isotopeName ) {

        float activity = getDecayedActivity( isotopeName );

        if ( unit == "mCi" ) {

            return activity;

        }
        
        return activity * 37000000; //mCi to Bq

    }

    public float getHalfLife( string isotopeName ) {

        foreach ( Isotope isotope in isotopes ) {

            if ( isotope.getIsotopeName() == isotopeName ) {

                return isotope.getHalfLife();

            }

        }

        return 0;

    }

    public float getGammaEnergy( string isotopeName ) {

        foreach ( Isotope isotope in isotopes ) {

            if ( isotope.getIsotopeName() == isotopeName ) {

                return isotope.getGammaDecayEnergy();

            }

        }

        return 0;

    }

    public float getBetaEnergy( string isotopeName ) {

        foreach ( Isotope isotope in isotopes ) {

            if ( isotope.getIsotopeName() == isotopeName ) {

                return isotope.getBetaDecayEnergy();

            }

        }

        return 0;

    }

    public List<Isotope> getIsotopes() {

        return this.isotopes;

    }
    
    public enum Units {

        mill_Ci,
        Ci,
        Bq,
        terra_Bq

    }

    public void freeze() {

        boxColldier.enabled = false;
        rigidbody.useGravity = false;

    }

    public void unfreeze() {

        boxColldier.enabled = true;
        rigidbody.useGravity = true;

    }

}
