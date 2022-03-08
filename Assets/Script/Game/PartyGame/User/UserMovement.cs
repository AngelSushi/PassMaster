using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.AI;
using UnityEngine.UI;
using System.Linq;

public class UserMovement : User {

    public NavMeshAgent agent;
    public bool waitDiceResult;
    public bool finishMovement;
    public bool finishTurn;
    public bool left;
    public bool front;
    public bool right;
    public bool stop;
    public bool lastStepIsArrow;
    public bool waitChest;
    public bool returnStepBack;
    public bool reverseCount;
    public bool doubleDice;
    public bool tripleDice;
    public bool useHourglass;
    public bool useLightning;
    public GameObject targetLightningStep;
    public bool reverseDice;    
    public GameObject lastStep;
    public bool isMooving;
    public GameObject giveUI;
    public GameObject changeUI;
    public int diceResult;
    public GameObject actualStep;
    public GameObject beginStep;
    public bool canJump;
    public bool isJumping;
    public Transform nextStep;
    public float jumpSpeed;
    public bool stack;
    public int beginResult;
    public GameObject[] stepPaths;
    public bool stepBack;
    private bool jump;
    private int random = -1;
    private float timer;
    private Vector3 point;
    private bool hasCheckPath;
    private bool hasShowChestHUD;
    private bool canMooveToChest = true;
    private bool hasCollideDice;
    private GameObject dice;
    private bool hasJump;
    public Rigidbody rb;
    private Color actualColor;
    public bool isParachuting;
    private Vector3 parachuteMovement;
    private bool bypassDirection;
    public bool canMoove;
    private bool hasFindChest;
    private List<GameObject> iaDirectionPath;
    [HideInInspector]
    public int currentTabIndex;
    private NavMeshPath path;
    public GameObject userCam;
    public bool constantJump;
    public UserType userType;
    public bool checkObjectToUse;
    public Animator animatorController;

    public bool isElectrocuted;
    public bool useShell;
    void Start() {
        path = new NavMeshPath();
    }

    public override void OnBeginTurn() {
        stepBack = false;
        point = Vector3.zero;

        if(waitDiceResult) {
            if(!isPlayer) 
                InitDice();      
        }
    }

    public override void OnDiceAction() { // Call when a physical user (not an IA) choose the dice action on hud
        if(waitDiceResult) {
            if(isPlayer) 
                InitDice();      
        }
    }

    public override void OnFinishTurn() {
        agent.enabled = false;
        left = false;
        front = false;
        isMooving = false;
        right = false;
        transform.GetChild(1).gameObject.SetActive(false);
        hasShowChestHUD = false;
        canMooveToChest = true;
        waitChest = false;
        hasCollideDice = false;
        waitDiceResult = true;
        hasJump = false;

        if(!isPlayer)
            checkObjectToUse = false;
    }

    public override void Update() {
        base.Update();

        if(!gameController.freeze) {
            
            animatorController.SetBool("IsMooving",isMooving);
            animatorController.SetBool("IsElectrocuted",isElectrocuted);

            if(isTurn) {
                canMoove = !stop;

                if(stop)
                    isMooving = false;  

                if(nextStep != null && !jump && !stop) {
                    if(!transform.GetChild(1).gameObject.activeSelf) {
                        RunDelayed(0.2f,() => {
                            GameObject camera = transform.GetChild(1).gameObject;
                            camera.transform.localPosition = new Vector3(-0.07f,2.41f,-3.83f);
                            camera.transform.localRotation = Quaternion.Euler(17.839f,0f,0f);
                            camera.gameObject.SetActive(true);
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
                    
                    if(lastStep == nextStep.gameObject) 
                        ChooseNextStep(StepType.NONE);

                    if(canMoove) {
                        agent.CalculatePath(nextStep.position,path);
                        ShowPath(Color.magenta,path);
                        CheckPath();
                        agent.SetPath(path);   
                    }

                }

                if(finishMovement) {
                    StepType type = actualStep.GetComponent<Step>().type;
                    isMooving = false;
                    if(type == StepType.BONUS)  
                        StartCoroutine(WaitBonus(true));
                    if(type == StepType.MALUS) 
                        StartCoroutine(WaitMalus(true));

                    ui.ClearDiceResult();
                    hasCheckPath = false;
                    finishMovement = false;
                }            

                if(!isPlayer && waitDiceResult) {
                    if(inventory.HasObjects()) {
                        checkObjectToUse = true;
                        inventory.UseItemBot();
                    }

                    if(!checkObjectToUse) {
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
            }

            else { // is not his turn

                if(returnStepBack) 
                    StartCoroutine(WaitTimeToReturn());
            
                if(stepBack) {
                    agent.enabled = false;
                    transform.position = Vector3.MoveTowards(transform.position,point,agent.speed * Time.deltaTime);
                }
                else 
                    point = Vector3.zero;
                
            }
        }
    }


    private void OnCollisionEnter(Collision hit) {
       if(isTurn) {
           if(hit.gameObject.tag == "Chest") {
               gameController.endAnimationController.checkCode = true;
           }

            if(hit.gameObject.tag == "Dice") {
                if(diceResult == 0 || diceResult == -1) {
                    diceResult = hit.gameObject.GetComponent<DiceController>().index;

                    if(diceResult == 0) 
                        diceResult = 6;
                    if(doubleDice) 
                        diceResult *= 2;
                    if(tripleDice)
                        diceResult *= 3;           
                }
                
                agent.enabled = true;
                if(isPlayer) diceResult = 63; 
                beginResult = diceResult; 
                stepPaths = new GameObject[beginResult]; 
                hasCollideDice = true;  
                
                actualColor = tripleDice ? new Color(1f,0.74f,0f) : doubleDice ? new Color(0.32f,0.74f,0.08f,1.0f) : reverseDice ? new Color(0.41f,0.13f,0.78f,1.0f) : new Color(0f,0.35f,1f,1.0f);
                ui.RefreshDiceResult(diceResult, actualColor,true);

                GameObject hitObj = hit.gameObject;
                hitObj.GetComponent<MeshRenderer>().enabled = false;

                ChooseNextStep(gameController.firstStep.GetComponent<Step>().type);

                RunDelayed(0.1f,() => {  hitObj.SetActive(false); });
            }

            if(hit.gameObject.tag == "Sol") {
                if(isJumping) 
                    isJumping = false;

                if(constantJump) {
                    Jump();
                }

            }
       } 
    }

    private void OnTriggerEnter(Collider hit) {
        if(isTurn) {
            StepType type = hit.gameObject.GetComponent<Step>() != null ? hit.gameObject.GetComponent<Step>().type : hit.gameObject.GetComponent<Direction>().type;

            if(type == StepType.NONE) 
                return;

            if(type == StepType.BONUS || type == StepType.MALUS || type == StepType.SHOP || type == StepType.BONUS_END || type == StepType.MALUS_END || type == StepType.STEP_END) {
                if(gameController.shopController.returnToStep || gameController.chestController.returnToStep) {
                    RunDelayed(0.35f,() => {

                        if(gameController.shopController.returnToStep) {
                            gameController.shopController.returnToStep = false;
                            gameController.EndUserTurn();
                            Debug.Log("return to step end turn");
                        }
                        else {
                            gameController.chestController.returnToStep = false;
                            gameController.hasGenChest = false;
                        }
        
                        return;
                    });
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

                if(type == StepType.STEP_END && gameController.endAnimationController.isInEndAnimation) {
                     RunDelayed(1.5f,() => {
                        gameController.endAnimationController.isInEndAnimation = false;
                        gameController.endAnimationController.checkCode = false;
                        gameController.EndUserTurn(); // Ne pas end tout de suite faire blackscreen + récompense de la step
                     });
                }

                if(diceResult < 0 && nextStep != null) 
                    finishMovement = true;

                if(stop) 
                    stop = false;

                if(type == StepType.BONUS_END || type == StepType.MALUS_END) {
                    if(lastStep.GetComponent<Direction>() == null) {
                        bypassDirection = true;
                        GetNextStep();
                        nextStep = nextStep.GetComponent<Direction>().directionsStep[1].transform;
                    }
                }

                if(type == StepType.SHOP && diceResult == 0) { // Faire en sorte qu'on puisse y aller sans que ca soit la dernière step 
                    if(isPlayer) {
                        RunDelayed(0.5f,() => {
                            Dialog shopDialog = gameController.dialog.GetDialogByName("AskShop");
                            if(gameController.dialog.currentDialog != shopDialog) {
                                gameController.dialog.isInDialog = true;
                                gameController.dialog.currentDialog = shopDialog;
                                StartCoroutine(gameController.dialog.ShowText(shopDialog.Content[0],shopDialog.Content.Length));
                            }
                        });
                    }
                    else {
                        gameController.shopController.AskShopBot(inventory,actualStep.GetComponent<Step>().shop);
                    }
                }
            }

            if(type == StepType.FIX_DIRECTION || type == StepType.FLEX_DIRECTION) {
                actualStep = hit.gameObject;
                if(ui.direction == null) 
                    ui.direction = hit.gameObject.GetComponent<Direction>();

                if(bypassDirection && reverseCount) {
                    reverseCount = false;
                }
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

               if(actualStep != null && nextStep != null && actualStep == nextStep.gameObject) {
                  ChooseNextStep(StepType.NONE);
               }

                if(type == StepType.STEP_END) {
                        if(diceResult == 0) {
                            if(!gameController.endAnimationController.isInEndAnimation) {
                                Debug.Log("play anim");
                                stop = true;
                                gameController.blackScreenAnim.Play();
                                gameController.endAnimationController.isInEndAnimation = true;
                            }
                        } 
                        else {
                            reverseCount = true;
                            Debug.Log("my count");
                        }          
                }

                if(nextStep == null && diceResult > 0) { // Le joueur/bot n'a pas encore commencé a bougé   
                    ChooseNextStep(type);

                    if(ui != null) 
                        ui.RefreshDiceResult(diceResult,actualColor,false);

                }

                else if(nextStep != null && diceResult == 0) { // Le joueur a fini son chemin
                    nextStep = null;
                    diceResult = -1;
                }
            }

            if(type == StepType.FIX_DIRECTION && lastStepIsArrow && isPlayer) {
                Debug.Log("actualStep: " + actualStep);
                nextStep = actualStep.GetComponent<Direction>().directionsStep[1].transform;
                lastStepIsArrow = false;
                return;
            }

            if(type == StepType.FIX_DIRECTION && (lastStep != null && lastStep.GetComponent<Step>() != null && (lastStep.GetComponent<Step>().type != StepType.BONUS_END || lastStep.GetComponent<Step>().type != StepType.MALUS_END)) && !bypassDirection && isTurn) {
                if(isPlayer) { // Joueur
                    if(!left && !right && !front) {
                        if(!ui.showDirection) 
                            ui.showDirection = true;
                        stop = true; 
                        return;
                    }
                    else {
                        agent.enabled = true;
                        if(left) 
                            nextStep = ui.direction.directionsStep[0].transform;
                        if(front) {
                            nextStep = ui.direction.directionsStep[1].transform;
                        // front = false;
                        }
                        if(right) 
                            nextStep = ui.direction.directionsStep[2].transform;

                        stop = !(left || front || right);
                    }
                }
                
                else { // Bot Facile = 50 ; Moyen = 70 ; Difficile  = 90 de chance d'aller vers le coffre
                    if(!hasFindChest) {
                        stop = true;
                        iaDirectionPath = new List<GameObject>();
                        Dictionary<GameObject,int> iaPathDirections = new Dictionary<GameObject, int>();
                        int percentageGoToChest = 0;

                        switch(GameController.difficulty) {
                            case 0: // Facile
                                percentageGoToChest = 50;
                                break;

                            case 1: // Moyen
                                percentageGoToChest = 70;
                                break;

                            case 2: // Difficile
                                percentageGoToChest = 90;
                                break;
                        }

                        int randomGoToChest = Random.Range(0,100);

                        GenerateIAPaths(iaPathDirections);

                        bool goToSmallest = randomGoToChest <= percentageGoToChest;
                        int lastSize = 0;

                        foreach(GameObject direction in iaPathDirections.Keys) {
                            if(goToSmallest) {
                                if(lastSize == 0 || lastSize >= iaPathDirections[direction]) 
                                    lastSize = iaPathDirections[direction];
                            }
                            else { 
                                if(lastSize == 0 || lastSize <= iaPathDirections[direction])
                                    lastSize = iaPathDirections[direction];
                            }
                        }

                        Debug.Log("value: " + goToSmallest);
                        Debug.Log("go to " + (goToSmallest ? " smallest " : " far"));
                        

                        Debug.Log("current direction: " + gameController.GetKeyByValue<GameObject,int>(lastSize,iaPathDirections));
                        RunDelayed(0.1f,() => {
                         //   nextStep = gameController.GetKeyByValue<GameObject,int>(lastSize,iaPathDirections).transform;

                            nextStep = actualStep.GetComponent<Direction>().directionsStep[1].transform;
                            stop = false;
                        });
                    }
                }
            }
        }
    }

    private void OnTriggerExit(Collider hit) {
        if(isTurn) {
            StepType type = hit.gameObject.GetComponent<Step>() != null ? hit.gameObject.GetComponent<Step>().type : hit.gameObject.GetComponent<Direction>().type;

            foreach(GameObject step in gameController.playerConflict.Values.ToList()) {
                if(hit.gameObject == step) { // On fait revenir le bot sur sa case
                    GameObject user = gameController.GetKeyByValue(step,gameController.playerConflict);
                    UserMovement userMovement = user.GetComponent<UserMovement>();
                    Step targetStep = step.GetComponent<Step>();

                    userMovement.point = hit.gameObject.transform.position;
                    userMovement.point.y = user.transform.position.y;
                    userMovement.returnStepBack = true;
                    gameController.playerConflict.Remove(user);
                }
            }

            if(type == StepType.FIX_DIRECTION || type == StepType.FLEX_DIRECTION) {
                if(!isJumping) 
                    lastStepIsArrow = true;
                if(bypassDirection) 
                    bypassDirection = false;

                hasCheckPath = false;
                ui.direction = null;

                if(!isPlayer && hasFindChest)
                    hasFindChest = false;
            }
            else 
                lastStepIsArrow = false;   

        /*    if(type == StepType.STEP_END) 
                ChooseNextStep(type); 
*/
            left = false;
            front = false;
            right = false;

            if(type == StepType.BONUS || type == StepType.MALUS || type == StepType.SHOP || type == StepType.FIX_DIRECTION || type == StepType.FLEX_DIRECTION || type == StepType.BONUS_END  || type == StepType.MALUS_END || type == StepType.STEP_END) 
                lastStep = hit.gameObject;
        }
    }

    #region Inputs Functions

    public void OnJump(InputAction.CallbackContext e) {
        if(e.started && canJump) 
            Jump();       
    } 

    public void OnMove(InputAction.CallbackContext e) {
        if(isParachuting && !gameController.freeze) 
            parachuteMovement = e.ReadValue<Vector2>();   
    } 

    public void OnGive(InputAction.CallbackContext e) {
        if(e.started && isTurn) {
            if(transform.gameObject.name == "User") 
                giveUI.SetActive(true);
        }
    }

    public void OnChange(InputAction.CallbackContext e) {
        if(e.started && isTurn) {
            if(transform.gameObject.name == "User") 
                changeUI.SetActive(true);      
        }
    }

    #endregion

    #region Customs Functions

    private void InitDice() {
        canJump = true;
        dice = gameController.dice;
        dice.SetActive(true);
        agent.enabled = false;
        dice.transform.position = new Vector3(transform.position.x,transform.position.y + 17,transform.position.z);
        dice.GetComponent<DiceController>().lockDice = false;
        dice.GetComponent<DiceController>().lastLockDice = true;
        dice.GetComponent<MeshRenderer>().enabled = true;

        int matIndex = tripleDice ? 3 : doubleDice ? 1 : reverseDice ? 2 : 0;
        dice.GetComponent<MeshRenderer>().material = gameController.diceMaterials[matIndex];

        stack = false;
    }

    private void ChooseNextStep(StepType type) {
        isMooving = true;

        if(type != StepType.FIX_DIRECTION && type != StepType.FLEX_DIRECTION && type != StepType.NONE && nextStep != null) {
            diceResult--;
        }

        GetNextStep();

        if(ui != null) {
            if(beginResult == diceResult) 
                ui.RefreshDiceResult(doubleDice ? diceResult / 2 : tripleDice ? diceResult / 3 : diceResult,actualColor,true);
            else
                ui.RefreshDiceResult(diceResult,actualColor,false);
        }

        finishMovement = (diceResult == 0);
    }

    private void CheckPath() {
        if(!hasCheckPath && stepPaths != null) { 

            if(beginStep == null)
                beginStep = nextStep.gameObject;

            Transform stepParent = actualStep != null ? actualStep.transform.parent : beginStep.transform.parent;
            int stepIndex = actualStep != null ? gameController.FindIndexInParent(stepParent.gameObject,actualStep) : gameController.FindIndexInParent(stepParent.gameObject,beginStep);
            int result = actualStep != null ? diceResult : beginResult;

            if(stepIndex == -1) 
                stepIndex = 0;

            int index = 0;
            for(int i = 0;i<result;i++) {
                index = i;


                if(stepIndex + i + 1 < stepParent.childCount) // On vérifie que le calcul du prochain index de la prochaine step est bien inférieur au nombre de step max
                    stepPaths[i] = stepParent.GetChild(stepIndex + i + 1).gameObject;
                else  // Le calcul du prochain index de la prochaine step est supérieur au nombre de step max, on retire donc le nombre de step max au calcul
                    stepPaths[i] = stepParent.GetChild(stepIndex + i + 1 - stepParent.childCount).gameObject;    
            }
            hasCheckPath = true;
        }

        StepBackMovement(stepPaths);
    }

    private void GetNextStep() {
        GameObject actualParent = actualStep != null ? actualStep.transform.parent.gameObject : gameController.firstStep.transform.parent.gameObject;
        int stepIndex = gameController.FindIndexInParent(actualParent,actualStep);

        if(!reverseDice && !reverseCount) { // Si le joueur n'utilise pas le dé inverse ou qu'il n'est pas en reverseCount
            if(stepIndex + 1 < actualParent.transform.childCount) 
                nextStep = actualParent.transform.GetChild(stepIndex + 1);
            else 
                nextStep = actualParent.transform.GetChild(0);
        } 
        else { // Le joueur utilise le dé inverse ou est en reverseCount
            if(stepIndex - 1 >= 0) 
                nextStep = actualParent.transform.GetChild(stepIndex - 1);          
            else 
                nextStep = actualParent.transform.GetChild(actualParent.transform.childCount - 1);    
        }                 
    }
     
    private void GenerateIAPaths(Dictionary<GameObject,int> iaPathDirections) {
        for(int i = 0;i < ui.direction.directionsStep.Length;i++) {
            GameObject end = ui.direction.gameObject;
            
            if(ui.direction.directionsStep[i] == null)
                continue;

            iaDirectionPath.Clear();

            bool sameParent = true;
            if(ui.direction.directionsStep[i].transform.parent != end.transform.parent) { // Le parent de la step de début est différent du parent de la step de fin
                end = ui.direction.directionsStep[i].transform.parent.GetChild(ui.direction.directionsStep[i].transform.parent.childCount - 1).gameObject;
                sameParent = false;
            }

            FindSmallestChestPath(ui.direction.directionsStep[i],end,iaDirectionPath,false,sameParent);

            iaPathDirections.Add(ui.direction.directionsStep[i],iaDirectionPath.Count);
        }
    }

    private bool FindSmallestChestPath(GameObject begin,GameObject end,List<GameObject> iaDirectionSteps,bool decrement,bool sameParent) {
        int indexEnd = gameController.FindIndexInParent(end.transform.parent.gameObject,end);

        if(end.GetComponent<Direction>() != null && !sameParent)
            indexEnd++;

        for(int i = gameController.FindIndexInParent(begin.transform.parent.gameObject,begin); i != indexEnd;) {
            if(i >= begin.transform.parent.childCount) 
                i -= begin.transform.parent.childCount;
            
            GameObject actualObj = begin.transform.parent.GetChild(i).gameObject;

            if(!iaDirectionPath.Contains(actualObj))
                iaDirectionPath.Add(actualObj);

            if(actualObj == gameController.stepChest) {
                hasFindChest = true;
                return true;
            }

            if(actualObj.GetComponent<Direction>() != null) {
                for(int j = 0;j<actualObj.GetComponent<Direction>().directionsStep.Length;j++) {
                    GameObject beginDirection = actualObj.GetComponent<Direction>().directionsStep[j];
                    if(beginDirection != null) { 
                        Direction nextDir = actualObj.GetComponent<Direction>().directionsStep[j].GetComponent<Direction>();

                        GameObject beginObj = beginDirection;
                        GameObject endObj = null;

                        if(nextDir != null) {
                            beginObj = nextDir.directionsStep[1].gameObject;
                            endObj = nextDir.directionsStep[1].gameObject.transform.parent.GetChild(nextDir.directionsStep[1].gameObject.transform.parent.childCount- 1).gameObject;
                        }
                        else {
                            if(beginObj == beginObj.transform.parent.GetChild(beginDirection.transform.parent.childCount - 2).gameObject) // -2 ici car on ne veut pas prendre en compte la direction
                                endObj = beginObj.transform.parent.GetChild(0).gameObject;              
                            else 
                                endObj = beginDirection.transform.parent.GetChild(beginDirection.transform.parent.childCount - 1).gameObject;
                        }

                        decrement = beginObj == beginObj.transform.parent.GetChild(beginDirection.transform.parent.childCount - 2).gameObject;

                        int beginSize = iaDirectionPath.Count;
                        bool result = FindSmallestChestPath(beginObj,endObj,iaDirectionSteps,decrement,beginObj.transform.parent == endObj.transform.parent);
                        int size = iaDirectionPath.Count - beginSize;

                        if(!result)  
                            EraseSteps(beginSize,size,iaDirectionSteps);   
                    }
                }
            }

            if(decrement)
                i--;
            else
                i++;

        }

        return false;
    }

    private void EraseSteps(int beginIndex,int size,List<GameObject> iaDirectionSteps) {
        List<GameObject> erase = new List<GameObject>();

        for(int i = 0;i<iaDirectionSteps.Count;i++) {
            if(i >= beginIndex && i <= beginIndex + size) 
                erase.Add(iaDirectionSteps[i]);
        }

        foreach(GameObject eraseObj in erase) 
            iaDirectionPath.Remove(eraseObj);
    }

    private void StepBackMovement(GameObject[] steps) { // Cette fonction sera a faire en sorte que le joueur recule si un autre joueur passe devant lui 
        if(stepPaths != null) {
            foreach(GameObject step in stepPaths) {
                foreach(GameObject user in gameController.players) {
                    UserMovement userMovement = user.GetComponent<UserMovement>();
                    if(step != null && !userMovement.isTurn && userMovement.actualStep == step) {
                        if(nextStep.gameObject == step) {
                            GameObject lastStep = steps[beginResult - 1];
                            Step targetStep = step.GetComponent<Step>();

                            if(lastStep != nextStep.gameObject) { // Les joueurs vont que se croiser
                                userMovement.stepBack = true;
                                if(userMovement.point == Vector3.zero) {
                                    userMovement.point = user.transform.position;
                                    
                                   /* if(step.GetComponent<Step>().xAxis) 
                                        userMovement.point.x += targetStep.positive ? 10 : -10;
                                    if(step.GetComponent<Step>().zAxis) 
                                        userMovement.point.z += targetStep.positive ? 10 : -10;
                                    */

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

    public void Jump() {
        rb.AddForce(jumpSpeed * Vector3.up,ForceMode.Impulse);    
    }

    #endregion

    #region Timers

    public static void ShowPath(Color color,NavMeshPath path) {
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

        if(actualStep != null && actualStep.GetComponent<Step>() != null && actualStep.GetComponent<Step>().chest != null && actualStep.GetComponent<Step>().chest.activeSelf) { 
            if(!gameController.dialog.isInDialog) {
                if(isPlayer) 
                    DisplayChestDialog();
                else
                    gameController.chestController.CheckChestBot(inventory);          
            }
        }
    }

    private IEnumerator WaitMalus(bool stepReward) {
        yield return new WaitForSeconds(0.5f);
        
        if(inventory.coins > 0) {
            audio.CoinsLoose();
            inventory.CoinLoose(3);
            ui.DisplayReward(false,3,stepReward);
            gameController.ActualizePlayerClassement();
        }   

        ui.ClearDiceResult();   

        random = -1;
        timer = 0f;

        if(actualStep != null && actualStep.GetComponent<Step>() != null && actualStep.GetComponent<Step>().chest != null && actualStep.GetComponent<Step>().chest.activeSelf) { 
            if(!gameController.dialog.isInDialog) {
                if(isPlayer) 
                    DisplayChestDialog();
                else
                    gameController.chestController.CheckChestBot(inventory);
            }
        }
    }

    public IEnumerator WaitMalus(bool stepReward,int amount) {
        yield return new WaitForSeconds(0.5f);
        
        if(inventory.coins > 0) {
            audio.CoinsLoose();
            inventory.CoinLoose(amount);
            ui.DisplayReward(false,amount,stepReward);
            gameController.ActualizePlayerClassement();
        }   

        ui.ClearDiceResult();   

        random = -1;
        timer = 0f;

        if(actualStep != null && actualStep.GetComponent<Step>() != null && actualStep.GetComponent<Step>().chest != null && actualStep.GetComponent<Step>().chest.activeSelf) { 
            if(!gameController.dialog.isInDialog) {
                if(isPlayer) 
                    DisplayChestDialog();
                else
                    gameController.chestController.CheckChestBot(inventory);          
            }
        }
    }

    private void DisplayChestDialog() {
        Dialog askChest = gameController.dialog.GetDialogByName("AskChestBuy");
        gameController.dialog.isInDialog = true;
        gameController.dialog.currentDialog = askChest;
        StartCoroutine(gameController.dialog.ShowText(askChest.Content[0],askChest.Content.Length));
    }

    private IEnumerator WaitTimeToReturn() {
        yield return new WaitForSeconds(0.25f);

        stepBack = false;
        returnStepBack = false;
    }

    #endregion
     
}
