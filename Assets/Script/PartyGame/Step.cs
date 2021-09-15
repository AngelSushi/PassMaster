using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Step : MonoBehaviour {
    public bool xAxis;
    public bool zAxis;
    public bool positive;
    public List<GameObject> playerInStep = new List<GameObject>();
    public GameObject stack;
    public GameObject chest;
    public GameObject shop;

    public Vector3 camPosition;
    public Quaternion camRotation;
}
