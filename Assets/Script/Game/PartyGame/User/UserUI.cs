using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using UnityEngine.AI;

public class UserUI : CoroutineSystem {

    public bool showHUD;
    public bool showTurnInfo;
    public bool showActionButton;
    public bool showDirection;
    public bool showChestHUD;
    public bool showShopHUD;
    public bool showShop;
    public bool cameraView;
    public bool useBomb;
    public bool useLightning;
    public GameObject infoLabel;
    public GameObject diceResult;
    public GameObject coinIconReward;
    public GameObject camera;
    public GameObject lightning;
    public Transform[] directions; // les parents des directions
    public Transform[] items; // les items du shoop
    public Transform[] actions; // buttons d'actions sur le menu principale du jeu
    public Transform[] playersPanels; // UI de chaque joueur avec ses pieces etc
    public Transform[] inventoryItems;
    public Transform[] isleParts;
    public Transform chestHUD;
    public Transform shopHUD;
    public Transform hoverInventoryItem; // L'ui du hover sur les items dans l'inventaire
    public ParticleSystem[] lightningParticles;
    public Text coinTextReward;
    public Text itemDescription; // texte de la description d'un iutem dans le shop
    public Text turnText,nightText; // texte pour afficher les tours et la nuit
    public Text shopText;
    public Direction direction;
    public GameController gameController;
    public UserMovement movement;

    private bool isInInventory;
    private int nextShopId = -1;
    private int index = -1;
    private float cameraSpeed = 200f;
    private Vector2 vecMove;

// Check que quand c'est pas le tour d'un joueur tt soit désactiver

    void Update() {  
        if(!gameController.freeze) {
            ManageHudState(showHUD);
            ManageCameraPosition();
            ManagerHudTurnState(showTurnInfo && gameController.part == GameController.GamePart.PARTYGAME);
            ManageActionButtonState(showActionButton);
            ManageHudDirection(showDirection);
            ManageChestHUD(showChestHUD);
            ManageShop(showShop);
            ManageShopHUD(showShopHUD);  
        } 
    }

    private void ManageCameraPosition() {

        float directionX = cameraView ? vecMove.x * cameraSpeed * Time.deltaTime : 0;
        float directionY = cameraView ? vecMove.y * cameraSpeed * Time.deltaTime : 0;

        camera.transform.Translate(directionX,directionY,0);
    }

    public void OnRight(InputAction.CallbackContext e) {
        if(e.started && !cameraView && showHUD && !hoverInventoryItem.transform.parent.gameObject.activeSelf && !gameController.freeze) {
            if(index < 2) 
                index++;

            switch(index) {
                case 0:
                    actions[0].GetChild(0).gameObject.SetActive(false);
                    actions[0].GetChild(1).gameObject.SetActive(true);
                    break;
                case 1:
                    actions[0].GetChild(0).gameObject.SetActive(true);
                    actions[0].GetChild(1).gameObject.SetActive(false);

                    actions[1].GetChild(0).gameObject.SetActive(false);
                    actions[1].GetChild(1).gameObject.SetActive(true);
                    break;
                case 2:
                    actions[1].GetChild(0).gameObject.SetActive(true);
                    actions[1].GetChild(1).gameObject.SetActive(false);

                    actions[2].GetChild(0).gameObject.SetActive(false);
                    actions[2].GetChild(1).gameObject.SetActive(true);
                    break;        
            }
        }

        if(e.started && !cameraView && !showHUD && showDirection && !gameController.freeze) {
            int max = -1;

            if(directions[0].gameObject.activeSelf) 
                max = 0;
            if(directions[1].gameObject.activeSelf) 
                max = 1;
            if(directions[2].gameObject.activeSelf) 
                max = 2;
            
            if(!directions[0].GetChild(1).gameObject.activeSelf && !directions[1].GetChild(1).gameObject.activeSelf && !directions[2].GetChild(1).gameObject.activeSelf) {
                if(directions[0].gameObject.activeSelf) 
                    index = -1;
                else if(directions[1].gameObject.activeSelf) 
                    index = 0;
                else if(directions[2].gameObject.activeSelf) 
                    index = 1;        
                
            }

            if(index < max) 
                index++;

            switch(index) {
                case 0:
                    directions[0].GetChild(0).gameObject.SetActive(false);
                    directions[0].GetChild(1).gameObject.SetActive(true);
                    break;

                case 1:
                    
                    directions[0].GetChild(0).gameObject.SetActive(true);
                    directions[0].GetChild(1).gameObject.SetActive(false);

                    directions[1].GetChild(0).gameObject.SetActive(false);
                    directions[1].GetChild(1).gameObject.SetActive(true);
                    break;

                case 2:
                    directions[1].GetChild(0).gameObject.SetActive(true);
                    directions[1].GetChild(1).gameObject.SetActive(false);

                    directions[2].GetChild(0).gameObject.SetActive(false);
                    directions[2].GetChild(1).gameObject.SetActive(true);
                    break;        
            }
        }

        if(e.started && showChestHUD && !gameController.freeze) {
            if(index < 1) 
                index++;

            switch(index) {
                case 0:
                    chestHUD.GetChild(1).gameObject.SetActive(true);
                    break;

                case 1:
                    chestHUD.GetChild(1).gameObject.SetActive(false);
                    chestHUD.GetChild(2).gameObject.SetActive(true);
                    break;    
            }
        }

        if(e.started && showShop && !gameController.freeze) {
            if(index < 6) {
                index++;
                movement.audio.ButtonHover();
            }
            if(nextShopId < 2) 
                nextShopId++;

            for(int i = 0;i<3;i++) 
                items[i].GetChild(3).gameObject.SetActive(false); 

            items[nextShopId].GetChild(3).gameObject.SetActive(true);

            if(index <= 2) {
                itemDescription.text = gameController.shopItems[index].description;
                items[nextShopId].GetChild(1).GetChild(0).gameObject.GetComponent<Text>().text = "" + gameController.shopItems[index].price;
                items[nextShopId].GetChild(2).gameObject.GetComponent<Text>().text = gameController.shopItems[index].name;
                items[nextShopId].GetChild(0).gameObject.GetComponent<RectTransform>().sizeDelta = gameController.shopItems[index].dimension;

            }

            if(index > 2) {
                int begin = 2;

                for(int i = index;i>index - 3;i--) {
                    items[begin].GetChild(0).gameObject.GetComponent<Image>().sprite = gameController.shopItems[i].img;
                    itemDescription.text = gameController.shopItems[i].description;
                    items[begin].GetChild(1).GetChild(0).gameObject.GetComponent<Text>().text = "" + gameController.shopItems[i].price;
                    items[begin].GetChild(2).gameObject.GetComponent<Text>().text = gameController.shopItems[i].name;
                    begin--;
                }
            }

        }

        if(e.started && showShopHUD && !gameController.freeze) {
            if(index < 1) 
                index++;

            switch(index) {
                case 0:
                    shopHUD.GetChild(1).gameObject.SetActive(true);
                    break;

                case 1:
                    shopHUD.GetChild(1).gameObject.SetActive(false);
                    shopHUD.GetChild(2).gameObject.SetActive(true);
                    break;    
            }
        }

        if(e.started && hoverInventoryItem.transform.parent.gameObject.activeSelf && isInInventory && !gameController.freeze) {
            if(index < 7) 
                index++;

            Vector2[] hoverPos = {new Vector2(-444,31),new Vector2(-299,31),new Vector2(-149,31),new Vector2(5,31),new Vector2(145,31),new Vector2(305,31),new Vector2(448,31),new Vector2(583,31)};
            
            hoverInventoryItem.gameObject.SetActive(true);
            hoverInventoryItem.localPosition = hoverPos[index];       
        }
    }

    public void OnLeft(InputAction.CallbackContext e) {
        if(e.started && !cameraView && showHUD && !hoverInventoryItem.transform.parent.gameObject.activeSelf && !gameController.freeze) {
            if(index > 0) 
                index--;

            switch(index) {
                case 0:
                    actions[1].GetChild(0).gameObject.SetActive(true);
                    actions[1].GetChild(1).gameObject.SetActive(false);

                    actions[0].GetChild(0).gameObject.SetActive(false);
                    actions[0].GetChild(1).gameObject.SetActive(true);
                    break;
                case 1:
                    actions[2].GetChild(0).gameObject.SetActive(true);
                    actions[2].GetChild(1).gameObject.SetActive(false);

                    actions[1].GetChild(0).gameObject.SetActive(false);
                    actions[1].GetChild(1).gameObject.SetActive(true);
                    break;
                case 2:
                    actions[2].GetChild(0).gameObject.SetActive(false);
                    actions[2].GetChild(1).gameObject.SetActive(true);
                    break;        
            }
        }

        if(e.started && !cameraView && !showHUD && showDirection && !gameController.freeze) {
            int min = -1;

            if(directions[0].gameObject.activeSelf) 
                min = 0;
            else if(directions[1].gameObject.activeSelf) 
                min = 1;
            else if(directions[2].gameObject.activeSelf) 
                min = 2;
            
            if(index > min) 
                index--;

            switch(index) {
                case 0:
                    directions[1].GetChild(0).gameObject.SetActive(true);
                    directions[1].GetChild(1).gameObject.SetActive(false);

                    directions[0].GetChild(0).gameObject.SetActive(false);
                    directions[0].GetChild(1).gameObject.SetActive(true);
                    break;
                case 1:
                    directions[2].GetChild(0).gameObject.SetActive(true);
                    directions[2].GetChild(1).gameObject.SetActive(false);

                    directions[1].GetChild(0).gameObject.SetActive(false);
                    directions[1].GetChild(1).gameObject.SetActive(true);
                    break;
                case 2:
                    directions[2].GetChild(0).gameObject.SetActive(false);
                    directions[2].GetChild(1).gameObject.SetActive(true);
                    break;        
            }
        }

        if(e.started && showChestHUD && !gameController.freeze) {
            if(index > 0) 
                index--;

            switch(index) {
                case 0:
                    chestHUD.GetChild(1).gameObject.SetActive(true);
                    chestHUD.GetChild(2).gameObject.SetActive(false);
                    break;

                case 1:
                    chestHUD.GetChild(1).gameObject.SetActive(false);
                    chestHUD.GetChild(2).gameObject.SetActive(true);
                    break;    
            }
        }

        if(e.started && showShop && !gameController.freeze) { 

            for(int i = 0;i<3;i++) 
                items[2+i].GetChild(3).gameObject.SetActive(false);

            int begin = 2; 

            if(index >= 2) {
                for(int i = index;i>index - 3;i--) {
                        
                    items[begin].GetChild(2).gameObject.GetComponent<Text>().text = gameController.shopItems[i].name;
                    items[begin].GetChild(0).gameObject.GetComponent<Image>().sprite = gameController.shopItems[i].img;
                    items[nextShopId].GetChild(1).GetChild(0).gameObject.GetComponent<Text>().text = "" + gameController.shopItems[i].price;
                    items[nextShopId].GetChild(0).gameObject.GetComponent<RectTransform>().sizeDelta = gameController.shopItems[index].dimension;
                    begin--;
                        
                }
            }
            else if(index == 1 || index == 0){
                    
                for(int i = 0;i<3;i++) {
                    items[i].GetChild(0).gameObject.GetComponent<Image>().sprite = gameController.shopItems[i].img;
                    items[i].GetChild(2).gameObject.GetComponent<Text>().text = gameController.shopItems[i].name;
                    items[i].GetChild(1).GetChild(0).gameObject.GetComponent<Text>().text = "" + gameController.shopItems[i].price;
                    items[nextShopId].GetChild(0).gameObject.GetComponent<RectTransform>().sizeDelta = gameController.shopItems[index].dimension; 
                }
            }
            
            if(index > 0) {
                index--;
                movement.audio.ButtonHover();
            }
            if(nextShopId > 0) 
                nextShopId--;
            
            if(index == 0) 
                items[0].GetChild(3).gameObject.SetActive(true);
            else if(index >= 1 && index < 6) 
                items[1].GetChild(3).gameObject.SetActive(true);
            else if(index == 6) 
                items[2].GetChild(3).gameObject.SetActive(true);

        }

        if(e.started && showShopHUD && !gameController.freeze) {
            if(index > 0) 
                index--;

            switch(index) {
                case 0:
                    shopHUD.GetChild(1).gameObject.SetActive(true);
                    shopHUD.GetChild(2).gameObject.SetActive(false);
                    break;

                case 1: 
                    shopHUD.GetChild(2).gameObject.SetActive(true);
                    break;    
            }
        }

        if(e.started && hoverInventoryItem.transform.parent.gameObject.activeSelf && !gameController.freeze) {
            if(index > 0) 
                index--;

            Vector2[] hoverPos = {new Vector2(-444,31),new Vector2(-299,31),new Vector2(-149,31),new Vector2(5,31),new Vector2(145,31),new Vector2(305,31),new Vector2(448,31),new Vector2(583,31)};
            
            hoverInventoryItem.gameObject.SetActive(true);
            hoverInventoryItem.localPosition = hoverPos[index];
        }
    }

    public void OnInteract(InputAction.CallbackContext e) {
        if(e.started && !hoverInventoryItem.transform.parent.gameObject.activeSelf &&  cameraView && useBomb && !gameController.freeze) {
            // Bombe

        }

        if(e.started && !hoverInventoryItem.transform.parent.gameObject.activeSelf &&  cameraView && useLightning && !gameController.freeze) {
            // Lightning

        }        
        if(e.started && showDirection && !gameController.freeze) {
            if(directions[0].GetChild(1).gameObject.activeSelf) {
                movement.left = true;
                directions[0].GetChild(0).gameObject.SetActive(true);
                directions[0].GetChild(1).gameObject.SetActive(false); 
            }
            else if(directions[1].GetChild(1).gameObject.activeSelf) {
                movement.front = true;
                directions[1].GetChild(0).gameObject.SetActive(true);
                directions[1].GetChild(1).gameObject.SetActive(false);
            }
            else if(directions[2].GetChild(1).gameObject.activeSelf) {
                movement.right = true;
                directions[2].GetChild(0).gameObject.SetActive(true);
                directions[2].GetChild(1).gameObject.SetActive(false);
            }

            movement.reverseCount = direction.reverseCount;
            showDirection = false;
            index = -1;
        }

        if(e.started && showChestHUD && !gameController.freeze) {
            if(index == 0) {              
                movement.isTurn = false;
                gameController.EndUserTurn();                
            }
            else if(index == 1) {
                // Buy
                int coins = movement.inventory.coins;

                if(coins >= 30) {
                    StartCoroutine(movement.WaitMalus(30,false));
                    movement.goToChest = true;
                }
                else {
                    movement.audio.BuyLoose();
                    gameController.EndUserTurn();
                    // Afficher un texte également
                }
            }

            showChestHUD = false;
            index = -1;
        }

        if(e.started && showShop && !gameController.freeze) {
            int actualCoins = movement.inventory.coins;
            shopText.gameObject.SetActive(true);

            if(actualCoins >= gameController.shopItems[index].price) { // Assez d'argent

                switch(index) {
                    case 0: // Dé double
                        bool hasDoubleDice = movement.inventory.hasDoubleDice;

                        if(!hasDoubleDice) { // Le joueur n'a pas encore l'objet
                            movement.inventory.hasDoubleDice = true;
                            DisplayShopText("Achat effectué avec succès",new Color(0,1.0f,0.12f,1.0f));
                            StartCoroutine(movement.WaitMalus(gameController.shopItems[index].price,false));
                            StartCoroutine(InfoLabelWaiting());
                        }
                        else { // Le joueur a déjà cet objet
                            movement.audio.BuyLoose();
                            DisplayShopText("Vous possédez déjà cet objet",new Color(1.0f,0f,0f,1.0f));
                            StartCoroutine(InfoLabelWaiting());
                        }
                        break;

                    case 1: // Dé inverse
                        bool hasReverseDice = movement.inventory.hasReverseDice;

                        if(!hasReverseDice) {
                            movement.inventory.hasReverseDice = true;
                            DisplayShopText("Achat effectué avec succès",new Color(0,1.0f,0.12f,1.0f));
                            StartCoroutine(movement.WaitMalus(gameController.shopItems[index].price,false));
                            StartCoroutine(InfoLabelWaiting());
                        }
                        else {
                            movement.audio.BuyLoose();
                            DisplayShopText("Vous possédez déjà cet objet",new Color(1.0f,0f,0f,1.0f));
                            StartCoroutine(InfoLabelWaiting());
                        }
                        break;

                    case 2: // Bombe
                        bool hasBomb = movement.inventory.hasBomb;

                        if(!hasBomb) {
                            movement.inventory.hasBomb = true;
                            DisplayShopText("Achat effectué avec succès",new Color(0,1.0f,0.12f,1.0f));
                            StartCoroutine(movement.WaitMalus(gameController.shopItems[index].price,false));
                            StartCoroutine(InfoLabelWaiting());
                        }   
                        else {
                            movement.audio.BuyLoose();
                            DisplayShopText( "Vous possédez déjà cet objet",new Color(1.0f,0f,0f,1.0f));
                            StartCoroutine(InfoLabelWaiting());
                        }
                        break;

                    case 3: // Sablier
                        bool hasHourglass = movement.inventory.hasHourglass;

                        if(!hasHourglass) {
                            movement.inventory.hasHourglass = true;
                            DisplayShopText("Achat effectué avec succès",new Color(0,1.0f,0.12f,1.0f));
                            StartCoroutine(movement.WaitMalus(gameController.shopItems[index].price,false));
                            StartCoroutine(InfoLabelWaiting());
                        }
                        else {
                            movement.audio.BuyLoose();
                            DisplayShopText( "Vous possédez déjà cet objet",new Color(1.0f,0f,0f,1.0f));
                            StartCoroutine(InfoLabelWaiting());
                        }
                        break;

                    case 4: // Eclair
                        bool hasLightning = movement.inventory.hasLightning;

                        if(!hasLightning) {
                            movement.inventory.hasLightning = true;
                            DisplayShopText("Achat effectué avec succès",new Color(0,1.0f,0.12f,1.0f));
                            StartCoroutine(movement.WaitMalus(gameController.shopItems[index].price,false));
                            StartCoroutine(InfoLabelWaiting());
                        }
                        else {
                            movement.audio.BuyLoose();
                            DisplayShopText( "Vous possédez déjà cet objet",new Color(1.0f,0f,0f,1.0f));
                            StartCoroutine(InfoLabelWaiting());
                        }
                        break;

                    case 5: // Etoile 
                        bool hasStar = movement.inventory.hasStar;

                        if(!hasStar) {
                            movement.inventory.hasStar = true;
                            DisplayShopText("Achat effectué avec succès",new Color(0,1.0f,0.12f,1.0f));
                            StartCoroutine(movement.WaitMalus(gameController.shopItems[index].price,false));
                            StartCoroutine(InfoLabelWaiting());
                        }
                        else {
                            movement.audio.BuyLoose();
                            DisplayShopText("Vous possédez déjà cet objet",new Color(1.0f,0f,0f,1.0f));
                            StartCoroutine(InfoLabelWaiting());
                        }
                        break;

                    case 6: // Parachute
                        bool hasParachute = movement.inventory.hasParachute;

                        if(!hasParachute) {
                            movement.inventory.hasParachute = true;                        
                            DisplayShopText("Achat effectué avec succès",new Color(0,1.0f,0.12f,1.0f));
                            StartCoroutine(movement.WaitMalus(gameController.shopItems[index].price,false));
                            StartCoroutine(InfoLabelWaiting());
                        }
                        else {
                            movement.audio.BuyLoose();
                            DisplayShopText("Vous possédez déjà cet objet",new Color(1.0f,0f,0f,1.0f));
                            StartCoroutine(InfoLabelWaiting());
                        }
                        break;
                }
            }
            
            else { // Pas assez d'argent
                movement.audio.BuyLoose();
                DisplayShopText("Vous n'avez pas assez d'argent",new Color(1.0f,0f,0f,1.0f));
                StartCoroutine(InfoLabelWaiting());
            }          
        }

        if(e.started && showShopHUD && !gameController.freeze) {

            switch(index) {
                case 0:
                    movement.stop = false;
                    if(movement.diceResult <= 0) 
                        gameController.EndUserTurn();
                    break;

                case 1:
                    movement.goToShop = true;
                    break;    
            }

            showShopHUD = false;
            index = -1;
        }

        if(e.started && showActionButton && !isInInventory && !cameraView && !gameController.freeze) {
            if(index == 0) {
                ManageInventory(true);
                isInInventory = true;
            }
            else if(index == 1) {
                showHUD = false;
                showActionButton = false;
                infoLabel.SetActive(false);
                if(movement.isPlayer) 
                    movement.isTurn = true;

                GetComponent<NavMeshAgent>().enabled = true;
                movement.waitDiceResult = true;
                index = -1;
            }

            else if(index == 2) {
                cameraView = true;
                camera = GameObject.FindGameObjectsWithTag("MainCamera")[0];

                camera.transform.position = new Vector3(transform.position.x,5747.6f,transform.position.z);
                camera.transform.rotation = Quaternion.Euler(90f,265.791f,0f); 

                DisplayInfoText(new Vector2(971,164),new Color(1.0f,1.0f,1.0f),"Appuyez sur ECHAP pour quitter le mode");
            }

            return;
        }

        if(e.started && hoverInventoryItem.transform.parent.gameObject.activeSelf && isInInventory && !cameraView && !gameController.freeze) {
            if(index <= 6 && index > -1) {
        
                if(hoverInventoryItem.transform.parent.gameObject.transform.childCount > (1+index) && inventoryItems[index].childCount > 0 && inventoryItems[index].GetChild(0).gameObject.activeSelf) { // Le joueur a l'objet 

                    switch(index) {
                        case 0: // Double Dice
                            movement.doubleDice = true;
                            ManageInventory(false);
                            isInInventory = false;
                            showHUD = false;
                            showActionButton = false;
                            movement.isTurn = true;
                            movement.waitDiceResult = true;
                            movement.inventory.hasDoubleDice = false;
                            break;

                        case 1: // Reverse Dice
                            movement.reverseDice = true;
                            ManageInventory(false);
                            isInInventory = false;
                            showHUD = false;
                            showActionButton = false;
                            movement.isTurn = true;
                            movement.waitDiceResult = true;
                            movement.inventory.hasReverseDice = false;
                            break;

                        case 2: // Bomb
                            

                            break;
                        case 3: //Hourglass
                            if(gameController.dayController.dayPeriod < 2) 
                                gameController.dayController.dayPeriod++;
                            else 
                                gameController.dayController.dayPeriod = 0;

                            // BLack screeen animation
                            ManageInventory(false);
                            isInInventory = false;

                            showHUD = false;
                            showActionButton = false;
                            movement.isTurn = true;
                            movement.waitDiceResult = true;

                            // Camera animation on voit le dayPeriod d'avant ecran noir puis le nouveau dayPeriod
                            movement.inventory.hasHourglass = false;

                            break;
                        case 4: // Lightning
                            cameraView = true;
                            useLightning = true;
                            camera = GameObject.FindGameObjectsWithTag("MainCamera")[0];

                            camera.transform.position = new Vector3(transform.position.x,5747.6f,transform.position.z);
                            camera.transform.rotation = Quaternion.Euler(90f,265.791f,0f);           
                            DisplayInfoText(new Vector2(971,164),new Color(1.0f,1.0f,1.0f), "Appuyez sur ECHAP pour quitter le mode");

                            ManageInventory(false);
                            isInInventory = false;

                            movement.inventory.hasLightning = false;
                            break;

                        case 5: // Star
                            transform.gameObject.GetComponent<MeshRenderer>().material.shader = gameController.invicibilityShader;
                            transform.GetChild(0).gameObject.GetComponent<MeshRenderer>().material.shader = gameController.invicibilityShader;
                            movement.audio.Invicibility();

                            ManageInventory(false);

                            showHUD = false;
                            showActionButton = false;
                            movement.isTurn = true;
                            movement.waitDiceResult = true;

                            gameController.mainCamera.transform.position = new Vector3(-454.4f,5226.9f,-15872.2f);
                            gameController.mainCamera.transform.rotation = Quaternion.Euler(0,275.83f,0f);
                            gameController.mainCamera.SetActive(true);
                            gameController.mainCamera.GetComponent<Camera>().enabled = true;
                            isInInventory = false;

                            movement.inventory.hasStar = false;
                            break;    

                        case 6: // Parachute
                            movement.agent.enabled = false;
                            movement.isParachuting = true;
                            ManageInventory(false); 
                            showHUD = false;
                            showActionButton = false;
                            isInInventory = false;

                            movement.inventory.hasParachute = false;
                            break;
                    }
                }
                else { // Le joueur n'a pas l'objet
                    if(index >= 0 && hoverInventoryItem.transform.parent.gameObject.activeSelf) {
                        DisplayInfoText(new Vector2(971,297),new Color(1.0f,0.0f,0.0f), "Vous ne possédez pas cet objet");
                        isInInventory = false;
                        StartCoroutine(InfoLabelWaiting());
                    }
                }
            }
            else {
                index = 0;
                ManageInventory(false);
                isInInventory = false;
            }
        }


    }

    public void UseObject(GameObject player,string objet) {
        DisplayInfoText(new Vector2(971,297),new Color(0f,0.53f,0.03f),player.name + "vient d'utiliser l'objet " + objet);
        StartCoroutine(InfoLabelWaiting());
    }
    public void OnMove(InputAction.CallbackContext e) {
        if(cameraView && !gameController.freeze) 
            vecMove = e.ReadValue<Vector2>();
    }

    public void OnQuit(InputAction.CallbackContext e) {
        if(e.started && cameraView && !gameController.freeze) {
            cameraView = false;
            camera.transform.position = new Vector3(-465.9f,5224f,-15847.6f);
            camera.transform.rotation = Quaternion.Euler(0f,265.791f,0f);
            infoLabel.SetActive(false);
            index = 2;
            
        }

        if(e.started && showShop && !gameController.freeze) {
            showShop = false;
            movement.returnToStep = true;
            index = -1;
        }
    }

    public void OnClickButton() {
        if(showShop && !gameController.freeze) {
            showShop = false;
            movement.returnToStep = true;
            index = -1;
        }
    }

    private void ManageInventory(bool active) {
        hoverInventoryItem.transform.parent.gameObject.SetActive(active);

        if(active)
            index = -1;

        if(hoverInventoryItem.transform.parent.gameObject.activeSelf) {
            UserInventory inventory = movement.inventory;

            bool[] hasItems = {inventory.hasDoubleDice,inventory.hasReverseDice,inventory.hasBomb,inventory.hasHourglass,inventory.hasLightning,inventory.hasStar,inventory.hasParachute};

            for(int i = 0;i<hasItems.Length;i++) {
                if(hasItems[i]) 
                    inventoryItems[i].GetChild(0).gameObject.SetActive(true);
                else 
                    inventoryItems[i].GetChild(0).gameObject.SetActive(false);
                
            }

            hoverInventoryItem.gameObject.SetActive(false);
        }
    }

    private void ManageShopHUD(bool active) {
        shopHUD.gameObject.SetActive(active);
        shopHUD.GetChild(0).gameObject.SetActive(active);
    }

    private void ManageHudState(bool active) {
        if(!gameController.GetComponent<DialogController>().isInDialog && gameController.hasGenChest) {
            int actualPlayer = gameController.actualPlayer;
            int hudIndex = 0;

            for(int i = 0;i<4;i++) {
                playersPanels[i].gameObject.SetActive(active);

                for(int j = actualPlayer;j<actualPlayer + 4;j++) {
                
                    int playerIndex = j;
                    if(j > 3) 
                        playerIndex -= 4;
                    
                    gameController.ChangeHUDSpritePlayer(playersPanels,hudIndex,gameController.players[playerIndex].name);

                    int rank = -1;
                    int rankIndex = 0;
                    foreach(GameObject player in gameController.classedPlayers.Keys) {
                        if(player == gameController.players[playerIndex]) 
                            rank = rankIndex;

                        rankIndex++;
                    }

                    if(rank >= 0 && rank < gameController.classedColors.Length && rank < gameController.classedPlayers.Keys.Count) {                        
                        playersPanels[i].GetChild(4).gameObject.GetComponent<Text>().text = gameController.GetKeyByValue(playerIndex,gameController.classedPlayers).GetComponent<UserInventory>().coins + "";
                        playersPanels[i].GetChild(6).gameObject.GetComponent<Text>().text = gameController.GetKeyByValue(playerIndex,gameController.classedPlayers).GetComponent<UserInventory>().cards + "";
                        playersPanels[i].GetChild(2).gameObject.GetComponent<Text>().text = rank + 1 + "";
                        playersPanels[i].gameObject.GetComponent<Text>().color = gameController.classedColors[rank];   
                    }
                    
                    hudIndex += 1;
                    if(hudIndex == 4) 
                        hudIndex = 0;
                }            
            }
        }
    }

    private int HasSamePoint(GameObject player) {
        int playerPoint = gameController.GetPlayerPoints(player);
        int playersIndex = 0;

        foreach(GameObject p in gameController.playerPoint.Keys) {
            if(p != player && gameController.playerPoint[p] == playerPoint) 
                playersIndex++;        
        }

        return playersIndex;
    }

    private void ManageActionButtonState(bool active) {
        for(int i = 0;i<3;i++) {
            actions[i].gameObject.SetActive(active);
            actions[i].GetChild(0).gameObject.SetActive(active);
            if(!active) 
                actions[i].GetChild(1).gameObject.SetActive(active);
        }

        
    }

    private void ManagerHudTurnState(bool active) {
        turnText.gameObject.SetActive(false);
        nightText.gameObject.SetActive(false);     
    }

    private void ManageHudDirection(bool active) {
        // front ou interior

        if(direction != null) {
            if(direction.left) {
                if(direction.nextStepLeft.name.Contains("front") || direction.nextStepLeft.name.Contains("interior")) {
                    if(movement.dayController.dayPeriod == 0 || movement.dayController.dayPeriod == 1) 
                        directions[0].gameObject.SetActive(true);
                }
                else 
                    directions[0].gameObject.SetActive(true);        
            }
            else 
                directions[0].gameObject.SetActive(false);

            if(direction.front) {
                if(direction.nextStepFront.name.Contains("front") || direction.nextStepFront.name.Contains("interior")) {
                    if(movement.dayController.dayPeriod == 0 || movement.dayController.dayPeriod == 1) 
                        directions[1].gameObject.SetActive(true);                  
                }
                else 
                    directions[1].gameObject.SetActive(true); 
            }
            else 
                directions[1].gameObject.SetActive(false);

            if(direction.right) {
                if(direction.nextStepRight.name.Contains("front") || direction.nextStepRight.name.Contains("interior")) {
                    if(movement.dayController.dayPeriod == 0 || movement.dayController.dayPeriod == 1) 
                        directions[2].gameObject.SetActive(true);           
                }
                else 
                    directions[2].gameObject.SetActive(true);         
            }
            else 
                directions[2].gameObject.SetActive(false);
        }

        if(!active) {
            directions[0].gameObject.SetActive(false);
            directions[1].gameObject.SetActive(false);
            directions[2].gameObject.SetActive(false);
        }
    }

    private void ManageChestHUD(bool active) {
        chestHUD.gameObject.SetActive(active);
    }

    private void ManageShop(bool active) {
        shopText.gameObject.transform.parent.gameObject.SetActive(active);
    }

    private IEnumerator InfoLabelWaiting() {
        yield return new WaitForSeconds(2f);
        infoLabel.SetActive(false);
        shopText.gameObject.SetActive(false);
    }

    public void RefreshDiceResult(int result,Color color) {
        if(!diceResult.activeSelf) 
            diceResult.SetActive(true);

        if(color == null) 
            color = new Color(0f,0.35f,1f,1.0f);

        diceResult.GetComponent<Text>().color = color;
        diceResult.GetComponent<Text>().text = result + "";
    }

    public void ClearDiceResult() {
        diceResult.GetComponent<Text>().text = "";
    }

    public void DisplayReward(bool bonus,int coins,bool stepReward) {

        coinIconReward.GetComponent<CoinsReward>().changePos = true;
        coinIconReward.GetComponent<CoinsReward>().stepReward = stepReward;
        coinIconReward.SetActive(true);
        coinIconReward.transform.position = new Vector3(959,315,0);
        coinIconReward.GetComponent<CoinsReward>().RunCoroutine();
       // coinTextReward.SetActive(true);  

        if(bonus) {
            coinTextReward.color = new Color(0f,0.35f,1f,1f);
            coinTextReward.text = "+" + coins;
        }
        else {
            coinTextReward.color = new Color(1.0f,0f,0f,1f);
            coinTextReward.text = "-" + coins;
        }
    }

    public int ReturnDiceResult() {
        return int.Parse(diceResult.GetComponent<Text>().text);
    }

    public bool IsDisplayingReward() {
        return coinIconReward.activeSelf;
    }

    public void ChangeTurnValue(int turn,int night) {
        turnText.text = "Tour: " + turn;
        nightText.text = "Nuit dans " + night + " tour(s)";
    }

    private void DisplayInfoText(Vector2 pos,Color color,string text) {
        infoLabel.transform.position = pos;
        infoLabel.GetComponent<Text>().color = color;
        infoLabel.GetComponent<Text>().text = text;
        infoLabel.GetComponent<AlphaController>().manageAlpha = true;
        infoLabel.SetActive(true);
    }

    private void DisplayShopText(string text,Color color) {
        shopText.gameObject.GetComponent<Text>().text = text;
        shopText.gameObject.GetComponent<Text>().color = color;
    }
}
