using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotativePike : CoroutineSystem {

    private bool hasRotate;
    private Vector3 end;
    void Start() {
        end = transform.position;
        end.z = 90;
    }

    void Update() {
        if(!hasRotate) {
            hasRotate = true;
            RunDelayed(0.015f,() => {
                transform.Rotate(0,-5,0);
                hasRotate = false;
            });
        }

        if((int)transform.position.z != (int)end.z) 
            transform.position = Vector3.MoveTowards(transform.position,end,80 * Time.deltaTime);
        else 
            Destroy(transform.gameObject);     
    }

    private void OnCollisionEnter(Collision hit) {
        if(hit.gameObject.tag == "Player" || hit.gameObject.tag == "Bot") 
            hit.gameObject.GetComponent<Rigidbody>().AddForce(Vector3.back * -2000,ForceMode.Acceleration);
    }
}
