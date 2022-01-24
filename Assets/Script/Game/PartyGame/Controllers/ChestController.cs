using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.AI;
public class ChestController : CoroutineSystem {

    public GameController gameController;
    public int chestCoinsPrice;
    private GameObject actualPlayer;
    private GameObject obj;
    private Vector3 chestPosition;
    private bool goToChest;
    private NavMeshPath chestPath;

    void Start() {
        gameController.dialog.OnDialogEnd += EventOnDialogEnd;
    }

    public override void Update() {
        if(goToChest) {
            actualPlayer.GetComponent<NavMeshAgent>().SetDestination(chestPosition);

            RunDelayed(0.65f,() => {
                if(!obj.GetComponent<Animation>().isPlaying) {
                    obj.GetComponent<Animation>().Play();
                    bool hasFullSecretCode = actualPlayer.GetComponent<UserInventory>().cards == gameController.secretCode.Length;
                    
                    if(hasFullSecretCode)   
                        actualPlayer.GetComponent<UserAudio>().CardGain();
                    else
                        actualPlayer.GetComponent<UserAudio>().FindSecretCode();

                    goToChest = false;

                    RunDelayed(0.2f,() => { // Faire la vérification avant si on a pas déjà assez de cartes
                        int secretNumber = actualPlayer.GetComponent<UserInventory>().AddCards();

                        Dialog chestDialog = hasFullSecretCode ? gameController.dialog.GetDialogByName("FindAllSecretCode") : gameController.dialog.GetDialogByName("FindNewCode");
                        string content = chestDialog.Content[0];

                        if(!hasFullSecretCode) {
                            content.Replace("%n","" + secretNumber);
                            content.Replace("%b","" + (gameController.secretCode.Length - actualPlayer.GetComponent<UserInventory>().cards));
                        }

                        gameController.dialog.isInDialog = true;
                        gameController.dialog.currentDialog = chestDialog;
                        StartCoroutine(gameController.dialog.ShowText(content,chestDialog.Content.Length));
                        StartCoroutine(actualPlayer.GetComponent<UserMovement>().WaitMalus(true,30));
                    });
                }
            });

            
        }
    }


    private void EventOnDialogEnd(object sender,DialogController.OnDialogEndArgs e) {
        if(e.dialog == null)
            return;

        if(e.dialog.id == 10 && e.answerIndex == 0) {
            if(e.actualPlayer.GetComponent<UserInventory>().coins < chestCoinsPrice) {
                RunDelayed(0.05f,() => {
                    Dialog moneyDialog = gameController.dialog.GetDialogByName("NotEnoughMoneyChest");
                    gameController.dialog.isInDialog = true;
                    gameController.dialog.currentDialog = moneyDialog;
                    StartCoroutine(gameController.dialog.ShowText(moneyDialog.Content[0],moneyDialog.Content.Length));
                });

                return;
            }

            chestPosition = e.position - GetDirection(e.actualPlayer.GetComponent<UserMovement>().actualStep,e.actualPlayer.GetComponent<UserMovement>().actualStep.GetComponent<Step>());
            actualPlayer = e.actualPlayer;
            obj = e.obj;
            chestPath = new NavMeshPath();
            goToChest = true;
        } 

        if(e.dialog.id == 11) 
            gameController.EndUserTurn();
    }

    private Vector3 GetDirection(GameObject obj,Step step) {
        if(step.useVectors.Length > 0) {
            bool forward = step.useVectors[0];
            bool back = step.useVectors[1];
            bool right = step.useVectors[2];
            bool left = step.useVectors[3];

            if(forward) {
                if(right && !left) 
                    return obj.transform.forward * 3f + obj.transform.right * 3f;
                else if(!right && left) 
                    return obj.transform.forward * 3f + obj.transform.right * -1 * 3f;
                else 
                    return obj.transform.forward * 3f;
            }
            else if(back) {
                if(right && !left) 
                    return obj.transform.forward * -1 * 3f + obj.transform.right * 3f;
                else if(!right && left) 
                    return obj.transform.forward * -1 * 3f + obj.transform.right * -1 * 3f;
                else 
                    return obj.transform.forward * -1 * 3f;
            }
            else if(right) 
                return obj.transform.right * 3f;
            else if(left)
                return obj.transform.right * -1 * 3f;

        }

        return Vector3.zero;
        
    }
}
