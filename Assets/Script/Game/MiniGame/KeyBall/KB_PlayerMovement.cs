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
    public Rigidbody rigidbody;
    public Transform cameraTransform;
    public float gravity;
    private float _groundedGravity = -.5f;
    
    /* Movement Variables */
    public float speed;
    public bool isMoving;
    [HideInInspector] public Vector3 movement;
    private Vector2 _input;
    public bool isOnBall;
    
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
        rigidbody = GetComponentInParent<Rigidbody>();

        gravity = (-2 * maxJumpHeight) / Mathf.Pow(_timeToReachHeightJump, 2);
        _initialJumpVelocity = (2 * maxJumpHeight) / _timeToReachHeightJump;
    }

    void FixedUpdate() {
        if (!controller.begin && !controller.finish) {
            if (!dead && !freeze) {
                animator.SetBool("IsMooving", isMoving);

                MovementPending();
            }
        }
    }
    
    

    #endregion

    #region Physics Functions
    private IEnumerator Slide(Vector2 startSlideInput) {
        while (startSlideInput != Vector2.zero) {
            startSlideInput = Vector2.Lerp(startSlideInput, Vector2.zero, Time.deltaTime * speed * slidingAmplifier);
          //  movement += new Vector3(startSlideInput.y, 0, startSlideInput.x);
            yield return new WaitForEndOfFrame();
        }

        movement = Vector3.zero;
        yield return null;
    }

   
    private void Movement()
    {
        
        Vector3 velocity = new Vector3(_input.y, 0, -_input.x) * speed;
        //Debug.Log("value " + (velocity * Time.fixedDeltaTime));
        
        
        
       // rigidbody.MovePosition(transform.parent.position + velocity * Time.fixedDeltaTime);
        if (_input != Vector2.zero) { 
            Quaternion rotation = Quaternion.LookRotation(new Vector3(velocity.x,0,velocity.z), Vector3.up); 
            transform.rotation = Quaternion.RotateTowards(transform.rotation, rotation, 500f * Time.deltaTime);
        }
    }

   // public void ApplyForce(Vector3 force) => characterController.Move(force);

    #endregion

    private void MovementPending() {
        float moveX = _input.y * speed;
        float moveZ = _input.x * speed * -1;

        if (moveX == 0 && moveZ == 0)
            return;

        if (isOnBall) {

         /*   if (_input.y > 0)
                rigidbody.AddForce(Vector3.right * speed * slidingAmplifier, ForceMode.VelocityChange);
            else if (_input.y < 0)
                rigidbody.AddForce(Vector3.right * speed * slidingAmplifier* -1, ForceMode.VelocityChange);
            if (_input.x > 0)
                rigidbody.AddForce(Vector3.forward * speed * slidingAmplifier * -1, ForceMode.VelocityChange);
            else if (_input.x < 0)
                rigidbody.AddForce(Vector3.forward * speed * slidingAmplifier, ForceMode.VelocityChange);
*/
         
            rigidbody.AddForce(new Vector3(moveX,0,moveZ));
            // Debug.Log("velocity " + rigidbody.velocity.magnitude);
            //rigidbody.velocity = Vector3.ClampMagnitude(rigidbody.velocity, speed);
            // Debug.Log("newVel " + rigidbody.velocity.magnitude);
        }
        else
        {
            rigidbody.velocity = new Vector3(moveX, 0, moveZ);
        }
    }

    #region Inputs Functions
    
    public void OnMove(InputAction.CallbackContext e) {
        _input = e.ReadValue<Vector2>();

        
        
      /*  float forwardInput = new Vector3(_input.y, 0, 0).magnitude * Mathf.Sign(movement.x);
        Vector3 forwardVec = forwardInput * cameraTransform.forward;

        float rightInput = new Vector3(0, 0, -_input.x).magnitude * Mathf.Sign(movement.z);
        Vector3 rightVec = rightInput * cameraTransform.right;

        Vector3 movementDirection = forwardVec + rightVec;
        
        float magnitude = Mathf.Clamp01(movementDirection.magnitude) * speed;

        
        Vector3 velocity = movementDirection * magnitude;
        rigidbody.AddForce(velocity,ForceMode.Force);
        */
        
        if (e.started)
            isMoving = true;
        if (e.canceled)
            isMoving = false;

    }

    public void OnJump(InputAction.CallbackContext e) {
        if (e.started)
            isOnBall = !isOnBall;
        
        if (canJump) 
            isJumpPressed = e.ReadValueAsButton();
    }


    #endregion

}
