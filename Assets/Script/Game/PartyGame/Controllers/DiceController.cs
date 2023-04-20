using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DiceController : CoroutineSystem {

    public float speed;

    public Texture2D[] diceTextures;
    public Texture2D[] doubleDiceTextures;
    public Texture2D[] tripleDiceTextures;
    public Texture2D[] reverseDiceTextures;

    public int index;

    public bool lockDice,lastLockDice = true;

    private MeshRenderer _meshRenderer;

    void Start() => _meshRenderer = GetComponent<MeshRenderer>();

    void Update() {
        
        if(!lockDice && lastLockDice) {
            StartCoroutine(RotateDice());
        }
        
        Debug.DrawLine(transform.position,transform.position + Vector3.forward * 100,Color.black);

        lastLockDice = lockDice;
          
    }

    public IEnumerator RotateDice() {
       while(!lockDice) {
            UserMovement movement = GameController.Instance.players[GameController.Instance.actualPlayer].GetComponent<UserMovement>();

            _meshRenderer.material.mainTexture =
                !movement.doubleDice && !movement.tripleDice && !movement.reverseDice ? diceTextures[index] :
                movement.doubleDice && !movement.tripleDice && !movement.reverseDice ? doubleDiceTextures[index] :
                !movement.doubleDice && movement.tripleDice && !movement.reverseDice ? tripleDiceTextures[index] :
                !movement.doubleDice && !movement.tripleDice && movement.reverseDice ? reverseDiceTextures[index] :
                diceTextures[index];

            
            transform.LookAt(GameController.Instance.mainCamera.transform.position);
            transform.eulerAngles = new Vector3(transform.eulerAngles.x, transform.eulerAngles.y, 180);
            index++;

            if(index == 6) index = 0;

            yield return new WaitForSeconds(speed);
       }

    }
}
