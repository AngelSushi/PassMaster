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
    private Vector3 _velocity,_lastFrameVelocity;
    private Vector3 _positionPerFrame;
    
    /* Sliding Variables */
    private Vector2 _lastInput;
    public float slidingAmplifier;
    
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

    private float calculatedDrag;
    #endregion

    #region Unity's Functions

    void Start() {
        controller = (KBController)KBController.instance;

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
        
        calculatedDrag = Mathf.Clamp01(1f - (2 * Time.fixedDeltaTime));
    }

    void Update() {
        if (!controller.begin && !controller.finish) {
            if (!dead && !freeze) {
                animator.SetBool("IsMooving", isMoving);

                HandleGravity();
                Movement();
                HandleJump();
            }
        }
    }

    private void FixedUpdate()
    {
      /*  float drag = Mathf.Clamp01(1.0f - (2 * Time.fixedDeltaTime)) ; // 2 is a constant can be replace by variable

        Vector3 velocityPerFrame = _lastFrameVelocity + (Physics.gravity * Time.fixedDeltaTime);
        velocityPerFrame *= drag;

        _positionPerFrame += velocityPerFrame * Time.fixedDeltaTime;
        Debug.Log("_position " + _positionPerFrame);


        _lastFrameVelocity = _velocity;*/

     // Movement();
     
     // _velocity += Physics.gravity * Time.fixedDeltaTime;
     // _velocity *= calculatedDrag;

     // Debug.Log("vel " + _velocity);
      //characterController.Move(_velocity * Time.fixedDeltaTime);
    }

    #endregion

    #region Physics Functions
    private IEnumerator Slide(Vector2 startSlideInput) {
        
        Debug.Log("startWith " + startSlideInput) ;
        
        while (startSlideInput.magnitude > 0.1f) {
            startSlideInput = Vector2.Lerp(startSlideInput, Vector2.zero, Time.deltaTime * (speed /2) * slidingAmplifier);
            movement += new Vector3(startSlideInput.y, 0, startSlideInput.x);
            
            Debug.Log("SlideInput " + startSlideInput);
            yield return new WaitForEndOfFrame();
        }

        movement = Vector3.zero;
        Debug.Log("end");
        yield return null;
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
        Vector3 forwardVec = movement.x * cameraTransform.forward;
        Vector3 rightVec = movement.z * cameraTransform.right;
        
        Debug.Log("forward " + forwardVec + " movement " + movement.x + " input " + _input.y);
        Debug.Log("rightVec " + rightVec);

        Vector3 movementDirection = forwardVec + rightVec;
        float magnitude = Mathf.Clamp01(movementDirection.magnitude) * speed;

        
        _velocity = movementDirection * magnitude;
        _velocity.y = movement.y;
        
        Debug.Log("velocity " + _velocity);
        
        characterController.Move(_velocity * Time.deltaTime);

        
        if (_input != Vector2.zero) { 
            Quaternion rotation = Quaternion.LookRotation(new Vector3(_velocity.x,0,_velocity.z), Vector3.up); 
           // transform.rotation = Quaternion.RotateTowards(transform.rotation, rotation, 500f * Time.deltaTime);
        }
    }

    public void ApplyForce(Vector3 force) => characterController.Move(force);

    #endregion
    
    #region Inputs Functions
    
    public void OnMove(InputAction.CallbackContext e) {
        _input = e.ReadValue<Vector2>();
        
        
        Debug.Log("input " + _input);
        //if(!isOnBall) 
         //   movement += new Vector3(_input.y, movement.y, _input.x);
       // else if(_input != Vector2.zero)
            movement += new Vector3(_input.y, movement.y, _input.x);

        if (e.started)
            isMoving = true;
        if (e.canceled)
            isMoving = false;

        
        //if (/*isOnBall &&*/ _input.normalized != _lastInput) 
          // StartCoroutine(Slide(_lastInput));
        
        _lastInput = _input.normalized;
    }

    public void OnJump(InputAction.CallbackContext e) {
        if (canJump) 
            isJumpPressed = e.ReadValueAsButton();
    }


    #endregion

}
