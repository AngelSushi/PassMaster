using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Direction : MonoBehaviour {

    [System.Serializable]
    public class DirectionInfos {
        public GameObject directionTarget;
        public List<GameObject> mustContains;
    }
    
    public bool[] directions; // size : 3 ; left ; front ; right
    public StepType type;
    public GameObject[] directionsStep; // size : 3 ; left ; front ; right 
    public bool[] bypassReverse = new bool[3]; // size 3 ; left ; front ; right

    public DirectionInfos[] directionInfos;

}
