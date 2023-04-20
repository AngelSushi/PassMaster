using System;
using System.Collections;
using System.Collections.Generic;
using EnvDTE;
using UnityEngine;

public class CoinsReward : MonoBehaviour {
    
    public int beginY;
    public bool hasFinishAnimation;
    private GameController controller;

    public Animation animation;

    
    private float _animTime = 0.5f;

    private void Awake() => controller = FindObjectOfType<GameController>();

    [ContextMenu("RunCoroutine")]
    public void RunCoroutine() {
        hasFinishAnimation = false;
        GameObject player = controller.players[controller.actualPlayer];
         
        Camera camera = player.GetComponent<UserMovement>().userCam.GetComponent<Camera>();

        
        Vector3 screenPos = camera.WorldToScreenPoint(player.transform.position);

        transform.GetComponent<RectTransform>().position = screenPos;

        transform.gameObject.SetActive(true);
        
        Keyframe[] keys = new Keyframe[2];
        
        AnimationClip currentClip = new AnimationClip();
        currentClip.name = player.name + "'s Coins Anim";
        
        AnimationEvent animEvent = new AnimationEvent();
        currentClip.legacy = true;
       
        
        keys[0] = new Keyframe(0.0f, beginY);
        keys[1] = new Keyframe(_animTime, beginY + 100.0f);
        
        AnimationCurve curve = new AnimationCurve(keys);
       
        
        currentClip.SetCurve("",typeof(Transform),"localPosition.y",curve);
        
        animEvent.time = _animTime + 0.03f;

        animEvent.functionName =  "EndOfCoinsAnimation";
        currentClip.AddEvent(animEvent); 
        
        animation.AddClip(currentClip,currentClip.name);
        animation.clip = currentClip;
        animation.Play();
        
    }

    private void EndOfCoinsAnimation() { // Called when the coins animation is finished on the board game 
        hasFinishAnimation = true;
        transform.gameObject.SetActive(false);

        if (GameController.Instance.part == GameController.GamePart.PARTYGAME) {
            UserMovement targetUser = controller.players[controller.actualPlayer].GetComponent<UserMovement>();

            switch (controller.players[controller.actualPlayer].GetComponent<UserMovement>().currentAction) {
                case UserAction.MALUS:
                case UserAction.BONUS:
                    if (IsOnStepChest(targetUser)) {
                        if (!controller.dialog.isInDialog) {
                            if (targetUser.isPlayer)
                                targetUser.DisplayChestDialog();
                            else
                                controller.chestController.CheckChestBot(targetUser.inventory);
                        }
                    }
                    else
                        controller.EndUserTurn();

                    break;

                case UserAction.CHEST:
                    if (!targetUser.isPlayer) {
                        controller.chestController.goToChest = false;
                        controller.chestController.returnToStep = true;
                    }

                    break;

                case UserAction.SHOP:
                    if (!targetUser.isPlayer) {
                        controller.shopController.mooveToShop = false;
                        controller.shopController.returnToStep = true;
                    }

                    break;
            }
        }
        else if (GameController.Instance.part == GameController.GamePart.MINIGAME) {
            GameController.Instance.mgController.startEndOfAnim = true;
        }

    }



    private bool IsOnStepChest(UserMovement user) {
        GameObject actualStep = user.actualStep;
        return actualStep != null && actualStep.GetComponent<Step>() != null && actualStep.GetComponent<Step>().chest != null && actualStep.GetComponent<Step>().chest.activeSelf && !controller.chestController.goToChest && !controller.chestController.returnToStep;
    }
}
