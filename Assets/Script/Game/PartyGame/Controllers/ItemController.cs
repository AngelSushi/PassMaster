using System.Collections;
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


    public void DropCoins(GameObject player,UserInventory inv) {

     //   player.GetComponent<UserMovement>().actualStep.GetComponent<Step>().itemsInStep.Add(ItemType.COINS,inv.coins / 2);
        player.GetComponent<UserMovement>().currentAction = UserAction.MALUS;
        StartCoroutine(player.GetComponent<UserMovement>().WaitMalus(inv.coins / 2));

        GameObject coinsObj = Instantiate(coins);
        coinsObj.transform.position = player.transform.GetChild(5).GetChild(0).gameObject.transform.position;

        if(coinsObj.GetComponent<Rigidbody>() == null)
            return;

        coinsObj.GetComponent<Rigidbody>().AddForce(40 * Vector3.up,ForceMode.Impulse);
        coinsObj.GetComponent<Rigidbody>().AddForce(20 * Vector3.forward,ForceMode.Impulse);
        
        coinsObj.transform.position = player.transform.GetChild(5).GetChild(0).gameObject.transform.position;

        if(coinsObj.GetComponent<Rigidbody>() == null)
            return;

        RunDelayed(0.3f,() => {
            Destroy(coinsObj.GetComponent<PathFollower>());
        });
        coinsObj.GetComponent<Rigidbody>().AddForce(40 * Vector3.up,ForceMode.Impulse);
        coinsObj.GetComponent<Rigidbody>().AddForce(20 * Vector3.forward,ForceMode.Impulse);
        
    }
}

