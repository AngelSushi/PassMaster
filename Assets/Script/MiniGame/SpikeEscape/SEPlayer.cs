using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class SEPlayer : MonoBehaviour {

    public Rigidbody rb;
    public float jumpForce;
    private SEController controller;

    void Start() {
        controller = SEController.instance;
    }

    public void OnJump(InputAction.CallbackContext e) {
        if(e.started && !controller.begin && !controller.finish) 
            rb.AddForce(Vector3.up * jumpForce,ForceMode.Impulse);
    }

    public void OnSneak(InputAction.CallbackContext e) {
        if( !controller.begin && !controller.finish) {
            if(e.started ) 
                transform.localScale = new Vector3(transform.localScale.x,2.82f,transform.localScale.z);    
            if(e.canceled) 
                transform.localScale = new Vector3(transform.localScale.x,5.125f,transform.localScale.z);
        }
    }

    private void OnTriggerEnter(Collider hit) {
        if(hit.gameObject.tag == "Lava") {
            transform.gameObject.SetActive(false);
            if(!controller.deadPlayers.Contains(transform.gameObject))
                controller.deadPlayers.Add(transform.gameObject);
        }
    } 
}
