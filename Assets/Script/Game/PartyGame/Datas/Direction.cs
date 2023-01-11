using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Direction : MonoBehaviour {

    
    public bool[] directions; // size : 3 ; left ; front ; right
    public StepType type;
    public GameObject[] directionsStep; // size : 3 ; left ; front ; right 
    public bool[] reverseCountDirections; // size 3 ; left ; front ; right

    public NodeAI node;

    private void Start() {
        node.SetupConnections(this.gameObject);
    }
}
