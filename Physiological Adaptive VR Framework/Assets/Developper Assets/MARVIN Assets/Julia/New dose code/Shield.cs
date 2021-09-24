using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shield : MonoBehaviour {

    //Used to contain information
    public string nom;

    public string getName() {

        return nom;

    }

    public Collider getCollider() {

        return GetComponent<Collider>();

    }

}
