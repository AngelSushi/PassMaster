using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetShifting : MonoBehaviour {

    #region Variables

    public float speed;
    public ArcheryController controller;
    public bool left;
    public bool bottom;

    #endregion

    #region Unity's Functions
    
    void Start() {
        if(bottom) 
            StartPositionTarget(controller.limitsBottom);
        else 
            StartPositionTarget(controller.limitsTop);
        
    }

    void Update() {
        if(!controller.begin) {
            if(bottom) {
                if(left) {
                    transform.position = Vector3.MoveTowards(transform.position,controller.limitsBottom[1].position,speed * Time.deltaTime);
                }
                else 
                    transform.position = Vector3.MoveTowards(transform.position,controller.limitsBottom[0].position,speed * Time.deltaTime);

                if(transform.position == controller.limitsBottom[0].position || transform.position == controller.limitsBottom[1].position) 
                    left = !left;               

            }
            else {
                if(left)
                    transform.position = Vector3.MoveTowards(transform.position,controller.limitsTop[1].position,speed * Time.deltaTime);
                else 
                    transform.position = Vector3.MoveTowards(transform.position,controller.limitsTop[0].position,speed * Time.deltaTime);

                if(transform.position == controller.limitsTop[0].position || transform.position == controller.limitsTop[1].position) 
                    left = !left;
            }
        }
            
    }

    #endregion

    #region Customs Functions

    private void StartPositionTarget(Transform[] limits) {
        if(left) 
            transform.position = limits[0].position;     
        else 
            transform.position = limits[1].position;
        
    }

    #endregion
}
