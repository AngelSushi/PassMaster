using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using UnityEngine.AI;
using System.Linq;
using UnityEngine.InputSystem;

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

    public DialogController shopDialogs; 

    public List<ShopItem> shopItems;

    private bool askBuy;

    private int lastSlot;
    private GameController gameController;

    private bool isBotBuying;

    private int _currentShopSlot = -1;
    
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
            shopItemTrans.GetChild(0).gameObject.GetComponent<RectTransform>().sizeDelta = shopItems[i].spriteScale;
            shopItemTrans.GetChild(1).gameObject.GetComponent<Text>().text = shopItems[i].name;
            shopItemTrans.GetChild(2).GetChild(1).gameObject.GetComponent<Text>().text = "" + shopItems[i].price;
        }

        gameController.inputs.FindAction("Menus/Next").started += OnNext;
        gameController.inputs.FindAction("Menus/Previous").started += OnPrevious;
        gameController.inputs.FindAction("Menus/Interact").started += OnInteract;
    }

    public override void Update() {
        if(mooveToShop) {
            actualPlayer.GetComponent<NavMeshAgent>().CalculatePath(shopPosition,shopPath);
            actualPlayer.GetComponent<NavMeshAgent>().SetPath(shopPath);
            
            Debug.Log("go to me please");

            if (actualPlayer.GetComponent<NavMeshAgent>().remainingDistance > 0.5f) {
                Quaternion targetRotation = Quaternion.LookRotation(shopPosition - actualPlayer.transform.position);
                actualPlayer.transform.rotation = Quaternion.Slerp(actualPlayer.transform.rotation, targetRotation, 1.5f * Time.deltaTime);
            }
        }

        if(returnToStep) {
            if(!actualPlayer.GetComponent<NavMeshAgent>().enabled)
                return;

            actualPlayer.GetComponent<NavMeshAgent>().CalculatePath(actualPlayer.GetComponent<UserMovement>().actualStep.transform.position,shopPath);
            actualPlayer.GetComponent<NavMeshAgent>().SetPath(shopPath);

            if (actualPlayer.GetComponent<NavMeshAgent>().remainingDistance > 0.5f) {
                Quaternion targetRotation = Quaternion.LookRotation(actualPlayer.GetComponent<UserMovement>().actualStep.transform.position - actualPlayer.transform.position);
                actualPlayer.transform.rotation = Quaternion.Slerp(actualPlayer.transform.rotation, targetRotation, 1.5f * Time.deltaTime);
            }

            if(shopObject != null && shopObject.transform.GetChild(0).gameObject.activeSelf)   
                shopObject.transform.GetChild(0).gameObject.SetActive(false);
        }
    }


    public void EnterShop() {
        mooveToShop = false;
        shopPosition = Vector3.zero;

        if (actualPlayer.GetComponent<UserMovement>().isPlayer) {
            Dialog shopEnter = gameController.dialog.GetDialogByName("EnterShop");

            if (gameController.dayController.dayPeriod == DayController.DayPeriod.DUSK) {
                string[] content = new string[shopEnter.Content.Length + 1];

                for (int i = 0; i < shopEnter.Content.Length; i++) {
                    content[i] = shopEnter.Content[i];
                }

                content[content.Length - 1] =
                    "Oh j'oubliais, mon magasin ferme bientôt et j'ai du stock a vider. Certains objets sont en promotion ça serait bête de rater l'occasion.";

                shopEnter.Content = content;
            }

            gameController.dialog.isInDialog.value = true;
            gameController.dialog.currentDialog = shopEnter;
            StartCoroutine(gameController.dialog.ShowText(shopEnter.Content[0], shopEnter.Content.Length));
        }
        else
            BuyItem(actualPlayer.GetComponent<UserMovement>(),FindMostBuyableItem(actualPlayer.GetComponent<UserInventory>()), false);

    }

    private void EndShop() {
        returnToStep = true;
        
        shopDialogs.EndDialog();
        actualPlayer.GetComponent<UserUI>().showShop.value = false;
        shopObject.transform.GetChild(0).gameObject.SetActive(false);
        
    }

    private void BuyItem(UserMovement user,ShopItem buyItem,bool isPlayer) {
        if (isPlayer) {
            user.audio.CoinsLoose();
            user.inventory.CoinLoose(buyItem.price);
            gameController.ActualizePlayerClassement();
        }
        else
            StartCoroutine(user.WaitMalus( buyItem.price));

        switch (buyItem.itemType) {
            case ItemType.DOUBLE_DICE:
                user.inventory.doubleDiceItem++;
                break;
                
            case ItemType.TRIPLE_DICE:
                user.inventory.tripleDiceItem++;
                break;
                
            case ItemType.REVERSE_DICE:
                user.inventory.reverseDiceItem++;
                break;
                
            case ItemType.HOURGLASS:
                user.inventory.hourglassItem++;
                break;
                
        }
    }

    #region User's Functions

    private void OnNext(InputAction.CallbackContext e) {
        if (e.started && actualPlayer != null && actualPlayer.GetComponent<UserUI>().showShop.value) {
            if (_currentShopSlot < shopItems.Count - 1) {
                _currentShopSlot++;
                
                if(_currentShopSlot - 1 >= 0) 
                    transform.GetChild(1).GetChild(_currentShopSlot - 1).GetChild(transform.GetChild(1).GetChild(_currentShopSlot - 1).childCount - 1).gameObject.SetActive(false);
                
                transform.GetChild(1).GetChild(_currentShopSlot).GetChild(transform.GetChild(1).GetChild(_currentShopSlot).childCount - 1).gameObject.SetActive(true);

                if (shopDialogs.isInDialog) 
                    shopDialogs.EndDialog();
                
                
                Dialog hoverDialog = shopDialogs.GetDialogByName("HoverItem_" + GetItemHoverDialog(_currentShopSlot));
                shopDialogs.isInDialog.value = true;
                shopDialogs.currentDialog = hoverDialog;
                shopDialogs.DisplayDialog();

                shopObject.transform.GetChild(0).gameObject.GetComponent<Animator>().SetBool("IsTalking",true);

                RunDelayed(4f, () => {
                    shopObject.transform.GetChild(0).gameObject.GetComponent<Animator>().SetBool("IsTalking", false);
                });
                
                actualPlayer.GetComponent<UserAudio>().ButtonHover();
            }
        }
    }

    private void OnPrevious(InputAction.CallbackContext e) {
        if (e.started && actualPlayer != null && actualPlayer.GetComponent<UserUI>().showShop.value) {
            if (_currentShopSlot > 0) {
                _currentShopSlot--;
                
                transform.GetChild(1).GetChild(_currentShopSlot + 1).GetChild(transform.GetChild(1).GetChild(_currentShopSlot + 1).childCount - 1).gameObject.SetActive(false);
                transform.GetChild(1).GetChild(_currentShopSlot).GetChild(transform.GetChild(1).GetChild(_currentShopSlot).childCount - 1).gameObject.SetActive(true);
                
                if (shopDialogs.isInDialog) 
                    shopDialogs.EndDialog();
                
                Dialog hoverDialog = shopDialogs.GetDialogByName("HoverItem_" + GetItemHoverDialog(_currentShopSlot));
                shopDialogs.isInDialog.value = true;
                shopDialogs.currentDialog = hoverDialog;
                shopDialogs.DisplayDialog();
                
                //shopObject.transform.GetChild(0).gameObject.GetComponent<Animator>().SetBool("IsTalking",false);
                shopObject.transform.GetChild(0).gameObject.GetComponent<Animator>().SetBool("IsTalking",true);

                RunDelayed(4f, () => {
                    shopObject.transform.GetChild(0).gameObject.GetComponent<Animator>().SetBool("IsTalking", false);
                });
                
                
                actualPlayer.GetComponent<UserAudio>().ButtonHover();
            }
        }
    }

    private void OnInteract(InputAction.CallbackContext e) {
        if (e.started && actualPlayer != null && actualPlayer.GetComponent<UserUI>().showShop.value && _currentShopSlot >= 0) {
            Debug.Log("currentSlot " + _currentShopSlot);
    
            UserMovement user = actualPlayer.GetComponent<UserMovement>();

            if (shopItems[_currentShopSlot].price > user.inventory.coins) {
                // Dialog Cant Purchase
                return;
            }
            

            Dialog closeShop = shopDialogs.GetDialogByName("CloseShop");
            shopDialogs.isInDialog.value = true;
            shopDialogs.currentDialog = closeShop;
            StartCoroutine(shopDialogs.ShowText(closeShop.Content[0], closeShop.Content.Length));
            
            shopObject.transform.GetChild(0).gameObject.GetComponent<Animator>().SetBool("IsTalking",true);

            RunDelayed(4f, () => {
                shopObject.transform.GetChild(0).gameObject.GetComponent<Animator>().SetBool("IsTalking", false);
            });
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
            mooveToShop = true;
            
            actualPlayer.GetComponent<UserMovement>().userCam.transform.SetParent(null,true); // Remettre le userCam dans le bon parent
            
            actualPlayer.GetComponent<UserMovement>().currentAction = UserAction.SHOP;
            
            gameController.UpdateSubPath(actualPlayer.GetComponent<UserMovement>(),true);
        }

        if(e.dialog.id == 7 && e.answerIndex == 0) { // Fin du dialogue BuyItem
            UserInventory inventory = e.actualPlayer.GetComponent<UserInventory>();
            
            int amount = int.Parse(transform.GetChild(1).GetChild(lastSlot).GetChild(3).GetChild(1).gameObject.GetComponent<Text>().text);
            int price = int.Parse(transform.GetChild(1).GetChild(lastSlot).GetChild(2).GetChild(1).gameObject.GetComponent<Text>().text);

            if(inventory.coins >= amount * price) {
                inventory.CoinLoose(amount * price);
                e.actualPlayer.GetComponent<UserAudio>().CoinsLoose();
                BuyItem( e.actualPlayer.GetComponent<UserMovement>(),shopItems[_currentShopSlot],true);
            }
            
            

            Dialog stateDialog = inventory.coins <= amount * price ? shopDialogs.GetDialogByName("NotEnoughMoneyShop") : shopDialogs.GetDialogByName("EndShop");

            shopDialogs.isInDialog.value = true;
            shopDialogs.currentDialog = stateDialog;
            StartCoroutine(shopDialogs.ShowText(stateDialog.Content[0],stateDialog.Content.Length));
        }

        if(e.dialog.id == 9) {
            returnToStep = true;
            e.actualPlayer.GetComponent<UserUI>().showShop.value = false;
            shopObject.transform.GetChild(0).gameObject.SetActive(false);
        }

        if (e.dialog.id == 15) {
            actualPlayer.GetComponent<UserUI>().showShop.value = true;
            shopObject.transform.GetChild(0).gameObject.SetActive(true);
        }
        
        if(e.dialog.id == 16) 
            EndShop();
    }
    

    private string GetItemHoverDialog(int slot) {

        switch (slot) {
            case 0:
                return "Hourglass";
            case 1:
                return "DoubleDice";
            case 2:
                return "TripleDice";
            case 3:
                return "ReverseDice";
            default:
                return "";
        }
    }

    #endregion

    #region Bot's Functions

    public void AskShopBot(UserInventory inv,GameObject shop) {
        ShopItem cheapestItem = FindCheapestItem();

        if(cheapestItem == null || inv.coins < cheapestItem.price) {
            gameController.EndUserTurn();
            return;
        }

        RunDelayed(0.1f,() => {
            actualPlayer = inv.gameObject;
            shopObject = shop;
            shopPosition = shop.transform.position - gameController.GetDirection(actualPlayer.GetComponent<UserMovement>().actualStep,actualPlayer.GetComponent<UserMovement>().actualStep.GetComponent<Step>(),3f);

            shopPath = new NavMeshPath();
            mooveToShop = true;
            
            isBotBuying = true;
            actualPlayer.GetComponent<UserMovement>().userCam.transform.SetParent(null,true); 
            
            actualPlayer.GetComponent<UserMovement>().currentAction = UserAction.SHOP;
            gameController.UpdateSubPath(actualPlayer.GetComponent<UserMovement>(),true);

           // FindMostBuyableItem(actualPlayer.GetComponent<UserInventory>());
            Debug.Log("here ask");
        });
        
    }

    private ShopItem FindCheapestItem() {
        int minPrice = 0;
        ShopItem cheapestItem = null;

        foreach(ShopItem item in shopItems) {
            if(minPrice == 0 || minPrice >= item.price) {
                minPrice = item.price;
                cheapestItem = item;
            }
        }
        
        Debug.Log("cheapest " + cheapestItem);

        return cheapestItem;
    }

    private ShopItem FindMostBuyableItem(UserInventory inventory) {
        int coins = inventory.coins;
        ShopItem buyableItem = null;
        
        Debug.Log("coins " + coins);

        foreach (ShopItem item in shopItems) {
            if (buyableItem == null || (item.price >= buyableItem.price && coins >= item.price))
                buyableItem = item;
        }

        Debug.Log("buy " + buyableItem);
        
        return buyableItem;

    }


    #endregion
}