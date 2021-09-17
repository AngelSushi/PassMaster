using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour {

    public float speed;
    public float jumpSpeed;
    public bool isBot;
    public bool canJump;
    public bool canMoove;
    public bool isInTutorial;
    public GameController gameController;

    private float verticalVelocity;
    private float gravity = 300.0f;
    private float jumpforce = 175f; // Height of the jump
    private DialogController dialogController;
    private TutorialController tutorialController;
    private int circleCount;
    private PlayerInputActions pInputAction;
    private bool isMooving;
    public bool isJumping;
    public bool jump;
    private int amplifier;
    private bool isSprinting;
    private bool doubleJump;
    private Vector3 moveVector;
    private Vector2 movement;
    private int count = 0;
    private int maxCount = 2;
    private bool keyJump;
    private CharacterController controller;
    private float turnSmooth = 0.1f;
    private float turnSmoothVelocity;

    private int random = -1;
    private float time;

    void Start() {

        controller = GetComponent<CharacterController>();

        dialogController = GetComponent<DialogController>();

        gameController = GameObject.FindGameObjectsWithTag("Game")[0].GetComponent<GameController>();

        if(gameController.GetPart() == GameController.GamePart.TUTORIAL) 
            tutorialController = GameObject.Find("tutorial").GetComponent<TutorialController>();
 
    }

    void Update(){

        if(!isBot) PlayerMovement();

        else {
           if(canJump) {
                if(random == -1) random = Random.Range(2,7);

                time += Time.deltaTime;

                if(time >= random) 
                    BotJump();      
           }
        }
    }

    private void BotJump() {
        Vector3 moveVector = Vector3.zero;

        if(controller.isGrounded) {          

            verticalVelocity = -gravity * Time.deltaTime * 3f;

            if(canJump && !isJumping) {
                verticalVelocity = jumpforce;
                isJumping = true;
            }          
        }

        else {

           if(canJump && !isJumping) {
                verticalVelocity = jumpforce;
                isJumping = true;
            } 

            verticalVelocity -= gravity * Time.deltaTime * 3f;
        }

        moveVector.y = verticalVelocity * jumpSpeed;

        controller.Move(moveVector * Time.deltaTime);      
    }

    void PlayerMovement() {
        
    //  if(gameController.GetPart() == GameController.GamePart.TUTORIAL) {
          if(!dialogController.isInDialog) {
            if(controller.isGrounded) {
                verticalVelocity = -gravity * Time.deltaTime * 3f;
             
                if(isJumping && canJump) {
                    verticalVelocity = jumpforce;

                    jump =false;
                }

            }

            else {

                if(doubleJump && count < maxCount) {
                    verticalVelocity = jumpforce;

                    doubleJump =false;
                    count++;
                }

                else 
                    verticalVelocity -= gravity * Time.deltaTime * 3f;
            }
            moveVector = Vector3.zero;

           if(isMooving) 
               moveVector.x = movement.y * speed ;   

            moveVector.y = verticalVelocity * jumpSpeed;

            if(isMooving) 
                moveVector.z = movement.x * speed * -1;   

            controller.Move(moveVector * Time.deltaTime);
                
            //transform.Rotate(0,movement.x,0);
        }
    }
    
    public void OnMove(InputAction.CallbackContext e) {
          movement = e.ReadValue<Vector2>();

        if(e.started)  
            isMooving = true;
        if(e.canceled)  
            isMooving = false;
    }

    public void OnSprint(InputAction.CallbackContext e) {
        if(e.started && !isSprinting) {
            speed *= 3;
            isSprinting = true;
        }

        else if(e.canceled) {
            speed /= 3;
            isSprinting = false;
        }
    }

    public void OnJump(InputAction.CallbackContext e) {
        if(!isJumping && canJump) {
            isJumping = true;
            jump = true;
        }    

        if(e.started && isJumping) 
            doubleJump = true;
        if(e.started) 
            keyJump = true;
        else if(e.canceled) 
            keyJump = false;       
    } 

    private void OnTriggerEnter(Collider hit) {
         if(hit.gameObject.tag == "Dice") {

            int result = hit.gameObject.GetComponent<DiceController>().index;
            if(result == 0) result = 6;


            if(gameController.GetActualPlayer() < 4) {
                //gameController.DisplayDiceResult(gameController.GetActualPlayer(),result);

                gameController.GetPlayers()[gameController.GetActualPlayer()].GetComponent<PlayerController>().canJump = false;

//                gameController.AddResult(result + 1,gameController.GetActualPlayer());
                
                /* if(gameController.GetActualPlayer() == 3) {
                    gameController.SortPlayers();
                } */

                gameController.SetActualPlayer(gameController.GetActualPlayer() + 1);

                gameController.GetPlayers()[gameController.GetActualPlayer()].GetComponent<PlayerController>().canJump = true;       
                
                if(gameController.GetDices()[gameController.GetActualPlayer()] != null) {
                   // GameObject dice = gameController.GetDices()[gameController.GetActualPlayer()];
                  
                    gameController.GetDices()[gameController.GetActualPlayer()].GetComponent<DiceController>().lockDice = false;
                    StartCoroutine(gameController.GetDices()[gameController.GetActualPlayer()].GetComponent<DiceController>().RotateDice());
                }

                
            }

            Destroy(hit.gameObject);

        }
    }



    void OnControllerColliderHit(ControllerColliderHit hit) {
        if(hit.gameObject.tag == "Wall") {
            if(!controller.isGrounded) {
                if(keyJump) {
                    verticalVelocity = jumpforce;
                    moveVector = hit.normal * speed; 
                }
            }
        }
        if(hit.gameObject.tag == "Sol") {
            if(isJumping)  {
                isJumping = false;
                count = 0;
            }
        }
    }
}

