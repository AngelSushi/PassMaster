using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemAction : MonoBehaviour {
    public enum ActionType {
        NONE,
        IN_RANGE,
        HAS_COINS,
        LEADERBOARD
    }

    public int itemID;
    public int actionPercentage;
    public int percentageToAdd;
    public ActionType actionType;
    public StepType rangeType;
    public int rangeMax;
    private GameController controller;
    public List<GameObject> stepsActionPath;

    void Start() {
        controller = GameController.Instance;
    }

    public bool DoAction(GameObject targetPlayer) {
        switch(actionType) {
            case ActionType.IN_RANGE:
                return CheckInRangeAction(targetPlayer);
            case ActionType.HAS_COINS:
                return CheckHasCoinsAction();
            case ActionType.LEADERBOARD:
                return CheckLeaderboardAction();
            default:
                return false;
        }
    }

    public bool CheckInRangeAction(GameObject targetPlayer) { 
        GameObject beginStep = targetPlayer.GetComponent<UserMovement>().actualStep;

        if(beginStep == null)
            beginStep = controller.firstStep;

        int beginIndex = controller.FindIndexInParent(beginStep.transform.parent.gameObject,beginStep);
        stepsActionPath.Clear();
        
        return FindSmallestPathTo(beginStep,rangeType,stepsActionPath,false,true); 
        
    }
    public bool CheckHasCoinsAction() { return true; }
    public bool CheckLeaderboardAction() { return true; }



    private bool FindSmallestPathTo(GameObject begin,StepType endType,List<GameObject> iaDirectionSteps,bool decrement,bool sameParent) {
        int beginIndex =  controller.FindIndexInParent(begin.transform.parent.gameObject,begin);

        for(int i = beginIndex; i != beginIndex + rangeMax;) {
            if(i >= begin.transform.parent.childCount) 
                i -= begin.transform.parent.childCount;
            
            GameObject actualObj = begin.transform.parent.GetChild(i).gameObject;

            if(!stepsActionPath.Contains(actualObj))
                stepsActionPath.Add(actualObj);

            if(actualObj.GetComponent<Step>() != null && actualObj.GetComponent<Step>().type == endType) {
                Debug.Log("is in range of " + endType);
                return true;
            }
            

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
                        bool result = FindSmallestPathTo(beginObj,endType,iaDirectionSteps,decrement,beginObj.transform.parent == endObj.transform.parent);
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

        Debug.Log("is not in range of " + endType);
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
}
