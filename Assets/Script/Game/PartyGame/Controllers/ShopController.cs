using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using UnityEngine.AI;
using System.Linq;

public class ShopController : CoroutineSystem {

    
    private GameObject actualPlayer,shopObject;
    private Vector3 shopPosition;
    [HideInInspector]
    private Vector3 beginPosition;
    [HideInInspector]
    public bool mooveToShop;
    [HideInInspector]
    public bool returnToStep;
    [HideInInspector]
    public bool hasFinishBuy = true;

    private NavMeshPath shopPath;

    public GameObject dialogShopParent;

    public DialogController shopDialogs; 

    public List<ShopItem> shopItems;

    private bool askBuy;

    private int lastSlot;
    private GameController gameController;

    private bool isBotBuying;

    private  List<ShopItem> expensiveItems;
    void Start() {
        gameController = GameController.Instance;
        shopDialogs.dialogArray = gameController.dialog.dialogArray;
        gameController.dialog.OnDialogEnd += EventOnDialogEnd;
        shopDialogs.OnDialogEnd += EventOnDialogEnd;

        shopItems = shopItems.OrderBy(item=>item.price).ToList();

        if(transform.GetChild(1).childCount != shopItems.Count) 
            Debug.Log("error in size of items");

        for(int i = 0;i<transform.GetChild(1).childCount;i++) {
            Transform shopItemTrans = transform.GetChild(1).GetChild(i);
            shopItemTrans.GetChild(0).gameObject.GetComponent<Image>().sprite = shopItems[i].sprite;
            shopItemTrans.GetChild(1).gameObject.GetComponent<Text>().text = shopItems[i].name;
            shopItemTrans.GetChild(2).GetChild(1).gameObject.GetComponent<Text>().text = "" + shopItems[i].price;
        }
    }

    public override void Update() {
        if(mooveToShop) {
            actualPlayer.GetComponent<NavMeshAgent>().CalculatePath(shopPosition,shopPath);
            actualPlayer.GetComponent<NavMeshAgent>().SetPath(shopPath);

            RunDelayed(0.65f,() => { // Pas synchro avec la vitesse du joueur
                shopPosition = Vector3.zero;
                mooveToShop = false;
                if(actualPlayer.GetComponent<UserMovement>().isPlayer) {
                    actualPlayer.GetComponent<UserUI>().showShop = true;

                    shopObject.transform.GetChild(0).gameObject.SetActive(true);
                    gameController.mainCamera.SetActive(true); 
                    actualPlayer.transform.GetChild(1).gameObject.SetActive(false);

                    gameController.mainCamera.GetComponent<Camera>().fieldOfView = 60;
                    gameController.mainCamera.transform.position = actualPlayer.transform.GetChild(1).gameObject.transform.position;
                    gameController.mainCamera.transform.rotation = actualPlayer.transform.GetChild(1).gameObject.transform.rotation;     
                }  
                else {
                    expensiveItems = shopItems;
                    expensiveItems.Reverse();
                } 
                       
            });
            
        }

// IA Shop
        if(expensiveItems != null && isBotBuying && !returnToStep && hasFinishBuy) {
            UserInventory inv = actualPlayer.GetComponent<UserInventory>();

            if(inv == null) {
                gameController.EndUserTurn();
                return;
            }

            foreach(ShopItem item in expensiveItems) {
                if(inv.coins >= item.price && hasFinishBuy) {
                    StartCoroutine(actualPlayer.GetComponent<UserMovement>().WaitMalus(false,item.price));
                    
                    hasFinishBuy = false;
                }
            }

            if(actualPlayer.GetComponent<UserMovement>().isTurn && inv.coins <= FindCheapestItem().price && hasFinishBuy) {
                returnToStep = true;
            }

            
        }
        
        if(returnToStep) {
            if(!actualPlayer.GetComponent<NavMeshAgent>().enabled)
                return;

            actualPlayer.GetComponent<NavMeshAgent>().CalculatePath(actualPlayer.GetComponent<UserMovement>().actualStep.transform.position,shopPath);
            actualPlayer.GetComponent<NavMeshAgent>().SetPath(shopPath);

            if(shopObject != null && shopObject.transform.GetChild(0).gameObject.activeSelf)   
                shopObject.transform.GetChild(0).gameObject.SetActive(false);
        }
    }

    private void EventOnDialogEnd(object sender,DialogController.OnDialogEndArgs e) {
        Debug.Log("event dialog end " + e.dialog.id + " answer " + e.answerIndex);
        
        if(e.dialog == null)
            return;

        if(e.dialog.id == 0 && e.answerIndex == 0) { // IL s'agit de la fin du dialogue de shop
            if(e.position == Vector3.zero || e.obj == null)
                return;

            actualPlayer = e.actualPlayer;
            shopObject = e.obj;
            shopPosition = e.position;
            shopPath = new NavMeshPath();
            mooveToShop = true;
            
            gameController.UpdateSubPath(actualPlayer.GetComponent<UserMovement>(),true);
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
            StartCoroutine(shopDialogs.ShowText(stateDialog.Content[0],stateDialog.Content.Length));
        }

        
        
        if(e.dialog.id == 9) {  
            
            returnToStep = true;
            e.actualPlayer.GetComponent<UserUI>().showShop = false;
            actualPlayer.transform.GetChild(1).gameObject.SetActive(true);
        }
    }


    #region User's Functions

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
                return "Shell";
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
            case "Shell":
                return "Carapace";
            case "Lightning":
                return "Eclair";
            case "Hourglass":
                return "Sablier";
            default:
                return ""; 
        }
    }

    #endregion

    #region Bot's Functions

    public void AskShopBot(UserInventory inv,GameObject shop) {
        ShopItem cheapestItem = FindCheapestItem();

        if(cheapestItem == null) {
            gameController.EndUserTurn();
            return;
        }

        if(inv.coins < cheapestItem.price) 
          gameController.EndUserTurn();
        else {
            RunDelayed(0.1f,() => {
                actualPlayer = inv.gameObject;
                shopObject = shop;
                shopPosition = shop.transform.position - gameController.GetDirection(actualPlayer.GetComponent<UserMovement>().actualStep,actualPlayer.GetComponent<UserMovement>().actualStep.GetComponent<Step>(),3f);

                shopPath = new NavMeshPath();
                mooveToShop = true;
                isBotBuying = true;
            });
        }
    }

    private ShopItem FindCheapestItem() {
        int minPrice = 0;
        ShopItem cheapestItem = null;

        foreach(ShopItem item in shopItems) {
            if(minPrice <= item.price) {
                minPrice = item.price;
                cheapestItem = item;
            }
        }

        return cheapestItem;
    }

    private ShopItem FindCheapestItem(List<ShopItem> items) {
        int minPrice = 0;
        ShopItem cheapestItem = null;

        foreach(ShopItem item in items) {
            if(minPrice <= item.price) {
                minPrice = item.price;
                cheapestItem = item;
            }
        }

        return cheapestItem;
    }


    #endregion
}