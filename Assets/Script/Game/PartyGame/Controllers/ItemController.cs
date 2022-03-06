﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PathCreation.Examples;
using PathCreation;

public enum ItemType {
    COINS,
    DOUBLE_DICE,
    TRIPLE_DICE,
    REVERSE_DICE,
    HOURGLASS,
    LIGHTNING,
    SHELL
}

public class ItemController : CoroutineSystem {

    public GameObject lightningEffect;
    public GameObject coins;
    public List<ItemAction> actions;

    [HideInInspector]
    public int[] itemsID = { 0,1,2,3,4,5};

    [HideInInspector]
    public int[] easyPercentage = {28,26,26,30,24,23};  

    [HideInInspector]
    public int[] mediumPercentage = {48,46,46,50,44,43};

    [HideInInspector]
    public int[] hardPercentage = {58,56,56,60,54,53}; 


    public void DropCoins(GameObject player,UserInventory inv) {

     //   player.GetComponent<UserMovement>().actualStep.GetComponent<Step>().itemsInStep.Add(ItemType.COINS,inv.coins / 2);
        StartCoroutine(player.GetComponent<UserMovement>().WaitMalus(false,inv.coins / 2));

        GameObject coinsObj = Instantiate(coins);

        coinsObj.AddComponent<PathFollower>();
        coinsObj.GetComponent<PathFollower>().pathCreator = player.transform.GetChild(2).GetChild(0).gameObject.GetComponent<PathCreator>();
        coinsObj.GetComponent<PathFollower>().endOfPathInstruction = EndOfPathInstruction.Stop;
        coinsObj.GetComponent<PathFollower>().speed = 2f;

        RunDelayed(0.3f,() => {
            Destroy(coinsObj.GetComponent<PathFollower>());
        });
    }
}

