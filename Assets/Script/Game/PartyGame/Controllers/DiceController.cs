using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DiceController : CoroutineSystem {

    public float speed;

    private int[] rotateX = {0,0,90,0,0,0};
    private int[] rotateY = {-12,167,-23,-12,-102,-280};
    private int[] rotateZ = {-85,175,80,-180,-180,-180};

    // 1 0 ; -12.334 ; -85
    // 2 0 ; 167.483 ; 175.084
    // 3 90 ; -23 ; 80
    // 4 0 ; -12.334 ; -180
    // 5 0 ; -101.74 ; -180
    // 6 0 ; -280 ; -180

    public int index;
    private int seconds = 0;
    private int wait = 25;

    public bool lockDice,lastLockDice = true;

    void Update() {
        
        if(!lockDice && lastLockDice) {
            StartCoroutine(RotateDice());
        }

        lastLockDice = lockDice;
          
    }

    public IEnumerator RotateDice() {
       while(!lockDice) {
            transform.rotation = Quaternion.Euler(0, 0, 0); // RESET ROTATION
            transform.Rotate(rotateX[index],rotateY[index],rotateZ[index]);

            index++;

            if(index == 6) index = 0;

            yield return new WaitForSeconds(speed);
       }

    }

 /*   private async void OnCollisionEnter(Collision hit) {
        if(hit.gameObject.tag == "User") {
            UserMovement movement = null;

            if(hit.gameObject.transform.parent.gameObject.tag == "Shell") 
                movement = hit.gameObject.transform.parent.gameObject.GetComponent<UserMovement>();
            else
                movement = hit.gameObject.GetComponent<UserMovement>();

            if(movement.diceResult == 0 || movement.diceResult == -1) {
                movement.diceResult = hit.gameObject.GetComponent<DiceController>().index;

                if(movement.diceResult == 0) 
                    movement.diceResult = 6;
                if(movement.doubleDice) 
                    movement.diceResult *= 2;
                if(movement.tripleDice)
                    movement.diceResult *= 3;           
                
                movement.agent.enabled = true;
                if(movement.isPlayer) movement.diceResult = 63; 
                movement.beginResult = movement.diceResult; 
                movement.stepPaths = new GameObject[movement.beginResult]; 
                movement.hasCollideDice = true;  
                
                movement.actualColor = movement.tripleDice ? new Color(1f,0.74f,0f) : movement.doubleDice ? new Color(0.32f,0.74f,0.08f,1.0f) : movement.reverseDice ? new Color(0.41f,0.13f,0.78f,1.0f) : new Color(0f,0.35f,1f,1.0f);
                movement.ui.RefreshDiceResult(movement.diceResult, movement.actualColor,true);

                GameObject hitObj = hit.gameObject;
                hitObj.GetComponent<MeshRenderer>().enabled = false;

                Debug.Log("beginDiceResult: " + movement.diceResult + " name: " + transform.gameObject);

                movement.ChooseNextStep(GameController.Instance.firstStep.GetComponent<Step>().type);

                RunDelayed(0.1f,() => {  transform.gameObject.SetActive(false); });
            }

        }
    }
    */
}
