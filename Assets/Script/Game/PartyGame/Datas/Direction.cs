using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Direction : MonoBehaviour {

    [System.Serializable]
    public class DirectionInfos {
        [Tooltip("The first step of mustContains")]public GameObject directionTarget;
        [Tooltip("Easiest path to go to the end when the ai has earned all keys")]public GameObject toEnd;
        public List<GameObject> mustContains;
    }
    
    public bool[] directions; // size : 3 ; left ; front ; right
    public StepType type;
    public GameObject[] directionsStep; // size : 3 ; left ; front ; right 
    [Tooltip("Use to know if we need to bypass the reverse dice or not when the player go on the direction")] public bool[] bypassReverse = new bool[3]; // size 3 ; left ; front ; right

    [Tooltip("Use to rotate and to position correctly the arrow when the player is using reverse dice. The first vector is the position and the second is the rotation")]public Vector3[] reverseTransform = new Vector3[2];

    [Tooltip("Use to know which direction ai should take. The first is when the player take the normal or the double dice. The second is when the player take the reverse dice. ")]public DirectionInfos[] directionInfos;

    private Vector3[] _lastTransform = new Vector3[2];
    private GameController _controller;

    private void Start() => _controller = GameController.Instance;
    
    private void Update() {

        if (_controller.players[_controller.actualPlayer].GetComponent<UserMovement>().reverseDice) {
            if (reverseTransform[1] != transform.eulerAngles) {
                
                _lastTransform[0] = transform.localPosition;
                _lastTransform[1] = transform.eulerAngles;
                transform.eulerAngles = reverseTransform[1];
                transform.localPosition = reverseTransform[0];
            }
        }
        else {
            if (transform.eulerAngles == reverseTransform[1]) {
                transform.localPosition = _lastTransform[0];
                transform.eulerAngles = _lastTransform[1];
            }
        }
        
    }
}
