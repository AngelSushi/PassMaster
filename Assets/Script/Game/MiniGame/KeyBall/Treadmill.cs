using System;
using System.Collections;
using System.Collections.Generic;
using ExcelDna.Integration;
using UnityEngine;

public class Treadmill : MonoBehaviour {

    [SerializeField] private Vector2 offsetSpeed;
    [SerializeField] private float attractiveForce;

    private MeshRenderer _meshRenderer;
    
    private void Start() {
        _meshRenderer = GetComponent<MeshRenderer>();
        _meshRenderer.material = new Material(_meshRenderer.material);
    }

    private void Update() => _meshRenderer.material.mainTextureOffset += offsetSpeed * Time.deltaTime;
    
    private void OnTriggerStay(Collider collider) {
        if (collider.tag.Equals("Player")) {
            if (collider.gameObject.GetComponentInChildren<KB_PlayerMovement>() != null) {
                KB_PlayerMovement playerMovement = collider.gameObject.GetComponentInChildren<KB_PlayerMovement>();
                //playerMovement.ApplyForce(Vector3.right * -1 * attractiveForce * Time.deltaTime);
            }
        }
    }
}
