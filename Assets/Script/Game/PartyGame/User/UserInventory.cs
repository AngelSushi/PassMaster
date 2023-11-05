using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using System;
using System.Linq;
using VSLangProj80;

public class UserInventory : CoroutineSystem {
    
    
    public int doubleDiceItem; 
    public int tripleDiceItem;
    public int reverseDiceItem; 
    public int hourglassItem;
    public int shellItem; 
    public int lightningItem;

    public int coins;
    public int cards;
    public int[] secretCode = new int[6];

    public int points { get { return cards * 100000 + coins; } set {points = value; } }

    public void CoinGain(int coinsGain) {
        coins += coinsGain;
    }

    public void CoinLoose(int coinsLoose) {
        if(coins - coinsLoose >= 0) coins -= coinsLoose; 
        
        else coins = 0;
    }

    public int AddCards() {
        cards++;

        int rand = UnityEngine.Random.Range(0,secretCode.Length);

        while(secretCode[rand] != -1 || rand >= secretCode.Length) 
            rand = UnityEngine.Random.Range(0,secretCode.Length);
        
        secretCode[rand] = GameController.Instance.secretCode[rand];

        return secretCode[rand];
    }

    public bool HasEnoughMoney(int money) {
        return money >= coins;
    }

    public bool HasObjects() {
        return doubleDiceItem != 0 || tripleDiceItem != 0 || reverseDiceItem != 0 || hourglassItem != 0 || lightningItem != 0 || shellItem != 0;
    }

    public void UseItemBot() {
        List<ItemAction> succeedActions = new List<ItemAction>();
        
        // Ajouter les pourcentages de base d'un item
        List<int> possessedItems = GetPossessedItems();

        foreach(ItemAction action in GameController.Instance.itemController.actions) {

            if(action.differentPlayerToTarget) {
                action.possessPlayer = transform.gameObject;

                foreach(GameObject player in GameController.Instance.players) {
                    if(player == action.possessPlayer) 
                        continue;

                    action.DoAction(player);

                    if(action.succeed)
                        break;
                }
            }
            else 
                action.DoAction(transform.gameObject);
            
            if(possessedItems.Contains(action.itemID) && action.succeed) 
                succeedActions.Add(action);
            
        }

        if (succeedActions.Count == 0) {
            Debug.Log("no succeed");
            return;
        }

        int max = 1;
        int lastMax = 0;
        ItemAction maxAction = null;
        for(int i = 0;i < succeedActions.Count;i++) {
            ItemAction currentAction = succeedActions[i];
            max = 1;
            
            for (int j = i + 1; j < succeedActions.Count; j++) {
                ItemAction nextAction = succeedActions[j];

                if (currentAction.itemID == nextAction.itemID) {
                    Debug.Log("action max");
                    max++;
                }
            }

            if (lastMax == 0 || lastMax <= max)
            {
                lastMax = max;
                maxAction = currentAction;
            }
        }

  
        switch(maxAction.itemID) {
            case 0:
                transform.gameObject.GetComponent<UserMovement>().doubleDice = true;
                break;

            case 1:
                transform.gameObject.GetComponent<UserMovement>().tripleDice = true;
                break;

            case 2:
                transform.gameObject.GetComponent<UserMovement>().reverseDice = true;
                break;

            case 3:
                transform.gameObject.GetComponent<UserMovement>().useHourglass = true;
                GameController.Instance.blackScreenAnim.Play();
                break;
        }


        transform.gameObject.GetComponent<UserMovement>().checkObjectToUse = false;
        transform.gameObject.GetComponent<UserMovement>().InitDice();

    }

    private List<int> GetPossessedItems() {
        List<int> possessedItemsId = new List<int>();

        if(doubleDiceItem > 0) // Id = 0
            possessedItemsId.Add(0);
        if(tripleDiceItem > 0) // Id = 1
            possessedItemsId.Add(1);
        if(reverseDiceItem > 0) // Id = 2
            possessedItemsId.Add(2);
        if(hourglassItem > 0) // Id = 3
            possessedItemsId.Add(3);
        if(lightningItem > 0) // Id = 4
            possessedItemsId.Add(4);
        if(shellItem > 0) // Id = 5
            possessedItemsId.Add(5);

        return possessedItemsId;
    }

}
