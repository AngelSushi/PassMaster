using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Step : MonoBehaviour {
    public bool[] useVectors; // size 4 : forward , back , right , left
    public bool positive;
    public StepType type;
    public List<GameObject> playerInStep = new List<GameObject>();
    public GameObject stack;
    public GameObject chest;
    public GameObject shop;

    public Vector3 camPosition;
    public Quaternion camRotation;

}
