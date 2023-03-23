using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System.Linq;


public enum ItemActionResult {
    NONE,
    SHOP,
    CHEST,
    END
}

public class ItemAction : MonoBehaviour {
    public string actionName;
    public int itemID;
    public ItemActionResult rangeResultType;
    public int rangeMax;
    public int coinsToHave;
    [HideInInspector]
    public GameObject possessPlayer; // The player who use the object
    [HideInInspector]
    public GameObject targetPlayer;
    public bool succeed;
    public bool differentPlayerToTarget;
    public UnityEvent actionsEvent;
    private GameController controller;
    private List<bool> succeedActions;


    void Start() {
        controller = GameController.Instance;
        succeedActions = new List<bool>();
    }

    public void DoAction(GameObject player) {
        targetPlayer = player;
        
        succeedActions.Clear();
        actionsEvent.Invoke();

        succeed = !(succeedActions.Count(succeedA => !succeedA) > 0); // Si une des actions est fausse -> succeed est faux
        
       // Debug.Log("current action check " + actionName + " succeed " + succeed);
    }

    public void CheckInRangeAction(/*bool checkType*/) { 
        GameObject beginStep = targetPlayer.GetComponent<UserMovement>().actualStep;
        if(beginStep == null)
            beginStep = controller.firstStep;
        
        succeedActions.Add(FindSmallestPathTo());         
    }

    public void CheckHasCoinsAction() { 
        
       succeedActions.Add(targetPlayer.GetComponent<UserInventory>().coins >= coinsToHave); 
    }

    public void CheckLeaderboardAction() { 
        List<GameObject> classedPlayers = new List<GameObject>(controller.players);
        classedPlayers = classedPlayers.OrderBy(player=>player.GetComponent<UserInventory>().points).ToList();
        classedPlayers.Reverse();

        bool result = classedPlayers[0] == targetPlayer;

        if(differentPlayerToTarget)
            result = classedPlayers[0] != targetPlayer;

        succeedActions.Add(result);
    }

    public void CheckCoinsAndCards() {
        succeedActions.Add(targetPlayer.GetComponent<UserInventory>().cards == possessPlayer.GetComponent<UserInventory>().cards && targetPlayer.GetComponent<UserInventory>().coins >= possessPlayer.GetComponent<UserInventory>().coins);
    }

    public void CheckCoinsAndObjects() {
        succeedActions.Add(targetPlayer.GetComponent<UserInventory>().coins > 0 || targetPlayer.GetComponent<UserInventory>().HasObjects());
    }

    public void CheckDayPeriod(int dayPeriod) {
        succeedActions.Add((int)GameController.Instance.dayController.dayPeriod == dayPeriod);
    }

    public void CheckIsInArea() {
        succeedActions.Add(GameController.Instance.dayController.deactivatedSteps.Contains(targetPlayer.GetComponent<UserMovement>().actualStep));
    }

    public void CheckHasObjects(int itemId) {
        switch (itemId) {
            case 0:
                succeedActions.Add(targetPlayer.GetComponent<UserInventory>().doubleDiceItem > 0);
                break;
            
            case 1:
                succeedActions.Add(targetPlayer.GetComponent<UserInventory>().tripleDiceItem > 0);
                break;
            
            case 2:
                succeedActions.Add(targetPlayer.GetComponent<UserInventory>().reverseDiceItem > 0);
                break;
            
            case 3:
                succeedActions.Add(targetPlayer.GetComponent<UserInventory>().hourglassItem > 0);
                break;
        }
    }

    private bool FindSmallestPathTo() {
        if (rangeMax < 0)
            targetPlayer.GetComponent<UserMovement>().reverseCount = true;
        
        GameObject[] stepPaths = new GameObject[Math.Abs(rangeMax)];
        
        targetPlayer.GetComponent<UserMovement>().hasCheckPath = false;
        targetPlayer.GetComponent<UserMovement>().CheckPath(false,stepPaths,Math.Abs(rangeMax));

        foreach (GameObject step in stepPaths) {
            if (step != null && step.GetComponent<Step>() != null && CheckResult(step.GetComponent<Step>())) {
                return true;
            }
        }
        
        targetPlayer.GetComponent<UserMovement>().reverseCount = false;

        return false;
    }

    private bool CheckResult(Step actualStep) {
        if(actualStep == null)
            return false;

        if(!differentPlayerToTarget) {
            switch(rangeResultType) {
                case ItemActionResult.SHOP:
                    if(actualStep.type == StepType.SHOP) 
                       // Debug.Log("is in range of " + actualStep.type);
                        return true;
                    break;

                case ItemActionResult.CHEST:
                    if(actualStep.chest != null && actualStep.chest.activeSelf) 
                      //  Debug.Log("is in range of " + actualStep.chest.name);
                        return true;
                    break;
                    

                case ItemActionResult.END:
                    if(actualStep.type == StepType.STEP_END) // Ajouter le fait qu'il doit avoir fini le code pour y aller  
                       // Debug.Log("is in range of " + actualStep.type);
                        return true;
                    break;
            }
        }
        
        return false;
    }
}
