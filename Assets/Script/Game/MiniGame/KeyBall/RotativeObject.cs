using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[SelectionBase]
public class RotativeObject : MonoBehaviour {
    private KBController _controller;
    [SerializeField] private bool reverse;
    [SerializeField] private Vector3 value;
    
    void Start() => _controller = (KBController) KBController.Instance;
    

    void Update() {
        if(!_controller.begin)
        {
            transform.Rotate(reverse ? value * -1 * Time.deltaTime : value * Time.deltaTime);
        }
    }


}
