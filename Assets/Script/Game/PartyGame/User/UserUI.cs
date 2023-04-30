using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using UnityEngine.AI;

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
    public Transform hoverInventoryItem; // L'ui du hover sur les items dans l'inventaire
    public Text coinTextReward;
    public Text itemDescription; // texte de la description d'un iutem dans le shop
    public Text turnText,nightText; // texte pour afficher les tours et la nuit
    public GameObject shopParent;
    public Direction direction;
    public Sprite userSprite;

    private bool isInInventory;
    private int nextShopId = -1;
    private int index = -1;
    private float cameraSpeed = 90f;
    private Vector2 vecMove;

    public ParticleSystem smokeEffect;

// Check que quand c'est pas le tour d'un joueur tt soit désactiver

    public override void Start() {
        base.Start();

        
        //showActionButton = new BVar();

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

            audio.ButtonHover();
            return;
        }

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

        if (e.started && hoverInventoryItem.transform.parent.gameObject.activeSelf && isInInventory && !gameController.freeze) {
            if (index < inventoryItems.Length)
                index++;

            hoverInventoryItem.gameObject.SetActive(true);

            Vector3 hoverPos = index < inventoryItems.Length - 1
                ? hoverInventoryItem.transform.parent.GetChild(1 + index).transform.localPosition
                : hoverInventoryItem.transform.parent.GetChild(1 + index).transform.localPosition +
                  new Vector3(0, 10f, 0);

            hoverInventoryItem.localPosition = hoverPos;
            
            audio.ButtonHover();
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
            
            
            audio.ButtonHover();
        }

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

        if(e.started && hoverInventoryItem.transform.parent.gameObject.activeSelf && !gameController.freeze) {
            if(index > 0) 
                index--;

            hoverInventoryItem.gameObject.SetActive(true);

            Vector3 hoverPos = index < inventoryItems.Length - 1
                ? hoverInventoryItem.transform.parent.GetChild(1 + index).transform.localPosition
                : hoverInventoryItem.transform.parent.GetChild(1 + index).transform.localPosition +
                  new Vector3(0, 10f, 0);
            
            hoverInventoryItem.localPosition = hoverPos;
            
            
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

        if(e.started & showActionButton && !isInInventory && !cameraView && !gameController.freeze) {
            if(index == 0) {
                ManageInventory(true);
            }
            else if(index == 1) {
                showHUD = false;
                showActionButton.value = false;
                infoLabel.SetActive(false);
                if(isPlayer) 
                    isTurn = true;
                
                movement.waitDiceResult = true;
                index = -1;
            }

            else if(index == 2) {
                cameraView.value = true;
                camera = GameObject.FindGameObjectsWithTag("MainCamera")[0];

                camera.transform.position = new Vector3(transform.position.x,5479f,transform.position.z);
                camera.transform.rotation = Quaternion.Euler(90f,265.791f,0f); 

                DisplayInfoText(new Vector2(971,164),new Color(1.0f,1.0f,1.0f),"Appuyez sur ECHAP pour quitter le mode");
            }

            return;
        }

        if(e.started && movement.useLightning & cameraView && !gameController.freeze) {
            Vector2 cursorPos = new Vector2(cursor.transform.position.x,cursor.transform.position.z);

            GameObject targetStep = null;

            for(int x = -7;x <7;x++) { // Range : 7
                for(int z = -7;z<7;z++) {
                    Vector2 calculatePos = new Vector2(cursorPos.x + x,cursorPos.y + z);

                    foreach(Step step in FindObjectsOfType(typeof(Step))) {
                        Vector2 stepPos = new Vector2(step.gameObject.transform.position.x,step.gameObject.transform.position.z);

                        if((int)calculatePos.x == (int)stepPos.x && (int)calculatePos.y == (int)stepPos.y) {
                            targetStep = step.gameObject;
                            break;
                        }
                    }

                    if(targetStep != null)
                        break;

                }

                if(targetStep != null)
                    break;
            }

            if(targetStep != null) {
                gameController.blackScreenAnim.Play();
                cameraView.value = false;
                movement.targetLightningStep = targetStep;
            }
            else {
                 DisplayInfoTextWithSub(new Vector2(971,164),new Color(1.0f,0.0f,0.0f),"Merci de choisir une meilleure position","");
                 
                 RunDelayed(3.5f,() => {
                      DisplayInfoTextWithSub(new Vector2(971,164),new Color(1.0f,1.0f,1.0f),"Appuyez sur E pour choisir où l'éclair va tomber","Echap pour quitter le mode");
                 });               
            }
           
        }


        if(e.started && hoverInventoryItem.transform.parent.gameObject.activeSelf && isInInventory && !cameraView && !gameController.freeze) {
            if(index < inventoryItems.Length && index > -1) {
        
                if(hoverInventoryItem.transform.parent.gameObject.transform.childCount > (1+index) && inventoryItems[index].childCount > 0 && inventoryItems[index].GetChild(0).gameObject.activeSelf) { // Le joueur a l'objet 
                    switch(index) {
                        case 0: // Double dice
                            movement.doubleDice = true;
                            CloseActionHUD(true);
                            break;

                        case 1: // Triple Dice
                            movement.tripleDice = true;
                            CloseActionHUD(true);
                            break;

                        case 2: // Reverse Dice
                            movement.reverseDice = true;
                            CloseActionHUD(true);
                            break;

                        case 3: // Hourglass
                            movement.useHourglass = true;
                            gameController.blackScreenAnim.Play();
                            // ANIM OU IL MET AU DESSUS DE SA TETE LE SALIER A LA ZELDA
                            //CloseActionHUD(true);
                            break;

                        case 4:  // Lightning
                            movement.useLightning = true;
                            cameraView.value = true;
                            showCursor = true;
                            camera = GameObject.FindGameObjectsWithTag("MainCamera")[0];

                            camera.transform.position = new Vector3(transform.position.x,5479f,transform.position.z);
                            camera.transform.rotation = Quaternion.Euler(90f,265.791f,0f); 

                            cursor.transform.position = new Vector3(camera.transform.position.x,cursor.transform.position.y,camera.transform.position.z);
                            CloseActionHUD(false);

                            DisplayInfoTextWithSub(new Vector2(971,164),new Color(1.0f,1.0f,1.0f),"Appuyez sur E pour choisir où l'éclair va tomber","Echap pour quitter le mode");
                            
                            break;

                        case 5: // Shell   
                            inventory.UseShell();
                            CloseActionHUD(true);
                            break;
                    }

                    

                }
                else { // Le joueur n'a pas l'objet
                    if(index >= 0 && hoverInventoryItem.transform.parent.gameObject.activeSelf) {
                        audio.Error();
                    }
                }
            }
            else {
                index = 0;
                ManageInventory(false);
            }
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

    private void ManageInventory(bool active) {
        hoverInventoryItem.transform.parent.gameObject.SetActive(active);
        isInInventory = active;

        if(active)
            index = -1;

        if(hoverInventoryItem.transform.parent.gameObject.activeSelf) {    
            int[] items = {inventory.doubleDiceItem,inventory.tripleDiceItem,inventory.reverseDiceItem,inventory.hourglassItem,inventory.lightningItem,inventory.shellItem};

            for(int i = 0;i<items.Length;i++) {
                if(items[i] > 0) {
                    if (i >= inventoryItems.Length)
                        return;
                    
                    inventoryItems[i].GetChild(0).gameObject.SetActive(true);
                    inventoryItems[i].GetChild(1).gameObject.SetActive(true);
                    inventoryItems[i].GetChild(1).gameObject.GetComponent<Text>().text = "" + items[i];
                }
                else {
                    if (i >= inventoryItems.Length)
                        return;
                    
                    inventoryItems[i].GetChild(0).gameObject.SetActive(false);
                    inventoryItems[i].GetChild(1).gameObject.SetActive(false);
                }
                
            }

            hoverInventoryItem.gameObject.SetActive(false);
        }
    }

    private void ManageHudState(bool active) {
        if(!gameController.GetComponent<DialogController>().isInDialog && gameController.hasGenChest) {
            int actualPlayer = gameController.actualPlayer;
            int hudIndex = 0;

           // for(int i = 0;i<4;i++) {
              //  playersPanels[i].gameObject.SetActive(active);

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
                    
                    //gameController.ChangeHUDSpritePlayer(playersPanels,hudIndex,targetUser.userType);

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
      //  }
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

    private void DisplayInfoText(Vector2 pos,Color color,string text) {
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
