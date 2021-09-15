using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class GiveCoins : MonoBehaviour {
    
    public GameObject player;
    public Text amount;
    public void GiveCoinsToPlayer() {
        transform.gameObject.SetActive(false);
        player.GetComponent<UserInventory>().coins += Int32.Parse(amount.text);
    }

    public void ChangeDiceResult() {
        transform.gameObject.SetActive(false);
        player.GetComponent<UserMovement>().diceResult = Int32.Parse(amount.text);
    }

}
