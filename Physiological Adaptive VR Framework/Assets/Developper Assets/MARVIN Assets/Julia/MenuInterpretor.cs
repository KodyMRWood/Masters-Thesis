using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuInterpretor : MonoBehaviour {

    //AUTHOR: Julia

    private ScintillatorScript scintillator;
    

    private Controller controller;

    // Start is called before the first frame update
    void Start() {

        scintillator = GameObject.Find( "Liquid Scintillator" ).GetComponent<ScintillatorScript>();
        

        controller = Controller.getController();

    }

    void SendMessage( string message ) {

        if ( message == "ToggleScintillatorDoor" ) {

            scintillator.toggle();

        }
        else if ( message == "ToggleParticles" ) {

            controller.toggleParticles();

        }

    }

}
