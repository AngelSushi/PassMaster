using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Step : MonoBehaviour {
    public bool[] useVectors; // size 4 : forward , back , right , left
    public bool positive;
    public StepType type;
    public List<GameObject> playerInStep = new List<GameObject>();
    public GameObject chest;
    public GameObject shop;
    public bool skipIA;
    public Vector3 avoidPos; // position of players to avoid other player on path

    public Dictionary<ItemType,int> itemsInStep = new Dictionary<ItemType, int>();


    private void Update()
    {
        Debug.DrawLine(transform.position,transform.position + Vector3.right * 6f, Color.red); // HAut
        Debug.DrawLine(transform.position,transform.position + Vector3.forward * 6f * -1, Color.magenta);
        Debug.DrawLine(transform.position,transform.position + Vector3.right * 6f + Vector3.forward * 6f * -1, Color.cyan); 
        
        
    }

    public void AddPlayerInStep(GameObject player) {
        if (playerInStep.Count == 3) {
            Debug.Log("Step can't have more than 3 players");
            return;
        }
        
        playerInStep.Add(player);
    }
    
    public void ManagePlayerInStep(GameObject player) {

        for (int i = 0; i < playerInStep.Count; i++) {
            GameObject playerStep = playerInStep[i];

            if (player != playerStep)
                continue;

            Vector3Int playerPosition = Vector3Int.FloorToInt(player.transform.position);
            Vector3Int targetPos = Vector3Int.FloorToInt(i == 1 ? transform.GetChild(3).position : transform.GetChild(2).position);

            StartCoroutine(MoveToStackPosition(player, playerPosition, targetPos));
        }
        
    }

    private IEnumerator MoveToStackPosition(GameObject player, Vector3Int playerPosition, Vector3Int targetPosition) {
        while (playerPosition != targetPosition) {
            yield return new WaitForEndOfFrame();
            Vector3.MoveTowards(playerPosition,targetPosition,player.GetComponent<NavMeshAgent>().speed);
            playerPosition = Vector3Int.FloorToInt(player.transform.position);
        }

        yield return null;
    }
    
}
