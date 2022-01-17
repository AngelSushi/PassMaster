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

    void Start() {
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

                for(int i = 0;i<dialogShopParent.transform.childCount;i++) 
                    dialogShopParent.transform.GetChild(i).gameObject.SetActive(true);
                
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
