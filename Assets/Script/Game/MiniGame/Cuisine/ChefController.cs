using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
public class ChefController : MonoBehaviour {

    public float speed;
    public Rigidbody rb;
    
    public GameObject actualIngredient;
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
    }
    

    public void OnMove(InputAction.CallbackContext e) {
        _movement = e.ReadValue<Vector2>();
        
        if(e.started) isMoving = true;
        if(e.canceled)  isMoving = false;    
    }


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
}
