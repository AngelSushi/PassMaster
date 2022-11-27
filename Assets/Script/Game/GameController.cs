using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using PathCreation;
using UnityEditor.AI;
using UnityEngine.AI;
using System.Linq;
using UnityEditor;
using Random=UnityEngine.Random;

using UnityEngine.SceneManagement;
using UnityEngine.Serialization;

[ExecuteInEditMode]
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

    public enum Difficulty
    {
        EASY,
        MEDIUM,
        HARD
    }

    public GameObject dice; 

    public GameObject firstStep;
    
    public int actualPlayer;
    public GameObject[] players = new GameObject[4];

    public Dictionary<GameObject,int> classedPlayers = new Dictionary<GameObject,int>();

    [HideInInspector] public GameObject mainCamera;
    public TextAsset dialogsFile,stepFile; 
    public JsonExcelArray excelArray;

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
    public Difficulty difficulty;
    public bool freeze;

    public List<Material> diceMaterials;

    public LoadingScene loadingScene;
    private GameObject[] objects;
    public int nightIndex;
    private  List<int> points = new List<int>();
    public Dictionary<GameObject,int> playerPoint = new Dictionary<GameObject,int>();
    private GamePart lastPart;
    public bool hasChangeState;
    private bool isFirstChest = true;
    public Animation blackScreenAnim;
    
    public GameObject stepChest;
    
    
    
    public DayController dayController;
    public DialogController dialog;
    public MiniGameController mgController;
    public OrderController orderController;
    public ShopController shopController;
    public ChestController chestController;
    public ItemController itemController;
    public EndAnimationController endAnimationController;

    public DebugController debugController;

    public GameObject shellPrefab;
    public NavMeshSurface mainPath;
    
    [HideInInspector]
    public int currentTabIndex;
    
    public static GameController Instance { get; private set;}

    private bool isAlreadyBaked;

    public bool showStepNames;
    public bool showStepDirections;

    private void OnEnable() => Instance = this;
    

    void Awake() {
        Instance = this;
        mainCamera = Camera.main.gameObject;
        Debug.Log("this awake");
    }

    void Start() {
        difficulty = Difficulty.HARD;

        classedPlayers.Add(players[0],1);
        classedPlayers.Add(players[1],2);
        classedPlayers.Add(players[2],3);
        classedPlayers.Add(players[3],4);

        dialog.dialogArray = JsonUtility.FromJson<DialogArray>(dialogsFile.text);

        for(int i = 0;i<players.Length;i++) {
            if(turn == 1)
                players[i].transform.position = posBegin[i];  
        }

        for(int i = 0;i<dialog.dialogArray.dialogs.Length;i++) 
            dialog.dialogArray.dialogs[i].id = i;
        
        UpdateSubPath(null,false);
    }
    
    void Update() {
        if(part != lastPart) 
            ChangePart();

        if(!hasGenChest && !dialog.isInDialog)
            GenerateChest();

        lastPart = part;
    }

    private void OnDrawGizmos() {
        foreach (Step step in FindObjectsOfType<Step>()) {
            if (showStepNames) {
                GUI.color = Color.black;
                Handles.Label(step.transform.position + Vector3.up * 10, step.name);
            }

            if (showStepDirections) {
                Debug.DrawRay(step.transform.position,Vector3.forward * 20,Color.magenta,2f);
                Debug.DrawRay(step.transform.position,Vector3.right * 20,Color.green,2f);
            }
        }
    }
    
    
    
    

    private void ChangePart() {
        if(part == GamePart.CHOOSE_ORDER) 
            orderController.BeginOrder();   

        if(part == GamePart.PARTYGAME) { // Faire en sorte que ca soit appelé 
            mainCamera.transform.position = new Vector3(players[0].transform.position.x,5479f,players[0].transform.position.z);
            mainCamera.transform.rotation = Quaternion.Euler(90f,265.791f,0f); 

            RandomSecretCode();
            
            nightIndex =
                difficulty == Difficulty.EASY ? 4 :
                difficulty == Difficulty.MEDIUM ? 3 :
                difficulty == Difficulty.HARD ? 2 : 4;
            
            ManageTurn();
            ActualizePlayerClassement();
        }
        if(part == GamePart.CHOOSE_MINIGAME) {
           // StartCoroutine(mgController.RandomMiniGame());
           turn++;
           BeginTurn(false);
        }

        

    }

    public void ManageTurn() {
        players[actualPlayer].GetComponent<UserUI>().ChangeTurnValue(turn,nightIndex);
        dayController.ChangeNaturalDayPeriod(difficulty,nightIndex);
    }

    private void GenerateChest() {
        actualChest = null;
        freeze = true;
        if(chestParent.transform.childCount == 0) {
            Debug.Log("There is no chest on the map. Please put someone");
            return;
        }

        if(randomIndex == -1) {
            randomIndex = Random.Range(0,chestParent.transform.childCount - 1);
        }

        int lastIndex = GetLastChest();
        bool hasLastChest = (lastIndex != -1);
 
        if(hasLastChest && !checkLastChest) {
            chestParent.transform.GetChild(lastIndex).gameObject.SetActive(false);
            while(randomIndex >= (lastIndex - 3) && randomIndex <= (lastIndex + 3)) {
                randomIndex = Random.Range(0,chestParent.transform.childCount - 1);
            }

            checkLastChest = true;
        }

        if( isFirstChest) {
            randomIndex =  2;
           // isFirstChest = false;
        }
        
        GameObject chest = chestParent.transform.GetChild(randomIndex).gameObject;
       // chest.SetActive(true);

        

        chest.GetComponent<Animation>().clip = chestController.chestAnimations[0];

        if(!blackScreenAnim.isPlaying) {
            float timeToWait = 2.4f;

            if(isFirstChest) {    
                blackScreenAnim["BSAnim"].time = 1f;
                timeToWait = 1.1f;
                isFirstChest = false;
            }

            blackScreenAnim.Play();

            chest.SetActive(true);
            foreach(Step step in FindObjectsOfType(typeof(Step))) {
                if(step.chest != null && step.chest.activeSelf) {
                    stepChest = step.gameObject;
                    break;
                }
            }
            chest.SetActive(false);

            if(timeToWait == 1.1f) { // First chest
                mainCamera.transform.position = new Vector3(stepChest.transform.position.x,stepChest.transform.position.y + 10,stepChest.transform.position.z) - GetDirection(stepChest,stepChest.GetComponent<Step>(),25f);
                mainCamera.transform.LookAt(chest.transform);
            }
            else { 
                RunDelayed(0.8f,() => {
                    
                    if(players[actualPlayer].GetComponent<User>().isTurn && players[actualPlayer].GetComponent<UserMovement>().userCam.activeSelf) 
                        players[actualPlayer].GetComponent<UserMovement>().userCam.SetActive(false);

                    mainCamera.transform.position = new Vector3(stepChest.transform.position.x,stepChest.transform.position.y + 10,stepChest.transform.position.z) - GetDirection(stepChest,stepChest.GetComponent<Step>(),25f);
                    mainCamera.transform.LookAt(chest.transform);
                });
            }

            RunDelayed(timeToWait,() => { // 2.4 pour full ; 1.1 
                chest.SetActive(true);
                chest.GetComponent<Animation>().Play();
                StartCoroutine(CameraEffects.Instance.Shake(2f,0.25f));
                hasGenChest = true;
                AudioController.Instance.ambiantSource.clip = AudioController.Instance.earthQuake;
                AudioController.Instance.ambiantSource.volume = 0.2f;
                AudioController.Instance.ambiantSource.Play();

                RunDelayed(2.5f,() => {
                    AudioController.Instance.ambiantSource.Stop();

                    actualChest = chest;
                    hasGenChest = true;
                    randomIndex = -1;
                    chest.GetComponent<Animation>().clip = chestController.chestAnimations[1];

                    if(!hasBeginGame) {
                        BeginTurn(false);
                        hasBeginGame = true;
                        freeze = false;
                    }
                    else {
                        players[actualPlayer].GetComponent<UserMovement>().actualStep.GetComponent<Step>().chest.SetActive(false);
                        players[actualPlayer].GetComponent<UserMovement>().isTurn = false;
                        
                        mainCamera.transform.position = new Vector3(-454.4f,5226.9f,-15872.2f);
                        mainCamera.transform.rotation = Quaternion.Euler(0,275.83f,0f);
                        mainCamera.SetActive(true);
                        mainCamera.GetComponent<Camera>().enabled = true;

                        EndUserTurn();

                        freeze = false;
                    }
                });
            });
        }
    }

    public Vector3 GetDirection(GameObject obj,Step step,float distance) {
        if(step.useVectors.Length > 0) {
            bool forward = step.useVectors[0];
            bool back = step.useVectors[1];
            bool right = step.useVectors[2];
            bool left = step.useVectors[3];

            if(forward) {
                if(right && !left) 
                    return obj.transform.forward * distance + obj.transform.right * distance;
                else if(!right && left) 
                    return obj.transform.forward * distance + obj.transform.right * -1 * distance;
                else 
                    return obj.transform.forward * distance;
            }
            else if(back) {
                if(right && !left) 
                    return obj.transform.forward * -1 * distance + obj.transform.right * distance;
                else if(!right && left) 
                    return obj.transform.forward * -1 *  distance + obj.transform.right * -1 * distance;
                else 
                    return obj.transform.forward * -1 * distance;
            }
            else if(right) 
                return obj.transform.right * distance;
            else if(left)
                return obj.transform.right * -1 * distance;

        }

        return Vector3.zero;
    }

    public int FindIndexInParent(GameObject parent,GameObject targetStep) {

        for(int i = 0;i<parent.transform.childCount;i++){
            if(parent.transform.GetChild(i).gameObject == targetStep) 
                return i;    
        }

        return -1;
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
            secretCode[i] = Random.Range(0,10);
        }
    }

    public int GetPlayerPoints(GameObject player) {
        return player.GetComponent<UserInventory>().cards * 100000 + player.GetComponent<UserInventory>().coins;
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

            int point = GetPlayerPoints(player);
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
        if (turn > 1)
        {
            ManagePlayerInStep(players[actualPlayer].GetComponent<UserMovement>().actualStep.GetComponent<Step>(),
                players[actualPlayer]);

            if (!repair && !hasChangeState && !debugController.skipMG)
            {
                ChangeStateScene(true, "NewMain");
                SceneManager.UnloadSceneAsync(mgController.actualMiniGame.minigameName);
                hasChangeState = true;
            }
        }
        else 
            UpdateSubPath(players[0].GetComponent<UserMovement>(),false);

        part = GamePart.PARTYGAME;

        players[0].transform.rotation = Quaternion.Euler(0f,-294.291f,0f);
        players[0].GetComponent<UserMovement>().isTurn = true;
        players[0].GetComponent<UserMovement>().enabled = true;
        players[0].GetComponent<UserUI>().enabled = true;
        players[0].GetComponent<UserAudio>().enabled = true;
        players[0].GetComponent<UserInventory>().enabled = true;
        players[0].GetComponent<UserUI>().showHUD = players[0].GetComponent<UserMovement>().isPlayer;
        players[0].GetComponent<UserUI>().showTurnInfo = players[0].GetComponent<UserMovement>().isPlayer;
        players[0].GetComponent<UserUI>().showActionButton = players[0].GetComponent<UserMovement>().isPlayer;
        

        ManageCameraPosition();
    }

    public void EndUserTurn() {

        if(players[actualPlayer].transform.parent.CompareTag("Shell")) {
            
            GameObject shell = players[actualPlayer].transform.parent.gameObject;

            UnityEditorInternal.ComponentUtility.CopyComponent(players[actualPlayer].transform.parent.gameObject.GetComponent<UserMovement>());
            UnityEditorInternal.ComponentUtility.PasteComponentAsNew(players[actualPlayer]);

            UnityEditorInternal.ComponentUtility.CopyComponent(players[actualPlayer].transform.parent.gameObject.GetComponent<UserUI>());
            UnityEditorInternal.ComponentUtility.PasteComponentAsNew(players[actualPlayer]);
            
            players[actualPlayer].GetComponent<UserMovement>().useShell = false;
            players[actualPlayer].GetComponent<UserMovement>().agent.radius = 0.5f;
            players[actualPlayer].GetComponent<UserMovement>().agent.height = 1.57f;

            int playerIndex = actualPlayer - 1;

            if(playerIndex < 0)
                playerIndex = players.Length - 1;

            players[actualPlayer].transform.parent = players[playerIndex].transform.parent;

            players[actualPlayer].GetComponent<UserUI>().movement = players[actualPlayer].GetComponent<UserMovement>();
            players[actualPlayer].transform.GetChild(1).gameObject.SetActive(false);

            Destroy(shell);
        }

        players[actualPlayer].GetComponent<UserMovement>().finishTurn = false;
        players[actualPlayer].GetComponent<UserMovement>().isTurn = false;

        if(!mainCamera.activeSelf) {
            mainCamera.SetActive(true);
        }
        
        if(actualPlayer < 3) {
            actualPlayer++;
            players[actualPlayer].GetComponent<UserMovement>().isTurn = true;
            
            if(players[actualPlayer].GetComponent<UserMovement>().actualStep != null)
                ManagePlayerInStep(players[actualPlayer].GetComponent<UserMovement>().actualStep.GetComponent<Step>(),players[actualPlayer]);

            if(!players[actualPlayer].activeSelf) {
                players[actualPlayer].SetActive(true);

            }

            if(turn == 1) {
                players[actualPlayer].transform.position = posBegin[0];
            }

            players[actualPlayer].GetComponent<UserMovement>().enabled = true;
            players[actualPlayer].GetComponent<UserUI>().enabled = true;
            players[actualPlayer].GetComponent<UserAudio>().enabled = true;
            players[actualPlayer].GetComponent<UserInventory>().enabled = true;
            players[actualPlayer].GetComponent<UserMovement>().waitDiceResult = true;
            players[actualPlayer].GetComponent<NavMeshAgent>().enabled = true;
            players[actualPlayer].SetActive(true);

            ManageCameraPosition();    

        }
        else { // Le tour est fini. Lancement d'un mini jeux
            part = GamePart.CHOOSE_MINIGAME;
            actualPlayer = 0;

            if (turn == 1) {
                mainPath.transform.GetChild(mainPath.transform.childCount - 1).gameObject.layer = 0;
                mainPath.BuildNavMesh();
            }
            
            ActualizePlayerClassement();
        }
    }

    public void ManagePlayerInStep(Step actualStep,GameObject playerIsTurn) {

        for (int i = 0; i < actualStep.playerInStep.Count; i++) {
            Vector3 stackPosition = i == 1 ? actualStep.gameObject.transform.GetChild(3).position : actualStep.gameObject.transform.GetChild(2).position;
            
            if (Vector3Int.FloorToInt(stackPosition) == Vector3Int.FloorToInt(playerIsTurn.transform.position)) {
                GameObject playerOnStep = players.Where(player => player.GetComponent<UserMovement>().actualStep == actualStep.gameObject && !actualStep.playerInStep.Contains(player)).ToList()[0];
                
                playerIsTurn.transform.position = playerOnStep.transform.position;
                playerOnStep.transform.position = stackPosition;
                
                Debug.Log("position " + stackPosition);
                Debug.Log("myPos " + playerOnStep.transform.position);
            }
        }
    }

    private void ManageCameraPosition() {
        if(turn == 1) 
            mainCamera.transform.position = new Vector3(firstStep.transform.position.x,firstStep.transform.position.y + 15,firstStep.transform.position.z) - (GetDirection(firstStep,firstStep.GetComponent<Step>(),25f) * 2.25f);
        else {
            GameObject actualStep = players[actualPlayer].GetComponent<UserMovement>().actualStep;
            mainCamera.transform.position = new Vector3(actualStep.transform.position.x,actualStep.transform.position.y + 15,actualStep.transform.position.z) - (GetDirection(actualStep,actualStep.GetComponent<Step>(),25f) * 2.25f);
        }


        Vector3 playerPosition = players[actualPlayer].transform.position;
        playerPosition.y += 5f;

        Vector3 direction = playerPosition - mainCamera.transform.position;
        mainCamera.transform.rotation = Quaternion.LookRotation(direction);
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

    public void ChangeHUDSpritePlayer(Transform[] panels,int index,UserType type) {
        // A check si on peut pas tt concaténer en 1 ligne . A REFAIRE SANS LE NOM
        switch(type) {
            case UserType.PLAYER: // Player
                panels[index].GetChild(0).gameObject.GetComponent<Image>().sprite = smallSprites[0];
                break;

            case UserType.BOT_001:
                panels[index].GetChild(0).gameObject.GetComponent<Image>().sprite =  smallSprites[1];
                break;

            case UserType.BOT_002:
                panels[index].GetChild(0).gameObject.GetComponent<Image>().sprite =  smallSprites[2];
                break;

            case UserType.BOT_003:
                panels[index].GetChild(0).gameObject.GetComponent<Image>().sprite =  smallSprites[3];
                break;            
        }
    }

    public void SortUserSprite() {
        for(int i = 0;i<players.Length;i++) {
            if(players[i].GetComponent<UserUI>().userSprite != smallSprites[i])
                smallSprites[i] = players[i].GetComponent<UserUI>().userSprite;
        }
    }

    public void ChangeStateScene(bool state,string sceneName = "") {
        
        objects = SceneManager.GetSceneByName(sceneName).GetRootGameObjects();
        
        foreach(GameObject obj in objects) {
            if ((obj.name == "SFX" && sceneName == "NewMain") || obj.GetComponent<GameController>() != null || obj.name == "EventSystem") {
                obj.SetActive(true);
                continue;
            }
            
            obj.SetActive(state);
        }
    }

    public void UpdateSubPath(UserMovement user,bool add) {

        if (add) { // The Path must have more locations

            if (isAlreadyBaked)
                return;
            
            Debug.Log("enter add locations");
            GameObject stepChest = user.actualStep.GetComponent<Step>().chest;
            
            
            stepChest.SetActive(true);
            bool isAlreadyActive = actualChest == stepChest;
            if(stepChest.transform.childCount > 1)
                stepChest.transform.GetChild(0).gameObject.SetActive(false);
            
            mainPath.BuildNavMesh();

            stepChest.SetActive(false);
            if(stepChest.transform.childCount > 1)
                stepChest.transform.GetChild(0).gameObject.SetActive(isAlreadyActive);

            isAlreadyBaked = true;
        }
        else {
            mainPath.BuildNavMesh();
            isAlreadyBaked = false;
        }
    }

}
