using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoinsReward : MonoBehaviour {

    public bool changePos;
    public bool stepReward;
    public bool minigame;
    public int beginY;
    public bool hasFinishAnimation;
    public GameController controller;

    public void RunCoroutine() {
        if(minigame)
            transform.localPosition = new Vector3(transform.localPosition.x,beginY,transform.localPosition.z);
        else 
            transform.position = new Vector2(transform.position.x - 40,transform.position.y);
        StartCoroutine(ChangePosition());
    }

    private IEnumerator ChangePosition() {
        while(changePos) {
            if(!minigame) { // Jeu de plateau
                if(transform.position.y < 425)  
                    transform.position = new Vector2(transform.position.x,transform.position.y + 5);
            
                else {
                    changePos = false;
                    
                    if(stepReward) 
                        controller.players[controller.actualPlayer].GetComponent<UserMovement>().finishTurn = true;
                    if(!controller.players[controller.actualPlayer].GetComponent<UserMovement>().isPlayer) 
                        controller.players[controller.actualPlayer].GetComponent<UserMovement>().hasBotBuyItem = true;

                    transform.gameObject.SetActive(false);
                    yield return null;
                }
            }
            else { // Mini jeu

                if((int)transform.localPosition.y < beginY + 60) {
                    transform.gameObject.SetActive(true);
                    transform.localPosition = new Vector2(transform.localPosition.x,transform.localPosition.y + 5);
                }
                else {
                    changePos = false;
                    transform.gameObject.SetActive(false);
                    hasFinishAnimation = true;
                    yield return null;
                }
            }

            yield return new WaitForSeconds(0.02f);
        }
    }


}
