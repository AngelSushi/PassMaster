using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System.Linq;


public enum ItemActionResult {
    SHOP,
    CHEST,
    END
}

public class ItemAction : MonoBehaviour {
    public string actionName;
    public int itemID;
    public float actionPercentage;
    public float percentageToAdd;
    public ItemActionResult rangeResultType;
    public int rangeMax;
    public int coinsToHave;
    [HideInInspector]
    public GameObject possessPlayer; // The player who use the object
   // [HideInInspector]
    public GameObject targetPlayer;
    public bool succeed;
    public bool differentPlayerToTarget;
    public UnityEvent actionsEvent;
    private GameController controller;
    private List<GameObject> stepsActionPath;
    private List<bool> succeedActions;


    void Start() {
        controller = GameController.Instance;
        stepsActionPath = new List<GameObject>();
        succeedActions = new List<bool>();
    }

    public void DoAction(GameObject player) {
        targetPlayer = player;
        succeedActions.Clear();
        actionsEvent.Invoke();

        succeed = !(succeedActions.Count(succeedA => !succeedA) > 0); // Si une des actions est fausse -> succeed est faux
    }

    public void CheckInRangeAction() { 
        GameObject beginStep = targetPlayer.GetComponent<UserMovement>().actualStep;
        if(beginStep == null)
            beginStep = controller.firstStep;

        int beginIndex = controller.FindIndexInParent(beginStep.transform.parent.gameObject,beginStep);
        stepsActionPath.Clear();

        succeedActions.Add(FindSmallestPathTo(beginStep,stepsActionPath,false,true)); 
        
    }

    public void CheckHasCoinsAction() { 
       succeedActions.Add(targetPlayer.GetComponent<UserInventory>().coins >= coinsToHave); 
    }

    public void CheckLeaderboardAction() { 
        List<GameObject> classedPlayers = new List<GameObject>(controller.players);
        classedPlayers = classedPlayers.OrderBy(player=>player.GetComponent<UserInventory>().points).ToList();
        classedPlayers.Reverse();

        succeedActions.Add(classedPlayers[0] == targetPlayer);    
    }

    public void CheckCoinsAndCards() {
        succeedActions.Add(targetPlayer.GetComponent<UserInventory>().cards == possessPlayer.GetComponent<UserInventory>().cards && targetPlayer.GetComponent<UserInventory>().coins >= possessPlayer.GetComponent<UserInventory>().coins);
    }

    public void CheckCoinsAndObjects() {
        succeedActions.Add(targetPlayer.GetComponent<UserInventory>().coins > 0 || targetPlayer.GetComponent<UserInventory>().HasObjects());
    }


    private bool FindSmallestPathTo(GameObject begin,List<GameObject> iaDirectionSteps,bool decrement,bool sameParent) {
        int beginIndex =  controller.FindIndexInParent(begin.transform.parent.gameObject,begin);

        for(int i = beginIndex; i != beginIndex + rangeMax;) {
            if(i >= begin.transform.parent.childCount) 
                i -= begin.transform.parent.childCount;
            
            GameObject actualObj = begin.transform.parent.GetChild(i).gameObject;

            if(!stepsActionPath.Contains(actualObj))
                stepsActionPath.Add(actualObj);


            if(CheckResult(actualObj.GetComponent<Step>())) 
                return true;            

            if(actualObj.GetComponent<Direction>() != null) {
                for(int j = 0;j<actualObj.GetComponent<Direction>().directionsStep.Length;j++) {
                    GameObject beginDirection = actualObj.GetComponent<Direction>().directionsStep[j];
                    if(beginDirection != null) { 
                        Direction nextDir = actualObj.GetComponent<Direction>().directionsStep[j].GetComponent<Direction>();

                        GameObject beginObj = beginDirection;
                        GameObject endObj = null;

                        if(nextDir != null) {
                            beginObj = nextDir.directionsStep[1].gameObject;
                            endObj = nextDir.directionsStep[1].gameObject.transform.parent.GetChild(nextDir.directionsStep[1].gameObject.transform.parent.childCount- 1).gameObject;
                        }
                        else {
                            if(beginObj == beginObj.transform.parent.GetChild(beginDirection.transform.parent.childCount - 2).gameObject) // -2 ici car on ne veut pas prendre en compte la direction
                                endObj = beginObj.transform.parent.GetChild(0).gameObject;              
                            else 
                                endObj = beginDirection.transform.parent.GetChild(beginDirection.transform.parent.childCount - 1).gameObject;
                        }

                        decrement = beginObj == beginObj.transform.parent.GetChild(beginDirection.transform.parent.childCount - 2).gameObject;

                        int beginSize = stepsActionPath.Count;
                        bool result = FindSmallestPathTo(beginObj,iaDirectionSteps,decrement,beginObj.transform.parent == endObj.transform.parent);
                        int size = stepsActionPath.Count - beginSize;

                        if(!result)  
                            EraseSteps(beginSize,size,iaDirectionSteps);   
                    }
                }
            }

            if(decrement)
                i--;
            else
                i++;

        }

        Debug.Log("is not in range");
        return false;
    }

    private void EraseSteps(int beginIndex,int size,List<GameObject> iaDirectionSteps) {
        List<GameObject> erase = new List<GameObject>();

        for(int i = 0;i<iaDirectionSteps.Count;i++) {
            if(i >= beginIndex && i <= beginIndex + size) 
                erase.Add(iaDirectionSteps[i]);
        }

        foreach(GameObject eraseObj in erase) 
            stepsActionPath.Remove(eraseObj);
    }

    private bool CheckResult(Step actualStep) {
        if(actualStep == null)
            return false;

        switch(rangeResultType) {
            case ItemActionResult.SHOP:
                if(actualStep.type == StepType.SHOP) {
                    Debug.Log("is in range of " + actualStep.type);
                    return true;
                }
                break;

            case ItemActionResult.CHEST:
                if(actualStep.chest != null && actualStep.chest.activeSelf) {
                    Debug.Log("is in range of " + actualStep.chest.name);
                    return true;
                }
                break;

            case ItemActionResult.END:
                if(actualStep.type == StepType.STEP_END) {
                    Debug.Log("is in range of " + actualStep.type);
                    return true;
                }
                break;
        }
        
        
        if(differentPlayerToTarget) {
            foreach(GameObject player in controller.players) {
                if(player.GetComponent<UserMovement>().actualStep != null && player.GetComponent<UserMovement>().actualStep == actualStep.gameObject) {
                    Debug.Log("is in range of " + player.name);
                    return true;
                }
            }
        }
        

        return false;
    }
}
