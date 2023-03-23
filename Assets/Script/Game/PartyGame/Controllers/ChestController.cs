﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.AI;

public class ChestController : CoroutineSystem {

    public int chestCoinsPrice;
    private GameObject actualPlayer;
    private GameObject obj;
    private Vector3 chestPosition;
    public bool goToChest;
    [HideInInspector]
    public bool returnToStep;
    private NavMeshPath chestPath;
    public AnimationClip[] chestAnimations;
    private bool hasPlayedAnim;

    void Start() => GameController.Instance.dialog.OnDialogEnd += EventOnDialogEnd;
    
    public void EndChestAnim() {
        StartCoroutine(actualPlayer.GetComponent<UserMovement>().WaitMalus(chestCoinsPrice));

        if (actualPlayer.GetComponent<UserMovement>().isPlayer && !GameController.Instance.dialog.isInDialog) {
            bool hasFullSecretCode = actualPlayer.GetComponent<UserInventory>().cards == GameController.Instance.secretCode.Length;
                
            if(!AudioController.Instance.ambiantSource.isPlaying) {
                if(!hasFullSecretCode)   
                    actualPlayer.GetComponent<UserAudio>().CardGain();
                else
                    actualPlayer.GetComponent<UserAudio>().FindSecretCode();
            }
            
            int secretNumber = actualPlayer.GetComponent<UserInventory>().AddCards();

            Dialog chestDialog = hasFullSecretCode ? GameController.Instance.dialog.GetDialogByName("FindAllSecretCode") : GameController.Instance.dialog.GetDialogByName("FindNewCode");
            string content = chestDialog.Content[0];

            if(!hasFullSecretCode) {
                content = content.Replace("%n","" + secretNumber);
                content = content.Replace("%b","" + (GameController.Instance.secretCode.Length - actualPlayer.GetComponent<UserInventory>().cards));
            }

            GameController.Instance.dialog.isInDialog.value = true;
            GameController.Instance.dialog.currentDialog = chestDialog;
            StartCoroutine(GameController.Instance.dialog.ShowText(content,chestDialog.Content.Length));
        }
    }

    public override void Update() {
        if(goToChest) {
            actualPlayer.GetComponent<NavMeshAgent>().SetDestination(chestPosition);

            if (actualPlayer.GetComponent<NavMeshAgent>().remainingDistance > 0.5f) {
                Quaternion targetRotation = Quaternion.LookRotation(chestPosition - actualPlayer.transform.position);
                actualPlayer.transform.rotation = Quaternion.Slerp(actualPlayer.transform.rotation, targetRotation, 1.5f * Time.deltaTime);   
            }
        }
        else if(returnToStep) {
            
            actualPlayer.GetComponent<NavMeshAgent>().CalculatePath(actualPlayer.GetComponent<UserMovement>().actualStep.transform.position,chestPath);
            actualPlayer.GetComponent<NavMeshAgent>().SetPath(chestPath);

            if (actualPlayer.GetComponent<NavMeshAgent>().remainingDistance > 0.5f) {
                Quaternion targetRotation = Quaternion.LookRotation(actualPlayer.GetComponent<UserMovement>().actualStep.transform.position - actualPlayer.transform.position);
                actualPlayer.transform.rotation = Quaternion.Slerp(actualPlayer.transform.rotation, targetRotation, 1.5f * Time.deltaTime);
            }
        }
    }


    private void EventOnDialogEnd(object sender,DialogController.OnDialogEndArgs e) {
        if(e.dialog == null) 
            return;
        
        if(e.dialog.id == 10 && e.answerIndex == 0) {
            if(e.actualPlayer.GetComponent<UserInventory>().coins < chestCoinsPrice) {
                RunDelayed(0.05f,() => {
                    Dialog moneyDialog = GameController.Instance.dialog.GetDialogByName("NotEnoughMoneyChest");
                    GameController.Instance.dialog.isInDialog.value = true;
                    GameController.Instance.dialog.currentDialog = moneyDialog;
                    StartCoroutine(GameController.Instance.dialog.ShowText(moneyDialog.Content[0],moneyDialog.Content.Length));
                });

                return;
            }

            chestPosition = e.position - GetDirection(e.actualPlayer.GetComponent<UserMovement>().actualStep,e.actualPlayer.GetComponent<UserMovement>().actualStep.GetComponent<Step>());
            actualPlayer = e.actualPlayer;
            
            obj = e.obj;
            chestPath = new NavMeshPath();
            goToChest = true;
            
            actualPlayer.GetComponent<UserMovement>().currentAction = UserAction.CHEST;
            actualPlayer.GetComponent<UserMovement>().userCam.transform.SetParent(null,true);
            

            GameController.Instance.UpdateSubPath(actualPlayer.GetComponent<UserMovement>(),true);
            GameController.Instance.actualChest.SetActive(true);
        }

        else if (e.answerIndex == 1) 
            GameController.Instance.EndUserTurn();


        if (e.dialog.id == 11)
            GameController.Instance.EndUserTurn();



        if (e.dialog.id == 12 || e.dialog.id == 13) {
            returnToStep = true;
        }
    }

    public void CheckChestBot(UserInventory inventory) { // Check if bot can go to chest 
        if(inventory.coins < chestCoinsPrice) {
            GameController.Instance.EndUserTurn();
            return;
        }

        GameObject actualBot = inventory.gameObject;

        chestPosition = actualBot.GetComponent<UserMovement>().actualStep.GetComponent<Step>().chest.transform.position - GetDirection(actualBot.GetComponent<UserMovement>().actualStep,actualBot.GetComponent<UserMovement>().actualStep.GetComponent<Step>());
        actualPlayer = actualBot;
        obj = GameController.Instance.actualChest;
        chestPath = new NavMeshPath();
        goToChest = true;
        actualPlayer.GetComponent<UserMovement>().currentAction = UserAction.CHEST;
        
        actualPlayer.GetComponent<UserMovement>().userCam.transform.SetParent(null,true);
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
