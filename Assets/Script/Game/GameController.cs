
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AI;
using System.Linq;
using UnityEditor;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using Random=UnityEngine.Random;

using UnityEngine.SceneManagement;

//[ExecuteInEditMode]
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

    public enum Difficulty {
        EASY,
        MEDIUM,
        HARD
    }

    public GameObject dice; 

    public GameObject firstStep;
    
    public int actualPlayer;


    public List<GameObject> allPlayers = new List<GameObject>();

    public GameObject[] players = new GameObject[4];
    public Player[] playersData;
    
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

    public LoadingScene loadingScene;
    private GameObject[] objects;
    public int nightIndex;
    private  List<int> points = new List<int>();
    public Dictionary<GameObject,int> playerPoint = new Dictionary<GameObject,int>();
    private GamePart lastPart;
    public bool hasChangeState;
    private bool isFirstChest = true;
    public Animation blackScreenAnim;
    public Animation circleTransitionAnim;
    
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

    public InputActionAsset inputs;
    [HideInInspector]
    public PlayerInput playerInput;
    
    
    private InputActionMap lastAction;

    public AudioSource mainAudio;

    public Vector3 beginCamPos, beginCamRot;

    private EventSystem _eventSystem;

    public EventSystem EventSystem
    {
        get => _eventSystem;
    }
    
    private void OnEnable() => Instance = this;
    

    void Awake() {
        Instance = this;
        playerInput = GetComponent<PlayerInput>();

        _eventSystem = FindObjectOfType<EventSystem>();
    }

    void Start() {
        difficulty = Difficulty.HARD;

        classedPlayers.Add(players[0],1);
        classedPlayers.Add(players[1],2);
        classedPlayers.Add(players[2],3);
        classedPlayers.Add(players[3],4);

        mainCamera = Camera.main.gameObject;


        dialog.dialogArray = JsonUtility.FromJson<DialogArray>(dialogsFile.text);

        for(int i = 0;i<players.Length;i++) {
            if(turn == 1)
                players[i].transform.position = posBegin[i];  
        }

        for(int i = 0;i<dialog.dialogArray.dialogs.Length;i++) 
            dialog.dialogArray.dialogs[i].id = i;
        
        UpdateSubPath(null,false);
        
        RandomSecretCode();
            
        nightIndex =
            difficulty == Difficulty.EASY ? 4 :
            difficulty == Difficulty.MEDIUM ? 3 :
            difficulty == Difficulty.HARD ? 2 : 4;
    }
    
    void Update() {
        if(part != lastPart) 
            ChangePart();
    
        if (part == GamePart.PARTYGAME && !hasGenChest && !dialog.isInDialog) {
            GenerateChest();
        }
        
        if (lastAction != null && playerInput != null && playerInput.currentActionMap != null && lastAction.name != playerInput.currentActionMap.name)
            Debug.Log("current action map " + playerInput.currentActionMap);


        playerInput = LocalMultiSetup.Instance.Players[actualPlayer].Input;
        lastAction = playerInput.currentActionMap;
        
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
          //  mainCamera.transform.position = new Vector3(players[0].transform.position.x,5479f,players[0].transform.position.z);
          //  mainCamera.transform.rotation = Quaternion.Euler(90f,265.791f,0f); 

            
            
            ManageTurn();
            ActualizePlayerClassement();
        }
        if(part == GamePart.CHOOSE_MINIGAME) {

            foreach (GameObject player in players) {
                player.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;
            }
            
            StartCoroutine(mgController.RandomMiniGame());
            //mainAudio.enabled = false;
            mainAudio.volume /= 2f;
            turn++;
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

        if(randomIndex == -1) 
            randomIndex = Random.Range(0,chestParent.transform.childCount - 1);

        int lastIndex = GetLastChest();
        bool hasLastChest = (lastIndex != -1);
 
        if(hasLastChest && !checkLastChest) {
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
            foreach(Step step in FindObjectsOfType<Step>()) {
                if(step.chest != null && step.chest == chest) {
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
                
                players[actualPlayer].GetComponent<UserMovement>().userCam.SetActive(true);
                players[actualPlayer].GetComponent<UserMovement>().agent.enabled = false;
                RunDelayed(0.8f,() => {

                    if (players[actualPlayer].GetComponent<User>().isTurn && players[actualPlayer].GetComponent<UserMovement>().userCam.activeSelf) {
                        players[actualPlayer].GetComponent<UserMovement>().userCam.SetActive(false);
                    }
                    
                    players[actualPlayer].GetComponent<UserMovement>().agent.enabled = true;
                    chestParent.transform.GetChild(lastIndex).gameObject.SetActive(false);

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
                        players[actualPlayer].GetComponent<UserMovement>().isTurn = false;
                        
                        //mainCamera.transform.position = new Vector3(-454.4f,5226.9f,-15872.2f);
                       // mainCamera.transform.rotation = Quaternion.Euler(0,275.83f,0f);

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
                if(!right && left) 
                    return obj.transform.forward * distance + obj.transform.right * -1 * distance;
                 
                return obj.transform.forward * distance;
            }
            if(back) {
                if(right && !left) 
                    return obj.transform.forward * -1 * distance + obj.transform.right * distance;
                if(!right && left) 
                    return obj.transform.forward * -1 *  distance + obj.transform.right * -1 * distance;
                 
                return obj.transform.forward * -1 * distance;
            }
            if(right) 
                return obj.transform.right * distance;
            if(left)
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


        for (int i = 0; i < points.Count; i++) {
            GetKeyByValue(points[i], playerPoint, classedPlayers);
        }

        for (int i = 0; i < classedPlayers.Count; i++) 
            classedPlayers.Keys.ToArray()[i].transform.SetSiblingIndex(i);
    }

    public void BeginTurn(bool repair) {
        actualPlayer = 0;
        if (turn > 1) {
            RunDelayed(1.5f, () => { // Wait the middle of transition
                GameObject step = players[actualPlayer].GetComponent<UserMovement>().actualStep != null
                    ? players[actualPlayer].GetComponent<UserMovement>().actualStep
                    : firstStep;

                ManagePlayerInStep(step.GetComponent<Step>(), players[actualPlayer]);

                if (!repair && !hasChangeState && !debugController.skipMG) {
                    ChangeStateScene(false,mgController.actualMiniGame.minigameName);

                    RunDelayed(1f, () => { ChangeStateScene(true, "NewMain"); });
                    
                    RunDelayed(2f,() => {
                        if (mgController.actualMiniGame != null)
                            SceneManager.UnloadSceneAsync(mgController.actualMiniGame.minigameName);
                    });
                    
                    hasChangeState = true;
                    mainAudio.enabled = true;
                }
            });
        }
        else 
            UpdateSubPath(players[0].GetComponent<UserMovement>(),false);

        part = GamePart.PARTYGAME;

        
        players[0].transform.rotation = Quaternion.Euler(0f, -294.291f, 0f);
        players[0].GetComponent<UserMovement>().isTurn = true;
        players[0].GetComponent<UserMovement>().enabled = true;
        //players[0].GetComponent<UserMovement>().rb.constraints = RigidbodyConstraints.FreezeRotation | RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezePositionZ;
        players[0].GetComponent<UserUI>().enabled = true;
        players[0].GetComponent<UserAudio>().enabled = true;
        players[0].GetComponent<UserInventory>().enabled = true;

        //if (!players[0].GetComponent<UserUI>().showActionButton.value && players[0].GetComponent<UserMovement>().currentAction != UserAction.DICE) 
        players[0].GetComponent<UserUI>().showActionButton.value = players[0].GetComponent<UserMovement>().isPlayer;
        
        players[0].GetComponent<UserUI>().showHUD = true;
    
        ManageCameraPosition();
    }

    public void EndUserTurn() {

        Debug.Log("end turn of " + players[actualPlayer].name);
        
        players[actualPlayer].GetComponent<UserUI>().ClearDiceResult();
        
        if(players[actualPlayer].transform.parent.CompareTag("Shell")) {
            
            GameObject shell = players[actualPlayer].transform.parent.gameObject;

            UnityEditorInternal.ComponentUtility.CopyComponent(players[actualPlayer].transform.parent.gameObject.GetComponent<UserMovement>());
            UnityEditorInternal.ComponentUtility.PasteComponentAsNew(players[actualPlayer]);

            UnityEditorInternal.ComponentUtility.CopyComponent(players[actualPlayer].transform.parent.gameObject.GetComponent<UserUI>());
            UnityEditorInternal.ComponentUtility.PasteComponentAsNew(players[actualPlayer]);
            
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

        players[actualPlayer].GetComponent<UserMovement>().isTurn = false;

        if(!mainCamera.activeSelf) {
            mainCamera.SetActive(true);
        }
        
        if(actualPlayer < 3) {

            actualPlayer++;
            players[actualPlayer].GetComponent<UserMovement>().isTurn = true;
            
            //players[actualPlayer].GetComponent<UserMovement>().rb.constraints = RigidbodyConstraints.FreezeRotation | RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezePositionZ;
            
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

            if (players[actualPlayer].GetComponent<UserMovement>().actualStep == null)
                return;

            List<GameObject> playersInStep = players[actualPlayer].GetComponent<UserMovement>().actualStep.GetComponent<Step>().playerInStep;
            
            if (playersInStep.Count > 0) {
                GameObject playerToAdd = null;

                float y = playersInStep[0].transform.position.y;
                
                playersInStep.Remove(players[actualPlayer]);

                Vector3 stepPosition = players[actualPlayer].GetComponent<UserMovement>().actualStep.transform.position;
                stepPosition.y = y;
                players[actualPlayer].transform.position = stepPosition;
                

            }

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
        Collider[] colliders = Physics.OverlapSphere(actualStep.transform.position,5,1 << 9);

        actualStep.RemovePlayerInStep(playerIsTurn);
        
        foreach (Collider collider in colliders) {
            if (collider.gameObject != playerIsTurn) {
                Debug.Log(collider.name + " is not correctly placed !");
                actualStep.playerInStep.Add(collider.gameObject);
                actualStep.SwitchPlayerInStep(collider.gameObject);
            }
            
        }
    }

    public void ManageCameraPosition() {
        GameObject actualStep = players[actualPlayer].GetComponent<UserMovement>().actualStep != null ? players[actualPlayer].GetComponent<UserMovement>().actualStep : firstStep;

        Debug.Log("actual " + actualStep);
        Debug.Log("mainCamera " + mainCamera);

        if (mainCamera == null)
            mainCamera = Camera.main.gameObject;
        
        mainCamera.transform.position = new Vector3(actualStep.transform.position.x,actualStep.transform.position.y + 15,actualStep.transform.position.z) - (GetDirection(actualStep,actualStep.GetComponent<Step>(),25f) * 2.25f);

        Vector3 playerPosition = players[actualPlayer].transform.position;
        playerPosition.y += 5f;

        Vector3 direction = playerPosition - mainCamera.transform.position;
        mainCamera.transform.rotation = Quaternion.LookRotation(direction);

        foreach (GameObject player in players) {
            if (players[actualPlayer] != player) {
                if (player.TryGetComponent<UserMovement>(out UserMovement movement)) 
                    player.SetActive(movement.actualStep != null);
            }
        }
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

    public void SortUserSprite() {
        for(int i = 0;i<players.Length;i++) {
            if(players[i].GetComponent<UserUI>().userSprite != smallSprites[i])
                smallSprites[i] = players[i].GetComponent<UserUI>().userSprite;
        }
    }

    public void ChangeStateScene(bool state,string sceneName = "") {
        
        objects = SceneManager.GetSceneByName(sceneName).GetRootGameObjects();
        
        foreach(GameObject obj in objects) {
            if ((obj.name == "SFX" && sceneName == "NewMain") || obj.GetComponent<GameController>() != null || obj.name == "EventSystem" || (obj.GetComponentInChildren<Canvas>() != null && !sceneName.Equals("NewMain"))) {
                obj.SetActive(true);
                continue;
            }
            
            obj.SetActive(state);
        }
    }

    private UserMovement lastBake;
    
    public void UpdateSubPath(UserMovement user,bool add) {

        if (add) { // The Path must have more locations
            if (isAlreadyBaked && (lastBake == null || lastBake == user))
                return;
            
            
            Debug.Log("update my path");
            
            GameObject stepChest = user.actualStep.GetComponent<Step>().chest;

            UpdateOtherSubPath(user,true);
            
            stepChest.SetActive(true);
            bool isAlreadyActive = actualChest == stepChest;
            if(stepChest.transform.childCount > 1)
                stepChest.transform.GetChild(0).gameObject.SetActive(false);

            mainPath.BuildNavMesh();

            
            if(stepChest != actualChest)
                stepChest.SetActive(false);
            
            UpdateOtherSubPath(user,false);
            
            if(stepChest.transform.childCount > 1)
                stepChest.transform.GetChild(0).gameObject.SetActive(isAlreadyActive);

            isAlreadyBaked = true;
            lastBake = user;
        }
        else {
            UpdateOtherSubPath(user,true);
            mainPath.BuildNavMesh();
            UpdateOtherSubPath(user,false);
            
            isAlreadyBaked = false;
        }
    }

    private void UpdateOtherSubPath(UserMovement user,bool active) {
        foreach (GameObject player in players) {
            
            if (player != players[actualPlayer] && player.GetComponent<UserMovement>() != user) { // If there is two players to stepback between
                // activer les path vers les chest des players qui ne sont pas sur la step

                UserMovement userMovement = player.GetComponent<UserMovement>();
                
                if (userMovement.point != Vector3.zero && actualChest != userMovement.actualStep.GetComponent<Step>().chest) {
                    userMovement.actualStep.GetComponent<Step>().chest.SetActive(active);
                }
            }
        }
    }
    
    
    #region Button's Action
    
    // Functions below are called in OnClick button action -- DONT DELETE

    public void EnableCameraView()
    {
        players[actualPlayer].GetComponent<UserUI>().cameraView.value = true;
        Vector3 position = players[actualPlayer].transform.position;
        
        Camera.main.transform.position = new Vector3(position.x,5479f,position.z);
        Camera.main.transform.rotation = Quaternion.Euler(90f,265.791f,0f); 

        players[actualPlayer].GetComponent<UserUI>().DisplayInfoText(new Vector2(971,164),new Color(1.0f,1.0f,1.0f),"Appuyez sur ECHAP pour quitter le mode");
    }

    public void DisableCameraView() => players[actualPlayer].GetComponent<UserUI>().cameraView.value = false;
    
    
    public void OpenInventory() => players[actualPlayer].GetComponent<UserUI>().ManageInventory(true);
    public void CloseInventory() => players[actualPlayer].GetComponent<UserUI>().ManageInventory(false);

    public void SpawnDice()
    {
        UserUI ui = players[actualPlayer].GetComponent<UserUI>();
        
        ui.showHUD = false;
        ui.showActionButton.value = false;
        ui.infoLabel.SetActive(false);
        if(ui.isPlayer) 
            ui.isTurn = true;
                
        ui.movement.waitDiceResult = true;

        Debug.Log("spawn dice");
    }

    public void UseDoubleDice()
    {
        GameObject targetPlayer = players[actualPlayer];
        if (targetPlayer.GetComponent<UserInventory>().doubleDiceItem > 0)
        {
            targetPlayer.GetComponent<UserMovement>().doubleDice = true;
            targetPlayer.GetComponent<UserUI>().CloseActionHUD(true);
        }
        else
        {
            targetPlayer.GetComponent<UserUI>().audio.Error();
        }
    }

    public void UseTripleDice()
    {
        GameObject targetPlayer = players[actualPlayer];
        if (targetPlayer.GetComponent<UserInventory>().tripleDiceItem > 0)
        {
            targetPlayer.GetComponent<UserMovement>().tripleDice = true;
            targetPlayer.GetComponent<UserUI>().CloseActionHUD(true);
        }
        else
        {
            targetPlayer.GetComponent<UserUI>().audio.Error();
        }
    }

    public void UseReverseDice()
    {
        GameObject targetPlayer = players[actualPlayer];
        if (targetPlayer.GetComponent<UserInventory>().reverseDiceItem > 0)
        {
            targetPlayer.GetComponent<UserMovement>().reverseDice = true;
            targetPlayer.GetComponent<UserUI>().CloseActionHUD(true);
        }
        else
        {
            targetPlayer.GetComponent<UserUI>().audio.Error();
        }
    }

    public void UseHourglass()
    {
        GameObject targetPlayer = players[actualPlayer];
        if (targetPlayer.GetComponent<UserInventory>().hourglassItem > 0)
        {
            targetPlayer.GetComponent<UserMovement>().useHourglass = true;
            blackScreenAnim.Play();
        }
        else
        {
            targetPlayer.GetComponent<UserUI>().audio.Error();
        }
    }

    public void MoveLeft()
    {
        players[actualPlayer].GetComponent<UserUI>().showDirection.value = false;
        players[actualPlayer].GetComponent<UserMovement>().left = true;
    }

    public void MoveFront()
    {
        players[actualPlayer].GetComponent<UserUI>().showDirection.value = false;
        players[actualPlayer].GetComponent<UserMovement>().front = true;
    }

    public void MoveBack()
    {
        players[actualPlayer].GetComponent<UserUI>().showDirection.value = false;
        players[actualPlayer].GetComponent<UserMovement>().right = true;
    }

    #endregion
}
