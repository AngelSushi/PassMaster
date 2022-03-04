using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class UserInventory : MonoBehaviour {
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
    
    private List<Vector3> drop = new List<Vector3>();
    private List<string> objectSpawn = new List<string>();
    private List<int> objList = new List<int>();


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

        Debug.Log("rand: "+ rand);
        
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
        Dictionary<int,float> itemsPercentage = new Dictionary<int, float>();
        
        // Ajouter les pourcentages de base d'un item
        List<int> possessedItems = GetPossessedItems();

        foreach(ItemAction action in GameController.Instance.itemController.actions) {
            foreach(int itemID in possessedItems) {
                if(action.itemID == itemID) {
                    switch(GameController.difficulty) {
                        case 0:
                            int idIndex = Array.IndexOf(GameController.Instance.itemController.itemsID,itemID);
                            action.actionPercentage = GameController.Instance.itemController.easyPercentage[idIndex];
                            break;

                        case 1:
                            int idIndexM = Array.IndexOf(GameController.Instance.itemController.itemsID,itemID);
                            action.actionPercentage = GameController.Instance.itemController.mediumPercentage[idIndexM];
                            break;

                        case 2:
                            int idIndexH = Array.IndexOf(GameController.Instance.itemController.itemsID,itemID);
                            action.actionPercentage = GameController.Instance.itemController.hardPercentage[idIndexH];
                            break;
                    }
                }
            }

            action.DoAction(transform.gameObject);
            
         //   if(possessedItems.Contains(action.itemID) && action.DoAction(transform.gameObject)) 
           //     itemsPercentage.Add(action.itemID,action.percentageToAdd);

            Debug.Log("name: " + action.name + " succeed: " + action.succeed);            
        }

        foreach(int itemID in possessedItems) {
            switch(itemID) {
                case 0:
                    transform.gameObject.GetComponent<UserMovement>().doubleDice = true;
                    Debug.Log("use double dice");
                    break;
            }
        }

        transform.gameObject.GetComponent<UserMovement>().checkObjectToUse = false;


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
