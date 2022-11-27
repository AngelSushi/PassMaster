using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Direction : MonoBehaviour {

    [Serializable]
    public class AIPath {
        public GameObject path;
        public GameObject end;
    }
    
    public bool[] directions; // size : 3 ; left ; front ; right
    public StepType type;
    public GameObject[] directionsStep; // size : 3 ; left ; front ; right 
    public bool[] reverseCountDirections; // size 3 ; left ; front ; right

    public List<AIPath> aiPaths;
    public List<AIPath> reversePaths;
}
