using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoseReceptor {

    //Used to store information

    private float mass; //Constant
    private float surfaceArea; //Constant
    private Transform receptorTransform;

    public DoseReceptor( float mass , float surfaceArea , Transform receptorTransform) {

        this.mass = mass;
        this.surfaceArea = surfaceArea;
        this.receptorTransform = receptorTransform;

    }

    public float getMass() {

        return mass;

    }

    public float getSurfaceArea() {

        return surfaceArea;

    }

    public Vector3 getPosistion() {

        return receptorTransform.position;


    }

}
