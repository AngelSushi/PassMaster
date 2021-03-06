using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : CoroutineSystem { // Controller des joueurs lors de la phase de tutoriel et la phase de choix de l'ordre

    public Rigidbody rb;
    public float jumpSpeed;
    public OrderController orderController;
    public bool isPlayer,mustJump;

    void Update() {

        if(!isPlayer && mustJump && orderController.begin) {
            mustJump = false;
            float timer = Random.Range(2f,4f);

            RunDelayed(timer,() => {
                rb.AddForce(Vector3.up * jumpSpeed,ForceMode.Impulse);  
            });
        }
    }

    public void OnJump(InputAction.CallbackContext e) {
        if(e.started && orderController.begin) 
            rb.AddForce(Vector3.up * jumpSpeed,ForceMode.Impulse);            
    } 

    public void OnCollisionEnter(Collision collision) {
        if(collision.gameObject.tag == "Dice" && orderController.begin) {
            if(collision.gameObject.GetComponent<DiceController>().index == 0)
                collision.gameObject.GetComponent<DiceController>().index = 6;

            orderController.ChangeUser();
        }
    }

    
}
