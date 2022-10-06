using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class TestScript : MonoBehaviour {

    public Rigidbody rb;
    public float speed;

    private bool curve;

    void Start() {
       // rb.velocity = transform.forward * speed;
    }

    void Update() {

        if(curve) {
           // rb.AddForce(100f * Vector3.up,ForceMode.Impulse);
           // Debug.Log("add force");
        }

    }

    public void OnInteract(InputAction.CallbackContext e) {
        if(e.started) {
            rb.AddForce(40 * Vector3.up,ForceMode.Impulse);
            rb.AddForce(20 * Vector3.forward,ForceMode.Impulse);
            curve = !curve;
            Debug.Log("change curve value");
        }
    }

   
}
