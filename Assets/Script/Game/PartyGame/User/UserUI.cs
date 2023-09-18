using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using UnityEngine.AI;
using UnityEngine.EventSystems;

public class UserUI : User {

    public bool showHUD;
    public bool showTurnInfo;
    public BVar showActionButton = new BVar();
    public BVar showDirection = new BVar();
    public BVar showShop = new BVar();
    public BVar cameraView = new BVar();
    private bool showCursor;
    public GameObject cursor;
    public GameObject infoLabel;
    public GameObject diceResult;
    public GameObject coinIconReward;
    public GameObject camera;
    public Transform[] directions; // les parents des directions
    
    public Transform[] actions; // buttons d'actions sur le menu principale du jeu
    
    
    public Transform[] playersPanels; // UI de chaque joueur avec ses pieces etc
    public Transform[] inventoryItems;
    public Text coinTextReward;
    public Text turnText,nightText; // texte pour afficher les tours et la nuit
    public GameObject shopParent;
    public Direction direction;
    public Sprite userSprite;

    private int index = -1;
    private float cameraSpeed = 90f;
    private Vector2 vecMove;

    public ParticleSystem smokeEffect;

// Check que quand c'est pas le tour d'un joueur tt soit désactiver

    public override void Start() {
        base.Start();

        gameController.inputs.FindAction("Menus/Right").started += OnRight;
        gameController.inputs.FindAction("Menus/Left").started += OnLeft;
        gameController.inputs.FindAction("Menus/Interact").started += OnInteract;
        gameController.inputs.FindAction("Player/Movement").performed += OnMove;
        gameController.inputs.FindAction("Player/Movement").canceled += OnMove;
        
        gameController.inputs.FindAction("Player/Quit").started += OnQuit;

        showDirection.switchValuePositive += SwitchValuePositive;
        showDirection.switchValueNegative += SwitchValueNegative;
        
        showActionButton.switchValuePositive += SwitchValuePositive;
        showActionButton.switchValueNegative += SwitchValueNegative;
        
        showShop.switchValuePositive += SwitchValuePositive;
        showShop.switchValueNegative += SwitchValueNegative;

        cameraView.switchValuePositive += SwitchValuePositive;
        cameraView.switchValueNegative += SwitchValueNegative;
    }

    private void OnDestroy()
    {
        showDirection.switchValuePositive -= SwitchValuePositive;
        showDirection.switchValueNegative -= SwitchValueNegative;
        
        showActionButton.switchValuePositive -= SwitchValuePositive;
        showActionButton.switchValueNegative -= SwitchValueNegative;
        
        showShop.switchValuePositive -= SwitchValuePositive;
        showShop.switchValueNegative -= SwitchValueNegative;

        cameraView.switchValuePositive -= SwitchValuePositive;
        cameraView.switchValueNegative -= SwitchValueNegative;
    }

    public override void Update() {  
        base.Update();
        
        if(!gameController.freeze) {
            
            showActionButton.UpdateValues();
            showShop.UpdateValues();
            showDirection.UpdateValues();
            cameraView.UpdateValues();

            ManageCameraPosition();
            ManageHudState(showHUD);
            ManagerHudTurnState(showTurnInfo && gameController.part == GameController.GamePart.PARTYGAME);
            ManageActionButtonState(showActionButton.value);
            ManageHudDirection(showDirection.value);
            ManageShop(showShop.value);
            
        } 
    }

    public void SwitchValuePositive() {
        if (showShop | showDirection | showActionButton) {
            if (showActionButton)
            {
                gameController.EventSystem.SetSelectedGameObject(actions[0].gameObject);
            }
            
            if (showDirection)
            {
                gameController.EventSystem.SetSelectedGameObject(directions[direction.directions.ToList().IndexOf(direction.directions.First(val => val))].gameObject);
            }

            gameController.playerInput.SwitchCurrentActionMap("Menus");
        }
        
        if(cameraView)
            gameController.playerInput.SwitchCurrentActionMap("Player");
    }

    public void SwitchValueNegative() {
        if(!cameraView)
            gameController.playerInput.SwitchCurrentActionMap("Menus");
        
        if (!showShop & !showDirection & !showActionButton) {
            gameController.playerInput.SwitchCurrentActionMap("Player");
        }
        
    }

    public override void OnBeginTurn() {}
    public override void OnFinishTurn() {}
    public override void OnDiceAction() {}

    private void ManageCameraPosition() {

        float directionX = cameraView ? vecMove.x * cameraSpeed * Time.deltaTime : 0;
        float directionY = cameraView ? vecMove.y * cameraSpeed * Time.deltaTime : 0;
        
        camera.transform.Translate(directionX,directionY,0);

        if(showCursor) 
            cursor.transform.position = new Vector3(camera.transform.position.x,cursor.transform.position.y,camera.transform.position.z);
    }

    public void OnRight(InputAction.CallbackContext e) {
        if(e.started && !cameraView && !showHUD & showDirection && !gameController.freeze) {
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
            
            
            audio.ButtonHover();
        }
    }

    public void OnLeft(InputAction.CallbackContext e) {
        if(e.started && !cameraView && !showHUD & showDirection && !gameController.freeze) {
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
            
            
            audio.ButtonHover();
        }
    }

    public void OnInteract(InputAction.CallbackContext e) {       
        if(e.started & showDirection && !gameController.freeze) {
            if(directions[0].GetChild(1).gameObject.activeSelf) {
                movement.left = true;
                Debug.Log("moove to left " + movement.isMooving);
                directions[0].GetChild(0).gameObject.SetActive(true);
                directions[0].GetChild(1).gameObject.SetActive(false); 
            }
            else if(directions[1].GetChild(1).gameObject.activeSelf) {
                Debug.Log("moove to front " + movement.isMooving);
                movement.front = true;
                directions[1].GetChild(0).gameObject.SetActive(true);
                directions[1].GetChild(1).gameObject.SetActive(false);
            }
            else if(directions[2].GetChild(1).gameObject.activeSelf) {
                
                Debug.Log("moove to right " + movement.isMooving);
                movement.right = true;
                directions[2].GetChild(0).gameObject.SetActive(true);
                directions[2].GetChild(1).gameObject.SetActive(false);
            }

            showDirection.value = false;
            index = -1;
        }
    }


    public void OnMove(InputAction.CallbackContext e) {
        if (e.performed && !gameController.freeze & cameraView) 
            vecMove = e.ReadValue<Vector2>();
        else
            vecMove = Vector2.zero;
    }

    public void CloseActionHUD(bool goToDice) {
        showHUD = false;
        showActionButton.value = false;
        ManageInventory(false);
        infoLabel.SetActive(false);
        GetComponent<NavMeshAgent>().enabled = goToDice;
        movement.waitDiceResult = goToDice;

        
        if(goToDice)
            index = -1;
    }


    public void OnQuit(InputAction.CallbackContext e) {
        if(e.started & cameraView && !gameController.freeze) {
            cameraView.value = false;
            actions[2].GetComponent<Button>().enabled = true;
            gameController.EventSystem.SetSelectedGameObject(actions[2].gameObject);
            gameController.ManageCameraPosition();
            infoLabel.SetActive(false);
            index = 2;
            return;
        }

        if(e.started & showShop && !gameController.freeze) {
            showShop.value = false;
            gameController.shopController.returnToStep = true;
          //  gameController.mainCamera.SetActive(false); 
            transform.GetChild(1).gameObject.SetActive(true);
            index = -1;
        }
    }

    public void ManageInventory(bool active) {
        inventoryItems[0].transform.parent.gameObject.SetActive(active);

        if(active)
            index = -1;
        
        int[] items = {inventory.doubleDiceItem,inventory.tripleDiceItem,inventory.reverseDiceItem,inventory.hourglassItem,inventory.lightningItem,inventory.shellItem};

        for(int i = 0;i<items.Length;i++) {
            if(items[i] > 0) {
                if (i >= inventoryItems.Length)
                    return;
                
                inventoryItems[i].GetChild(0).gameObject.SetActive(true);
                inventoryItems[i].GetChild(0).gameObject.GetComponent<Text>().text = "" + items[i];
            }
            else {
                if (i >= inventoryItems.Length)
                    return;
                
                inventoryItems[i].GetChild(0).gameObject.SetActive(false);
            }
        }
    }

    private void ManageHudState(bool active) {
        if(!gameController.GetComponent<DialogController>().isInDialog && gameController.hasGenChest) {
            int actualPlayer = gameController.actualPlayer;
            int hudIndex = 0;

            for(int j = actualPlayer;j<actualPlayer + 4;j++) {
            
                int playerIndex = j;
                if(j > 3) 
                    playerIndex -= 4;

                UserMovement targetUser = gameController.players[playerIndex].GetComponent<UserMovement>();

                int rank = 0;
                for (int k = 0;k < gameController.classedPlayers.Keys.Count;k++) {
                    GameObject player = gameController.classedPlayers.Keys.ToArray()[k];
                    
                    if (player.GetComponent<UserMovement>() == targetUser) {
                        rank = k;
                        break;
                    }
                }

                Player playerData = gameController.playersData.Where(playerData => playerData.name == targetUser.name).ToList()[0];
                
                playersPanels[hudIndex].GetChild(0).gameObject.GetComponent<Image>().sprite = playerData.uiIcon;
                playersPanels[hudIndex].gameObject.SetActive(active);
                playersPanels[hudIndex].GetChild(2).GetComponent<Text>().text = (rank + 1) + "";
                playersPanels[hudIndex].GetChild(2).GetComponent<Text>().color = gameController.classedColors[rank];
                
                playersPanels[hudIndex].GetChild(4).GetComponent<Text>().text = targetUser.inventory.coins.ToString();
                playersPanels[hudIndex].GetChild(6).GetComponent<Text>().text = targetUser.inventory.cards.ToString();
                playersPanels[hudIndex].GetChild(1).GetComponent<Text>().text = targetUser.name;



                hudIndex += 1;
                if (hudIndex == 4) 
                    break;
            }            
        }
    }

    private void ManageActionButtonState(bool active) {
        for(int i = 0;i<3;i++) {
            actions[i].gameObject.SetActive(active);
        }
            
        
    }

    private void ManagerHudTurnState(bool active) {
        turnText.gameObject.SetActive(active);
        nightText.gameObject.SetActive(active);     
    }

    private void ManageHudDirection(bool active) { // front ou interior
        
        
        if(direction != null) {
            for (int i = 0; i < direction.directions.Length; i++) {
                
                // i == 1 ==> direction is front 
                // i == 0 ==> direction is left
                // i == 2 ==> direction is right 
                
                bool activeDirection = i == 1 ? direction.directions[i] : movement.reverseDice ? i == 0 ? direction.directions[2] : direction.directions[0] : direction.directions[i];
                directions[i].gameObject.SetActive(activeDirection);
            }

            if(!active) {
                foreach(Transform direction in directions)
                    direction.gameObject.SetActive(false);
            }
        }
        
    }

    private void ManageShop(bool active) {
        shopParent.transform.GetChild(0).gameObject.SetActive(active);
        shopParent.transform.GetChild(1).gameObject.SetActive(active);
    }

    private IEnumerator InfoLabelWaiting() {
        yield return new WaitForSeconds(2f);
        infoLabel.SetActive(false);
    }

    public void RefreshDiceResult(int result,Color color) {
        if(!diceResult.activeSelf) 
            diceResult.SetActive(true);
        
        if(color.a == 0)
            color.a = 1f;

        diceResult.GetComponent<Text>().color = color;
        diceResult.GetComponent<Text>().text = result >= 0 ? result + "" : "0";
    }

    public void ClearDiceResult() {
        diceResult.GetComponent<Text>().text = "";
    }

    public void DisplayReward(bool bonus,int coins) {
        coinIconReward.SetActive(true);
        coinIconReward.GetComponent<CoinsReward>().RunCoroutine();

        if(bonus) {
            coinTextReward.color = new Color(0f,0.35f,1f,1f);
            coinTextReward.text = "+" + coins;
        }
        else {
            coinTextReward.color = new Color(1.0f,0f,0f,1f);
            coinTextReward.text = "-" + coins;
        }
    }

    public void ChangeTurnValue(int turn,int night) {
        turnText.text = "Tour: " + turn;
        nightText.text = night > 1 ? "Nuit dans " + night + " tours" : "Nuit dans 1 tour";
    }

    public void DisplayInfoText(Vector2 pos,Color color,string text) {
        infoLabel.transform.position = pos;
        infoLabel.GetComponent<Text>().color = color;
        infoLabel.GetComponent<Text>().text = text;
        infoLabel.GetComponent<AlphaController>().manageAlpha = true;
        infoLabel.SetActive(true);
    }

    private void DisplayInfoTextWithSub(Vector2 pos,Color color,string text,string subText) {
        infoLabel.transform.position = pos;
        infoLabel.GetComponent<Text>().color = color;
        infoLabel.GetComponent<Text>().text = text;
        infoLabel.GetComponent<AlphaController>().actualColor = color;
        infoLabel.GetComponent<AlphaController>().manageAlpha = true;
        infoLabel.SetActive(true);

        Vector2 subPos = pos;
        subPos.y -= 50;

        Transform subTransform = infoLabel.transform.GetChild(0);
        subTransform.position = subPos;
        subTransform.gameObject.GetComponent<Text>().color = color;
        subTransform.gameObject.GetComponent<Text>().text = subText;
        subTransform.gameObject.GetComponent<AlphaController>().actualColor = color;
        subTransform.gameObject.GetComponent<AlphaController>().manageAlpha = true;

    }
}
