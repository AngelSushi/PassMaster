using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotativeObject : MonoBehaviour {

    // - rotation vers la droite

    public KBController controller;
    public bool isStopWatch;
    public bool left;
    public float speed;

    void Start() {
        
    }

    void Update() {
        
        if(!controller.begin) {
            if(isStopWatch) 
                transform.Rotate(0,speed,0);
            else {
                if(left) {
                    transform.Rotate(speed,0,0);

                    if(transform.eulerAngles.x >= 320) {
                        left = !left;
                    }
                }
                else {
                    transform.Rotate(-speed,0,0);

                    if(transform.eulerAngles.x >= 320) {
                        left = !left; 
                    }
                }
            }
        }
    }
}
