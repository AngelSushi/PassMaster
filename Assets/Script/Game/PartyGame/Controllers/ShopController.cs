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
    private bool mooveToShop;
    [HideInInspector]
    public bool returnToStep;
    private NavMeshPath shopPath;

    public GameObject dialogShopParent;

    public DialogController shopDialogs; 

    private bool askBuy;

    private int lastSlot;

    void Start() {
        shopDialogs.dialogArray = gameController.dialog.dialogArray;
        gameController.dialog.OnDialogEnd += EventOnDialogEnd;
        shopDialogs.OnDialogEnd += EventOnDialogEnd;
    }

    public override void Update() {
        if(mooveToShop) {
            actualPlayer.GetComponent<NavMeshAgent>().CalculatePath(shopPosition,shopPath);
            actualPlayer.GetComponent<NavMeshAgent>().SetPath(shopPath);

            RunDelayed(0.65f,() => { // Pas synchro avec la vitesse du joueur
                shopPosition = Vector3.zero;
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

        if(returnToStep) {
            actualPlayer.GetComponent<NavMeshAgent>().CalculatePath(actualPlayer.GetComponent<UserMovement>().actualStep.transform.position,shopPath);
            actualPlayer.GetComponent<NavMeshAgent>().SetPath(shopPath);

            if(shopObject != null && shopObject.transform.GetChild(0).gameObject.activeSelf)   
                shopObject.transform.GetChild(0).gameObject.SetActive(false);
        }
    }

    private void EventOnDialogEnd(object sender,DialogController.OnDialogEndArgs e) {
        if(e.dialog == null)
            return;

        if(e.dialog.id == 0 && e.answerIndex == 0) { // IL s'agit de la fin du dialogue de shop
            if(e.position == Vector3.zero || e.obj == null)
                return;

            actualPlayer = e.actualPlayer;
            shopObject = e.obj;
            shopPosition = e.position;
            shopPath = new NavMeshPath();
          //  mooveToShop = true;
        }

        if(e.dialog.id == 7 && e.answerIndex == 0) { // Fin du dialogue BuyItem
            UserInventory inventory = e.actualPlayer.GetComponent<UserInventory>();
            
            int amount = int.Parse(transform.GetChild(1).GetChild(lastSlot).GetChild(3).GetChild(1).gameObject.GetComponent<Text>().text);
            int price = int.Parse(transform.GetChild(1).GetChild(lastSlot).GetChild(2).GetChild(1).gameObject.GetComponent<Text>().text);

            if(inventory.coins >= amount * price) {
                inventory.CoinLoose(amount * price);
                e.actualPlayer.GetComponent<UserAudio>().CoinsLoose();
            }

            Dialog stateDialog = inventory.coins <= amount * price ? shopDialogs.GetDialogByName("NotEnoughMoneyShop") : shopDialogs.GetDialogByName("EndShop");

            shopDialogs.isInDialog = true;
            shopDialogs.currentDialog = stateDialog;
            StartCoroutine(stateDialog.Content[0],stateDialog.Content.Length);
        }

        if(e.dialog.id == 9) {  
            returnToStep = true;
            e.actualPlayer.GetComponent<UserUI>().showShop = false;
            gameController.mainCamera.SetActive(false); 
            actualPlayer.transform.GetChild(1).gameObject.SetActive(true);
        }
    }

    public void OnEnterHoverButton(int slot) {
        if(askBuy)
            return;

        shopDialogs.isInDialog = true;
        Dialog hoverDialog = shopDialogs.GetDialogByName("HoverItem_" + ConvertSlotToItem(slot));
        shopDialogs.currentDialog = hoverDialog;
        StartCoroutine(shopDialogs.ShowText(hoverDialog.Content[0],hoverDialog.Content.Length));

    }

    public void OnLeaveHoverButton(int slot) {
        if(askBuy)
            return;

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

    public void BuyItem(int slot) {
        if(askBuy && slot == lastSlot) 
            return;

        Dialog buyItem = shopDialogs.GetDialogByName("BuyItem");

        int amount = int.Parse(transform.GetChild(1).GetChild(slot).GetChild(3).GetChild(1).gameObject.GetComponent<Text>().text);
        String content = buyItem.Content[0] + "x" + amount + " " + TranslateItem(ConvertSlotToItem(slot)) + " ? ";

        shopDialogs.isInDialog = true;
        shopDialogs.currentDialog = buyItem;
        askBuy = true;
        lastSlot = slot;
        StartCoroutine(shopDialogs.ShowText(content,buyItem.Content.Length));
    }

    private string TranslateItem(string item) {
        switch(item) {
            case "DoubleDice":
                return "Dé double";
            case "TripleDice":
                return "Dé triple";
            case "ReverseDice":
                return "Dé inverse";
            case "Bomb":
                return "Bombe";
            case "Lightning":
                return "Eclair";
            case "Hourglass":
                return "Sablier";
            default:
                return ""; 
        }
    }

}
