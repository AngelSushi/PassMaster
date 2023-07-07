using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ingredient : MonoBehaviour {

    [SerializeField] private IngredientData data;

    public IngredientData Data
    {
        get => data;
        private set => data = value;
    }
    
    [SerializeField] private bool isCut;

    public bool IsCut
    {
        get => isCut;
        set => isCut = value;
    }
    
    [SerializeField] private bool isCook;

    public bool IsCook
    {
        get => isCook;
        set => isCook = value;
    }

    [SerializeField] private bool isBurn;

    public bool IsBurn
    {
        get => isBurn;
        set => isBurn = value;
    }

    [SerializeField] private Vector3 ingredientRotation;

    public Vector3 IngredientRotation
    {
        get => ingredientRotation;
        private set => ingredientRotation = value;
    }
    
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
