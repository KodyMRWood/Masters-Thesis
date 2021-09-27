using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoseController : MonoBehaviour {

    public GameObject eventManager;

    public bool applyCorrectionCode = false;
    private Dictionary<string , Dictionary<float , float>> attenConstants = new Dictionary<string , Dictionary<float , float>>();
    private float airAttenuation = 0; //Please change and update later
    public bool debug;

    //This class is the controller for the dose system
    //Only one of these per scene

    // Use this for initialization
    void Start() {

        readCSV();

    }

    int t = 0;

    // Update is called once per frame
    void Update() {

        if ( t == 90 * 1 ) {

            calculateDose();
            t = 0;

        }

        t++;

    }



    private void calculateDose() {

        List<Source> sources = getSources();
        List<Shield> shields = getShields();
        List<DoseBody> doseBodies = getDoseBodies();
        
        foreach ( DoseBody body in doseBodies ) {


            float[] rates = calculateCountAndDoseRateForDoseBody(body , sources , shields);

            float countRate = rates[ 0 ];
            body.setCountRate(countRate);
            //If they are training dose will go up
            //if (eventManager.gameObject.GetComponent<Event_Mananger_Script>().startTraining)
            //{
            //    float doseRate = rates[ 1 ];
            //    body.setDoseRate(doseRate);
            //}
            ////Or it will stay at zero or the number after they have completed the training
            //else if (!eventManager.gameObject.GetComponent<Event_Mananger_Script>().startTraining)
            //{
            //    body.setDoseRate(0.0f);
            //}


        }

    }

    private List<Isotope> recurseDecayChain( ref List<Isotope> decayChain , Isotope parent , int round ) {

        if ( round > 10 ) {
            
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

    private float[] calculateCountAndDoseRateForDoseBody( DoseBody doseBody , List<Source> sources , List<Shield> shields ) {

        float countRate = 0;
        float doseRate = 0;

        foreach ( DoseReceptor doseReceptor in doseBody.getDoseReceptors() ) {

            foreach ( Source source in sources ) {

                List<Isotope> decayChain = new List<Isotope>();

                foreach ( Isotope isotope in source.getIsotopes() ) {

                    recurseDecayChain( ref decayChain , isotope , 0 );

                }

                foreach ( Isotope isotope in decayChain ) {
                    
                    float averageParticleEnergy = isotope.getGammaDecayEnergy() + isotope.getBetaDecayEnergy();
                    
                    float attenuatedActivity = source.getActivity( isotope.getIsotopeName() );

                    Vector3 origin = doseReceptor.getPosistion();

                    //Sort shields
                    shields = sortShields( shields , doseReceptor.getPosistion() );

                    bool passedThroughShieldAny = false;

                    for ( int i = 0 ; i < shields.Count ; i++ ) {

                        bool passedShield = false;

                        Shield shield = shields[ i ];

                        Vector3[] points = lineShieldIntersection( origin , source.getPosistion() , shield );

                        if ( points[ 0 ] != Vector3.zero && points[ 1 ] != Vector3.zero ) {

                            passedShield = true;
                            passedThroughShieldAny = true;

                            //This is our thickness
                            float thickness = Vector3.Distance( points[ 0 ] , points[ 1 ] );

                            Vector3 closestPoint = points[ 0 ];
                            Vector3 furthestPoint = points[ 1 ];

                            if ( Vector3.Distance( origin , points[ 1 ] ) < Vector3.Distance( origin , points[ 0 ] ) ) {

                                closestPoint = points[ 1 ];
                                furthestPoint = points[ 0 ];

                            }

                            float airAttenuationDistance = Vector3.Distance( origin , closestPoint );

                            attenuatedActivity = materialAttenuate( attenuatedActivity , airAttenuation , airAttenuationDistance );

                            if ( debug ) {

                                DrawLine( origin , closestPoint , Color.red );
                                DrawLine( closestPoint , furthestPoint , Color.green );

                            }


                            string assumed = "Concrete (Ordinary)";
                            string renderName = shield.getName();

                            if ( attenConstants.ContainsKey( renderName ) ) {

                                assumed = renderName;

                            }

                            if ( attenConstants.ContainsKey( assumed ) ) {

                                Dictionary<float , float> attenData = attenConstants[ assumed ];

                                float materialAttenuationConstant = 10; //Concrete for 1000 keV, uses this if it cant find a useful energy

                                List<float> keyList = new List<float>( attenData.Keys );

                                if ( averageParticleEnergy < keyList[ 0 ] ) { //If the particle energy is below the lowest energy

                                    //y = mx + b
                                    float m = ( attenData[ keyList[ 1 ] ] - attenData[ keyList[ 0 ] ] ) / ( keyList[ 1 ] - keyList[ 0 ] );
                                    float b = attenData[ keyList[ 1 ] ] - ( m * keyList[ 1 ] );

                                    materialAttenuationConstant = ( m * averageParticleEnergy ) + b;

                                }
                                else if ( averageParticleEnergy > keyList[ attenData.Keys.Count - 1 ] ) { //If the particle energy is larger than the highest particle energy

                                    //y = mx + b
                                    float m = ( attenData[ keyList[ attenData.Keys.Count - 2 ] ] - attenData[ keyList[ attenData.Keys.Count - 1 ] ] ) / ( keyList[ attenData.Keys.Count - 2 ] - keyList[ attenData.Keys.Count - 1 ] );
                                    float b = attenData[ keyList[ attenData.Keys.Count - 1 ] ] - ( m * keyList[ attenData.Keys.Count - 1 ] );

                                    materialAttenuationConstant = ( m * averageParticleEnergy ) + b;

                                }
                                else { //Particle energy is somewhere in known particle energy range

                                    for ( int k = 1 ; k < attenData.Keys.Count - 2 ; k++ ) {

                                        if ( averageParticleEnergy >= keyList[ k ] && averageParticleEnergy <= keyList[ k + 1 ] ) {

                                            //y = mx + b
                                            float m = ( attenData[ keyList[ k + 1 ] ] - attenData[ keyList[ k ] ] ) / ( keyList[ k + 1 ] - keyList[ k ] );
                                            float b = attenData[ keyList[ k ] ] - ( m * keyList[ k ] );

                                            materialAttenuationConstant = ( m * averageParticleEnergy ) + b;

                                            break;

                                        }


                                    }

                                }

                                attenuatedActivity = materialAttenuate( attenuatedActivity , materialAttenuationConstant , thickness );

                                if ( passedShield && applyCorrectionCode ) {
                                    
                                    float deadTime = attenuatedActivity / ( 1 - ( attenuatedActivity * 200 ) );
                                    float materialAttenuationCorrection = Mathf.Exp( -( 3.1f + ( ( ( float ) thickness / 100 ) * 0.0513678f ) ) );

                                    float distanceDoseDetector = ( doseReceptor.getPosistion() - shield.GetComponent<Transform>().position ).magnitude * 100; //cm

                                    float buildup = 1f; //Fix later

                                    attenuatedActivity *= doseBody.getEfficiency() * deadTime * materialAttenuationCorrection;

                                    if ( distanceDoseDetector < 50 && isotope.getBetaDecayEnergy() != 0 ) {

                                        //Less than 50cm, & beta so we gotta consider scattering
                                        attenuatedActivity += ( ( 50 - distanceDoseDetector ) * 0.095f * attenuatedActivity );

                                    }

                                }


                            }




                            origin = furthestPoint;

                        }

                    }



                    if ( debug ) {

                        DrawLine( origin , source.getPosistion() , Color.blue );

                    }

                    attenuatedActivity = materialAttenuate( attenuatedActivity , airAttenuation , Vector3.Distance( origin , source.getPosistion() ) );

                    //Attenuate distance
                    attenuatedActivity = ( ( attenuatedActivity ) / ( 4 * Mathf.PI * Mathf.Pow( ( doseReceptor.getPosistion() - source.getPosistion() ).magnitude , 2 ) ) ) * doseReceptor.getSurfaceArea();

                   
                    if ( applyCorrectionCode && !passedThroughShieldAny ) {
                        

                        //Correction

                        float distanceDoseSource = ( doseReceptor.getPosistion() - source.GetComponent<Transform>().position ).magnitude * 100; //cm

                        float deadTime = attenuatedActivity / ( 1 - ( attenuatedActivity * 200 ) );
                        float geometricFactor = 0.37f;

                        if ( distanceDoseSource < 3 ) { //Less than 3 centimeters
                            
                            geometricFactor = ( float )( distanceDoseSource * ( ( -0.03180557 + ( 19.65851 - -0.03180557 ) / ( 1 + Mathf.Pow( ( distanceDoseSource / 1.494623f ) , 1.501856f ) ) ) / ( ( 0.00113283 + ( 12233.67 - 0.00113283 ) / ( 1 + Mathf.Pow( ( distanceDoseSource / 0.04303171f ) , 2.002215f ) ) ) ) ) );
    
                        }

                        attenuatedActivity *= doseBody.getEfficiency() * deadTime * geometricFactor;

                    }

                    

                    countRate += attenuatedActivity;

                    //Dose rate
                    //averageActivity * particleEnergies[ i ] yields keV/s
                    //keV/s / 6241506479963235 yields j/s, conversion factor
                    //Dividing that by weight yields j/kg*s, and a Sv=j/kg
                    doseRate += ( attenuatedActivity * averageParticleEnergy * 1000 * 3600 ) / ( 6241506479963235 * doseReceptor.getMass() ); //Yields mSv/hr


                }


            }

        }

        return new float[]{  countRate , doseRate };

    }

   

    //Will return 2 points if line intersects box, will return 1 point if it 'knicks' the box, or return 0 points. Well the array length will always be 2 but they're just filled with zero vectors
    private Vector3[] lineShieldIntersection(Vector3 origin , Vector3 destination , Shield shield ) {

        RaycastHit hitA;
        RaycastHit hitB;
        Vector3 startA = Vector3.zero;
        Vector3 startB = Vector3.zero;


        Collider collider = shield.getCollider();

        //Checks to see if the source hit one face
        if ( collider.Raycast(new Ray(origin , ( ( origin - destination ) * -1f ).normalized) , out hitA , 1000f) ) {
            
            if ( hitA.transform.name == shield.name ) {

                startA = hitA.point;

                //Now that we get a hit, we work backwards to get another ray cast, from the target to the source
                if ( collider.Raycast(new Ray(destination , ( ( destination - origin ) * -1f ).normalized) , out hitB , 1000f) ) {
                    
                    if ( hitB.transform.name == shield.name ) {

                        startB = hitB.point;

                    }

                }

            }

        }


        Vector3[] points = new Vector3[2];

        points[0] = startA;
        points[1] = startB;


        return points;

    }

    

    private float materialAttenuate(float initialAcitvity , float attenuationConstant , float distance) {

        return initialAcitvity * Mathf.Exp(( float )( -attenuationConstant * distance ));


    }

    public List<Shield> sortShields( List<Shield> shields , Vector3 origin) {

        float[] distances = new float[ shields.Count ];
        for ( int i = 0 ; i < shields.Count ; i++ ) {

            distances[i] = ( shields[i].transform.position - origin ).magnitude;

        }

        int n = shields.Count;

        for ( int i = 0 ; i < n - 1 ; i++ ) {

            for ( int j = 0 ; j < n - i - 1 ; j++ ) {

                if ( distances[j] > distances[j + 1] ) {

                    Shield tempObject;
                    float tempDistance;

                    tempDistance = distances[j];
                    distances[j] = distances[j + 1];
                    distances[j + 1] = tempDistance;


                    tempObject = shields[j];
                    shields[j] = shields[j + 1];
                    shields[j + 1] = tempObject;

                }

            }

        }

        return shields;

    }

    //Finds all game objects with a 'Source' component and returns a list of type Source
    public List<Source> getSources() {

        GameObject[] allObjects = UnityEngine.Object.FindObjectsOfType<GameObject>();
        List<Source> sources = new List<Source>();

        foreach ( GameObject gameObject in allObjects ) {

            if ( gameObject.GetComponent<Source>() != null ) {

                sources.Add(gameObject.GetComponent<Source>());

            }

        }

        return sources;

    }

    //Finds all game objects with a 'Shield' component and returns a list of type Shield
    public List<Shield> getShields() {

        GameObject[] allObjects = UnityEngine.Object.FindObjectsOfType<GameObject>();
        List<Shield> shields = new List<Shield>();

        foreach ( GameObject gameObject in allObjects ) {

            if ( gameObject.GetComponent<Shield>() != null ) {

                shields.Add(gameObject.GetComponent<Shield>());

            }

        }

        return shields;

    }

    //Finds all game objects with a 'DoseBody' component and returns a list of type DoseBody
    public List<DoseBody> getDoseBodies() {

        GameObject[] allObjects = UnityEngine.Object.FindObjectsOfType<GameObject>();
        List<DoseBody> doseBodies = new List<DoseBody>();

        foreach( GameObject gameObject in allObjects ){

            if ( gameObject.GetComponent<DoseBody>() != null ) {

                doseBodies.Add( gameObject.GetComponent<DoseBody>() );

            }

        }

        return doseBodies;

    }

    private void readCSV() {

        string fileData = System.IO.File.ReadAllText("Assets/Developper Assets/MARVIN Assets/Julia/Attenuation Coefficients - All.csv");
        string[] lines = fileData.Split("\n".ToCharArray());

        if ( lines.Length > 0 ) {

            int n = lines[0].Split(",".ToCharArray()).Length;
            int j = lines.Length;

            string[,] rawData = new string[n , j];

            for ( int y = 0 ; y < lines.Length ; y++ ) {

                for ( int x = 0 ; x < lines[y].Split(",".ToCharArray()).Length ; x++ ) {

                    rawData[x , y] = lines[y].Split(",".ToCharArray())[x];

                }

            }

            

            for ( int y = 1 ; y < j ; y++ ) {

                if ( rawData[0 , y] != "" ) {

                    if ( !attenConstants.ContainsKey(rawData[0 , y]) ) {

                        attenConstants.Add(rawData[0 , y] , new Dictionary<float , float>());

                    }


                    if ( !attenConstants[rawData[0 , y]].ContainsKey(float.Parse(rawData[2 , y])) ) {

                        //2 is for column Energy (kEV)
                        //5 is for column Linear Attenuation Coeffficient(m-1)
                        attenConstants[rawData[0 , y]].Add(float.Parse(rawData[2 , y]) , float.Parse(rawData[5 , y]));
                   

                    }


                }

            }


        }


    }

    private void DrawLine(Vector3 start , Vector3 end , Color color , float duration = 1f) {
        GameObject myLine = new GameObject();
        myLine.transform.position = start;
        myLine.AddComponent<LineRenderer>();
        LineRenderer lr = myLine.GetComponent<LineRenderer>();
        lr.material = new Material(Shader.Find("Sprites/Default"));
        Gradient gradient = new Gradient();
        gradient.SetKeys(
            new GradientColorKey[] { new GradientColorKey(color , 1.0f) } ,
            new GradientAlphaKey[] {  new GradientAlphaKey(1 , 1.0f) }
        );

        lr.startWidth = 0.01f;
        lr.endWidth = 0.01f;
        lr.colorGradient = gradient;
        lr.SetPosition(0 , start);
        lr.SetPosition(1 , end);
        GameObject.Destroy(myLine , duration);
    }

}
