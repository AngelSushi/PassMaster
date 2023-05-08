using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ingredient : MonoBehaviour {

    public IngredientData data;
    
    public bool isCut;
    public bool isCook;


   // [Tooltip("Vector 0 : Hold Rotation, Vector 1 : Box Rotation, Vector 2 : Plate Rotation")] public Vector3[] ingredientRotation = new Vector3[3];
    public Vector3 ingredientRotation;
   
    [HideInInspector] public GameObject basic;
    [HideInInspector] public GameObject modified;

    private void Start() {
        basic = transform.GetChild(0).gameObject;
        modified = transform.childCount >= 2 ? transform.GetChild(1).gameObject : null;
    }

    private void Update() { 
        Debug.DrawLine(transform.position,transform.position + Vector3.forward * 15,Color.blue);

        Vector3 forwardPosition = transform.position + Vector3.right * 20;
       // Quaternion targetRotation = Quaternion.LookRotation(forwardPosition);
        transform.LookAt(forwardPosition);
        //transform.rotation = targetRotation;
    }
}
