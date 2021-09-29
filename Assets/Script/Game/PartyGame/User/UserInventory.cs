using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PathCreation;
using PathCreation.Examples;

public class UserInventory : User {
    public bool hasDoubleDice; 
    public bool hasReverseDice; 
    public bool hasHourglass;
    public bool hasStar; 
    public bool hasLightning;
    public bool hasParachute;
    public bool hasBomb;

    public int coins;
    public int cards;
    public int[] secretCode = new int[6];
    
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

    public void AddCards(int cardsGain) {
        cards += cardsGain;
    }

    public override void OnBeginTurn() {}
    public override void OnFinishTurn() {}

}
