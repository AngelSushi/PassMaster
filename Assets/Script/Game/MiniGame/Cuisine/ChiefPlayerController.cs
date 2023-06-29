using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

public class ChiefPlayerController : ChiefController {

    public float speed; // 15 last speed
    public Rigidbody rb;

    private Vector2 _movement;
    [HideInInspector] public Vector3 move;
    private Quaternion _rotation;

    private CookController _cookController;
    private Animator _animator;

    
    void Start() {
        _cookController = (CookController)CookController.instance;
        _animator = GetComponent<Animator>();
        
        _cookController.inputs.FindAction("Cuisine/Movement").started += OnMove;
        _cookController.inputs.FindAction("Cuisine/Movement").performed += OnMove;
        _cookController.inputs.FindAction("Cuisine/Movement").canceled += OnMove;

        _cookController.inputs.FindAction("Cuisine/Interact").started += OnInteract;
        
    }
    
    void Update() {
        //_animator.SetBool("IsMooving",isMoving);

        if(isMoving && canMoove)
            Movement();
        else 
            transform.rotation = _rotation;
        
        Debug.DrawLine(transform.position,transform.position + transform.forward * 10,Color.magenta);
    }

      private void Movement() {
        float moveX = _movement.x * speed * -1;
        float moveZ = _movement.y * speed * -1;
        
        move = new Vector3(moveX,rb.velocity.y,moveZ);
        
        rb.MovePosition(transform.position + move * Time.deltaTime);

        _rotation = Quaternion.LookRotation(move, Vector3.up);
        transform.rotation = _rotation;
    }
    

    public void OnMove(InputAction.CallbackContext e) {
        _movement = e.ReadValue<Vector2>();
        
        if(e.started) isMoving = true;
        if(e.canceled)  isMoving = false;    
    }


    public void OnInteract(InputAction.CallbackContext e) {
        if (e.started) {
            foreach (RaycastHit hit in Physics.RaycastAll(transform.position, transform.forward,5)) {
                if (hit.collider != null) {
                    if (hit.collider.gameObject.TryGetComponent<Box>(out Box box)) {
                        box.BoxInteract(actualIngredient != null ? actualIngredient : actualPlate != null ? actualPlate : null,this);
                        break;
                    }
                }
            }
        }
    }
}
