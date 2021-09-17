using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using UnityEngine.AI;

public class UserUI : CoroutineSystem {

    public GameObject hudParent;
    public GameObject shopParent;
    public GameObject inventoryParent;
    public GameObject islesParent;
    public GameObject infoLabel;
    public GameObject diceResult;
    public GameObject coinIconReward;
    public GameObject coinTextReward;
    public GameController gameController;

    public bool showHUD;
    public bool showTurnInfo;
    public bool showActionButton;
    public bool showDirection;
    public bool showChestHUD;
    public bool showShopHUD;
    public bool showShop;

    public Direction direction;

    public int index = -1;
    private int nextShopId = -1;
    public bool cameraView;
    public GameObject camera;
    private Vector2 vecMove;
    private float cameraSpeed = 200f;

    public GameObject bombPos;

    public bool useBomb;
    public bool useLightning;

    public Animation bsAnimation;

    public GameObject lightning;
    public ParticleSystem[] lightningParticles;

    private bool isInInventory;

    public UserMovement movement;

// Check que quand c'est pas le tour d'un joueur tt soit désactiver

    void Update() {  
        if(!gameController.freeze) {
            ManageHudState(showHUD);
            ManageCameraPosition();
            ManagerHudTurnState(showTurnInfo && gameController.GetPart() == GameController.GamePart.PARTYGAME);
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
        bombPos.SetActive(useBomb || useLightning); 
        bombPos.transform.position = new Vector3(camera.transform.position.x,bombPos.transform.position.y,camera.transform.position.z);
    }

    public void OnRight(InputAction.CallbackContext e) {
        if(e.started && !cameraView && showHUD && !inventoryParent.activeSelf && !gameController.freeze) {
            if(index < 2) 
                index++;

            switch(index) {
                case 0:
                    hudParent.transform.GetChild(4).GetChild(0).gameObject.SetActive(false);
                    hudParent.transform.GetChild(4).GetChild(1).gameObject.SetActive(true);
                    break;
                case 1:
                    hudParent.transform.GetChild(4).GetChild(0).gameObject.SetActive(true);
                    hudParent.transform.GetChild(4).GetChild(1).gameObject.SetActive(false);

                    hudParent.transform.GetChild(5).GetChild(0).gameObject.SetActive(false);
                    hudParent.transform.GetChild(5).GetChild(1).gameObject.SetActive(true);
                    break;
                case 2:
                    hudParent.transform.GetChild(5).GetChild(0).gameObject.SetActive(true);
                    hudParent.transform.GetChild(5).GetChild(1).gameObject.SetActive(false);

                    hudParent.transform.GetChild(6).GetChild(0).gameObject.SetActive(false);
                    hudParent.transform.GetChild(6).GetChild(1).gameObject.SetActive(true);
                    break;        
            }
        }

        if(e.started && !cameraView && !showHUD && showDirection && !gameController.freeze) {
            int max = -1;

            if(hudParent.transform.GetChild(10).gameObject.activeSelf) 
                max = 0;
            if(hudParent.transform.GetChild(11).gameObject.activeSelf) 
                max = 1;
            if(hudParent.transform.GetChild(12).gameObject.activeSelf) 
                max = 2;
            
            if(!hudParent.transform.GetChild(10).GetChild(1).gameObject.activeSelf && !hudParent.transform.GetChild(11).GetChild(1).gameObject.activeSelf && !hudParent.transform.GetChild(12).GetChild(1).gameObject.activeSelf) {
                if(hudParent.transform.GetChild(10).gameObject.activeSelf) 
                    index = -1;
                else if(hudParent.transform.GetChild(11).gameObject.activeSelf) 
                    index = 0;
                else if(hudParent.transform.GetChild(12).gameObject.activeSelf) 
                    index = 1;        
                
            }

            if(index < max) 
                index++;

            switch(index) {
                case 0:
                    hudParent.transform.GetChild(10).GetChild(0).gameObject.SetActive(false);
                    hudParent.transform.GetChild(10).GetChild(1).gameObject.SetActive(true);
                    break;

                case 1:
                    
                    hudParent.transform.GetChild(10).GetChild(0).gameObject.SetActive(true);
                    hudParent.transform.GetChild(10).GetChild(1).gameObject.SetActive(false);

                    hudParent.transform.GetChild(11).GetChild(0).gameObject.SetActive(false);
                    hudParent.transform.GetChild(11).GetChild(1).gameObject.SetActive(true);
                    break;

                case 2:
                    hudParent.transform.GetChild(11).GetChild(0).gameObject.SetActive(true);
                    hudParent.transform.GetChild(11).GetChild(1).gameObject.SetActive(false);

                    hudParent.transform.GetChild(12).GetChild(0).gameObject.SetActive(false);
                    hudParent.transform.GetChild(12).GetChild(1).gameObject.SetActive(true);
                    break;        
            }
        }

        if(e.started && showChestHUD && !gameController.freeze) {
            if(index < 1) 
                index++;

            switch(index) {
                case 0:
                    hudParent.transform.GetChild(13).GetChild(1).gameObject.SetActive(true);
                    break;

                case 1:
                    hudParent.transform.GetChild(13).GetChild(1).gameObject.SetActive(false);

                    hudParent.transform.GetChild(13).GetChild(2).gameObject.SetActive(true);
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
                shopParent.transform.GetChild(i+2).GetChild(3).gameObject.SetActive(false); 

            shopParent.transform.GetChild(2 + nextShopId).GetChild(3).gameObject.SetActive(true);

            if(index <= 2) {
                shopParent.transform.GetChild(1).GetChild(0).gameObject.GetComponent<Text>().text = gameController.GetShopItems()[index].description;
                shopParent.transform.GetChild(2 + nextShopId).GetChild(1).GetChild(0).gameObject.GetComponent<Text>().text = "" + gameController.GetShopItems()[index].price;
                shopParent.transform.GetChild(2 + nextShopId).GetChild(2).gameObject.GetComponent<Text>().text = gameController.GetShopItems()[index].name;
                shopParent.transform.GetChild(2 + nextShopId).GetChild(0).gameObject.GetComponent<RectTransform>().sizeDelta = gameController.GetShopItems()[index].dimension;

            }

            if(index > 2) {
                int begin = 2;

                for(int i = index;i>index - 3;i--) {
                    shopParent.transform.GetChild(2 + begin).GetChild(0).gameObject.GetComponent<Image>().sprite = gameController.GetShopItems()[i].img;
                    shopParent.transform.GetChild(1).GetChild(0).gameObject.GetComponent<Text>().text = gameController.GetShopItems()[i].description;
                    shopParent.transform.GetChild(2 + begin).GetChild(1).GetChild(0).gameObject.GetComponent<Text>().text = "" + gameController.GetShopItems()[i].price;
                    shopParent.transform.GetChild(2 + begin).GetChild(2).gameObject.GetComponent<Text>().text = gameController.GetShopItems()[i].name;
                    begin--;
                }
            }

        }

        if(e.started && showShopHUD && !gameController.freeze) {
            if(index < 1) 
                index++;

            switch(index) {
                case 0:
                    hudParent.transform.GetChild(15).GetChild(1).gameObject.SetActive(true);
                    break;

                case 1:
                    hudParent.transform.GetChild(15).GetChild(1).gameObject.SetActive(false);
                    hudParent.transform.GetChild(15).GetChild(2).gameObject.SetActive(true);
                    break;    
            }
        }

        if(e.started && inventoryParent.activeSelf && isInInventory && !gameController.freeze) {
            if(index < 7) 
                index++;

            Vector2[] hoverPos = {new Vector2(-444,31),new Vector2(-299,31),new Vector2(-149,31),new Vector2(5,31),new Vector2(145,31),new Vector2(305,31),new Vector2(448,31),new Vector2(583,31)};
            
            inventoryParent.transform.GetChild(9).gameObject.SetActive(true);
            inventoryParent.transform.GetChild(9).localPosition = hoverPos[index];       
        }
    }

    public void OnLeft(InputAction.CallbackContext e) {
        if(e.started && !cameraView && showHUD && !inventoryParent.activeSelf && !gameController.freeze) {
            if(index > 0) 
                index--;

            switch(index) {
                case 0:
                    hudParent.transform.GetChild(5).GetChild(0).gameObject.SetActive(true);
                    hudParent.transform.GetChild(5).GetChild(1).gameObject.SetActive(false);

                    hudParent.transform.GetChild(4).GetChild(0).gameObject.SetActive(false);
                    hudParent.transform.GetChild(4).GetChild(1).gameObject.SetActive(true);
                    break;
                case 1:
                    hudParent.transform.GetChild(6).GetChild(0).gameObject.SetActive(true);
                    hudParent.transform.GetChild(6).GetChild(1).gameObject.SetActive(false);

                    hudParent.transform.GetChild(5).GetChild(0).gameObject.SetActive(false);
                    hudParent.transform.GetChild(5).GetChild(1).gameObject.SetActive(true);
                    break;
                case 2:
                    hudParent.transform.GetChild(6).GetChild(0).gameObject.SetActive(false);
                    hudParent.transform.GetChild(6).GetChild(1).gameObject.SetActive(true);
                    break;        
            }
        }

        if(e.started && !cameraView && !showHUD && showDirection && !gameController.freeze) {
            int min = -1;

            if(hudParent.transform.GetChild(10).gameObject.activeSelf) min = 0;
            else if(hudParent.transform.GetChild(11).gameObject.activeSelf) min = 1;
            else if(hudParent.transform.GetChild(12).gameObject.activeSelf) min = 2;
            
            if(index > min) 
                index--;

            switch(index) {
                case 0:
                    hudParent.transform.GetChild(11).GetChild(0).gameObject.SetActive(true);
                    hudParent.transform.GetChild(11).GetChild(1).gameObject.SetActive(false);

                    hudParent.transform.GetChild(10).GetChild(0).gameObject.SetActive(false);
                    hudParent.transform.GetChild(10).GetChild(1).gameObject.SetActive(true);
                    break;
                case 1:
                    hudParent.transform.GetChild(12).GetChild(0).gameObject.SetActive(true);
                    hudParent.transform.GetChild(12).GetChild(1).gameObject.SetActive(false);

                    hudParent.transform.GetChild(11).GetChild(0).gameObject.SetActive(false);
                    hudParent.transform.GetChild(11).GetChild(1).gameObject.SetActive(true);
                    break;
                case 2:
                    hudParent.transform.GetChild(12).GetChild(0).gameObject.SetActive(false);
                    hudParent.transform.GetChild(12).GetChild(1).gameObject.SetActive(true);
                    break;        
            }
        }

        if(e.started && showChestHUD && !gameController.freeze) {
            if(index > 0) 
                index--;

            switch(index) {
                case 0:
                    hudParent.transform.GetChild(13).GetChild(1).gameObject.SetActive(true);
                    hudParent.transform.GetChild(13).GetChild(2).gameObject.SetActive(false);
                    break;

                case 1:
                    hudParent.transform.GetChild(13).GetChild(1).gameObject.SetActive(false);
                    hudParent.transform.GetChild(13).GetChild(2).gameObject.SetActive(true);
                    break;    
            }
        }

        if(e.started && showShop && !gameController.freeze) { 

            for(int i = 0;i<3;i++) 
                shopParent.transform.GetChild(i+2).GetChild(3).gameObject.SetActive(false);

            int begin = 2; 

            if(index >= 2) {
                for(int i = index;i>index - 3;i--) {
                        
                    shopParent.transform.GetChild(2 + begin).GetChild(2).gameObject.GetComponent<Text>().text = gameController.GetShopItems()[i].name;
                    shopParent.transform.GetChild(2 + begin).GetChild(0).gameObject.GetComponent<Image>().sprite = gameController.GetShopItems()[i].img;
                    shopParent.transform.GetChild(2 + nextShopId).GetChild(1).GetChild(0).gameObject.GetComponent<Text>().text = "" + gameController.GetShopItems()[i].price;
                    shopParent.transform.GetChild(2 + nextShopId).GetChild(0).gameObject.GetComponent<RectTransform>().sizeDelta = gameController.GetShopItems()[index].dimension;
                    begin--;
                        
                }
            }
            else if(index == 1 || index == 0){
                    
                for(int i = 0;i<3;i++) {
                    shopParent.transform.GetChild(2+i).GetChild(0).gameObject.GetComponent<Image>().sprite = gameController.GetShopItems()[i].img;
                    shopParent.transform.GetChild(2 + i).GetChild(2).gameObject.GetComponent<Text>().text = gameController.GetShopItems()[i].name;
                    shopParent.transform.GetChild(2 + i).GetChild(1).GetChild(0).gameObject.GetComponent<Text>().text = "" + gameController.GetShopItems()[i].price;
                    shopParent.transform.GetChild(2 + nextShopId).GetChild(0).gameObject.GetComponent<RectTransform>().sizeDelta = gameController.GetShopItems()[index].dimension; 
                }
            }
            
            if(index > 0) {
                index--;
                movement.audio.ButtonHover();
            }
            if(nextShopId > 0) 
                nextShopId--;
            
            if(index == 0) 
                shopParent.transform.GetChild(2).GetChild(3).gameObject.SetActive(true);
            else if(index >= 1 && index < 6) 
                shopParent.transform.GetChild(3).GetChild(3).gameObject.SetActive(true);
            else if(index == 6) 
                shopParent.transform.GetChild(4).GetChild(3).gameObject.SetActive(true);

        }

        if(e.started && showShopHUD && !gameController.freeze) {
            if(index > 0) 
                index--;

            switch(index) {
                case 0:
                    hudParent.transform.GetChild(15).GetChild(1).gameObject.SetActive(true);
                    hudParent.transform.GetChild(15).GetChild(2).gameObject.SetActive(false);
                    break;

                case 1: 
                    hudParent.transform.GetChild(15).GetChild(2).gameObject.SetActive(true);
                    break;    
            }
        }

        if(e.started && inventoryParent.activeSelf && !gameController.freeze) {
            if(index > 0) 
                index--;

            Vector2[] hoverPos = {new Vector2(-444,31),new Vector2(-299,31),new Vector2(-149,31),new Vector2(5,31),new Vector2(145,31),new Vector2(305,31),new Vector2(448,31),new Vector2(583,31)};
            
            inventoryParent.transform.GetChild(9).gameObject.SetActive(true);
            inventoryParent.transform.GetChild(9).localPosition = hoverPos[index];
        }
    }

    public void OnInteract(InputAction.CallbackContext e) {
        if(e.started && !inventoryParent.activeSelf &&  cameraView && useBomb && !gameController.freeze) {
            // Bombe
            
            float x = bombPos.transform.position.x;
            float z = bombPos.transform.position.z;

            // On peut détruire un pont alors qu'il est en rouge il me semble avec ca

            if((x >= -830.37f && z >= -15509.91f && x <= -810.06f && z <= -15372.94f) || (x <= -998.031f && z <= -15285.96f && x >= -1133.05f && z >= -15304.83f)) {
                bool alreadyBreak = false;
                if(x >= -830.37f && z >= -15509.91f && x <= -810.06f && z <= -15372.94f)
                    alreadyBreak = islesParent.transform.GetChild(2).GetChild(0).gameObject.GetComponent<Bridge>().breakBridge;
                else if(x <= -998.031f && z <= -15285.96f && x >= -1133.05f && z >= -15304.83f)
                    alreadyBreak = islesParent.transform.GetChild(3).GetChild(0).gameObject.GetComponent<Bridge>().breakBridge;

                if(!alreadyBreak) {
                    ResetBombMat();
                    GameObject bomb = Instantiate(gameController.GetPrefabObjects()[2],new Vector3(x,camera.transform.position.y,z),gameController.GetPrefabObjects()[2].transform.rotation);
                    movement.audio.BombFall();
                    bombPos.SetActive(false);
                    infoLabel.SetActive(false);
                }
                else 
                    movement.audio.BuyLoose();
            }
            else 
                movement.audio.BuyLoose();
        }

        if(e.started && !inventoryParent.activeSelf &&  cameraView && useLightning && !gameController.freeze) {
            // Lightning

        }        
        if(e.started && showDirection && !gameController.freeze) {
            if(hudParent.transform.GetChild(10).GetChild(1).gameObject.activeSelf) {
                movement.left = true;
                hudParent.transform.GetChild(10).GetChild(0).gameObject.SetActive(true);
                hudParent.transform.GetChild(10).GetChild(1).gameObject.SetActive(false); 
            }
            else if(hudParent.transform.GetChild(11).GetChild(1).gameObject.activeSelf) {
                gmovement.front = true;
                hudParent.transform.GetChild(11).GetChild(0).gameObject.SetActive(true);
                hudParent.transform.GetChild(11).GetChild(1).gameObject.SetActive(false);
            }
            else if(hudParent.transform.GetChild(12).GetChild(1).gameObject.activeSelf) {
                movement.right = true;
                hudParent.transform.GetChild(12).GetChild(0).gameObject.SetActive(true);
                hudParent.transform.GetChild(12).GetChild(1).gameObject.SetActive(false);
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
            shopParent.transform.GetChild(5).gameObject.SetActive(true);

            if(actualCoins >= gameController.GetShopItems()[index].price) { // Assez d'argent

                switch(index) {
                    case 0: // Dé double
                        bool hasDoubleDice = movement.inventory.hasDoubleDice;

                        if(!hasDoubleDice) { // Le joueur n'a pas encore l'objet
                            movement.inventory.hasDoubleDice = true;
                            // Affichez l'ui
                            shopParent.transform.GetChild(5).gameObject.GetComponent<Text>().text = "Achat effectué avec succès";
                            shopParent.transform.GetChild(5).gameObject.GetComponent<Text>().color = new Color(0,1.0f,0.12f,1.0f);
                            StartCoroutine(movement.WaitMalus(gameController.GetShopItems()[index].price,false));
                            StartCoroutine(InfoLabelWaiting());
                        }
                        else { // Le joueur a déjà cet objet
                            movement.audio.BuyLoose();
                            shopParent.transform.GetChild(5).gameObject.GetComponent<Text>().text = "Vous possédez déjà cet objet";
                            shopParent.transform.GetChild(5).gameObject.GetComponent<Text>().color = new Color(1.0f,0f,0f,1.0f);
                            StartCoroutine(InfoLabelWaiting());
                        }
                        break;

                    case 1: // Dé inverse
                        bool hasReverseDice = movement.inventory.hasReverseDice;

                        if(!hasReverseDice) {
                            movement.inventory.hasReverseDice = true;
                            //Affichez ui
                            shopParent.transform.GetChild(5).gameObject.GetComponent<Text>().text = "Achat effectué avec succès";
                            shopParent.transform.GetChild(5).gameObject.GetComponent<Text>().color = new Color(0,1.0f,0.12f,1.0f);
                            StartCoroutine(movement.WaitMalus(gameController.GetShopItems()[index].price,false));
                            StartCoroutine(InfoLabelWaiting());
                        }
                        else {
                            movement.audio.BuyLoose();
                            shopParent.transform.GetChild(5).gameObject.GetComponent<Text>().text = "Vous possédez déjà cet objet";
                            shopParent.transform.GetChild(5).gameObject.GetComponent<Text>().color = new Color(1.0f,0f,0f,1.0f);
                            StartCoroutine(InfoLabelWaiting());
                        }
                        break;

                    case 2: // Bombe
                        bool hasBomb = movement.inventory.hasBomb;

                        if(!hasBomb) {
                            movement.inventory.hasBomb = true;
                            // Affichez ui
                            shopParent.transform.GetChild(5).gameObject.GetComponent<Text>().text = "Achat effectué avec succès";
                            shopParent.transform.GetChild(5).gameObject.GetComponent<Text>().color = new Color(0,1.0f,0.12f,1.0f);
                            StartCoroutine(movement.WaitMalus(gameController.GetShopItems()[index].price,false));
                            StartCoroutine(InfoLabelWaiting());
                        }   
                        else {
                            movement.audio.BuyLoose();
                            shopParent.transform.GetChild(5).gameObject.GetComponent<Text>().text = "Vous possédez déjà cet objet";
                            shopParent.transform.GetChild(5).gameObject.GetComponent<Text>().color = new Color(1.0f,0f,0f,1.0f);
                            StartCoroutine(InfoLabelWaiting());
                        }
                        break;

                    case 3: // Sablier
                        bool hasHourglass = movement.inventory.hasHourglass;

                        if(!hasHourglass) {
                            movement.inventory.hasHourglass = true;
                            // Affichez ui
                            shopParent.transform.GetChild(5).gameObject.GetComponent<Text>().text = "Achat effectué avec succès";
                            shopParent.transform.GetChild(5).gameObject.GetComponent<Text>().color = new Color(0,1.0f,0.12f,1.0f);
                            StartCoroutine(movement.WaitMalus(gameController.GetShopItems()[index].price,false));
                            StartCoroutine(InfoLabelWaiting());
                        }
                        else {
                            movement.audio.BuyLoose();
                            shopParent.transform.GetChild(5).gameObject.GetComponent<Text>().text = "Vous possédez déjà cet objet";
                            shopParent.transform.GetChild(5).gameObject.GetComponent<Text>().color = new Color(1.0f,0f,0f,1.0f);
                            StartCoroutine(InfoLabelWaiting());
                        }
                        break;

                    case 4: // Eclair
                        bool hasLightning = movement.inventory.hasLightning;

                        if(!hasLightning) {
                            movement.inventory.hasLightning = true;
                            // Affichez ui
                            shopParent.transform.GetChild(5).gameObject.GetComponent<Text>().text = "Achat effectué avec succès";
                            shopParent.transform.GetChild(5).gameObject.GetComponent<Text>().color = new Color(0,1.0f,0.12f,1.0f);
                            StartCoroutine(movement.WaitMalus(gameController.GetShopItems()[index].price,false));
                            StartCoroutine(InfoLabelWaiting());
                        }
                        else {
                            movement.audio.BuyLoose();
                            shopParent.transform.GetChild(5).gameObject.GetComponent<Text>().text = "Vous possédez déjà cet objet";
                            shopParent.transform.GetChild(5).gameObject.GetComponent<Text>().color = new Color(1.0f,0f,0f,1.0f);
                            StartCoroutine(InfoLabelWaiting());
                        }
                        break;

                    case 5: // Etoile 
                        bool hasStar = movement.inventory.hasStar;

                        if(!hasStar) {
                            movement.inventory.hasStar = true;
                            // Affichez ui
                            shopParent.transform.GetChild(5).gameObject.GetComponent<Text>().text = "Achat effectué avec succès";
                            shopParent.transform.GetChild(5).gameObject.GetComponent<Text>().color = new Color(0,1.0f,0.12f,1.0f);
                            StartCoroutine(movement.WaitMalus(gameController.GetShopItems()[index].price,false));
                            StartCoroutine(InfoLabelWaiting());
                        }
                        else {
                            movement.audio.BuyLoose();
                            shopParent.transform.GetChild(5).gameObject.GetComponent<Text>().text = "Vous possédez déjà cet objet";
                            shopParent.transform.GetChild(5).gameObject.GetComponent<Text>().color = new Color(1.0f,0f,0f,1.0f);
                            StartCoroutine(InfoLabelWaiting());
                        }
                        break;

                    case 6: // Parachute
                        bool hasParachute = movement.inventory.hasParachute;

                        if(!hasParachute) {
                            movement.inventory.hasParachute = true;
                           
                            shopParent.transform.GetChild(5).gameObject.GetComponent<Text>().text = "Achat effectué avec succès";
                            shopParent.transform.GetChild(5).gameObject.GetComponent<Text>().color = new Color(0,1.0f,0.12f,1.0f);
                            StartCoroutine(movement.WaitMalus(gameController.GetShopItems()[index].price,false));
                            StartCoroutine(InfoLabelWaiting());
                        }
                        else {
                            movement.audio.BuyLoose();
                            shopParent.transform.GetChild(5).gameObject.GetComponent<Text>().text = "Vous possédez déjà cet objet";
                            shopParent.transform.GetChild(5).gameObject.GetComponent<Text>().color = new Color(1.0f,0f,0f,1.0f);
                            StartCoroutine(InfoLabelWaiting());
                        }
                        break;
                }
            }
            
            else { // Pas assez d'argent
                movement.audio.BuyLoose();
                shopParent.transform.GetChild(5).gameObject.GetComponent<Text>().text = "Vous n'avez pas assez d'argent";
                shopParent.transform.GetChild(5).gameObject.GetComponent<Text>().color = new Color(1.0f,0f,0f,1.0f);
                StartCoroutine(InfoLabelWaiting());
            }

            
        }

        if(e.started && showShopHUD && !gameController.freeze) {

            switch(index) {
                case 0:
                    movement.stop = false;
                    if(movement.diceResult <= 0) gameController.EndUserTurn();
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

        if(e.started && inventoryParent.activeSelf && isInInventory && !cameraView && !gameController.freeze) {
            if(index <= 6 && index > -1) {
        
                if(inventoryParent.transform.childCount > (1+index) && inventoryParent.transform.GetChild(1 + index).childCount > 0 && inventoryParent.transform.GetChild(1 + index).GetChild(0).gameObject.activeSelf) { // Le joueur a l'objet 

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
                            movement.hasReverseDice = false;
                            break;

                        case 2: // Bomb
                            cameraView = true;
                            useBomb = true;
                            isInInventory = false;
                            camera = GameObject.FindGameObjectsWithTag("MainCamera")[0];

                            camera.transform.position = new Vector3(transform.position.x,5747.6f,transform.position.z);
                            camera.transform.rotation = Quaternion.Euler(90f,265.791f,0f); 

                            DisplayInfoText(new Vector2(971,164),new Color(1.0f,1.0f,1.0f), "Appuyez sur ECHAP pour quitter le mode");
                            
                            ManageInventory(false);// Changez le mat des iles et du bridge
                            ApplyBombMat();
                            movement.inventory.hasBomb = false;

                            break;
                        case 3: //Hourglass
                            if(gameController.GetDayController().dayPeriod < 2) gameController.GetDayController().dayPeriod++;
                            else gameController.GetDayController().dayPeriod = 0;
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
                            transform.gameObject.GetComponent<MeshRenderer>().material.shader = gameController.GetInvicibilityShader();
                            transform.GetChild(0).gameObject.GetComponent<MeshRenderer>().material.shader = gameController.GetInvicibilityShader();
                            movement.audio.Invicibility();

                            ManageInventory(false);

                            showHUD = false;
                            showActionButton = false;
                            movement.isTurn = true;
                            movement.waitDiceResult = true;

                            gameController.getMainCamera().transform.position = new Vector3(-454.4f,5226.9f,-15872.2f);
                            gameController.getMainCamera().transform.rotation = Quaternion.Euler(0,275.83f,0f);
                            gameController.getMainCamera().SetActive(true);
                            gameController.getMainCamera().GetComponent<Camera>().enabled = true;
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
                    if(index >= 0 && inventoryParent.activeSelf) {
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

            if(useBomb) 
                ResetBombMat();
            
        }

        if(e.started && gameController.GetPlayers()[0].GetComponent<UserUI>().showShop && !gameController.freeze) {
            showShop = false;
            // Lui faire revenir sur la step
            movement.returnToStep = true;
            index = -1;
        }

        //index = -1;
    }

    public void OnClickButton() {
        if(showShop && !gameController.freeze) {
            showShop = false;
            // Lui faire rentrer sur la step
            movement.returnToStep = true;
            index = -1;
        }
    }

    private void ApplyBombMat() {
        for(int i = 0;i<islesParent.transform.GetChild(2).childCount;i++) 
            islesParent.transform.GetChild(2).GetChild(i).gameObject.GetComponent<MeshRenderer>().material = gameController.GetBombIsleMat()[0];
        
        for(int i = 0;i<islesParent.transform.GetChild(3).childCount;i++) 
            islesParent.transform.GetChild(3).GetChild(i).gameObject.GetComponent<MeshRenderer>().material = gameController.GetBombIsleMat()[0];
        
        for(int i = 0;i<islesParent.transform.GetChild(4).childCount;i++) 
            islesParent.transform.GetChild(4).GetChild(i).gameObject.GetComponent<MeshRenderer>().material = gameController.GetBombIsleMat()[0];
        
                            
        islesParent.transform.GetChild(0).gameObject.GetComponent<MeshRenderer>().material = gameController.GetBombIsleMat()[1];
        islesParent.transform.GetChild(1).gameObject.GetComponent<MeshRenderer>().material = gameController.GetBombIsleMat()[2];

        Bridge bridge01 = islesParent.transform.GetChild(2).GetChild(0).gameObject.GetComponent<Bridge>();
        Bridge bridge02 = islesParent.transform.GetChild(3).GetChild(0).gameObject.GetComponent<Bridge>();

        if(!bridge01.breakBridge) 
            islesParent.transform.GetChild(2).GetChild(0).gameObject.GetComponent<MeshRenderer>().material = gameController.GetBombIsleMat()[3];
        else 
            islesParent.transform.GetChild(2).GetChild(0).gameObject.GetComponent<MeshRenderer>().material = gameController.GetBombIsleMat()[4];

        if(!bridge02.breakBridge) 
            islesParent.transform.GetChild(3).GetChild(0).gameObject.GetComponent<MeshRenderer>().material = gameController.GetBombIsleMat()[3];
        else
            islesParent.transform.GetChild(3).GetChild(0).gameObject.GetComponent<MeshRenderer>().material = gameController.GetBombIsleMat()[4];
        
    }

    private void ResetBombMat() {

        for(int i = 0;i<islesParent.transform.GetChild(2).childCount;i++) 
            islesParent.transform.GetChild(2).GetChild(i).gameObject.GetComponent<MeshRenderer>().material = gameController.GetIsleMat()[3];  

        for(int i = 0;i<islesParent.transform.GetChild(3).childCount;i++) 
            islesParent.transform.GetChild(3).GetChild(i).gameObject.GetComponent<MeshRenderer>().material = gameController.GetIsleMat()[3];
        
        for(int i = 0;i<islesParent.transform.GetChild(4).childCount;i++) 
            islesParent.transform.GetChild(4).GetChild(i).gameObject.GetComponent<MeshRenderer>().material = gameController.GetIsleMat()[0];
        
                            
        islesParent.transform.GetChild(0).gameObject.GetComponent<MeshRenderer>().material = gameController.GetIsleMat()[2];
        islesParent.transform.GetChild(1).gameObject.GetComponent<MeshRenderer>().material = gameController.GetIsleMat()[1];
        
      //  islesParent.transform.GetChild(2).GetChild(0).gameObject.GetComponent<MeshRenderer>().material = gameController.GetIsleMat()[3];
      //  islesParent.transform.GetChild(3).GetChild(0).gameObject.GetComponent<MeshRenderer>().material = gameController.GetIsleMat()[3];
    }


    private void ManageInventory(bool active) {
        inventoryParent.SetActive(active);

        if(active)
            index = -1;

        if(inventoryParent.activeSelf) {
            UserInventory inventory = movement.inventory;

            bool[] hasItems = {inventory.hasDoubleDice,inventory.hasReverseDice,inventory.hasBomb,inventory.hasHourglass,inventory.hasLightning,inventory.hasStar,inventory.hasParachute};

            for(int i = 0;i<hasItems.Length;i++) {
                if(hasItems[i]) 
                    inventoryParent.transform.GetChild(1 + i).GetChild(0).gameObject.SetActive(true);
                else 
                    inventoryParent.transform.GetChild(1 + i).GetChild(0).gameObject.SetActive(false);
                
            }

            inventoryParent.transform.GetChild(9).gameObject.SetActive(false);
        }
    }

    private void ManageShopHUD(bool active) {
        hudParent.transform.GetChild(15).gameObject.SetActive(active);
        hudParent.transform.GetChild(15).GetChild(0).gameObject.SetActive(active);
    }

    private void ManageHudState(bool active) {
        if(!gameController.GetComponent<DialogController>().isInDialog && gameController.hasGenChest) {
            int actualPlayer = gameController.GetActualPlayer();
            int hudIndex = 0;

            for(int i = 0;i<4;i++) {
                hudParent.transform.GetChild(i).gameObject.SetActive(active);

              /*  

                hudParent.transform.GetChild(i).GetChild(4).gameObject.GetComponent<Text>().text = gameController.GetKeyByValue(i,gameController.GetClassedPlayers()).GetComponent<UserInventory>().coins + "";
                hudParent.transform.GetChild(i).GetChild(6).gameObject.GetComponent<Text>().text = gameController.GetKeyByValue(i,gameController.GetClassedPlayers()).GetComponent<UserInventory>().cards + "";
                hudParent.transform.GetChild(i).GetChild(2).gameObject.GetComponent<Text>().text = rank + 1 + "";
                hudParent.transform.GetChild(i).GetChild(2).gameObject.GetComponent<Text>().color = gameController.GetClassedColors()[rank];
                    
*/

                for(int j = actualPlayer;j<actualPlayer + 4;j++) {
                
                    int playerIndex = j;
                    if(j > 3) 
                        playerIndex -= 4;
                    
                    gameController.ChangeHUDSpritePlayer(hudParent,hudIndex,gameController.GetPlayers()[playerIndex].name);

                    int rank = -1;
                    int rankIndex = 0;
                    foreach(GameObject player in gameController.GetClassedPlayers().Keys) {
                        if(player == gameController.GetPlayers()[playerIndex]) 
                            rank = rankIndex;

                        rankIndex++;
                    }

                    if(rank >= 0 && rank < gameController.GetClassedColors().Length && rank < gameController.GetClassedPlayers().Keys.Count) {
                      /*  int playerPoint = gameController.GetPlayerPoints(gameController.GetPlayers()[playerIndex]);
                        int samePoint = HasSamePoint(gameController.GetPlayers()[playerIndex]);

                        if(samePoint > 0) {
                            foreach(GameObject player in gameController.GetClassedPlayers().Keys) {
                                if(gameController.GetPlayerPoints(player) == playerPoint) {
                                    rank = gameController.FindPlayerClassement(player);
                                    break;
                                }
                            } 
                        } */

                            
                        hudParent.transform.GetChild(hudIndex).GetChild(4).gameObject.GetComponent<Text>().text = gameController.GetKeyByValue(playerIndex,gameController.GetClassedPlayers()).GetComponent<UserInventory>().coins + "";
                        hudParent.transform.GetChild(hudIndex).GetChild(6).gameObject.GetComponent<Text>().text = gameController.GetKeyByValue(playerIndex,gameController.GetClassedPlayers()).GetComponent<UserInventory>().cards + "";
                        hudParent.transform.GetChild(hudIndex).GetChild(2).gameObject.GetComponent<Text>().text = rank + 1 + "";
                        hudParent.transform.GetChild(hudIndex).GetChild(2).gameObject.GetComponent<Text>().color = gameController.GetClassedColors()[rank];
                    
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

        foreach(GameObject p in gameController.GetPlayersPoints().Keys) {
            if(p != player && gameController.GetPlayersPoints()[p] == playerPoint) 
                playersIndex++;        
        }

        return playersIndex;
    }

    private void ManageActionButtonState(bool active) {
        for(int i = 0;i<3;i++) {
            hudParent.transform.GetChild(4 + i).gameObject.SetActive(active);
            hudParent.transform.GetChild(4 + i).GetChild(0).gameObject.SetActive(active);
            if(!active) 
                hudParent.transform.GetChild(4 + i).GetChild(1).gameObject.SetActive(active);
        }

        
    }

    private void ManagerHudTurnState(bool active) {
        for(int i = 0;i<2;i++) 
            hudParent.transform.GetChild(7 + i).gameObject.SetActive(active);       
    }

    private void ManageHudDirection(bool active) {
        // front ou interior

        if(direction != null) {
            if(direction.left) {
                if(direction.nextStepLeft.name.Contains("front") || direction.nextStepLeft.name.Contains("interior")) {
                    if(movement.dayController.dayPeriod == 0 || movement.dayController.dayPeriod == 1) 
                        hudParent.transform.GetChild(10).gameObject.SetActive(true);
                }
                else 
                    hudParent.transform.GetChild(10).gameObject.SetActive(true);        
            }
            else 
                hudParent.transform.GetChild(10).gameObject.SetActive(false);

            if(direction.front) {
                if(direction.nextStepFront.name.Contains("front") || direction.nextStepFront.name.Contains("interior")) {
                    if(islesParent.transform.GetChild(3).GetChild(0).gameObject.GetComponent<Bridge>().breakBridge)
                        return;

                    if(movement.dayController.dayPeriod == 0 || movement.dayController.dayPeriod == 1) 
                        hudParent.transform.GetChild(11).gameObject.SetActive(true);                  
                }
                else {
                    if(islesParent.transform.GetChild(3).GetChild(0).gameObject.GetComponent<Bridge>().breakBridge) 
                        return;

                    hudParent.transform.GetChild(11).gameObject.SetActive(true);
                }
            }
            else 
                hudParent.transform.GetChild(11).gameObject.SetActive(false);

            if(direction.right) {
                if(direction.nextStepRight.name.Contains("front") || direction.nextStepRight.name.Contains("interior")) {
                    if(islesParent.transform.GetChild(2).GetChild(0).gameObject.GetComponent<Bridge>().breakBridge) 
                        return;

                    if(movement.dayController.dayPeriod == 0 || movement.dayController.dayPeriod == 1) 
                        hudParent.transform.GetChild(12).gameObject.SetActive(true);           
                }
                else {
                    if(islesParent.transform.GetChild(2).GetChild(0).gameObject.GetComponent<Bridge>().breakBridge) 
                        return;

                    hudParent.transform.GetChild(12).gameObject.SetActive(true);
                }
            }
            else 
                hudParent.transform.GetChild(12).gameObject.SetActive(false);
        }

        if(!active) {
            hudParent.transform.GetChild(10).gameObject.SetActive(false);
            hudParent.transform.GetChild(11).gameObject.SetActive(false);
            hudParent.transform.GetChild(12).gameObject.SetActive(false);
        }
    }

    private void ManageChestHUD(bool active) {
        hudParent.transform.GetChild(13).gameObject.SetActive(active);
    }

    private void ManageShop(bool active) {
        shopParent.SetActive(active);
    }

    private IEnumerator InfoLabelWaiting() {
        yield return new WaitForSeconds(2f);
        infoLabel.SetActive(false);
        shopParent.transform.GetChild(5).gameObject.SetActive(false);
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
            coinTextReward.GetComponent<Text>().color = new Color(0f,0.35f,1f,1f);
            coinTextReward.GetComponent<Text>().text = "+" + coins;
        }
        else {
            coinTextReward.GetComponent<Text>().color = new Color(1.0f,0f,0f,1f);
            coinTextReward.GetComponent<Text>().text = "-" + coins;
        }
    }

    public int ReturnDiceResult() {
        return int.Parse(diceResult.GetComponent<Text>().text);
    }

    public bool IsDisplayingReward() {
        return coinIconReward.activeSelf;
    }


    private void BlackScreenAnimation(int dayPeriod) {
        
        camera.transform.rotation = Quaternion.Euler(-31.101f,camera.transform.rotation.y,camera.transform.rotation.z); // la rotation doit se faire progressivement
        int lastDayPeriod = gameController.GetDayController().dayPeriod;

        GameObject targetStep;

        RunDelayed(1f, () => { 
            bsAnimation.Play();

            RunDelayed(3.5f,() => {
                gameController.GetDayController().dayPeriod = dayPeriod;

                RunDelayed(1f,() => {
                    bsAnimation.Play();

                    RunDelayed(3.5f,() => {
                        // Position camera devant 
                        targetStep = GetStepByCoords();

                        camera.transform.position = targetStep.GetComponent<Step>().camPosition;
                        camera.transform.rotation = targetStep.GetComponent<Step>().camRotation;
                         
                        RunDelayed(1f, () => {
                            // Récupérer la step via le bombPos

                            lightning.transform.position = new Vector3(targetStep.transform.position.x,lightning.transform.position.y,targetStep.transform.position.z);
                            movement.audio.Lightning();

                            foreach(ParticleSystem ps in lightningParticles) {
                                ps.Play();
                            }
                        });
                    });
                });
            });
            
        });


    }

    public void ChangeTurnValue(int turn,int night) {
        hudParent.transform.GetChild(7).gameObject.GetComponent<Text>().text = "Tour: " + turn;
        hudParent.transform.GetChild(8).gameObject.GetComponent<Text>().text = "Nuit dans " + night + " tour(s)";
    }

    private GameObject GetStepByCoords() {
        foreach(GameObject obj in gameController.GetAllSteps()) {
            
            float minX = obj.transform.position.x - 3;
            float maxX = obj.transform.position.x + 3;
            float minZ = obj.transform.position.z - 3;
            float maxZ = obj.transform.position.z + 3;

            if(obj.transform.position.x >= 0 && obj.transform.position.z >= 0) {
                if(bombPos.transform.position.x >= minX && bombPos.transform.position.x <= maxX) {
                    if(bombPos.transform.position.z >= minZ && bombPos.transform.position.z <= maxZ) 
                        return obj;
                }
            }
            else if(obj.transform.position.x <= 0 && obj.transform.position.z >= 0) {
                if(bombPos.transform.position.x <= maxX && bombPos.transform.position.x >= minX) {
                    if(bombPos.transform.position.z >= minZ && bombPos.transform.position.z <= maxZ) 
                        return obj;
                }
            }
            else if(obj.transform.position.x >= 0 && obj.transform.position.z <= 0) {
                if(bombPos.transform.position.x >= minX && bombPos.transform.position.x >= maxX) {
                    if(bombPos.transform.position.z <= maxZ && bombPos.transform.position.z >= minZ) 
                        return obj;                
                }
            }
            else {
                if(bombPos.transform.position.x <= maxX && bombPos.transform.position.x >= minX) {
                    if(bombPos.transform.position.z <= maxZ && bombPos.transform.position.z >= minZ) 
                        return obj;                   
                }
            }
        }

        return null;
    }

    private void DisplayInfoText(Vector2 pos,Color color,string text) {
        infoLabel.transform.position = pos;
        infoLabel.GetComponent<Text>().color = color;
        infoLabel.GetComponent<Text>().text = text;
        infoLabel.GetComponent<AlphaController>().manageAlpha = true;
        infoLabel.SetActive(true);
    }
}
