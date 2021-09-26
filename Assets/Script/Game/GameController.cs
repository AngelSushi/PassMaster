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

    // Refaire la bombe qu'elle se pose la ou est le joueur plutot que de détruire le pont . Le pont est inaccessible la nuit

    public enum GamePart {
        DIALOG_START_ALPHA,
        DIALOG_TUTORIAL,
        TUTORIAL,
        CHOOSE_ORDER,
        PARTYGAME,
        CHOOSE_MINIGAME,
        MINIGAME
    }

    public GameObject dice; 
    public GameObject prefabInStep;
    public GameObject stepParent;

    //public GamePart partOfGame = GamePart.TUTORIEL;
    public GameObject light;
    public GameObject[] diceResultObj = new GameObject[4];
    public Text[] diceResultText = new Text[4];
    


    public int actualPlayer;
    public GameObject[] players = new GameObject[4];

    public Dictionary<GameObject,int> classedPlayers = new Dictionary<GameObject,int>();

    private SortedDictionary<int,int> results = new SortedDictionary<int,int>();
    
    public int[] turnOrder = new int[4];

    public GameObject mainCamera;
    public TextAsset dialogsFile; 

    public GamePart part;

// Chest Manage
    public GameObject chestParent;
    public GameObject actualChest;

    public Vector3[] posBegin = new Vector3[4];

    public Dictionary<GameObject,GameObject> playerConflict = new Dictionary<GameObject,GameObject>();

    public Vector3[] stackPos = new Vector3[4];
    public Vector3[] stackSize = new Vector3[4];

    public Sprite[] smallSprites = new Sprite[4];
    public Color[] classedColors = new Color[4];
    public int turn;


    public int[] secretCode;

    public bool hasGenChest;
    private bool checkLastChest;
    private int randomIndex = -1;
    private bool hasBeginGame;
    public static int difficulty;
    public bool freeze;

    public List<ShopItem> shopItems;

    public List<Material> diceMaterials;
    public List<GameObject> prefabObjects = new List<GameObject>();

    public Shader invicibilityShader;

    public LoadingScene loadingScene;

    public List<GameObject> allSteps;

    public List<GameObject> allMainObjects;

    public int nightIndex;

    private  List<int> points = new List<int>();
    public Dictionary<GameObject,int> playerPoint = new Dictionary<GameObject,int>();

    private GamePart lastPart;

    public bool hasChangeState;
    private bool isFirstChest = true;

    public DayController dayController;
    public DialogController dialog;
    public MiniGameController mgController;
    public OrderController orderController;


    void Start() {
        GameController.difficulty = 2;

        classedPlayers.Add(players[0],1);
        classedPlayers.Add(players[1],2);
        classedPlayers.Add(players[2],3);
        classedPlayers.Add(players[3],4);

        dialog.dialogs = JsonUtility.FromJson<DialogArray>(dialogsFile.text);
        
        ChangeStepName();
    }
    
    void Update() {
        if(part != lastPart) 
            ChangePart();

        if(!hasGenChest && !dialog.isInDialog)
            GenerateChest();

        lastPart = part;
    }

    private void ChangePart() {
        if(part == GamePart.CHOOSE_ORDER) 
            orderController.BeginOrder();   

        if(part == GamePart.PARTYGAME) { // Faire en sorte que ca soit appelé 
            mainCamera.transform.position = new Vector3(players[0].transform.position.x,5479f,players[0].transform.position.z);
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
        if(part == GamePart.CHOOSE_MINIGAME) {
            mgController.RandomMiniGame();
            return;
        }

        

    }

    public void ManageTurn() {
        players[actualPlayer].GetComponent<UserUI>().ChangeTurnValue(turn,nightIndex);

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
        actualChest = null;
        freeze = true;


        if(randomIndex == -1) {
            randomIndex = Random.Range(0,chestParent.transform.childCount - 1);
            mainCamera.transform.position = new Vector3(-1062.2f,5479f,-15821.6f);
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

        Vector3 cameraPosition = new Vector3(chest.transform.position.x,5479f,chest.transform.position.z);
        
        mainCamera.transform.rotation = Quaternion.Euler(90f,265.791f,0f); 
        mainCamera.transform.position = Vector3.MoveTowards(mainCamera.transform.position,cameraPosition,75 * Time.deltaTime);
        
        if(mainCamera.transform.position == cameraPosition) {
            players[actualPlayer].GetComponent<UserAudio>().SpawnChest();
            chest.SetActive(true);
            actualChest = chest;
            hasGenChest = true;
            
            if(!hasBeginGame) {
                BeginTurn(false);
                hasBeginGame = true;
                freeze = false;
                randomIndex = -1;
            }
            else {
                players[actualPlayer].GetComponent<UserMovement>().actualStep.GetComponent<Step>().chest.SetActive(false);
                players[actualPlayer].GetComponent<UserMovement>().isTurn = false;
                players[actualPlayer].GetComponent<UserMovement>().returnToStep = true;
                
                mainCamera.transform.position = new Vector3(-454.4f,5226.9f,-15872.2f);
                mainCamera.transform.rotation = Quaternion.Euler(0,275.83f,0f);
                mainCamera.SetActive(true);
                mainCamera.GetComponent<Camera>().enabled = true;

                EndUserTurn();

                freeze = false;
            }
            // Marche que pour le tour 1 attention jpense 
        }
    }

    public int GetLastChest() {
        for(int i = 0;i<chestParent.transform.childCount;i++) {
            if(chestParent.transform.GetChild(i).gameObject.activeSelf) 
                return i; 
        }

        return -1;
    }

    public void RandomSecretCode() {
        for(int i = 0;i<secretCode.Length;i++) {
            secretCode[i] = Random.Range(0,9);
        }
    }

    public int GetPlayerPoints(GameObject player) {
        return player.GetComponent<UserInventory>().cards * 100000 + player.GetComponent<UserInventory>().coins;
    }

    public GameObject GetActualStepChest() {
        Step[] steps = GameObject.FindObjectsOfType<Step>();

        foreach(Step step in steps) {
            if(step.chest == actualChest) 
                return step.gameObject;    
        }

        return null;
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

        foreach(GameObject player in players) {
            UserInventory inv = player.GetComponent<UserInventory>();

            int point = inv.cards * 100000 + inv.coins;
            if(!playerPoint.Keys.Contains(player))
                playerPoint.Add(player,point);
        }

        points = playerPoint.Values.ToList();
        
        points.Sort();
        points.Reverse();

        
        for(int i = 0;i<points.Count;i++) 
            GetKeyByValue(points[i],playerPoint,classedPlayers);   
    }

    public void BeginTurn(bool repair) {

        actualPlayer = 0;
        if(turn > 1 && !repair && !hasChangeState) {
            ChangeStateScene("Main",true);
            SceneManager.UnloadSceneAsync(mgController.actualMiniGame.minigameName);
            hasChangeState = true;
        }

        part = GameController.GamePart.PARTYGAME;

        List<GameObject> playersInStack = GetPlayersInStack();

        for(int i = 0;i<players.Length;i++) {
            if(turn == 1)
                players[i].transform.position = posBegin[i];

            players[0].transform.rotation = Quaternion.Euler(0f,-294.291f,0f);
        }
            
        
/*
        players[0].transform.GetChild(1).gameObject.SetActive(players[0].transform.GetChild(1).gameObject.activeSelf);  

        players[1].SetActive(!(!playersInStack.Contains(players[1])));
        players[1].GetComponent<UnityEngine.AI.NavMeshAgent>().enabled = false;

        Debug.Log("tuuuuurn");

        players[2].SetActive(!(!playersInStack.Contains(players[2])));

        players[2].GetComponent<UnityEngine.AI.NavMeshAgent>().enabled = false;

        players[3].SetActive(!(!playersInStack.Contains(players[3])));
        players[3].GetComponent<UnityEngine.AI.NavMeshAgent>().enabled = false;

        */
        players[0].GetComponent<UserMovement>().isTurn = true;

       /* if(! players[0].activeSelf) {
            players[0].SetActive(true);
            players[0].GetComponent<UserMovement>().actualStep.GetComponent<Step>().playerInStep.Remove(players[0]);
            ActualPlayersInStep(players[0].GetComponent<UserMovement>().actualStep,players[0]);
        }
        */

        players[0].GetComponent<UserMovement>().enabled = true;
        players[0].GetComponent<UserUI>().enabled = true;
        players[0].GetComponent<UserAudio>().enabled = true;
        players[0].GetComponent<UserInventory>().enabled = true;
        players[0].GetComponent<UserUI>().showHUD = players[0].GetComponent<UserMovement>().isPlayer;
        players[0].GetComponent<UserUI>().showTurnInfo = players[0].GetComponent<UserMovement>().isPlayer;
        players[0].GetComponent<UserUI>().showActionButton = players[0].GetComponent<UserMovement>().isPlayer;

        ManageCameraPosition();       

        light.transform.rotation = Quaternion.Euler(39.997f,-74.92f,-0.283f);
    }

    public void EndUserTurn() {
        players[actualPlayer].GetComponent<UserMovement>().finishTurn = false;
        players[actualPlayer].GetComponent<UserMovement>().isTurn = false;
        players[actualPlayer].GetComponent<UserMovement>().hasGenDice = false;
        
        if(actualPlayer < 3) {
            actualPlayer++;
            players[actualPlayer].GetComponent<UserMovement>().isTurn = true;

            if(! players[actualPlayer].activeSelf) {
                players[actualPlayer].SetActive(true);
                players[actualPlayer].GetComponent<UserMovement>().actualStep.GetComponent<Step>().playerInStep.Remove(players[actualPlayer]);
                ActualPlayersInStep(players[actualPlayer].GetComponent<UserMovement>().actualStep,players[actualPlayer]);
            }

            if(turn == 1) 
                players[actualPlayer].transform.position = posBegin[0];

            players[actualPlayer].GetComponent<UserMovement>().waitDiceResult = true;
            players[actualPlayer].GetComponent<NavMeshAgent>().enabled = true;
            players[actualPlayer].SetActive(true);

            if(turn > 1) 
                ManageCameraPosition();    

        }
        else { // Le tour est fini. Lancement d'un mini jeux
            part = GamePart.CHOOSE_MINIGAME;
            actualPlayer = 0;
            
            ActualizePlayerClassement();
        }
    }

    private void ManageCameraPosition() {
        if(turn == 1) {
            mainCamera.transform.position = new Vector3(-804f,5213f,-15807f);
            mainCamera.transform.rotation = Quaternion.Euler(0,275.83f,0f);
        }
        else {
            mainCamera.transform.position = players[actualPlayer].GetComponent<UserMovement>().actualStep.GetComponent<Step>().camPosition;
            mainCamera.transform.rotation = players[actualPlayer].GetComponent<UserMovement>().actualStep.GetComponent<Step>().camRotation;
        }
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
        // A REFAIRE SANS LE NOM
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

    public void ChangeHUDSpritePlayer(Transform[] panels,int index,String name) {
        // A check si on peut pas tt concaténer en 1 ligne . A REFAIRE SANS LE NOM
        switch(name) {
            case "User":
                panels[index].GetChild(0).gameObject.GetComponent<Image>().sprite = smallSprites[0];
                break;

            case "Bot_001":
                panels[index].GetChild(0).gameObject.GetComponent<Image>().sprite =  smallSprites[1];
                break;

            case "Bot_002":
                panels[index].GetChild(0).gameObject.GetComponent<Image>().sprite =  smallSprites[2];
                break;

            case "Bot_003":
                panels[index].GetChild(0).gameObject.GetComponent<Image>().sprite =  smallSprites[3];
                break;            
        }
    }

    public Sprite GetSpriteByUser(int user) {
        return smallSprites[user];
    }

    public void SortUserSprite() {
        for(int i = 0;i<players.Length;i++) {
            if(players[i].GetComponent<UserUI>().userSprite != smallSprites[i])
                smallSprites[i] = players[i].GetComponent<UserUI>().userSprite;
        }
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

        for(int i = 0;i<isleTwo.transform.childCount;i++) 
            isleTwo.transform.GetChild(i).gameObject.name = "step_00" + i;

        GameObject isleThreeFront = stepParent.transform.GetChild(2).GetChild(1).gameObject;

        for(int i = 0;i<isleThreeFront.transform.childCount;i++)
            isleThreeFront.transform.GetChild(i).gameObject.name = "step_front_00" + i;       

        GameObject isleThreeLeft = stepParent.transform.GetChild(2).GetChild(2).gameObject;

        for(int i = 0;i<isleThreeLeft.transform.childCount;i++) 
            isleThreeLeft.transform.GetChild(i).gameObject.name = "step_left_00" + i;        

        GameObject isleThreeRight = stepParent.transform.GetChild(2).GetChild(3).gameObject;

        for(int i = 0;i<isleThreeRight.transform.childCount;i++) 
            isleThreeRight.transform.GetChild(i).gameObject.name = "step_right_00" + i;
        
    }

    public void ChangeStateScene(string sceneName,bool state) {
        GameObject[] objects = SceneManager.GetSceneByName(sceneName).GetRootGameObjects();

        foreach(GameObject obj in allMainObjects) {
            if(obj.name == "SFX" && sceneName == "Main")
                return;
            
            obj.SetActive(state);
        }
    }

    
}