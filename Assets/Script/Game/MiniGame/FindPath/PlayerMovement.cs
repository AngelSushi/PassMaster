using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : CoroutineSystem {

    #region Variables

    public float speed;
    public float jumpSpeed;
    public CharacterController controller;
    public FP_Controller gameController;
    public bool canJump;
    public GameObject lastStep;

    public GameObject mainCamera;

    private bool isMooving;
    private bool isJumping;
    
    private float verticalVelocity;
    private float gravity = 300.0f;
    private float jumpforce = 175f;
    private bool isInLava; 
    private float time;
    private float seconds;
    private int count;
    private Vector3 movement;
    private Vector3 moveVector;
    private Quaternion lastRotation;

    public ButtonPush button;

    #endregion

    
    // Le bouton donne la prochaine ligne par rapport a la ou s'est arrêter le joueur
    void Update() {
        
        
        if(isInLava && !gameController.begin && !gameController.finish) {
            
            time+= Time.deltaTime;

        /*    RunDelayed(1f,() => {
                Debug.Log("run delayed");
                transform.gameObject.GetComponent<MeshRenderer>().enabled = !transform.gameObject.GetComponent<MeshRenderer>().enabled;
                transform.GetChild(0).gameObject.GetComponent<MeshRenderer>().enabled = !transform.GetChild(0).gameObject.GetComponent<MeshRenderer>().enabled;
            });
        */

            if(time >= 0.2) {
                transform.gameObject.GetComponent<MeshRenderer>().enabled = !transform.gameObject.GetComponent<MeshRenderer>().enabled;
                transform.GetChild(0).gameObject.GetComponent<MeshRenderer>().enabled = !transform.GetChild(0).gameObject.GetComponent<MeshRenderer>().enabled;
                time = 0;
                seconds++;
            }

            if(seconds >= 15) {
                isInLava = false;
                transform.gameObject.GetComponent<MeshRenderer>().enabled = true;
                transform.GetChild(0).gameObject.GetComponent<MeshRenderer>().enabled = true;
                seconds = 0;
            }
        }

        if(!button.isPushing && !gameController.begin && !gameController.finish)
            mainCamera.transform.position = new Vector3(transform.position.x,mainCamera.transform.position.y,transform.position.z);
            
        Movement();
    }

    private void Movement() {
        if(controller.isGrounded) {
                verticalVelocity = -gravity * Time.deltaTime * 3f;
             
                if(isJumping && canJump) {
                    verticalVelocity = jumpforce;

                    isJumping = false;
                }

            }

            else {
                verticalVelocity -= gravity * Time.deltaTime * 3f;
                
            }
            moveVector = Vector3.zero;

           if(isMooving) {
               moveVector.z = movement.y * speed * -1;
            }

            moveVector.y = verticalVelocity * jumpSpeed;

            if(isMooving) {
                moveVector.x = movement.x * speed * -1;
            }

            if(!isInLava && !gameController.finish) {
                controller.Move(moveVector * Time.deltaTime);
                if(!isJumping)
                    transform.rotation = Quaternion.LookRotation(moveVector);
                lastRotation = transform.rotation;
                
                if(!isMooving) {
                    if(!isJumping) 
                        transform.rotation = Quaternion.Euler(0f,-180f,transform.rotation.z);
                }
            }
    }

    public void OnMove(InputAction.CallbackContext e) {
        if(!gameController.begin) {
             movement = e.ReadValue<Vector2>();

            if(e.started)  isMooving = true;
            if(e.canceled)  isMooving = false;
        }

    }

    public void OnJump(InputAction.CallbackContext e) {
        if(e.started && !isJumping && canJump && !gameController.begin) {
            isJumping = true;      
        }
    }

    private void OnTriggerEnter(Collider hit) {
        if(hit.gameObject.tag == "Lava") {
            isInLava = true;
            transform.position = gameController.spawnPoints[0].transform.position;
        }

        if(hit.gameObject.tag == "End") {
            gameController.winPlayer = transform.gameObject;
            gameController.finish = true;
        }
    }

}
