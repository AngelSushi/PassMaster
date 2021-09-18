using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour {
   
    public bool zoomBox;
    public GameController controller;
    void Start() {
   
    }

    void Update() {
        
        if(controller.part != GameController.GamePart.PARTYGAME) {
            if(zoomBox) {
                transform.position = Vector3.MoveTowards(transform.position,new Vector3(1663,5880,-19244),13500 * Time.deltaTime); 
            }
            else {
                transform.position = new Vector3(-1940,5880,-18705);
            }
        }


        

    }

    
}
