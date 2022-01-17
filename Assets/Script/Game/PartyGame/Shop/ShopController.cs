using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class ShopController : MonoBehaviour {

    public GameController gameController;

    private GameObject actualPlayer;

    void Start() {
        gameController.dialog.OnDialogEnd += EventOnDialogEnd;
    }



    private void EventOnDialogEnd(object sender,DialogController.OnDialogEndArgs e) {
        if(e.dialog.id == 0) { // IL s'agit de la fin du dialogue de shop
            actualPlayer = e.actualPlayer;
        }
    }

    public void AddQuantity(int slot) {
        int actualAmount = int.Parse(transform.GetChild(1).GetChild(slot).GetChild(3).GetChild(1).gameObject.GetComponent<Text>().text);
        actualAmount++;

        if(actualAmount >= 100)
            actualAmount = 99;

        transform.GetChild(1).GetChild(slot).GetChild(3).GetChild(1).gameObject.GetComponent<Text>().text = "" + actualAmount;
    }
    
    public void RemoveQuantity(int slot) {
        int actualAmount = int.Parse(transform.GetChild(1).GetChild(slot).GetChild(3).GetChild(1).gameObject.GetComponent<Text>().text);
        actualAmount--;

        if(actualAmount <= 1)
            actualAmount = 1;

        transform.GetChild(1).GetChild(slot).GetChild(3).GetChild(1).gameObject.GetComponent<Text>().text = "" + actualAmount;
    }

}
