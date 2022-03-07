using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class TestScript : MonoBehaviour {

    public Rigidbody rb;
    public float speed;

    private bool curve;

    void Start() {
        rb.velocity = transform.forward * speed;
    }

    void Update() {

        if(curve) {
            rb.AddForce(10000f * Vector3.up,ForceMode.Impulse);
        }

    }

    public void OnInteract(InputAction.CallbackContext e) {
        if(e.started) {
            curve = !curve;
        }
    }

   
}
