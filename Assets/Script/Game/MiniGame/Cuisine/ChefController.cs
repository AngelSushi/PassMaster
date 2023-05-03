using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
public class ChefController : MonoBehaviour {

    public float speed;
    public Rigidbody rb;
    
    public GameObject actualIngredient;
<<<<<<< HEAD
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
=======
    public GameObject actualPlate;
    
    private Vector2 _movement;
    [HideInInspector] public Vector3 move;
    public bool isMoving;
    public bool canMoove;
    private Quaternion _rotation;

    private CookController _cookController;
    private Animator _animator;


    private Box _currentCollisionBox;

    public Transform ingredientSpawn;
    
    void Start() {
        _cookController = (CookController)CookController.instance;
        _animator = GetComponent<Animator>();
        
        _cookController.inputs.FindAction("Cuisine/Movement").started += OnMove;
        _cookController.inputs.FindAction("Cuisine/Movement").performed += OnMove;
        _cookController.inputs.FindAction("Cuisine/Movement").canceled += OnMove;

        _cookController.inputs.FindAction("Cuisine/Interact").started += OnInteract;
        
    }
    
    void Update() {
        _animator.SetBool("IsMooving",isMoving);

        if(isMoving && canMoove)
            Movement();
        else 
            transform.rotation = _rotation;
    }

      private void Movement() {
        float moveX = _movement.x * speed * -1;
        float moveZ = _movement.y * speed * -1;
        
        move = new Vector3(moveX,rb.velocity.y,moveZ);
        
        rb.MovePosition(transform.position + move * Time.deltaTime);

        _rotation = Quaternion.LookRotation(move, Vector3.up);
        transform.rotation = _rotation;
>>>>>>> main
    }
    

    public void OnMove(InputAction.CallbackContext e) {
        _movement = e.ReadValue<Vector2>();
        
        if(e.started) isMoving = true;
        if(e.canceled)  isMoving = false;    
    }
    

<<<<<<< HEAD
    public void OnInteract(InputAction.CallbackContext e) {
        if(e.started) {
            if(actualBox != null) {
                Box box = (Box)actualBox.GetComponent<Box>();

                Debug.Log("box: " + box);

                box.Interact(this);
            }   
        }     
    }

=======

    public void OnInteract(InputAction.CallbackContext e) {
        if (e.started) {
            foreach (RaycastHit hit in Physics.RaycastAll(transform.position, transform.forward)) {
                if (hit.collider != null) {
                    if (hit.collider.gameObject.TryGetComponent<Box>(out Box box)) {
                        box.BoxInteract(actualIngredient != null ? actualIngredient : actualPlate != null ? actualPlate : null,this);
                        break;
                    }
                }
            }
        }
    }
>>>>>>> main
}
