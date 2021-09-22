using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Direction : MonoBehaviour {
    
    public bool left;
    public bool front;
    public bool right;

    public StepType type;
    public GameObject nextStepLeft;
    public GameObject nextStepFront;
    public GameObject nextStepRight;
    public bool reverseCount;
}
