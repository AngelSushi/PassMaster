using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputTestScript : MonoBehaviour {

    public InputActionAsset asset;
    private Vector2 move;
    void Start() {
        transform.gameObject.GetComponent<PlayerInput>().actions = asset;
    }

    void Update() {
        Vector3 movement = new Vector3(move.x,0,move.y);

        transform.Translate(movement * 10 * Time.deltaTime);
    }

    public void OnMove(InputAction.CallbackContext e) {
        move = e.ReadValue<Vector2>();
        Debug.Log("enter");
    }

    public void OnInteract(InputAction.CallbackContext e) {
        Debug.Log("interact");
    }
}
