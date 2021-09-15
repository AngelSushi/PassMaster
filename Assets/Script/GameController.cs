using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using PathCreation;
using UnityEngine.AI;
using System.Linq;
using Random=UnityEngine.Random;

using UnityEngine.SceneManagement;

public class GameController : CoroutineSystem {

    // CE SCRIPT SERT A GERER TT LE DEROULER DUNE PARTIE

    // chaque joueur possèdera un index, il te suffit juste de l'incrémenter pour passer au suivant et de le remttre à 0 pour revenir au premier
    // pour 4 joueurs, par ex: 1er = 0, 2e = 1, 3e = 2, 4e = 3
    // savoir qui joue c'est connaitre cet index
    // donner la main c'est affecter cet index

    // QUE LES VARIABLES INTERESSANTS AU DEROULEMENT DU JEU

    public enum GamePart {
        DIALOG_START_ALPHA,
        DIALOG_TUTORIAL,
        TUTORIAL,
        CHOOSE_ORDER,
        PARTYGAME,
        CHOOSE_MINIGAME,
        MINIGAME
    }

    public DayController dayController;
    public GameObject prefabDice; 
    public GameObject prefabInStep;
    public DialogController dialog;
    public GameObject stepParent;

    //public GamePart partOfGame = GamePart.TUTORIEL;
    public GameObject light;
    public GameObject[] diceResultObj = new GameObject[4];
    public Text[] diceResultText = new Text[4];
    


    private int actualPlayer;
    public GameObject[] players = new GameObject[4];
    public Dictionary<GameObject,int> classedPlayers = new Dictionary<GameObject,int>();
    private GameObject[] dices = new GameObject[4];

    private SortedDictionary<int,int> results = new SortedDictionary<int,int>();
    
    public int[] turnOrder = new int[4];

    public GameObject mainCamera;
    public TextAsset dialogsFile; 

    public GamePart part;

// Chest Manage
    public GameObject chestParent;
    public GameObject currentChest;

    public Vector3[] posBegin = new Vector3[4];

    public Dictionary<GameObject,GameObject> playerConflict = new Dictionary<GameObject,GameObject>();

    public Vector3[] stackPos = new Vector3[4];
    public Vector3[] stackSize = new Vector3[4];

    public Sprite[] smallSprites = new Sprite[4];
    public Color[] classedColors = new Color[4];
    public int turn;

    private bool hasLoadScene;

    public int[] secretCode = new int[6];

    public bool hasGenChest;
    private bool checkLastChest;
    private int randomIndex = -1;
    private bool hasBeginGame;
    private bool hasTurnChange;
    public static int difficulty;
    public bool freeze;

    public List<ShopItem> shopItems;

    public List<Material> diceMaterials;
    public List<Material> bombIsleMat;
    public List<Material> isleMat;
    public List<GameObject> prefabObjects = new List<GameObject>();

    public Shader invicibilityShader;

    public LoadingScene loadingScene;

    public List<GameObject> allSteps;

    public List<GameObject> allMainObjects;

    public int nightIndex;

    public int bridgeRemainingTurn = -1;
    public int bridgeRemainingTurn01 = -1;

    private bool moveToBridge;
    private Vector3 bridgePos;
    private int bridgeIndex = -1;

    private  List<int> points = new List<int>();
    private Dictionary<GameObject,int> playerPoint = new Dictionary<GameObject,int>();
    private bool begin = true;

    private GamePart lastPart;

    public bool oneInTurn;

    private bool hasChangeState;
    private bool isFirstChest = true;
    void Start() {
        GameController.difficulty = 2;

        classedPlayers.Add(GetPlayers()[0],1);
        classedPlayers.Add(GetPlayers()[1],2);
        classedPlayers.Add(GetPlayers()[2],3);
        classedPlayers.Add(GetPlayers()[3],4);

        if(GetComponent<DialogController>() != null) GetComponent<DialogController>().dialogs = JsonUtility.FromJson<DialogArray>(dialogsFile.text);
        
        dialog = GetComponent<DialogController>();
        ChangeStepName();

        if(GetPart() == GamePart.DIALOG_START_ALPHA) {
            for(int i = 0;i<players.Length;i++) {
                players[i].SetActive(false);
            }

            actualPlayer = 0;
            players[0].SetActive(true);
            players[0].transform.position = new Vector3(-1259f,5206f,-15801f);
            GetComponent<DialogController>().isInDialog = true;
            stepParent.SetActive(false);
          //  mainCamera.transform.rotation = Quaternion.Euler(-36.941f,358.267f,0f);
            
            mainCamera.transform.position = new Vector3(GetPlayers()[0].transform.position.x,5747.6f,GetPlayers()[0].transform.position.z);
            mainCamera.transform.rotation = Quaternion.Euler(90f,265.791f,0f); 

            
            Dialog currentDialog = dialog.GetDialogByName("StartTextAlpha");
            dialog.currentDialog = currentDialog;
            dialog.isInDialog = true;
            dialog.finish = false;
            StartCoroutine(dialog.ShowText(currentDialog.Content[0],currentDialog.Content.Length));
            part = GamePart.DIALOG_START_ALPHA;

        }

        if(GetPart() == GamePart.DIALOG_TUTORIAL) {
            for(int i = 0;i<players.Length;i++) {
                players[i].SetActive(false);
            }

            actualPlayer = 0;
            players[0].SetActive(true);
            players[0].transform.position = new Vector3(-1259f,5206f,-15801f);
            GetComponent<DialogController>().isInDialog = true;
            stepParent.SetActive(false);
          //  mainCamera.transform.rotation = Quaternion.Euler(-36.941f,358.267f,0f);
            
            mainCamera.transform.position = new Vector3(GetPlayers()[0].transform.position.x,5747.6f,GetPlayers()[0].transform.position.z);
            mainCamera.transform.rotation = Quaternion.Euler(90f,265.791f,0f); 
            Debug.Log("rotation: " + mainCamera.transform.rotation);
            Debug.Log("camera: " + mainCamera);

            
            Dialog currentDialog = dialog.GetDialogByName("AskTextTutorial");
            dialog.currentDialog = currentDialog;
            dialog.isInDialog = true;
            dialog.finish = false;
            StartCoroutine(dialog.ShowText(currentDialog.Content[0],currentDialog.Content.Length));

        }

        if(GetPart() == GamePart.PARTYGAME) { // Faire en sorte que ca soit appelé 
            mainCamera.transform.position = new Vector3(GetPlayers()[0].transform.position.x,5747.6f,GetPlayers()[0].transform.position.z);
            mainCamera.transform.rotation = Quaternion.Euler(90f,265.791f,0f); 

            RandomSecretCode();

            if(difficulty == 0)
                nightIndex = 4;
            if(difficulty == 1)
                nightIndex= 3;
            if(difficulty == 2)
                nightIndex = 2;

            ManageTurn();
            ActualizePlayerClassement();
        }


    }
    
    void Update() {

        foreach(GameObject player in players) {
            if(player.GetComponent<UserMovement>().isTurn) {
                oneInTurn = true;
                break;
            }
        }

        if(GetPart() != lastPart) 
            ChangePart();

        if(GetPart() == GamePart.CHOOSE_MINIGAME) {
            RandomMiniGame();
            return;
        }

        if(GetPart() == GamePart.PARTYGAME && !hasGenChest && GetComponent<DialogController>() != null && !GetComponent<DialogController>().isInDialog) {
            GenerateChest();
        }

        if(moveToBridge) {

            getMainCamera().transform.position = Vector3.MoveTowards(getMainCamera().transform.position,bridgePos,150 * Time.deltaTime);
            getMainCamera().transform.rotation = Quaternion.Euler(90,275.83f,0f);

            if(getMainCamera().transform.position == bridgePos) {
                RunDelayed(1f,() => {
                    if(bridgeIndex == 0) {
                        Destroy(GetPlayers()[0].GetComponent<UserUI>().islesParent.transform.GetChild(2).gameObject);
                        if(GetPlayers()[0].GetComponent<UserUI>().islesParent.transform.childCount >= 5) {
                            GameObject bridge = GetPlayers()[0].GetComponent<UserUI>().islesParent.transform.GetChild(5).gameObject;
                            Debug.Log("bridge: " + bridge);
                            if(!bridge.activeSelf) {
                                bridge.SetActive(true);
                                bridge.transform.SetSiblingIndex(2);
                            }
                        }
                    }
                    else if(bridgeIndex == 1) {
                        Destroy(GetPlayers()[0].GetComponent<UserUI>().islesParent.transform.GetChild(3).gameObject);
                        
                        if(GetPlayers()[0].GetComponent<UserUI>().islesParent.transform.childCount >= 6) {
                            GameObject bridge = GetPlayers()[0].GetComponent<UserUI>().islesParent.transform.GetChild(6).gameObject;
                            Debug.Log("bridge: " + bridge);
                            if(!bridge.activeSelf) {
                                bridge.SetActive(true);
                                bridge.transform.SetSiblingIndex(3);
                            }
                        }
                    }
                    bridgeIndex = -1;
                    moveToBridge = false;
                    BeginTurn(false,true);
                });
            }
        }

        lastPart = GetPart();
    }

    private void ChangePart() {
        if(GetPart() == GamePart.PARTYGAME && begin) { // Faire en sorte que ca soit appelé 
            mainCamera.transform.position = new Vector3(GetPlayers()[0].transform.position.x,5747.6f,GetPlayers()[0].transform.position.z);
            mainCamera.transform.rotation = Quaternion.Euler(90f,265.791f,0f); 

            RandomSecretCode();

            if(difficulty == 0)
                nightIndex = 4;
            if(difficulty == 1)
                nightIndex= 3;
            if(difficulty == 2)
                nightIndex = 2;

            begin = false;
            ManageTurn();
            ActualizePlayerClassement();
        }
    }

    private void ManageTurn() {
        GetPlayers()[GetActualPlayer()].GetComponent<UserUI>().ChangeTurnValue(turn,nightIndex);

        switch(difficulty) {
            case 0: // Facile
                // 2 jour ; 2 crépuscule ; 1 nuit
                if(nightIndex == 4 || nightIndex == 3)
                    dayController.dayPeriod = 0;
                if(nightIndex == 2 || nightIndex == 1)
                    dayController.dayPeriod = 1;
                if(nightIndex == 0)
                    dayController.dayPeriod = 2;

                break;

            case 1: // Medium
                // 2 jour ; 1 crépuscule ; 1 nuit
                if(nightIndex == 3 || nightIndex == 2)
                    dayController.dayPeriod = 0;
                if(nightIndex == 1)
                    dayController.dayPeriod = 1;
                if(nightIndex == 0)
                    dayController.dayPeriod = 2;

                break;

            case 2: // Hard
                // 1 jour ; 1 crépuscule ; 1 nuit
                if(nightIndex == 2)
                    dayController.dayPeriod = 0;
                if(nightIndex == 1)
                    dayController.dayPeriod = 1;
                if(nightIndex == 0)
                    dayController.dayPeriod = 2;

                break;        
        }
        
    }

    private void GenerateChest() {
        currentChest = null;
        freeze = true;

        if(randomIndex == -1) {
            randomIndex = Random.Range(0,chestParent.transform.childCount - 1);
            mainCamera.transform.position = new Vector3(-1062.2f,5747.6f,-15821.6f);
        }

        int lastIndex = GetLastChest();
        bool hasLastChest = (lastIndex == -1);

        if(hasLastChest && !checkLastChest) {
            while(randomIndex >= (lastIndex - 3) && randomIndex <= (lastIndex + 3)) {
                randomIndex = Random.Range(0,chestParent.transform.childCount - 1);
            }

            checkLastChest = true;
        }
        if( isFirstChest) {
            randomIndex = 7;
            isFirstChest = false;
        }
        GameObject chest = chestParent.transform.GetChild(randomIndex).gameObject;

        Vector3 cameraPosition = new Vector3(chest.transform.position.x,5747.6f,chest.transform.position.z);
        mainCamera.transform.rotation = Quaternion.Euler(90f,265.791f,0f); 
        mainCamera.transform.position = Vector3.MoveTowards(mainCamera.transform.position,cameraPosition,150 * Time.deltaTime);
        
        if(mainCamera.transform.position == cameraPosition) {
            GetPlayers()[actualPlayer].GetComponent<UserAudio>().SpawnChest();
            chest.SetActive(true);
            currentChest = chest;
            hasGenChest = true;
            
            if(!hasBeginGame) {
                BeginTurn(true,false);
                hasBeginGame = true;
                freeze = false;
                randomIndex = -1;
            }
            else {
                GetPlayers()[GetActualPlayer()].GetComponent<UserMovement>().actualStep.GetComponent<Step>().chest.SetActive(false);
                GetPlayers()[GetActualPlayer()].GetComponent<UserMovement>().isTurn = false;
                GetPlayers()[GetActualPlayer()].GetComponent<UserMovement>().returnToStep = true;
                
                getMainCamera().transform.position = new Vector3(-454.4f,5226.9f,-15872.2f);
                getMainCamera().transform.rotation = Quaternion.Euler(0,275.83f,0f);
                getMainCamera().SetActive(true);
                getMainCamera().GetComponent<Camera>().enabled = true;

                EndUserTurn();

                freeze = false;
            }
            // Marche que pour le tour 1 attention jpense 
        }
    }

    public int GetLastChest() {
        for(int i = 0;i<chestParent.transform.childCount;i++) {
            if(chestParent.transform.GetChild(i).gameObject.activeSelf) {
                return i;
            }
        }

        return -1;
    }

    public void RandomSecretCode() {
        for(int i = 0;i<secretCode.Length;i++) {
            secretCode[i] = Random.Range(0,9);
        }
    }

    public List<GameObject> GetAllSteps() {
        return allSteps;
    }

    public int[] GetSecretCode() {
        return secretCode;
    }

    public GameObject[] GetPlayers(){
        return players;
    }
    
    public Vector3[] GetPosBegin() {
        return posBegin;
    }

    public Dictionary<GameObject,int> GetPlayersPoints(){
        return playerPoint;
    }

    public List<int> GetPoints() {
        return points;
    }

    public Color[] GetClassedColors() {
        return classedColors;
    }

    public GameObject[] GetDices(){
        return dices;
    }
    
    public List<Material> GetDiceMaterials() {
        return diceMaterials;
    }
    
    public List<Material> GetBombIsleMat() {
        return bombIsleMat;
    }

    public List<Material> GetIsleMat() {
        return isleMat;
    }

    public int GetDifficulty(){
        return difficulty;
    }

    public List<GameObject> GetPrefabObjects() {
        return prefabObjects;
    }

    public GamePart GetPart() {
        return part;
    }

    public void SetPart(GamePart gPart) {
        this.part = gPart;
    }

    public int GetActualPlayer() {
        return actualPlayer;
    }

    public Dictionary<GameObject,int> GetClassedPlayers() {
        return classedPlayers;
    }

    public int GetTurn() {
        return turn;
    }

    public DayController GetDayController() {
        return dayController;
    }

    public GameObject getMainCamera() {
        return mainCamera;
    }

    public int GetPlayerPoints(GameObject player) {
        return player.GetComponent<UserInventory>().cards * 100000 + player.GetComponent<UserInventory>().coins;
    }


    public void SetActualPlayer(int nextPlayer) {
        actualPlayer = nextPlayer;
    }

    public Shader GetInvicibilityShader(){
        return invicibilityShader;
    }

    public List<ShopItem> GetShopItems() {
        return shopItems;
    }

    public GameObject GetActualChest() {
        return currentChest;
    }

    public GameObject GetActualStepChest() {
        Step[] steps = GameObject.FindObjectsOfType<Step>();

        foreach(Step step in steps) {
            if(step.chest == currentChest) {
                return step.gameObject;
            }
        }

        return null;
    }

    public Dictionary<GameObject,GameObject> GetPlayerConflict() {
        return playerConflict;
    }

    public int FindPlayerClassement(GameObject player) {

        foreach(GameObject p in classedPlayers.Keys) {
            if(player == p) 
                return classedPlayers[player];
        }
        return -1;
    }


    public void ActualizePlayerClassement() {
        classedPlayers.Clear();
        playerPoint.Clear();

        foreach(GameObject player in GetPlayers()) {
            UserInventory inv = player.GetComponent<UserInventory>();

            int point = inv.cards * 100000 + inv.coins;
            if(!playerPoint.Keys.Contains(player))
                playerPoint.Add(player,point);
        }

        points = playerPoint.Values.ToList();
        
        points.Sort();
        points.Reverse();

        
        for(int i = 0;i<points.Count;i++) {
            GetKeyByValue(points[i],playerPoint,classedPlayers);
        }   


        // Regardez les numéros de chaque joueur. Celui qui aura le plus de code sera premier
        // Si il y a deux personnes avec le même nombre de carte, regardez les pièces
        // Si ils ont le même nombre de pièce est de carte --> égalité
    }

    public void BeginTurn(bool changePos,bool repair) {
        if(bridgeRemainingTurn > 0) {
            bridgeRemainingTurn--;
            if(bridgeRemainingTurn == 0) {
                bridgeRemainingTurn = -1;
                moveToBridge = true;
                bridgePos = GetPlayers()[0].GetComponent<UserUI>().islesParent.transform.GetChild(2).position;
                bridgePos = new Vector3(bridgePos.x,5747,bridgePos.z);
                getMainCamera().transform.position = new Vector3(getMainCamera().transform.position.x,5747,getMainCamera().transform.position.z);
                bridgeIndex = 0;
            }
        }

        if(bridgeRemainingTurn01 > 0) {
            bridgeRemainingTurn01--;
            if(bridgeRemainingTurn01 == 0) {
                bridgeRemainingTurn01 = -1;
                moveToBridge = true;
                bridgePos = GetPlayers()[0].GetComponent<UserUI>().islesParent.transform.GetChild(3).position;
                bridgePos = new Vector3(bridgePos.x,5747,bridgePos.z);
                getMainCamera().transform.position = new Vector3(getMainCamera().transform.position.x,5747,getMainCamera().transform.position.z);
                bridgeIndex = 1;
            }
        }

        SetActualPlayer(0);
        if(turn > 1 && !repair && !hasChangeState) {
            ChangeStateScene("Main",true);
            SceneManager.UnloadSceneAsync(actualMiniGame.minigameName);
            hasChangeState = true;
        }

        SetPart(GameController.GamePart.PARTYGAME);

        for(int i = 0;i<4;i++) {
            GetPlayers()[i].SetActive(false);
            Destroy(GetPlayers()[i].GetComponent<PlayerController>());
        }

        if(moveToBridge) 
            return;

        List<GameObject> playersInStack = GetPlayersInStack();
        GetComponent<DialogController>().enabled = false;

        stepParent.SetActive(true);

        GetPlayers()[0].SetActive(true);

        if(changePos) GetPlayers()[0].transform.position = GetPosBegin()[0];
        GetPlayers()[0].transform.rotation = Quaternion.Euler(0f,-294.291f,0f);
        if(GetPlayers()[0].transform.GetChild(1).gameObject.activeSelf) GetPlayers()[0].transform.GetChild(1).gameObject.SetActive(false);  

        if(!playersInStack.Contains(GetPlayers()[1])) GetPlayers()[1].SetActive(true);
        GetPlayers()[1].GetComponent<UnityEngine.AI.NavMeshAgent>().enabled = false;
        if(changePos) GetPlayers()[1].transform.position =GetPosBegin()[1];
        GetPlayers()[1].transform.rotation = Quaternion.Euler(0f,-294.291f,0f);

         if(!playersInStack.Contains(GetPlayers()[2])) GetPlayers()[2].SetActive(true);
        GetPlayers()[2].GetComponent<UnityEngine.AI.NavMeshAgent>().enabled = false;
       if(changePos) GetPlayers()[2].transform.position = GetPosBegin()[2];
        GetPlayers()[2].transform.rotation = Quaternion.Euler(0f,-294.291f,0f);

         if(!playersInStack.Contains(GetPlayers()[3])) GetPlayers()[3].SetActive(true);
        GetPlayers()[3].GetComponent<UnityEngine.AI.NavMeshAgent>().enabled = false;
        if(changePos) GetPlayers()[3].transform.position = GetPosBegin()[3];  
        GetPlayers()[3].transform.rotation = Quaternion.Euler(0f,-294.291f,0f);

        
        GetPlayers()[0].GetComponent<UserMovement>().isTurn = true;

        if(! GetPlayers()[0].activeSelf) {
            GetPlayers()[0].SetActive(true);
            GetPlayers()[0].GetComponent<UserMovement>().actualStep.GetComponent<Step>().playerInStep.Remove(GetPlayers()[0]);
            ActualPlayersInStep(GetPlayers()[0].GetComponent<UserMovement>().actualStep,GetPlayers()[0]);
        }

        GetPlayers()[0].GetComponent<UserMovement>().enabled = true;
        GetPlayers()[0].GetComponent<UserUI>().enabled = true;
        GetPlayers()[0].GetComponent<UserAudio>().enabled = true;
        GetPlayers()[0].GetComponent<UserInventory>().enabled = true;
        if(GetPlayers()[0].GetComponent<UserMovement>().isPlayer) GetPlayers()[0].GetComponent<UserUI>().showHUD = true;
        GetPlayers()[0].GetComponent<UserUI>().showTurnInfo = true;
        if(GetPlayers()[0].GetComponent<UserMovement>().isPlayer) GetPlayers()[0].GetComponent<UserUI>().showHUD = true;
        GetPlayers()[0].GetComponent<UserUI>().showActionButton = true;


        if(turn == 1) {
            getMainCamera().transform.position = new Vector3(-454.4f,5226.9f,-15872.2f);
            getMainCamera().transform.rotation = Quaternion.Euler(0,275.83f,0f);
            getMainCamera().SetActive(true);
            getMainCamera().GetComponent<Camera>().enabled = true;
        }
        else {
            // Turn
            ManageCameraPosition();
        }

        light.transform.rotation = Quaternion.Euler(39.997f,-74.92f,-0.283f);
    }

    public void EndUserTurn() {
        players[actualPlayer].GetComponent<UserMovement>().finishTurn = false;
        players[actualPlayer].GetComponent<UserMovement>().isTurn = false;
        players[actualPlayer].GetComponent<UserMovement>().hasGenDice = false;
        
        if(actualPlayer < 3) {
            SetActualPlayer(actualPlayer + 1);
            players[actualPlayer].GetComponent<UserMovement>().isTurn = true;

            if(! GetPlayers()[actualPlayer].activeSelf) {
                GetPlayers()[actualPlayer].SetActive(true);
                GetPlayers()[actualPlayer].GetComponent<UserMovement>().actualStep.GetComponent<Step>().playerInStep.Remove(GetPlayers()[actualPlayer]);
                ActualPlayersInStep(GetPlayers()[actualPlayer].GetComponent<UserMovement>().actualStep,GetPlayers()[actualPlayer]);
            }

            if(turn == 1) players[actualPlayer].transform.position = posBegin[0];
            players[actualPlayer].GetComponent<UserMovement>().waitDiceResult = true;
            players[actualPlayer].GetComponent<NavMeshAgent>().enabled = true;
            players[actualPlayer].SetActive(true);

            if(turn > 1) {
                // ManageCameraPosition
                ManageCameraPosition();
            }

        }
        else { // Le tour est fini
            // Lancement d'un mini jeux
            SetPart(GamePart.CHOOSE_MINIGAME);
            actualPlayer = 0;
            
            ActualizePlayerClassement();
        }
    }

    private void ManageCameraPosition() {
       // getMainCamera().transform.position = new Vector3(players[actualPlayer].transform.position.x,getMainCamera().transform.position.y,getMainCamera().transform.position.z - 50);
        getMainCamera().transform.position = GetPlayers()[actualPlayer].GetComponent<UserMovement>().actualStep.GetComponent<Step>().camPosition;
        getMainCamera().transform.rotation = GetPlayers()[actualPlayer].GetComponent<UserMovement>().actualStep.GetComponent<Step>().camRotation;
    }

    private List<GameObject> GetPlayersInStack() {
        List<GameObject> localPlayersInStep = new List<GameObject>();

        foreach(Step step in FindObjectsOfType(typeof(Step))) {
            if(step.playerInStep.Count > 0) {
                foreach(GameObject user in step.playerInStep) {
                    if(!localPlayersInStep.Contains(user)) 
                        localPlayersInStep.Add(user);
                }
            }
        }

        return localPlayersInStep;
    }


    public GameObject GetKeyByValue<Key, Value>(Value value,Dictionary<Key,Value> dict) {

        foreach(Key key in dict.Keys) {
            if(dict[key].Equals(value)) {
                return key as GameObject;
            }
        }

        return null;
    }

    public GameObject GetKeyByValue<Key, Value>(Value value,Dictionary<Key,Value> dict,Dictionary<Key,int> check) {
        int index = -1;

        foreach(Key key in dict.Keys) {
            index++;
            if(dict[key].Equals(value) && !check.ContainsKey(key)) {
                check.Add(key,index);
                return key as GameObject;
            }
        }

        return null;
    }

    public void WaitToBreak() {
        StartCoroutine(WaitBridge());
    }

    private IEnumerator WaitBridge() {
        yield return new WaitForSeconds(1.0f);

        GetPlayers()[GetActualPlayer()].GetComponent<UserUI>().useBomb = false;
        GetPlayers()[GetActualPlayer()].GetComponent<UserUI>().cameraView = false;
        GetPlayers()[GetActualPlayer()].GetComponent<UserUI>().showHUD = false;
        GetPlayers()[GetActualPlayer()].GetComponent<UserUI>().showActionButton = false;
        GetPlayers()[GetActualPlayer()].GetComponent<UserMovement>().isTurn = true;
        GetPlayers()[GetActualPlayer()].GetComponent<UserMovement>().waitDiceResult = true;

        getMainCamera().transform.position = new Vector3(-454.4f,5226.9f,-15872.2f);
        getMainCamera().transform.rotation = Quaternion.Euler(0,275.83f,0f);
        getMainCamera().SetActive(true);
        getMainCamera().GetComponent<Camera>().enabled = true;
    }

    public void ActualPlayersInStep(GameObject step,GameObject user) {

        if(step.GetComponent<Step>().stack == null) {
            
            step.GetComponent<Step>().stack = Instantiate(prefabInStep,new Vector3(step.transform.position.x,step.transform.position.y + 35,step.transform.position.z),prefabInStep.transform.rotation);

            step.GetComponent<Step>().stack.transform.parent = GameObject.FindGameObjectsWithTag("StacksObject")[0].transform;

            int length = step.GetComponent<Step>().playerInStep.Count;

            if(length > 0) {
                step.GetComponent<Step>().stack.transform.GetChild(0).localScale = new Vector3(stackSize[length -1].x,stackSize[length - 1].y,stackSize[length -1].z);
                step.GetComponent<Step>().stack.transform.GetChild(1).localPosition = new Vector3(stackPos[length -1].x,0,0);

                step.GetComponent<Step>().stack.transform.GetChild(1).GetChild(0).gameObject.SetActive(true);
                ChangeStackSpritePlayer(step,0,user.name);
            }
            else {
                Destroy(step.GetComponent<Step>().stack);
            }

        }
        else { // Il y a plusieurs joueurs dans la step
            int length = step.GetComponent<Step>().playerInStep.Count;

            if(length == 0) {
                Destroy(step.GetComponent<Step>().stack);
                return;
            }

            step.GetComponent<Step>().stack.transform.GetChild(0).localScale = new Vector3(stackSize[length -1].x,stackSize[length - 1].y,stackSize[length -1].z);
            step.GetComponent<Step>().stack.transform.GetChild(1).localPosition = new Vector3(stackPos[length -1].x,0,0);

            for(int i = 0;i<length;i++) {
                step.GetComponent<Step>().stack.transform.GetChild(1).GetChild(i).gameObject.SetActive(true);

                ChangeStackSpritePlayer(step,i,step.GetComponent<Step>().playerInStep[i].name);
            }

            for(int i = length;i<4;i++) {
                step.GetComponent<Step>().stack.transform.GetChild(1).GetChild(i).gameObject.SetActive(false);
            }
        }
    }

    public void ChangeStackSpritePlayer(GameObject step,int index,String name) {
        switch(name) {
            case "User":
                step.GetComponent<Step>().stack.transform.GetChild(1).GetChild(index).gameObject.GetComponent<SpriteRenderer>().sprite = smallSprites[0];
                break;

            case "Bot_001":
                step.GetComponent<Step>().stack.transform.GetChild(1).GetChild(index).gameObject.GetComponent<SpriteRenderer>().sprite = smallSprites[1];
                break;

            case "Bot_002":
                step.GetComponent<Step>().stack.transform.GetChild(1).GetChild(index).gameObject.GetComponent<SpriteRenderer>().sprite = smallSprites[2];
                break;

            case "Bot_003":
                step.GetComponent<Step>().stack.transform.GetChild(1).GetChild(index).gameObject.GetComponent<SpriteRenderer>().sprite = smallSprites[3];
                break;            
        }

        
        step.GetComponent<Step>().stack.transform.GetChild(1).GetChild(index).gameObject.GetComponent<SpriteRenderer>().transform.localScale = new Vector3(8.940888f,9.624872f,8.940888f);
    }

    public void ChangeHUDSpritePlayer(GameObject hudParent,int index,String name) {
        switch(name) {
            case "User":
                hudParent.transform.GetChild(index).GetChild(0).gameObject.GetComponent<Image>().sprite = smallSprites[0];
                break;

            case "Bot_001":
                hudParent.transform.GetChild(index).GetChild(0).gameObject.GetComponent<Image>().sprite =  smallSprites[1];
                break;

            case "Bot_002":
                hudParent.transform.GetChild(index).GetChild(0).gameObject.GetComponent<Image>().sprite =  smallSprites[2];
                break;

            case "Bot_003":
                hudParent.transform.GetChild(index).GetChild(0).gameObject.GetComponent<Image>().sprite =  smallSprites[3];
                break;            
        }
    }

// Choix de l'ordre des joueurs
    public void DisplayDiceResult(int index,int result) {
       if(index <= 3) {

           diceResultText[index].text = "" + result;
       }
    }
    public void AddResult(int result,int player) {
        results.Add(player,result);
    }

    public void SortPlayers() {
        
        // TRIER LES NUMEROS DU PLUS HAUT AU PLUS BAS
        
        List<int> diceResults = new List<int>();

        foreach(int result in results.Values) {
            diceResults.Add(result);
        }

        diceResults.Sort();
        diceResults.Reverse();

        for(int i = 0;i<diceResults.Count;i++) {

            
        }

       //dialog.isInDialog = true;
        //dialog.currentDialog = dialog.GetDialogByName("EndTextAlpha");
        //StartCoroutine(dialog.ShowText(dialog.currentDialog.Content[0],dialog.currentDialog.Content.Length));

        // RECUPERER DANS LE DICTIONNAIRE LINDEX DU JOUEUR QUI A FAIT LE NUMERO DICERESULTS[i]
        

    }

    private void ChangeStepName() {
        GameObject isleOneBeach = stepParent.transform.GetChild(0).GetChild(0).gameObject;

        for(int i = 0;i<isleOneBeach.transform.childCount;i++) {
            isleOneBeach.transform.GetChild(i).gameObject.name = "step_beach_00" + i;
        }

        GameObject isleOneInterior = stepParent.transform.GetChild(0).GetChild(1).gameObject;

        for(int i = 0;i<isleOneInterior.transform.childCount;i++) {
            isleOneInterior.transform.GetChild(i).gameObject.name = "step_interior_00" + i;
        }

        GameObject isleTwo = stepParent.transform.GetChild(1).gameObject;

        for(int i = 0;i<isleTwo.transform.childCount;i++) {
            isleTwo.transform.GetChild(i).gameObject.name = "step_00" + i;
        }

        GameObject isleThreeFront = stepParent.transform.GetChild(2).GetChild(1).gameObject;

        for(int i = 0;i<isleThreeFront.transform.childCount;i++){
            isleThreeFront.transform.GetChild(i).gameObject.name = "step_front_00" + i;
        }

        GameObject isleThreeLeft = stepParent.transform.GetChild(2).GetChild(2).gameObject;

        for(int i = 0;i<isleThreeLeft.transform.childCount;i++) {
            isleThreeLeft.transform.GetChild(i).gameObject.name = "step_left_00" + i;
        }

        GameObject isleThreeRight = stepParent.transform.GetChild(2).GetChild(3).gameObject;

        for(int i = 0;i<isleThreeRight.transform.childCount;i++) {
            isleThreeRight.transform.GetChild(i).gameObject.name = "step_right_00" + i;
        }
    }

    public void ChangeStateScene(string sceneName,bool state) {
        GameObject[] objects = SceneManager.GetSceneByName(sceneName).GetRootGameObjects();

        foreach(GameObject obj in allMainObjects) {
            if(obj.name == "SFX" && sceneName == "Main")
                return;
            
            obj.SetActive(state);
        }
    }

    public List<MiniGame> minigames = new List<MiniGame>();
    public GameObject render;
    public float maxTimer;
    public float timer;
    public int index;
    public float step;
    private float speed = 0.08f;
    public MiniGame actualMiniGame;

    #region MiniGame Selector

    private void RandomMiniGame() {
        if(maxTimer == 0) {
            render.transform.parent.gameObject.SetActive(true);
            maxTimer = Random.Range(4.0f,6.0f);
        }

        if(timer <= maxTimer) {
            timer += Time.deltaTime;
            step += Time.deltaTime;
            // 0 a 3.5 secondes ca va a la vitesse de 0.2
            if(step >= speed) {
                step = 0;

                render.GetComponent<Image>().sprite = minigames[index].minigameSprite;
                render.transform.parent.GetChild(2).GetChild(0).gameObject.GetComponent<Text>().text = minigames[index].minigameName;
                render.transform.parent.GetChild(2).GetChild(1).gameObject.GetComponent<Text>().text = minigames[index].minigameDesc;
                index++;

                if(maxTimer - timer <= 2) {
                    speed += 0.1f;
                }

                if(index >= 3)
                    index = 0;
            }

        }
        else {
            actualMiniGame = ConvertMiniGameWithName(render.transform.parent.GetChild(2).GetChild(0).gameObject.GetComponent<Text>().text);
            RunDelayed(1.0f,() => {
                
                if(!hasLoadScene) {
                    render.transform.parent.gameObject.SetActive(false);
                    SetPart(GamePart.MINIGAME);

                    hasTurnChange = false;
                    dayController.mainAudio.Stop();
                    GetPlayers()[actualPlayer].GetComponent<UserUI>().showTurnInfo = false;
                    loadingScene.loadScene = true;
                    hasLoadScene = true;
                    speed = 0.08f;
                    timer = 0;
                    maxTimer = 0;
                    index = 0;
                    step = 0;
                    hasChangeState = false;
                }

            });
        }


    }

    private MiniGame ConvertMiniGameWithName(string name) {
        switch(name) {
            case "FindPath":
                return minigames[0];
                break;
            
            case "Archery":
                return minigames[1];
                break;

            case "KeyBall":
                return minigames[2];
                break;
        }

        return null;
    }

    public bool hasClassementShow;
    public bool displayReward;
    public bool moove;
    private Dictionary<GameObject,GameObject> playerMove = new Dictionary<GameObject,GameObject>();
    private Dictionary<GameObject,Vector2> playerPositionToGo = new Dictionary<GameObject,Vector2>();
    
    private List<GameObject> classedPlayer;


    public void EndMiniGame(GameObject[] classementPanels,List<GameObject> winners,GameObject endText) {
        hasLoadScene = false;

        RunDelayed(3f,() => {
            if(!hasClassementShow) {
                    
                endText.gameObject.SetActive(false);
                ActualizePlayerClassement();
                classementPanels[0].transform.parent.gameObject.SetActive(true);

                for(int i = 0;i<classementPanels.Length;i++)  {

                    classementPanels[i].SetActive(true);
                    classedPlayer = GetClassedPlayers().Keys.ToList();
                    UserInventory inv = classedPlayer[i].GetComponent<UserInventory>();

                    classementPanels[i].transform.GetChild(1).gameObject.GetComponent<Text>().text = "" +  (FindPlayerClassement(GetKeyByValue(i,GetClassedPlayers())) + 1);
                    classementPanels[i].transform.GetChild(1).gameObject.GetComponent<Text>().color = GetClassedColors()[FindPlayerClassement(GetKeyByValue(i,GetClassedPlayers()))];
                    classementPanels[i].transform.GetChild(0).gameObject.GetComponent<Image>().sprite = GetPlayerSprite(classedPlayer[i]);
                        
                    string name = classedPlayer[i].name;
                    if(classedPlayer[i].name.Contains("00")) 
                        name = classedPlayer[i].name.Replace("0","");

                    classementPanels[i].transform.GetChild(2).gameObject.GetComponent<Text>().text = name;
                    classementPanels[i].transform.GetChild(3).GetChild(1).gameObject.GetComponent<Text>().text = "" + inv.coins;
                    classementPanels[i].transform.GetChild(4).GetChild(1).gameObject.GetComponent<Text>().text =  "" + inv.cards;
                }

                hasClassementShow = true;
            }

            Debug.Log("winners: " + winners);
            Debug.Log("count: " + winners.Count);

            if(winners != null && winners.Count > 0) { // Un joueur a gagné 
                RunDelayed(1.5f,() => {
                    Debug.Log("displayReward: " + displayReward);   
                    if(!displayReward) {
                        foreach(GameObject winner in winners) {

                            ConvertPlayer(winner).transform.parent.gameObject.SetActive(true);
                            ConvertPlayer(winner).transform.parent.parent.gameObject.SetActive(true);
                            ConvertPlayer(winner).SetActive(true);
                            ConvertPlayer(winner).GetComponent<UserAudio>().CoinsGain();
                            classementPanels[ConvertPlayerIndex(winner)].transform.parent.GetChild(5).gameObject.SetActive(true);
                            ConvertPlayer(winner).GetComponent<UserInventory>().coins += 15;
                            classementPanels[ConvertPlayerIndex(winner)].transform.parent.GetChild(5).GetChild(0).gameObject.GetComponent<Text>().text = "+" + 15;

                            int index = -1;
                            foreach(GameObject obj in classedPlayers.Keys) {
                                index++;
                                if(obj.name == winner.name) {
                                    switch(index) {
                                        case 0:
                                            classementPanels[ConvertPlayerIndex(winner)].transform.parent.GetChild(5).gameObject.GetComponent<CoinsReward>().beginY = 204;
                                            break;

                                        case 1:
                                            classementPanels[ConvertPlayerIndex(winner)].transform.parent.GetChild(5).gameObject.GetComponent<CoinsReward>().beginY = 46;
                                            break;

                                        case 2:
                                            classementPanels[ConvertPlayerIndex(winner)].transform.parent.GetChild(5).gameObject.GetComponent<CoinsReward>().beginY = -112;
                                            break;

                                        case 3:
                                            classementPanels[ConvertPlayerIndex(winner)].transform.parent.GetChild(5).gameObject.GetComponent<CoinsReward>().beginY = -271;
                                            break;
                                    }
                                }
                            }
                                
                            classementPanels[ConvertPlayerIndex(winner)].transform.parent.GetChild(5).gameObject.GetComponent<CoinsReward>().changePos = true;
                            classementPanels[ConvertPlayerIndex(winner)].transform.parent.GetChild(5).gameObject.GetComponent<CoinsReward>().hasFinishAnimation = false;
                            classementPanels[ConvertPlayerIndex(winner)].transform.parent.GetChild(5).gameObject.GetComponent<CoinsReward>().minigame = true;
                            classementPanels[ConvertPlayerIndex(winner)].transform.parent.GetChild(5).gameObject.GetComponent<CoinsReward>().RunCoroutine();

                        }
                        displayReward = true;
                    }

                    
                    if(classementPanels != null && classementPanels[ConvertPlayerIndex(winners[0])].transform.parent.GetChild(5).gameObject.GetComponent<CoinsReward>().hasFinishAnimation) {
                        Debug.Log("enterActualizing");
                        RunDelayed(1f,() => {
                            
                            ActualizePlayerClassement();

                            List<GameObject> newClassedPlayers = GetClassedPlayers().Keys.ToList();

                            for(int i = 0;i<newClassedPlayers.Count;i++) {
                                if(newClassedPlayers[i] != classedPlayer[i]) {
                                    for(int j = 0;j<classedPlayer.Count;j++) {
                                        if(classedPlayer[j] == newClassedPlayers[i]) {
                                            if(!playerMove.Keys.Contains(newClassedPlayers[j]))
                                                playerMove.Add(newClassedPlayers[j],classedPlayer[j]);
                                        }
                                    }
                                }
                            }                               

                            if(!moove) {

                                if(playerMove.Keys.Count == 0) {
                                    for(int i = 0;i<newClassedPlayers.Count;i++) {
                                        UserInventory inv = ConvertPlayer(newClassedPlayers[i]).GetComponent<UserInventory>();
                                        classementPanels[i].transform.GetChild(3).GetChild(1).gameObject.GetComponent<Text>().text = "" + inv.coins;
                                        classementPanels[i].transform.GetChild(1).gameObject.GetComponent<Text>().text = ""  + (FindPlayerClassement(newClassedPlayers[i]) + 1);     
                                        classementPanels[i].transform.GetChild(1).gameObject.GetComponent<Text>().color = GetClassedColors()[FindPlayerClassement(newClassedPlayers[i])];
                                    }
                                }

                                foreach(GameObject pMove in playerMove.Keys) {

                                    UserInventory inv = pMove.GetComponent<UserInventory>();
                                    int index = -1;
                                    int indexMove = -1;
                                    GameObject playerInfo = null;
                                    GameObject playerMoveInfo = null;

                                    for(int i = 0;i<classedPlayer.Count;i++) {
                                        if(pMove == classedPlayer[i])
                                            index = i;
                                        if(classedPlayer[i] == playerMove[pMove]) 
                                            indexMove = i;
                                    }

                                    if(index == 0)
                                        playerInfo = classementPanels[0];
                                    else if(index == 1)
                                        playerInfo = classementPanels[1];
                                    else if(index == 2)
                                        playerInfo = classementPanels[2];
                                    else if(index == 3)
                                        playerInfo = classementPanels[3];

                                    if(indexMove == 0) 
                                        playerMoveInfo = classementPanels[0];
                                    else if(indexMove == 1) 
                                        playerMoveInfo = classementPanels[1];                           
                                    else if(indexMove == 2)
                                        playerMoveInfo = classementPanels[2];
                                    else if(indexMove == 3)
                                        playerMoveInfo = classementPanels[3];
                                            
                                    if(!playerPositionToGo.Keys.Contains(playerInfo)) 
                                        playerPositionToGo.Add(playerInfo,playerMoveInfo.transform.position);

                                    playerInfo.transform.GetChild(3).GetChild(1).gameObject.GetComponent<Text>().text = "" + inv.coins;
                                    playerInfo.transform.GetChild(1).gameObject.GetComponent<Text>().text = ""  + (FindPlayerClassement(pMove) + 1);     
                                    playerInfo.transform.GetChild(1).gameObject.GetComponent<Text>().color = GetClassedColors()[FindPlayerClassement(pMove)];
                                                                                    
                                }

                                moove = true;
                            }

                            foreach(GameObject pInfo in playerPositionToGo.Keys) {
                                pInfo.transform.position = Vector2.MoveTowards(pInfo.transform.position,playerPositionToGo[pInfo],100 * Time.deltaTime);
                            }

                            float wait;

                            if(playerPositionToGo.Keys.Count == 0) 
                                wait = 3;
                            else 
                                wait = 14;
                            
                            RunDelayed(wait,() => {
                                if(!hasTurnChange) {
                                    turn++;
                                    nightIndex--;
                                    if(nightIndex < 0) {
                                        if(difficulty == 0)
                                        nightIndex = 4;
                                        if(difficulty == 1)
                                            nightIndex= 3;
                                        if(difficulty == 2)
                                            nightIndex = 2;
                                    }
                                    
                                    ManageTurn();

                                    hasTurnChange = true;
                                }

                                displayReward = false;
                                hasClassementShow = false;
                                moove = false;
                                playerMove.Clear();
                                playerPositionToGo.Clear();
                                classedPlayer.Clear();
                                SetPart(GamePart.PARTYGAME);
                                BeginTurn(false,false);
                            });
                        });
                    }
                    else {
                        RunDelayed(3f,() => {
                            if(!hasTurnChange) {
                                turn++;
                                nightIndex--;
                                if(nightIndex < 0) {
                                    if(difficulty == 0)
                                        nightIndex = 4;
                                    if(difficulty == 1)
                                        nightIndex= 3;
                                    if(difficulty == 2)
                                        nightIndex = 2;
                                }
                                            
                                ManageTurn();
                                hasTurnChange = true;
                            }

                            displayReward = false;
                            hasClassementShow = false;
                            moove = false;
                            playerMove.Clear();
                            playerPositionToGo.Clear();
                            classedPlayer.Clear();
                            SetPart(GamePart.PARTYGAME);
                            BeginTurn(false,false);

                        });
                    }
                });
            }
            else {
                RunDelayed(3f,() => {
                    if(!hasTurnChange) {
                        turn++;
                        nightIndex--;
                        if(nightIndex < 0) {
                            if(difficulty == 0)
                                nightIndex = 4;
                            if(difficulty == 1)
                                nightIndex= 3;
                            if(difficulty == 2)
                                nightIndex = 2;
                        }
                                    
                        ManageTurn();
                        hasTurnChange = true;
                    }

                    displayReward = false;
                    hasClassementShow = false;
                    moove = false;
                    playerMove.Clear();
                    playerPositionToGo.Clear();
                    classedPlayer.Clear();
                    SetPart(GamePart.PARTYGAME);
                    BeginTurn(false,false);

                });
            }
        });    
    }

    private bool ContainsTag(string tag,List<GameObject> winners) {
        foreach(GameObject winner in winners) {
            if(winner.tag == tag)
                return true;
        }

        return false;
    }
    private GameObject ConvertPlayer(GameObject player) {
        switch(player.name) {
            case "User":
                return GetPlayers()[0];
                break;

            case "Bot_001":
                return GetPlayers()[1];
                break;

            case "Bot_002":
                return GetPlayers()[2];
                break;

            case "Bot_003":
                return GetPlayers()[3];
                break;            
        }

        return null;
    }

    private int ConvertPlayerIndex(GameObject player) {
        switch(player.name) {
            case "User":
                return 0;
                break;

            case "Bot_001":
                return 1;
                break;

            case "Bot_002":
                return 2;
                break;

            case "Bot_003":
                return 3;
                break;            
        }

        return -1;
    }
    
    private Sprite GetPlayerSprite(GameObject player) {
        switch(player.name) {
            case "User":
                return smallSprites[0];
                break;

            case "Bot_001":
                return smallSprites[1];
                break;

            case "Bot_002":
                return smallSprites[2];
                break;

            case "Bot_003":
                return smallSprites[3];
                break;            
        }

        return null;

    }

    #endregion

}
