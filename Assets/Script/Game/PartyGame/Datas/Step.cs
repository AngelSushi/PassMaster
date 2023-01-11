using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Serialization;

[SelectionBase]
public class Step : MonoBehaviour {
    public bool[] useVectors; // size 4 : forward , back , right , left
    public bool positive;
    public StepType type;
    public List<GameObject> playerInStep = new List<GameObject>();
    public GameObject chest;
    public GameObject shop;
    public Vector3 avoidPos; // position of players to avoid other player on path

    public NodeAI node;
    
    private bool mooveToStack;
    private Vector3 targetPosition;
    private GameObject targetPlayer;

    private void Start() {
        node.SetupConnections(gameObject);
    }
    
    private void Update() {
        if (mooveToStack) {
            targetPlayer.transform.position = Vector3.MoveTowards(targetPlayer.transform.position,targetPosition,30 * Time.deltaTime);

            if (Vector3Int.FloorToInt(targetPlayer.transform.position) == Vector3Int.FloorToInt(targetPosition))
                mooveToStack = false;
        }
    }

    public void AddPlayerInStep(GameObject player) {
        if (playerInStep.Count == 3 ) {
            Debug.Log("Step can't have more than 3 players");
            return;
        }

        if (!playerInStep.Contains(player)) {
            playerInStep.Add(player);

            if (!mooveToStack) {
                ManagePlayerInStep(player);
                mooveToStack = true;
            }
        }
    }
    
    public void ManagePlayerInStep(GameObject player) {
        targetPlayer = player;
        player.GetComponent<NavMeshAgent>().enabled = false;
        
        for (int i = 0; i < playerInStep.Count; i++) {
            GameObject playerStep = playerInStep[i];

            if (player != playerStep)
                continue;

            targetPosition = i == 1 ? transform.GetChild(3).position : transform.GetChild(2).position;
            targetPosition.y = player.transform.position.y;
        }
        
    }


}
