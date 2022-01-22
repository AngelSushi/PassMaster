using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using UnityEngine.AI;

public class UserUI : User {

    public bool showHUD;
    public bool showTurnInfo;
    public bool showActionButton;
    public bool showDirection;
    public bool showChestHUD;
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
    public Transform chestHUD;
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
        } 
    }

    
    public override void OnBeginTurn() {}
    public override void OnFinishTurn() {}
    public override void OnDiceAction() {}

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

        

        if(e.started && hoverInventoryItem.transform.parent.gameObject.activeSelf && !gameController.freeze) {
            if(index > 0) 
                index--;

            Vector2[] hoverPos = {new Vector2(-444,31),new Vector2(-299,31),new Vector2(-149,31),new Vector2(5,31),new Vector2(145,31),new Vector2(305,31),new Vector2(448,31),new Vector2(583,31)};
            
            hoverInventoryItem.gameObject.SetActive(true);
            hoverInventoryItem.localPosition = hoverPos[index];
        }
    }

    public void OnInteract(InputAction.CallbackContext e) {       
        if(e.started && showDirection && !gameController.freeze) {
            if(directions[0].GetChild(1).gameObject.activeSelf) {
                movement.left = true;
                movement.reverseCount = direction.reverseCountDirections[0];
                directions[0].GetChild(0).gameObject.SetActive(true);
                directions[0].GetChild(1).gameObject.SetActive(false); 
            }
            else if(directions[1].GetChild(1).gameObject.activeSelf) {
                movement.front = true;
                movement.reverseCount = direction.reverseCountDirections[1];
                directions[1].GetChild(0).gameObject.SetActive(true);
                directions[1].GetChild(1).gameObject.SetActive(false);
            }
            else if(directions[2].GetChild(1).gameObject.activeSelf) {
                movement.right = true;
                movement.reverseCount = direction.reverseCountDirections[2];
                directions[2].GetChild(0).gameObject.SetActive(true);
                directions[2].GetChild(1).gameObject.SetActive(false);
            }

           // movement.reverseCount = direction.reverseCount;
            showDirection = false;
            index = -1;
        }

        if(e.started && showChestHUD && !gameController.freeze) {
            if(index == 0) {              
                isTurn = false;
                gameController.EndUserTurn();                
            }
            else if(index == 1) {
                // Buy
                int coins = inventory.coins;

                if(coins >= 30) {
                    StartCoroutine(movement.WaitMalus(30,false));
                    movement.goToChest = true;
                }
                else {
                    audio.BuyLoose();
                    gameController.EndUserTurn();
                    // Afficher un texte également
                }
            }

            showChestHUD = false;
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
                if(isPlayer) 
                    isTurn = true;

                GetComponent<NavMeshAgent>().enabled = true;
                movement.waitDiceResult = true;
                index = -1;
            }

            else if(index == 2) {
                cameraView = true;
                camera = GameObject.FindGameObjectsWithTag("MainCamera")[0];

                camera.transform.position = new Vector3(transform.position.x,5479f,transform.position.z);
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
                            isTurn = true;
                            movement.waitDiceResult = true;
                            inventory.hasDoubleDice = false;
                            break;

                        case 1: // Reverse Dice
                            movement.reverseDice = true;
                            ManageInventory(false);
                            isInInventory = false;
                            showHUD = false;
                            showActionButton = false;
                            isTurn = true;
                            movement.waitDiceResult = true;
                            inventory.hasReverseDice = false;
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
                            isTurn = true;
                            movement.waitDiceResult = true;

                            // Camera animation on voit le dayPeriod d'avant ecran noir puis le nouveau dayPeriod
                            inventory.hasHourglass = false;

                            break;
                        case 4: // Lightning
                            cameraView = true;
                            useLightning = true;
                            camera = GameObject.FindGameObjectsWithTag("MainCamera")[0];

                            camera.transform.position = new Vector3(transform.position.x,5479f,transform.position.z);
                            camera.transform.rotation = Quaternion.Euler(90f,265.791f,0f);           
                            DisplayInfoText(new Vector2(971,164),new Color(1.0f,1.0f,1.0f), "Appuyez sur ECHAP pour quitter le mode");

                            ManageInventory(false);
                            isInInventory = false;

                            inventory.hasLightning = false;
                            break;

                        case 5: // Star
                            transform.gameObject.GetComponent<MeshRenderer>().material.shader = gameController.invicibilityShader;
                            transform.GetChild(0).gameObject.GetComponent<MeshRenderer>().material.shader = gameController.invicibilityShader;
                            audio.Invicibility();

                            ManageInventory(false);

                            showHUD = false;
                            showActionButton = false;
                            isTurn = true;
                            movement.waitDiceResult = true;

                            gameController.mainCamera.transform.position = new Vector3(-454.4f,5226.9f,-15872.2f);
                            gameController.mainCamera.transform.rotation = Quaternion.Euler(0,275.83f,0f);
                            gameController.mainCamera.SetActive(true);
                            gameController.mainCamera.GetComponent<Camera>().enabled = true;
                            isInInventory = false;

                            inventory.hasStar = false;
                            break;    

                        case 6: // Parachute
                            movement.agent.enabled = false;
                            movement.isParachuting = true;
                            ManageInventory(false); 
                            showHUD = false;
                            showActionButton = false;
                            isInInventory = false;

                            inventory.hasParachute = false;
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


    public void OnMove(InputAction.CallbackContext e) {
        if(cameraView && !gameController.freeze) 
            vecMove = e.ReadValue<Vector2>();
    }

    public void OnQuit(InputAction.CallbackContext e) {
        if(e.started && cameraView && !gameController.freeze) {
            cameraView = false;
            camera.transform.position = new Vector3(-804f,5213f,-15807f);
            camera.transform.rotation = Quaternion.Euler(0,275.83f,0f);
            infoLabel.SetActive(false);
            index = 2;
            return;
        }

        if(e.started && showShop && !gameController.freeze) {
            showShop = false;
            gameController.shopController.returnToStep = true;
            gameController.mainCamera.SetActive(false); 
            transform.GetChild(1).gameObject.SetActive(true);
            index = -1;
        }
    }

    private void ManageInventory(bool active) {
        hoverInventoryItem.transform.parent.gameObject.SetActive(active);

        if(active)
            index = -1;

        if(hoverInventoryItem.transform.parent.gameObject.activeSelf) {    
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
                    
                    gameController.ChangeHUDSpritePlayer(playersPanels,hudIndex,gameController.players[playerIndex].GetComponent<UserMovement>().id);

                    int rank = -1;
                    int rankIndex = 0;
                    foreach(GameObject player in gameController.classedPlayers.Keys) {
                        if(GameObject.ReferenceEquals(player,gameController.players[playerIndex])) 
                            rank = rankIndex;

                        rankIndex++;
                    }

                    if(rank >= 0 && rank < gameController.classedColors.Length && rank < gameController.classedPlayers.Keys.Count) { 
                        playersPanels[playerIndex].GetChild(4).gameObject.GetComponent<Text>().text = gameController.players[playerIndex].GetComponent<UserInventory>().coins + "";
                        playersPanels[playerIndex].GetChild(6).gameObject.GetComponent<Text>().text = gameController.players[playerIndex].GetComponent<UserInventory>().cards + "";
                        playersPanels[playerIndex].GetChild(2).gameObject.GetComponent<Text>().text = rank + 1 + "";
                        playersPanels[playerIndex].GetChild(2).gameObject.GetComponent<Text>().color = gameController.classedColors[rank];   
                        playersPanels[playerIndex].GetChild(1).gameObject.GetComponent<Text>().text = gameController.players[playerIndex].name;
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

    private void ManageHudDirection(bool active) { // front ou interior
        if(direction != null) {
            if(direction.directions[0]) {
                if(direction.directionsStep[0].name.Contains("front") || direction.directionsStep[0].name.Contains("interior")) {
                    if(gameController.dayController.dayPeriod == 0 || gameController.dayController.dayPeriod == 1) 
                        directions[0].gameObject.SetActive(true);
                }
                else 
                    directions[0].gameObject.SetActive(true);        
            }
            else 
                directions[0].gameObject.SetActive(false);

            if(direction.directions[1]) {
                if(direction.directionsStep[1].name.Contains("front") || direction.directionsStep[1].name.Contains("interior")) {
                    if(gameController.dayController.dayPeriod == 0 || gameController.dayController.dayPeriod == 1) 
                        directions[1].gameObject.SetActive(true);                  
                }
                else 
                    directions[1].gameObject.SetActive(true); 
            }
            else 
                directions[1].gameObject.SetActive(false);

            if(direction.directions[2]) {
                if(direction.directionsStep[2].name.Contains("front") || direction.directionsStep[2].name.Contains("interior")) {
                    if(gameController.dayController.dayPeriod == 0 || gameController.dayController.dayPeriod == 1) 
                        directions[2].gameObject.SetActive(true);           
                }
                else 
                    directions[2].gameObject.SetActive(true);         
            }
            else 
                directions[2].gameObject.SetActive(false);
        }

        if(!active) {
            foreach(Transform direction in directions)
                direction.gameObject.SetActive(false);
        }
    }

    private void ManageChestHUD(bool active) {
        chestHUD.gameObject.SetActive(active);
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

}
