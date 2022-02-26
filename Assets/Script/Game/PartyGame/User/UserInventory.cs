using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PathCreation;
using PathCreation.Examples;

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

        int rand = Random.Range(0,secretCode.Length);

        while(secretCode[rand] != -1 || rand >= secretCode.Length) 
            rand = Random.Range(0,secretCode.Length);

        Debug.Log("rand: "+ rand);
        
        secretCode[rand] = GameController.Instance.secretCode[rand];

        return secretCode[rand];
    }

    public bool HasEnoughMoney(int money) {
        return money >= coins;
    }

}
