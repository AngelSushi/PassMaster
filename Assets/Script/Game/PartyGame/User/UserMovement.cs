using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.AI;
using UnityEngine.UI;
using System.Linq;

public class UserMovement : CoroutineSystem {

    public int id;
    public NavMeshAgent agent;
    public bool waitDiceResult;
    public bool isPlayer;
    public bool finishMovement;
    public bool finishTurn;
    public bool left;
    public bool front;
    public bool right;
    public bool stop;
    public bool lastStepIsArrow;
    public bool waitChest;
    public bool returnToStep;
    public bool goToChest;
    public bool returnStepBack;
    public bool goToShop;
    public bool canMooveToShop = true;
    public bool reverseCount;
    public bool doubleDice;
    public bool reverseDice;    
    public bool hasBotBuyItem;
    public bool drop;
    public bool reverse;
    public GameObject lastStep;
    public UserUI ui;
    public UserInventory inventory;
    public UserAudio audio;
    public bool isMooving;
    public GameObject giveUI;
    public GameObject changeUI;
    public bool isTurn;
    public bool hasGenDice;
    public int diceResult;
    public GameObject actualStep;
    public GameObject beginStep;
    public bool canJump;
    public bool isJumping;
    public DayController dayController;
    public Transform nextStep;
    public float jumpSpeed;
    public bool stack;
    public int beginResult;
    public GameObject[] stepPaths;
    public bool stepBack;

    private int isle;
    private bool jump;

    private GameController gameController;

    private int random = -1;
    private float timer;

    private Vector3 point;
    private bool hasCheckPath;

    private bool hasShowChestHUD;
    private bool canMooveToChest = true;

    private bool hasCollideDice;
    private GameObject dice;
    private bool hasJump;
    private bool hasShowShopHUD;
    private bool hasShowShop;

    public Rigidbody rb;
    // Object
    
    private Color actualColor;

    public bool isParachuting;

    private Vector3 parachuteMovement;

    private bool hasBuyItem;

    private bool bypassDirection;

    private bool isInShopCoroutine;

    private void Start() {
        gameController = GameObject.FindGameObjectsWithTag("Game")[0].GetComponent<GameController>();
        dayController = GameObject.FindGameObjectsWithTag("Day")[0].GetComponent<DayController>();
    }

    private void Update() {

        if(!gameController.freeze) {
            if(isTurn) {
                stepBack = false;
                point = Vector3.zero;

                if(!hasGenDice) {
                    if(waitDiceResult) {
                        isMooving = true;
                        if(isPlayer) {
                            if(!ui.showHUD) {
                                canJump = true;
                                dice = gameController.dice;
                                dice.SetActive(true);
                                agent.enabled = false;
                                dice.transform.position = new Vector3(transform.position.x,transform.position.y + 17,transform.position.z);
                                dice.GetComponent<DiceController>().lockDice = false;
                                dice.GetComponent<DiceController>().lastLockDice = true;
                                dice.GetComponent<MeshRenderer>().enabled = true;

                                int matIndex = doubleDice ? 1 : reverseDice ? 2 : 0;
                                dice.GetComponent<MeshRenderer>().material = gameController.diceMaterials[matIndex];

                                hasGenDice = true;
                                stack = false;
                            }
                            else {
                                if(GameObject.FindGameObjectsWithTag("Dice").Length > 0) 
                                    ui.showHUD = false;
                            }
                        }
                        else { // Bot
                            canJump = true;

                            dice = gameController.dice;
                            dice.SetActive(true);
                            agent.enabled = false;
                            dice.transform.position = new Vector3(transform.position.x,transform.position.y + 17,transform.position.z);
                            dice.GetComponent<DiceController>().lockDice = false;
                            dice.GetComponent<DiceController>().lastLockDice = true;
                            dice.GetComponent<MeshRenderer>().enabled = true;

                            int matIndex = doubleDice ? 1 : reverseDice ? 2 : 0;
                            dice.GetComponent<MeshRenderer>().material = gameController.diceMaterials[matIndex];

                            hasGenDice = true;
                            stack = false;
                        }
                    }
                }
                else {
                    if(stop) 
                        agent.enabled = false;

                    if(nextStep != null && !jump && !stop) {
                        if(!transform.GetChild(1).gameObject.activeSelf) {
                            RunDelayed(0.2f,() => {
                                transform.GetChild(1).gameObject.transform.localPosition = new Vector3(-0.07f,2.41f,-3.83f);
                                transform.GetChild(1).gameObject.transform.localRotation = Quaternion.Euler(17.839f,0f,0f);
                                transform.GetChild(1).gameObject.SetActive(true);
                            });
                        }

                        if(!hasCollideDice) {
                            beginResult = diceResult;
                            stepPaths = new GameObject[beginResult]; 
                            hasCollideDice = true;
                            waitDiceResult = false;
                            ui.showHUD = false;
                            if(dice != null) 
                               dice.SetActive(false);
                        }

                        //agent.enabled = true;
                        NavMeshPath path = new NavMeshPath();
                        agent.CalculatePath(nextStep.position,path);
                        ShowPath(Color.magenta,path);
                        CheckPath();
                        agent.SetPath(path);
                    }

                    if(stop && goToShop) {
                        agent.enabled = false;

                        if(canMooveToShop) {
                            Vector3 shopPosition = actualStep.GetComponent<Step>().shop.transform.position;
                            shopPosition.y = transform.position.y;
                            transform.position = Vector3.MoveTowards(transform.position,shopPosition,agent.speed * Time.deltaTime);
                        }
                        else { // Le joueur a collide avec l'entité 
                            if(isPlayer) {
                                if(!ui.showShop && !hasShowShop && lastStep.GetComponent<Step>().type  == StepType.SHOP) {
                                    ui.showShop = true;
                                    hasShowShop = true;
                                }
                            }
                            else {
                                // Est dans le shop

                                if(hasBuyItem && hasBotBuyItem) {

                                    int random = Random.Range(0,100);

                                    int percentage = -1;

                                    if(GameController.difficulty == 0)
                                        percentage = 30;
                                    else if(GameController.difficulty == 1) // Medium
                                        percentage = 60;
                                    else if(GameController.difficulty == 2) // Hard
                                        percentage = 85;

                                    if(random >= 0 && random <= percentage) {
                                        hasBuyItem = ShopBot();
                                    }
                                    else {
                                        canMooveToShop = true;
                                        returnToStep = true;
                                    }

                                    hasBotBuyItem = false;
                                }
                                if(!hasBuyItem) {
                                    canMooveToShop = true;
                                    returnToStep = true;
                                }

                                hasShowShop = true;                                   
                            }
                        }
                    }
                }

                if(returnToStep) {
                    Vector3 returnStep = new Vector3(actualStep.transform.position.x,transform.position.y,actualStep.transform.position.z);

                    if(transform.position != returnStep && actualStep != null) {
                        transform.position = Vector3.MoveTowards(transform.position,returnStep,40 * Time.deltaTime);
                        return;
                    }
                }


                if(finishMovement) {
                    StepType type = actualStep.GetComponent<Step>().type;
                    if(type == StepType.BONUS)  
                        StartCoroutine(WaitBonus(true));
                    if(type == StepType.MALUS) 
                        StartCoroutine(WaitMalus(true));

                    ui.ClearDiceResult();
                    hasCheckPath = false;
                    finishMovement = false;
                }

                if(finishTurn) { // Mettre tout dans gameController pour une meilleur gestion et plus propre
                    nextStep = null;

                    if(actualStep != null && actualStep.GetComponent<Step>() != null && actualStep.GetComponent<Step>().chest != null && actualStep.GetComponent<Step>().chest.activeSelf) {
                        if(isPlayer) {
                            if(!hasShowChestHUD) {
                                ui.showChestHUD = true;
                                hasShowChestHUD = true;
                            }

                            if(goToChest && hasShowChestHUD) {
                                agent.enabled = false;

                                if(canMooveToChest) {
                                    Vector3 chestPosition = new Vector3(actualStep.GetComponent<Step>().chest.transform.position.x,transform.position.y,actualStep.GetComponent<Step>().chest.transform.position.z);
                                    transform.position = Vector3.MoveTowards(transform.position,chestPosition,agent.speed * Time.deltaTime);               
                                }
                                else { // Le joueur collide avec le coffre

                                    if(!waitChest) {                                      
                                        actualStep.GetComponent<Step>().chest.GetComponent<Animator>().SetBool("Open",true);                                        
                                        StartCoroutine(WaitChest());
                                        waitChest = true;
                                    }

                                }
                            }                      
                        }
                        else { // le bot
                            if(inventory.coins >= 30 && inventory.cards < 6) {
                                if(canMooveToChest) {
                                    Vector3 chestPosition = new Vector3(actualStep.GetComponent<Step>().chest.transform.position.x,transform.position.y,actualStep.GetComponent<Step>().chest.transform.position.z);
                                    transform.position = Vector3.MoveTowards(transform.position,chestPosition,agent.speed * Time.deltaTime);               
                                }
                                else { // Le joueur collide avec le coffre

                                    actualStep.GetComponent<Step>().chest.GetComponent<Animator>().SetBool("Open",true);
                                    if(!waitChest) {
                                        
                                        StartCoroutine(WaitChest());
                                        waitChest = true;
                                    }

                                }
                            }
                            else {
                                if(canMooveToChest) 
                                    gameController.EndUserTurn();
                            }
                        }
                    }

                    else 
                        gameController.EndUserTurn();
                    
                    return;
                }             

                if(!isPlayer && waitDiceResult) {
                    if(random == -1) 
                        random = Random.Range(1,5);           

                    timer += Time.deltaTime;

                    if(timer >= random && !hasJump) {
                        Jump();
                        hasJump = true;
                        ui.showHUD = false;
                    }
                    else 
                        ui.showHUD = true;          
                }           
            }

            else {
                agent.enabled = false;
                left = false;
                front = false;
                isMooving = false;
                right = false;
                transform.GetChild(1).gameObject.SetActive(false);
                hasShowChestHUD = false;
                goToChest = false;
                canMooveToChest = true;
                waitChest = false;
                hasCollideDice = false;
                waitDiceResult = true;
                hasJump = false;

                if(drop) 
                    inventory.DropItem(transform.gameObject);

                if(returnToStep) {
                    
                    Vector3 returnStep = new Vector3(actualStep.transform.position.x,transform.position.y,actualStep.transform.position.z);

                    if(transform.position != returnStep && actualStep != null) {
                        transform.position = Vector3.MoveTowards(transform.position,returnStep,agent.speed * Time.deltaTime);
                        return;
                    }

                  // returnToStep = false;
                }

                if(returnStepBack) 
                    StartCoroutine(WaitTimeToReturn());
                

                if(stepBack) {
                    agent.enabled = false;
                    transform.position = Vector3.MoveTowards(transform.position,point,agent.speed * Time.deltaTime);
                }
                else 
                    point = Vector3.zero;
                

                if(stack) { // Pb jaune vert interchangé
                    transform.gameObject.SetActive(false);
                    if(actualStep.GetComponent<Step>().type != StepType.FIX_DIRECTION && !actualStep.GetComponent<Step>().playerInStep.Contains(transform.gameObject)) {
                        actualStep.GetComponent<Step>().playerInStep.Add(transform.gameObject);
                        gameController.ActualPlayersInStep(actualStep,transform.gameObject);
                    }
                    
                }
            }
        }
    }


    private void OnCollisionEnter(Collision hit) {
       if(isTurn) {
           if(hit.gameObject.tag == "Chest") 
                canMooveToChest = false;

            if(hit.gameObject.tag == "Shop_Entity") {        
                canMooveToShop = false;

                if(!isPlayer)
                     hasBuyItem = ShopBot();
            }

            if(hit.gameObject.tag == "Dice") {
                if(diceResult == 0 || diceResult == -1) {
                    diceResult = hit.gameObject.GetComponent<DiceController>().index;

                    if(diceResult == 0) 
                        diceResult = 6;
                    if(doubleDice) 
                        diceResult *= 2;           
                }
                
                agent.enabled = true;
                beginResult = diceResult;  
                stepPaths = new GameObject[beginResult]; 
                hasCollideDice = true;     
                if(ui != null) {
                    if(doubleDice) 
                        actualColor = new Color(0.32f,0.74f,0.08f,1.0f);
                    if(reverseDice) 
                        actualColor = new Color(0.41f,0.13f,0.78f,1.0f);
                    if(!doubleDice && !reverseDice) 
                        actualColor = new Color(0f,0.35f,1f,1.0f);
                }

                ui.RefreshDiceResult(diceResult, actualColor);
                GameObject hitObj = hit.gameObject;
                hitObj.GetComponent<MeshRenderer>().enabled = false;
               
               RunDelayed(0.1f,() => {  hitObj.SetActive(false); });
            }

       } 
    }

    private void OnTriggerEnter(Collider hit) {
        if(isTurn) {
            StepType type = hit.gameObject.GetComponent<Step>() != null ? hit.gameObject.GetComponent<Step>().type : hit.gameObject.GetComponent<Direction>().type;

            if(type == null) 
                return;

            if(type == StepType.BONUS || type == StepType.MALUS || type == StepType.SHOP || type == StepType.FIX_DIRECTION || type == StepType.FLEX_DIRECTION || type == StepType.BONUS_END || type == StepType.MALUS_END || type == StepType.STEP_END) {

                if(type != StepType.SHOP) {
                    if(hasShowShopHUD) 
                        hasShowShopHUD = false;
                    if(hasShowShop) 
                        hasShowShop = false;

                    if(!isPlayer) {
                        if(goToShop) 
                            goToShop = false;
                    }
                }

                if(isJumping) {
                    isJumping = false;  
                    jump = false;
                    waitDiceResult = false;
                    beginStep = hit.gameObject;
                }
                else {
                    if(actualStep != hit.gameObject && diceResult > 0) {
                        actualStep = hit.gameObject;
                        if(diceResult > 0) 
                            ChooseNextStep(type);
                    }
                }

                if(diceResult < 0 && nextStep != null) 
                    finishMovement = true;

                if(type == StepType.BONUS_END && reverseCount) {
                    reverseCount = false;
                    nextStep = hit.gameObject.GetComponent<Direction>().nextStepFront.transform;
                    bypassDirection = true;
                }

                if(type == StepType.FLEX_DIRECTION && !ui.showHUD && !isJumping && !bypassDirection && !lastStepIsArrow && ui.direction == null) { // Le joueur collisionne avec la flèche a 4 côtés
                    Direction direction = hit.gameObject.GetComponent<Direction>();

                    FlexDirection flexDirection = lastStep.GetComponent<FlexDirection>();

                    direction.nextStepFront = flexDirection.frontDirection;
                    direction.nextStepLeft = flexDirection.leftDirection;
                    direction.nextStepRight = flexDirection.rightDirection;

                    ui.direction = direction;

                    if(isPlayer) 
                        ui.showDirection = true;           
                    else {
// A REVOIR
                        NavMeshPath chestPath = new NavMeshPath();

                        if(inventory.cards < 6) 
                            agent.CalculatePath(gameController.GetActualStepChest().transform.position,chestPath);   
                       
                        GameObject pathStep = CheckStepPath(chestPath);

                        if(pathStep == direction.nextStepLeft) 
                            left = true;
                        else if(pathStep == direction.nextStepRight)
                            right = true;
                        else if(pathStep == direction.nextStepFront)
                            front = true;  
 
                    }
                    if(reverse) 
                        reverse = false;
                }

                if(type != StepType.SHOP) {
                    if(isInShopCoroutine) 
                        isInShopCoroutine = false;
                }

                if(stop) 
                    stop = false;
                
            }
        }
    }

    private void OnTriggerStay(Collider hit) {
        if(isTurn) {
            StepType type = hit.gameObject.GetComponent<Step>() != null ? hit.gameObject.GetComponent<Step>().type : hit.gameObject.GetComponent<Direction>().type;

            if(type == StepType.BONUS || type == StepType.MALUS || type == StepType.SHOP || type == StepType.BONUS_END || type == StepType.MALUS_END || type == StepType.STEP_END) {
               if(transform.gameObject.activeSelf) {
                   if(hit.gameObject.GetComponent<Step>() != null && hit.gameObject.GetComponent<Step>().playerInStep.Contains(transform.gameObject)) {
                       hit.gameObject.GetComponent<Step>().playerInStep.Remove(transform.gameObject);
                       gameController.ActualPlayersInStep(hit.gameObject,transform.gameObject);
                   }

                   
               }
    // A modifier avec un rundelayed
                if(type == StepType.STEP_END) {
                    RunDelayed(0.1f,() => {
                        if(diceResult > 0) {
                            if(diceResult == 1) {} // Go to grotte
                            else 
                                reverse = true;
                        }
                    });              
                }

                if(type == StepType.FIX_DIRECTION && isPlayer && !bypassDirection && isTurn) {
                        if(!lastStepIsArrow) {
                            if(!left && !right && !front) {
                                stop = true;
                                return;
                            }

                            else {
                                if(left) 
                                    nextStep = ui.direction.nextStepLeft.transform;
                                if(front)
                                    nextStep = ui.direction.nextStepFront.transform;
                                if(right) 
                                    nextStep = ui.direction.nextStepRight.transform;
                            }

                            stop = false;
                            return;
                        }
                        else 
                            stop = false;
                }

                if(type == StepType.SHOP && returnToStep) 
                    StartCoroutine(WaitToReturnStep());
                

                if(type == StepType.SHOP  && beginStep != hit.gameObject && !hasShowShopHUD) {
                    // Au lieu de la fonction faire une coroutine qui attends genre 1 demi seconde et après faire le reste
                    
                    if(isPlayer) 
                        StartCoroutine(WaitToShop());
                    else { // Bot
                        if(!returnToStep && !goToShop && !isInShopCoroutine) {
                            StartCoroutine(WaitToShopBot());
                            isInShopCoroutine = true;
                        }
                    }
                }

                if(nextStep == null && diceResult > 0) { // Le joueur/bot n'a pas encore commencé a bougé   
                    ChooseNextStep(type);

                    if(ui != null) 
                        ui.RefreshDiceResult(diceResult,actualColor);

                }

                else if(nextStep != null && diceResult == 0) { // Le joueur a fini son chemin
                    nextStep = null;
                    diceResult = -1;
                }
            }
        }
    }

    private void OnTriggerExit(Collider hit) {
        if(isTurn) {
            StepType type = hit.gameObject.GetComponent<Step>() != null ? hit.gameObject.GetComponent<Step>().type : hit.gameObject.GetComponent<Direction>().type;

            foreach(GameObject step in gameController.playerConflict.Values.ToList()) {
                if(hit.gameObject == step) {
                    // On fait revenir le bot sur sa case
                    GameObject user = gameController.GetKeyByValue(step,gameController.playerConflict);
                    UserMovement userMovement = user.GetComponent<UserMovement>();
                    Step targetStep = step.GetComponent<Step>();
                    
                    Debug.Log("hit: " + hit.gameObject);
                    Debug.Log("enterBack: " + userMovement.point);

                    userMovement.point = hit.gameObject.transform.position;/*user.transform.position*/;
                    userMovement.point.y = user.transform.position.y;
                    userMovement.returnStepBack = true;

                  /*  if(targetStep.xAxis) 
                        userMovement.point.x += targetStep.positive ? 10 : -10;
                    if(targetStep.zAxis) 
                        userMovement.point.z += targetStep.positive ? 10 : -10;
                    */
                    gameController.playerConflict.Remove(user); // Error
                }
            }

            if(type == StepType.FIX_DIRECTION || type == StepType.FLEX_DIRECTION) {
                if(!isJumping) 
                    lastStepIsArrow = true;
                if(bypassDirection) 
                    bypassDirection = false;

                hasCheckPath = false;
                ui.direction = null;
            }
            else 
                lastStepIsArrow = false;   

            if(type == StepType.STEP_END) 
                ChooseNextStep(type); 

            if((type == StepType.BONUS_END || type == StepType.MALUS_END) && GameObject.ReferenceEquals(lastStep.transform.parent.gameObject,actualStep.transform.parent.gameObject)) 
                nextStep = actualStep.GetComponent<Direction>().nextStepFront.transform;                    

            left = false;
            front = false;
            right = false;

            if(type == StepType.BONUS || type == StepType.MALUS || type == StepType.SHOP || type == StepType.FIX_DIRECTION || type == StepType.FLEX_DIRECTION || type == StepType.BONUS_END  || type == StepType.MALUS_END || type == StepType.STEP_END) 
                lastStep = hit.gameObject;

        }
    }

    #region Inputs Functions

    public void OnJump(InputAction.CallbackContext e) {
        if(e.started) 
            Jump();       
    } 

    public void OnMove(InputAction.CallbackContext e) {
        if(isParachuting && !gameController.freeze) 
            parachuteMovement = e.ReadValue<Vector2>();   
    } 

    public void OnGive(InputAction.CallbackContext e) {
        if(e.started && isTurn) {
            if(transform.gameObject.name == "User") {
                giveUI.SetActive(true);
            }
        }
    }

    public void OnChange(InputAction.CallbackContext e) {
        if(e.started && isTurn) {
            if(transform.gameObject.name == "User") {
                changeUI.SetActive(true);
            }
        }
    }

    #endregion

    #region Customs Functions

    private void ChooseNextStep(StepType type) {
        if(type != StepType.FIX_DIRECTION && type != StepType.FLEX_DIRECTION && nextStep != null) 
            diceResult--;

        Debug.Log("2");
        GetNextStep();

        if(ui != null) 
            ui.RefreshDiceResult(diceResult,actualColor);

        finishMovement = (diceResult == 0);
    }

    private void CheckPath() {
        if(!hasCheckPath && stepPaths != null) { 
            Transform stepParent = actualStep != null ? actualStep.transform.parent : beginStep.transform.parent;
            int stepIndex = actualStep != null ? FindIndexInParent(stepParent.gameObject,actualStep) : FindIndexInParent(stepParent.gameObject,beginStep);
            int result = actualStep != null ? diceResult : beginResult;

            if(stepIndex == -1) 
                stepIndex = 0;

            for(int i = 0;i<result;i++) {
                if(stepIndex + i + 1 < stepParent.childCount) // On vérifie que le calcul du prochain index de la prochaine step est bien inférieur au nombre de step max
                    stepPaths[i] = stepParent.GetChild(stepIndex + i + 1).gameObject;
                else // Le calcul du prochain index de la prochaine step est supérieur au nombre de step max, on retire donc le nombre de step max au calcul
                    stepPaths[i] = stepParent.GetChild(stepIndex + i + 1 - stepParent.childCount).gameObject;
            }
            hasCheckPath = true;
        }

        StepBackMovement(stepPaths);
    }

    private void GetNextStep() {
        GameObject actualParent = actualStep.transform.parent.gameObject;
        int stepIndex = FindIndexInParent(actualParent,actualStep);

        if(!reverseDice) { // Si le joueur n'utilise pas le dé inverse
            if(stepIndex + 1 < actualParent.transform.childCount) 
                nextStep = actualParent.transform.GetChild(stepIndex + 1);
            else 
                nextStep = actualParent.transform.GetChild(0);
        } 
        else { // Le joueur utilise le dé inverse
            if(stepIndex - 1 > 0) 
                nextStep = actualParent.transform.GetChild(stepIndex - 1);          
            else 
                nextStep = actualParent.transform.GetChild(actualParent.transform.childCount - 1);    
        }                 
    }

    private int FindIndexInParent(GameObject parent,GameObject targetStep) {
        for(int i = 0;i<parent.transform.childCount;i++){
            if(parent.transform.GetChild(i).gameObject == targetStep) 
                return i;    
        }

        return -1;
    }

    private GameObject CheckStepPath(NavMeshPath path) {
        
        for(int i = 0;i<path.corners.Length - 1;i++) {

            // On sait que : 
            //  Xm = Xa + k(Xb-Xa)
            // Avec cette équation on en déduit que : k = (Xm - Xa) / (Xb - Xa)
            // On génère un nombre aléatoire entre Xa et Xb et on vérifie que k est compris dans l'intervalle [0;1]
            // On fait la même chose pour Z

            Vector3 corner = path.corners[i];
            Vector3 nextCorner = path.corners[i+1];

            GameObject[] directionSteps = {ui.direction.nextStepLeft,ui.direction.nextStepRight,ui.direction.nextStepFront};
            // LE coffre n'est pas sur le path ca ne risque pas de marcher récupérer la step du coffre

            foreach(GameObject step in directionSteps) {
                if(step != null) {
                    Transform transform = step.transform;

                    float x = transform.position.x;
                    float z = transform.position.z;

                    float numérateurX = x - corner.x;
                    float dénominateurX = nextCorner.x - corner.x;

                    if(numérateurX / dénominateurX >= 0 && numérateurX / dénominateurX <= 1) {
                        
                        float numérateurZ = z - corner.z;
                        float dénominateurZ = nextCorner.z - corner.z;

                        if(numérateurZ / dénominateurZ >= 0 && numérateurZ / dénominateurZ <= 1) 
                            return step;
                        
                    }
                }
            }         
        }
        return null;
    }

    private void StepBackMovement(GameObject[] steps) { // Cette fonction sera a faire en sorte que le joueur recule si un autre joueur passe devant lui 
        if(stepPaths != null) {
            foreach(GameObject step in stepPaths) {
                foreach(GameObject user in gameController.players) {
                    UserMovement userMovement = user.GetComponent<UserMovement>();
                    if(step != null && userMovement.actualStep == step) {
                        if(nextStep.gameObject == step) {
                            GameObject lastStep = steps[beginResult - 1];
                            Step targetStep = step.GetComponent<Step>();

                            if(lastStep != nextStep.gameObject) { // Les joueurs vont que se croiser
                                userMovement.stepBack = true;
                                if(userMovement.point == Vector3.zero) {
                                    userMovement.point = user.transform.position;
                                    
                                    if(step.GetComponent<Step>().xAxis) 
                                        userMovement.point.x += targetStep.positive ? 10 : -10;
                                    if(step.GetComponent<Step>().zAxis) 
                                        userMovement.point.z += targetStep.positive ? 10 : -10;
                                    
                                    gameController.playerConflict.Add(user,step);
                                }
                            }
                            else { // Il va y avoir plusieurs joueurs sur la même case
                                userMovement.stack = true;
                                stack = true;
                            }
                        }
                    }
                }
            }
        }
    }

    private void Jump() {
        rb.AddForce(jumpSpeed * Vector3.up,ForceMode.Impulse);    
    }

    private GameObject GetNearStep() {
        List<GameObject> steps = gameController.allSteps;
       
        Transform nearestStep = null;
        float miniumDistance = Mathf.Infinity;
        Vector3 currentPos = transform.position;

        foreach (GameObject stepTrans in steps) {
            float distance = Vector3.Distance(stepTrans.transform.position, currentPos);
            if (distance < miniumDistance) {
                nearestStep = stepTrans.transform;
                miniumDistance = distance;
            }
        }

        return nearestStep.gameObject;

    }

    private void UseItemBot() {

        int random = Random.Range(0,100);

        int percentage = -1;

        if(GameController.difficulty == 0)
            percentage = 30;
        if(GameController.difficulty == 1)
            percentage = 60;
        if(GameController.difficulty == 2)
            percentage = 85;

        if(random <= percentage) { // Utilise un item
            List<int> hasItems = new List<int>();
            bool[] items = {inventory.hasDoubleDice,inventory.hasReverseDice,inventory.hasBomb,inventory.hasHourglass,inventory.hasLightning,inventory.hasStar,inventory.hasParachute};

            for(int i = 0;i<items.Length;i++) {
                if(items[i]) hasItems.Add(i);
            }

            if(hasItems.Count > 0) { // Le bot a au moins 1 objet
                int itemToUse = Random.Range(0,hasItems.Count - 1);

                switch(itemToUse) {
                    case 0: // Dé double
                        doubleDice = true;
                        inventory.hasDoubleDice = false;
                        ui.UseObject(transform.gameObject,"Dé double");
                        break;

                    case 1: // Dé inverse
                        reverseDice = true;
                        inventory.hasReverseDice = false;
                        ui.UseObject(transform.gameObject,"Dé inverse");
                        break;    

                    case 2: // Bomb
                        break;

                    case 3: // Hourglass
                        if(gameController.dayController.dayPeriod < 2) 
                            gameController.dayController.dayPeriod++;
                        else 
                            gameController.dayController.dayPeriod = 0;

                        inventory.hasHourglass = false;
                        ui.UseObject(transform.gameObject,"Sablier");
                        break;    

                    case 4: // Lightning
                        break;

                    case 5: // Star
                        //GetComponent<MeshRenderer>().material.shader = gameController.GetInvicibilityShader();
                        //transform.GetChild(0).gameObject.GetComponent<MeshRenderer>().material.shader = gameController.GetInvicibilityShader();  
                        //audio.Invicibility();
                        //GetComponent<UserUI>().UseObject(transform.gameObject,"Invincibilité");
                        break;

                    case 6: // Parachute
                        break;         
                }


            }
        }    
        
    }

    private bool ShopBot() {
        int item = Random.Range(0,gameController.shopItems.Count - 1);

        if(inventory.coins >= gameController.shopItems[item].price) {
            switch(item) {
                case 0: // Dé double
                    if(!inventory.hasDoubleDice) {
                        inventory.hasDoubleDice = true;
                        StartCoroutine(WaitMalus(gameController.shopItems[item].price,false));
                        return true;
                    }
                    break;

                case 1: // Dé inverse
                    if(!inventory.hasReverseDice) {
                        inventory.hasReverseDice = true;
                        StartCoroutine(WaitMalus(gameController.shopItems[item].price,false));
                        return true;
                    }
                    break;

                case 2:
                    if(!inventory.hasBomb) {
                        inventory.hasBomb = true;
                        StartCoroutine(WaitMalus(gameController.shopItems[item].price,false));
                        return true;
                    }
                break;

                case 3:
                    if(!inventory.hasHourglass) {
                        inventory.hasHourglass = true;
                        StartCoroutine(WaitMalus(gameController.shopItems[item].price,false));
                        return true;
                    }
                break;

                case 4:
                    if(!inventory.hasLightning) {
                        inventory.hasLightning = true;
                        StartCoroutine(WaitMalus(gameController.shopItems[item].price,false));
                        return true;
                    }
                    break;

                case 5:
                    if(!inventory.hasStar) {
                        inventory.hasStar = true;
                        StartCoroutine(WaitMalus(gameController.shopItems[item].price,false));
                        return true;
                    }
                    break;

                case 6:
                    if(!inventory.hasParachute) {
                        inventory.hasParachute = true;
                        StartCoroutine(WaitMalus(gameController.shopItems[item].price,false));
                        return true;
                    }
                    break;
            }
        }
        
        return false;
    } 

    #endregion

    #region Timers

    private void ShowPath(Color color,NavMeshPath path) {
        for(int i = 0;i<path.corners.Length - 1;i++) 
            Debug.DrawLine(path.corners[i], path.corners[i + 1], color);
    }

    private IEnumerator WaitBonus(bool stepReward) {
        yield return new WaitForSeconds(0.5f);

        inventory.CoinGain(3);
        audio.CoinsGain();
        ui.DisplayReward(true,3,stepReward);
        ui.ClearDiceResult();
        gameController.ActualizePlayerClassement();

        random = -1;
        timer = 0f;
    }

    public IEnumerator WaitBonus(bool stepReward,int amount) {
        yield return new WaitForSeconds(0.5f);

        inventory.CoinGain(amount);
        audio.CoinsGain();
        ui.DisplayReward(true,3,stepReward);
        ui.ClearDiceResult();
        gameController.ActualizePlayerClassement();

        random = -1;
        timer = 0f;
    }

    private IEnumerator WaitMalus(bool stepReward) {
        yield return new WaitForSeconds(0.5f);
        
        if(inventory.coins > 0) {
            audio.CoinsLoose();
            inventory.CoinLoose(3);
            ui.DisplayReward(false,3,stepReward);
            gameController.ActualizePlayerClassement();
        } 
        else finishTurn = true;  

        ui.ClearDiceResult();   

        random = -1;
        timer = 0f;
    }

    public IEnumerator WaitMalus(int malusCoins,bool stepReward) {
        yield return new WaitForSeconds(0.2f);
        audio.CoinsLoose();
        inventory.CoinLoose(malusCoins);
        ui.DisplayReward(false,malusCoins,stepReward);
        gameController.ActualizePlayerClassement();
         
    }
    
    private IEnumerator WaitToReturnStep() {
        yield return new WaitForSeconds(0.05f);
        if(diceResult <= 0 && returnToStep) gameController.EndUserTurn();
        returnToStep = false;
    }

    private IEnumerator WaitToShop() {
        yield return new WaitForSeconds(0.05f);
        
        stop = true;
        ui.showShopHUD = true;                    
        hasShowShopHUD = true;

    }

    private IEnumerator WaitToShopBot(){
        yield return new WaitForSeconds(0.05f);

        stop = true;
        int random = /*Random.Range(0,100) */ 28;
        int percentage = -1;

        if(GameController.difficulty == 0)
            percentage = 30;
        else if(GameController.difficulty == 1) // Medium
            percentage = 60;
        else if(GameController.difficulty == 2) // Hard
            percentage = 85;


       /* if(random >= 0 && random <= percentage && inventory.coins >= 10)  // Lance le shop
            goToShop = true;
        else { */
            stop = false;
            if(diceResult <= 0) gameController.EndUserTurn();
        //}
        hasShowChestHUD = true;  

    }

    public IEnumerator WaitChest() {
        agent.enabled = false;
        
        if(inventory.cards + 1 == 6) 
            audio.FindSecretCode();
        else 
            audio.CardGain();

        if(!isPlayer) 
            StartCoroutine(WaitMalus(30,false));

        yield return new WaitForSeconds(1f);      

        int[] secretCode = gameController.secretCode;
        int random = Random.Range(0,secretCode.Length - 1);

        int targetCode = secretCode[random];
        bool finishCode = true;

        List<int> indexes = new List<int>();

        for(int i = 0;i<secretCode.Length;i++) {
            if(secretCode[i] == targetCode) 
                indexes.Add(i); 
            if(inventory.secretCode[i] == -1) 
                finishCode = false;
            
        }

        foreach(int index in indexes) {
            if(inventory.secretCode[index] != -1) {
                inventory.secretCode[index] = targetCode;
                break;
            }
        }
        
        inventory.AddCards(1);

        Dialog currentDialog = gameController.dialog.GetDialogByName("FindNewCode");


        if(!finishCode) {
            currentDialog.Content[0] = currentDialog.Content[0].Replace("%n","" + targetCode);
            currentDialog.Content[0] = currentDialog.Content[0].Replace("%b","" + (6 - inventory.cards));
        }

        else 
            currentDialog = gameController.dialog.GetDialogByName("FindAllSecretCode");   

        gameController.dialog.currentDialog = currentDialog;
        gameController.dialog.isInDialog = true;
        gameController.dialog.finish = false;
        StartCoroutine(gameController.dialog.ShowText(currentDialog.Content[0],currentDialog.Content.Length));

    }

    private IEnumerator WaitTimeToReturn() {
        yield return new WaitForSeconds(0.25f);

        stepBack = false;
        returnStepBack = false;
    }

    #endregion
    
   
    
    
}
