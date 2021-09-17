using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class KB_PlayerMovement : MonoBehaviour {
    #region Variables
    
    public KBController controller;
    public float speed;
    public float jumpSpeed;
    //public CharacterController controller;
    public Rigidbody rb;
    public bool canJump;
    private bool jump;
    private int countJump;
    public Vector3 respawnPos;

    public bool isOnBall;
    public bool dead;
    public bool freeze;
    private float time;
    private float seconds;

    private bool isMoving;
    public bool isJumping;
    public float timeToDestroy;
    public bool thruster;
    public float thrusterForce;
    
    private Vector3 movement;
    private float maxSpeed = 105;
    private string lastHit;
    private float timer;
    private float thrusterTime;

    public GameObject lastFloor;

    #endregion

    #region Unity's Functions

    void Start() {
    }

    void Update() {
        
       if(!controller.begin && !controller.finish) {
            //if(isOnBall && !isJumping) 
           // transform.position = new Vector3(transform.position.x,59.5f,transform.position.z);

            if(!dead && !freeze) {
                Movement();

                if(thruster) {
                    thrusterTime += Time.deltaTime;
                    rb.AddForce(new Vector3(0,0,thrusterForce));
                    if(thrusterTime >= 0.05f) {
                        thrusterTime = 0;
                        thruster = false;
                    }
                }

                if(rb.velocity.magnitude > (speed + 10)){
                    rb.velocity = Vector3.ClampMagnitude(rb.velocity, (speed + 10));
                }

            
            // Debug.Log("magnitude: " + rb.velocity.magnitude);

                timer += Time.deltaTime;

                //Debug.Log("velocityZ: " + rb.velocity.z);

                if(isOnBall) {
                    if( (rb.velocity.x > 0.1 || rb.velocity.x < -0.1) || (rb.velocity.z > 0.1 || rb.velocity.z < -0.1)) {
                        transform.GetChild(2).Rotate(2,0,0);
                    }
                }
                
                if(isOnBall && isMoving && speed < maxSpeed && timer >= 0.5 && movement.y > 0) {
                // speed += 6;
                    timer = 0;
                    return;
                }
                
                if(!isMoving) {
                // speed = 75;
                }
            }
            else if(dead) {
                time += Time.deltaTime;

                if(time >= 0.2) {  
                    transform.gameObject.GetComponent<MeshRenderer>().enabled = !transform.gameObject.GetComponent<MeshRenderer>().enabled;
                    transform.GetChild(0).gameObject.GetComponent<MeshRenderer>().enabled = !transform.GetChild(0).gameObject.GetComponent<MeshRenderer>().enabled;
                    time = 0;
                    seconds++;
                }

                if(seconds == 15) {
                    dead = false;
                    transform.gameObject.GetComponent<MeshRenderer>().enabled = true;
                    transform.GetChild(0).gameObject.GetComponent<MeshRenderer>().enabled = true;
                    seconds = 0;
                    time = 0;
                }
            }
       }
       else {
           if(controller.finish && isJumping && jump && countJump < 3) {
               rb.AddForce(Vector3.up * jumpSpeed,ForceMode.Impulse);  
               jump = false;
               countJump++;
           }
       }
    }

    private void Movement() {
        float moveX = movement.y * speed * -1;
        float moveZ = movement.x * speed;

        if(!isOnBall) rb.velocity = new Vector3(moveX, rb.velocity.y,moveZ);
        else {
           // rb.velocity = new Vector3(moveX * speed * Time.deltaTime, rb.velocity.y,rb.velocity.z);
            Vector3 force = new Vector3(moveX,0,moveZ);
            rb.AddForce(force);
        }
    }


    private void OnCollisionEnter(Collision hit) {
        if(hit.gameObject.tag == "RedBall") {
            isOnBall = true;
            Vector3 centerPos = hit.gameObject.GetComponent<SphereCollider>().bounds.center;
            transform.position = new Vector3(centerPos.x,centerPos.y + 10.5f,centerPos.z);
            hit.gameObject.transform.parent = transform; 

            if(isJumping && (lastHit == "Sol" || lastHit == "Untagged")) 
                isJumping = false;
        }

        if(hit.gameObject.tag == "Sol") {
            isJumping = false;
            jump = true;
        }

        if(hit.gameObject.tag == "Portal") 
            transform.position = controller.portalPoints[controller.ConvertPlayerInt(transform.gameObject)];   
   

        lastHit = hit.gameObject.tag; 
    }

    private void OnCollisionExit(Collision hit) {   
        if(hit.gameObject.tag == "Sol") {
            respawnPos = transform.position;
            lastFloor = hit.gameObject;
        }
    }

    public void OnMove(InputAction.CallbackContext e) {
        movement = e.ReadValue<Vector2>();

        if(e.started) isMoving = true;
        if(e.canceled)  isMoving = false;

            

    }

    public void OnJump(InputAction.CallbackContext e) {
        if(e.started && !isJumping && canJump) {
            isJumping = true;
            respawnPos = transform.position;
            rb.AddForce(Vector3.up * jumpSpeed,ForceMode.Impulse);     
        }
    }

    #endregion

}
