using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class User : DoseBody {
    
    //Collection of dose receptors
    private List<DoseReceptor> doseReceptors = new List<DoseReceptor>();
    

    public override void secondaryStart() {
        
        doseReceptors.Add( new DoseReceptor(77, 1.7f / 2 , getTransform()) );
        setIsPlayer( true );

    }

    public void Update() {



    }

    public override List<DoseReceptor> getDoseReceptors() {

        return doseReceptors;

    }




}
