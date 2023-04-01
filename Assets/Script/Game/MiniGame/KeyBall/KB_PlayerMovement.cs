using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;
using UnityEngine.Serialization;
using UnityEngine.TextCore.Text;

public class KB_PlayerMovement : MonoBehaviour {
    #region Variables
    
    
    /* Controller Variables */
    public KBController controller;
    public GameController gameController;
    public Animator animator;
    public CharacterController characterController;
    public Transform cameraTransform;
    public float gravity;
    private float _groundedGravity = -.5f;
    
    /* Movement Variables */
    public float speed;
    public bool isMoving;
    [HideInInspector] public Vector3 movement;
    private Vector2 _input;
    public bool isOnBall;
    
    /* State Variables */
    public bool dead;
    public bool freeze;

    /* Jump Variables */
    public bool canJump;
    [Tooltip("True during all the time of the jump")]public bool isJumping;
    [Tooltip("True when the play click on the jump button")] public bool isJumpPressed;
    public float maxJumpHeight;
    public float maxJumpTime;
    [Tooltip("The multiplicator of speed when character is falling")]public float fallMultiplier;
    private float _initialJumpVelocity;

    private float _timeToReachHeightJump {
        get { return maxJumpTime / 2; }
    }
    
    #endregion

    #region Unity's Functions

    void Start() {
        controller = (KBController)KBController.Instance;

        if(gameController == null)
            gameController = GameController.Instance;

        gameController.inputs.FindAction("Keyball/Movement").started += OnMove;
        gameController.inputs.FindAction("Keyball/Movement").performed += OnMove;
        gameController.inputs.FindAction("Keyball/Movement").canceled += OnMove;
        gameController.inputs.FindAction("Keyball/Jump").started += OnJump;
        gameController.inputs.FindAction("Keyball/Jump").canceled += OnJump;

        animator = GetComponent<Animator>();
        characterController = GetComponentInParent<CharacterController>();

        gravity = (-2 * maxJumpHeight) / Mathf.Pow(_timeToReachHeightJump, 2);
        _initialJumpVelocity = (2 * maxJumpHeight) / _timeToReachHeightJump;
    }
    

    void Update() {
        if (!controller.begin && !controller.finish) {
            if (!dead && !freeze) {
                animator.SetBool("IsMooving", isMoving);
              //  animator.SetBool("IsJumping", isJumping);
                
                HandleGravity();
                Movement();
                
                Debug.Log("grounded " + characterController.isGrounded);
                HandleJump();
            }
        }

    }

    void HandleGravity() {
        bool isFalling = movement.y <= 0.0f || !isJumpPressed;
        
        if (characterController.isGrounded && !isJumping)
            movement.y = _groundedGravity;
        else if (isFalling) {
            float previousYVelocity = movement.y;
            float newYVelocity = movement.y + (gravity * fallMultiplier * Time.deltaTime);
            float nextYVelocity = (previousYVelocity + newYVelocity) * .5f;
            movement.y = nextYVelocity;
        }
        else {
            float previousYVelocity = movement.y;
            float newYVelocity = movement.y + (gravity * Time.deltaTime);
            float nextYVelocity = (previousYVelocity + newYVelocity) * .5f;
            movement.y = nextYVelocity;
        }


        if (isOnBall) {
            float previousXVelocity = movement.x;
            float newXVelocity = movement.x + (gravity * Time.deltaTime);
            float nextYVelocity = (previousXVelocity + newXVelocity);
            
            if(nextYVelocity >= 0) 
                movement.x = nextYVelocity;

            float previousZVelocity = movement.z;
            float newZVelocity = movement.z + (gravity * Time.deltaTime);
            float nextZVelocity = (previousZVelocity + newZVelocity);
            
            if(nextZVelocity >= 0)
                movement.z = nextZVelocity;
        }
    }

    void HandleJump() {
        if (!isJumping && characterController.isGrounded && isJumpPressed) {
            isJumping = true;
            float previousYVelocity = movement.y;
            float newYVelocity = movement.y + _initialJumpVelocity;
            float nextYVelocity = (previousYVelocity + newYVelocity) * .5f; // Velocity Verlet Integration to avoid difference with FPS
            movement.y = nextYVelocity;
        }
        else if (!isJumpPressed && isJumping && characterController.isGrounded) {
            isJumping = false;
        }
    }

    private void Movement() {
        float forwardInput = new Vector3(movement.x, 0, 0).magnitude * Mathf.Sign(movement.x);
        Vector3 forwardVec = forwardInput * cameraTransform.forward;

        float rightInput = new Vector3(0, 0, -movement.z).magnitude * Mathf.Sign(movement.z);
        Vector3 rightVec = rightInput * cameraTransform.right;

        Vector3 movementDirection = forwardVec + rightVec;
        float magnitude = Mathf.Clamp01(movementDirection.magnitude) * speed;

        Vector3 velocity = movementDirection * magnitude;
        velocity.y = movement.y;
  
        characterController.Move(velocity * Time.deltaTime);

        if (_input != Vector2.zero) {
            Quaternion rotation = Quaternion.LookRotation(new Vector3(velocity.x,0,velocity.z), Vector3.up);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, rotation, 500f * Time.deltaTime);
        }
    }

    public void OnMove(InputAction.CallbackContext e) {
        _input = e.ReadValue<Vector2>();
        
        if(_input != Vector2.zero)
            movement = new Vector3(_input.y, movement.y, _input.x);

        if(e.started) isMoving = true;
        if(e.canceled)  isMoving = false;
        
    }

    public void OnJump(InputAction.CallbackContext e) {
        if (canJump) 
            isJumpPressed = e.ReadValueAsButton();
    }

    public void ApplyForce(Vector3 force) => characterController.Move(force * Time.deltaTime);
    

    #endregion

}
