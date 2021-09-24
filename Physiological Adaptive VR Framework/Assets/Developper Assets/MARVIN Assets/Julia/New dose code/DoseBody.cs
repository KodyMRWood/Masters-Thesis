using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class DoseBody : MonoBehaviour {

    public abstract List<DoseReceptor> getDoseReceptors();
    public abstract void secondaryStart();

    private float doseRate;
    private float countRate;
    private Transform hiddenTransform;
    protected float efficiency = 1f; //Overriden by GeigerController
    protected bool isPlayer = false;

    private float accumulatedDose = 0f;
    private float lastTime = 0f;


    public Transform getTransform() {

        return hiddenTransform;

    }

    void Start() {
        
        hiddenTransform = GetComponent<Transform>();
        secondaryStart();
        
    }


    public bool getIsPlayer() {

        return isPlayer;

    }

    public void setIsPlayer( bool isPlayer ) {

        this.isPlayer = isPlayer;

    }

    public void setDoseRate( float doseRate ) {

        this.doseRate = doseRate;

        if ( lastTime != 0 ) {

            float deltaTime = ( Time.time - lastTime ) / ( 3600f );
            accumulatedDose += deltaTime * doseRate;


        }

        lastTime = Time.time;

    }


    public void setCountRate( float countRate ) {

        this.countRate = countRate;

    }

    public float getDoseRate() {

        return doseRate;

    }

    public float getCountRate() {

        return countRate;

    }

    public float getEfficiency() {

        return efficiency;

    }

    public float getAccumulatedDose() {

        return accumulatedDose;

    }

}
