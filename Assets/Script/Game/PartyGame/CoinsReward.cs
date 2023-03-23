using System;
using System.Collections;
using System.Collections.Generic;
using EnvDTE;
using UnityEngine;

public class CoinsReward : MonoBehaviour {
    
    public bool minigame;
    public int beginY;
    public bool hasFinishAnimation;
    private GameController controller;

    public Animation animation;

    private AnimationClip _currentClip;
    
    private float animTime = 0.5f;

    private void Awake() => controller = FindObjectOfType<GameController>();

    [ContextMenu("RunCoroutine")]
    public void RunCoroutine() {
        GameObject player = controller.players[controller.actualPlayer];
         
        Camera camera = player.GetComponent<UserMovement>().userCam.GetComponent<Camera>();

        
        Vector3 screenPos = camera.WorldToScreenPoint(player.transform.position);

        transform.GetComponent<RectTransform>().position = screenPos;
        /*new Vector3(camera.WorldToScreenPoint(player.transform.position).x, beginY, transform.localPosition.z); */
        
        
        transform.gameObject.SetActive(true);

        
        Keyframe[] keys = new Keyframe[2];
        Keyframe[] keysX = new Keyframe[2];
        
        _currentClip = new AnimationClip();
        _currentClip.name = player.name + "'s Coins Anim";
        AnimationEvent animEvent = new AnimationEvent();
        _currentClip.legacy = true;
       
        
        keys[0] = new Keyframe(0.0f, beginY);
        keys[1] = new Keyframe(animTime, beginY + 100.0f);
        
        
        keysX[0] = new Keyframe(0.0f, screenPos.x);
        keysX[1] = new Keyframe(animTime, screenPos.x);
        
        AnimationCurve curve = new AnimationCurve(keys);
        AnimationCurve curveX = new AnimationCurve(keysX);
       
        
        _currentClip.SetCurve("",typeof(Transform),"localPosition.y",curve);
      //  _currentClip.SetCurve("",typeof(Transform),"localPosition.x",curveX);
        
        animEvent.time = animTime + 0.03f;

        animEvent.functionName = minigame ? "EndOfCoinsAnimationMG" : "EndOfCoinsAnimation";
        _currentClip.AddEvent(animEvent); 
        
        animation.AddClip(_currentClip,_currentClip.name);
        animation.clip = _currentClip;
        animation.Play();
        
    }

    private void EndOfCoinsAnimation() { // Called when the coins animation is finished on the board game 
        transform.gameObject.SetActive(false);

        UserMovement targetUser = controller.players[controller.actualPlayer].GetComponent<UserMovement>();

        switch (controller.players[controller.actualPlayer].GetComponent<UserMovement>().currentAction) {
            case UserAction.MALUS:
            case UserAction.BONUS:
                if (IsOnStepChest(targetUser)) {
                    if(!controller.dialog.isInDialog) {
                        if(targetUser.isPlayer) 
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
        
      //  animation.RemoveClip(_currentClip);
      //  animation.clip = null;
    }

    private void EndOfCoinsAnimationMG() { } // Called when the coins animation is finished on the minigame


    private bool IsOnStepChest(UserMovement user) {
        GameObject actualStep = user.actualStep;
        return actualStep != null && actualStep.GetComponent<Step>() != null && actualStep.GetComponent<Step>().chest != null && actualStep.GetComponent<Step>().chest.activeSelf && !controller.chestController.goToChest && !controller.chestController.returnToStep;
    }
}
