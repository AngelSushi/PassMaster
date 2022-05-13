using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
public class ChefController : MonoBehaviour {

    public float speed;
    public Rigidbody rb;
    public GameObject actualIngredient;
    public Plate plate;

    private Vector2 movement;
    private Vector3 move;
    public bool isMoving;
    public bool canMoove;
    private Box actualBox;
    private CookAction action;
    private Quaternion rotation;

    void Update() {
        if(isMoving && canMoove)
            Movement();
        else 
            transform.rotation = rotation;
    }

    private void Movement() {
        float moveX = movement.x * speed * -1;
        float moveZ = movement.y * speed * -1;

        move = new Vector3(moveX,rb.velocity.y,moveZ);
        rb.velocity = move;

        rotation = Quaternion.LookRotation(move, Vector3.up);
        transform.rotation = rotation;
        
    }

    private void OnTriggerEnter(Collider hit) {
        if(hit.gameObject.GetComponent<Box>() != null)
            actualBox = hit.gameObject.GetComponent<Box>();
    }


    private void OnTriggerExit(Collider hit) {
        if(hit.gameObject.GetComponent<Box>() != null && actualBox != null)
            actualBox = null;
    }

    public void OnMove(InputAction.CallbackContext e) {
        movement = e.ReadValue<Vector2>();

        if(e.started) isMoving = true;
        if(e.canceled)  isMoving = false;    
    }
    

    public void OnInteract(InputAction.CallbackContext e) {
        if(e.started) {
            if(actualBox != null) {
                Box box = (Box)actualBox.GetComponent<Box>();

                box.Interact(this);
            }   
        }     
    }

}
