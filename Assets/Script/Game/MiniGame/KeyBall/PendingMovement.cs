using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PendingMovement : MonoBehaviour {
    
    public KBController controller;
    public GameController gameController;
    public float speed;
    private Rigidbody rb;

    private Vector2 input;
    
    void Start()
    {
        controller = (KBController)KBController.Instance;

        if (gameController == null)
            gameController = GameController.Instance;

        gameController.inputs.FindAction("Keyball/Movement").started += OnMove;
        gameController.inputs.FindAction("Keyball/Movement").performed += OnMove;
        gameController.inputs.FindAction("Keyball/Movement").canceled += OnMove;

        rb = GetComponent<Rigidbody>();
    }


    private void Update()
    {
       // if(input != Vector2.zero)
           // rb.velocity = new Vector3(input.y, 0, -input.x) * speed;
    }

    public void OnMove(InputAction.CallbackContext e) {
        Debug.Log("rb " + (e.ReadValue<Vector2>() * speed));
        input = e.ReadValue<Vector2>();
        if (e.started)
        {
            rb.AddForce(new Vector3(1000,0,0),ForceMode.Acceleration);
        }
    }
    
}
