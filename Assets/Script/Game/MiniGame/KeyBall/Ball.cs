using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ball : MonoBehaviour {

    public SphereCollider _collider;
    public bool showCollision;

    private CharacterController _ballController;

    void Start() => _ballController = GetComponent<CharacterController>();
    
    void Update() {
        Vector3 position = transform.position;
        position.y += _collider.radius / 3;
        
        if (Physics.CheckSphere(position,3,LayerMask.GetMask("Player"))) {
            Collider playerCollider = Physics.OverlapSphere(position, 3, LayerMask.GetMask("Player"))[0];

            KB_PlayerMovement playerMovement = playerCollider.GetComponentInChildren<KB_PlayerMovement>();
            
            if (playerMovement.isOnBall)
                return;

            playerCollider.transform.parent = transform;
            
            playerMovement.isOnBall = true;
            playerMovement.characterController = _ballController;
            playerMovement.transform.parent.localPosition = new Vector3(1f, -4.8f, 63.9f);
        }
    }

    private void OnDrawGizmos() {
        if (_collider == null || !showCollision)
            return;
        
        Gizmos.color = Color.yellow;
        Vector3 position = transform.position;
        position.y += _collider.radius / 3;
        Gizmos.DrawSphere(position,3);
    }
}
