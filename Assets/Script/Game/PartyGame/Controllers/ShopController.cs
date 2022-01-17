using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using UnityEngine.AI;

public class ShopController : CoroutineSystem {

    public GameController gameController;

    private GameObject actualPlayer,shopObject;
    private Vector3 shopPosition;
    private Vector3 beginPosition;
    private bool mooveToShop;
    private NavMeshPath shopPath;

    public GameObject dialogShopParent;

    public DialogController shopDialogs; 

    void Start() {
        shopDialogs.dialogArray = gameController.dialog.dialogArray;
        gameController.dialog.OnDialogEnd += EventOnDialogEnd;
    }

    public override void Update() {
        if(mooveToShop) {
            actualPlayer.GetComponent<NavMeshAgent>().CalculatePath(shopPosition,shopPath);
            actualPlayer.GetComponent<NavMeshAgent>().SetPath(shopPath);

            RunDelayed(0.65f,() => { // Pas synchro avec la vitesse du joueur
                shopPosition = Vector3.zero;
                beginPosition = Vector3.zero;
                mooveToShop = false;
                actualPlayer.GetComponent<UserUI>().showShop = true;

                shopObject.transform.GetChild(0).gameObject.SetActive(true);
                gameController.mainCamera.SetActive(true); 
                actualPlayer.transform.GetChild(1).gameObject.SetActive(false);

                gameController.mainCamera.GetComponent<Camera>().fieldOfView = 60;
                gameController.mainCamera.transform.position = actualPlayer.transform.GetChild(1).gameObject.transform.position;
                gameController.mainCamera.transform.rotation = actualPlayer.transform.GetChild(1).gameObject.transform.rotation;                
            });
            
        }
    }

    private void EventOnDialogEnd(object sender,DialogController.OnDialogEndArgs e) {
        if(e.dialog.id == 0) { // IL s'agit de la fin du dialogue de shop
            if(e.shopPosition == Vector3.zero || e.shopObject == null)
                return;

            actualPlayer = e.actualPlayer;
            shopObject = e.shopObject;
            shopPosition = e.shopPosition;
            beginPosition = e.actualPlayer.transform.position;
            shopPath = new NavMeshPath();
            mooveToShop = true;
        }
    }

    public void OnEnterHoverButton(int slot) {

        shopDialogs.isInDialog = true;
        Dialog hoverDialog = shopDialogs.GetDialogByName("HoverItem_" + ConvertSlotToItem(slot));
        shopDialogs.currentDialog = hoverDialog;
        StartCoroutine(shopDialogs.ShowText(hoverDialog.Content[0],hoverDialog.Content.Length));

    }

    public void OnLeaveHoverButton(int slot) {
        shopDialogs.EndDialog();
    }

    private string ConvertSlotToItem(int slot) {
        switch(slot) {
            case 0:
                return "DoubleDice";
            case 1:
                return "TripleDice";
            case 2:
                return "ReverseDice";
            case 3:
                return "Bomb";
            case 4:
                return "Lightning";
            case 5:
                return "Hourglass";
            default:
                return "";
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
