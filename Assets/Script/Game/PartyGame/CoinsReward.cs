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
            if(!minigame) { // Board game
                if(transform.position.y < 425)  
                    transform.position = new Vector2(transform.position.x,transform.position.y + 5);
            
                else {
                    changePos = false;
                    
                    if(stepReward) {
                        controller.players[controller.actualPlayer].GetComponent<UserMovement>().finishTurn = true;
<<<<<<< HEAD
<<<<<<< Updated upstream
<<<<<<< Updated upstream
                        if(!controller.dialog.isInDialog) {
=======
                        if(!controller.dialog.isInDialog) 
>>>>>>> main
                            controller.EndUserTurn();
                        
                    }
                    else { // reward is in shop
                        controller.shopController.hasFinishBuy = true;
                    }
<<<<<<< HEAD
=======
<<<<<<< Updated upstream
=======
>>>>>>> Stashed changes
=======
                        if(!controller.dialog.isInDialog && !GameController.Instance.shopController.mooveToShop && !GameController.Instance.chestController.goToChest) {
                            controller.EndUserTurn();
                        }
                    }
>>>>>>> Stashed changes
<<<<<<< Updated upstream
>>>>>>> Stashed changes
=======
>>>>>>> Stashed changes
                    if(!controller.players[controller.actualPlayer].GetComponent<UserMovement>().isPlayer) 
                        controller.players[controller.actualPlayer].GetComponent<UserMovement>().hasBotBuyItem = true;
=======
>>>>>>> main

                    transform.gameObject.SetActive(false);
                    yield return null;
                }
            }
            else { // leaderboard mini game

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
