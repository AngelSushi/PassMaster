using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class ArcheryCursor : MonoBehaviour {

    void Start() {
        //Cursor.visible = false;
    }

    void Update() {
        Vector3 mouse = Mouse.current.position.ReadValue();
        transform.position = mouse;
    }
}
