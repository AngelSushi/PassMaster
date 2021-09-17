using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour {
    
    public int x;
    public int z;
    public bool hasRotate;
    public Quaternion rotate;
    public Quaternion lastRotation;
    void Update() {
        x = (int)transform.position.x;
        z = (int)transform.position.z;

            if((int)transform.position.x == -430 && (int)transform.position.z < -15175) 
                rotate = Quaternion.Euler(90,180,0);   
            else if((int)transform.position.z == -16101 && (int)transform.position.x < -427) 
                rotate = Quaternion.Euler(90,270,0);         
            else if((int)transform.position.x == -1401 && (int)transform.position.z > -16100) 
                rotate = Quaternion.Euler(90,180,180);
            else if((int) transform.position.z == -15172 && (int)transform.position.x > -1397)
                rotate = Quaternion.Euler(90,90,0);

            if(transform.eulerAngles.y != rotate.y) 
                transform.rotation = Quaternion.Slerp(transform.rotation, rotate, 4*Time.deltaTime);

    }
}
