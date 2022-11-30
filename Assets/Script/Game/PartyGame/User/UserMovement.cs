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
    public bool waitChest;
    public bool reverseCount;
    public bool doubleDice;
    public bool tripleDice;
    public bool useHourglass;
    public bool useLightning;
    public bool useShell;
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
    public bool bypassDirection;
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
    private Vector3 lastPosition;
    void Start() {
        path = new NavMeshPath();
        lastPosition = transform.position;
    }

    public override void OnBeginTurn() {
        stepBack = false;
        point = Vector3.zero;
        ui.enabled = true;
        
        transform.LookAt(gameController.mainCamera.transform);

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
    //    agent.enabled = false;
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

        reverseDice = false;
        tripleDice = false;
        doubleDice = false;

        ui.enabled = false;

        if(!isPlayer)
            checkObjectToUse = false;
        
        
    }

    public override void Update() {
        base.Update();

        if(!gameController.freeze) {

           if(animatorController != null) 
               animatorController.SetBool("IsMooving",isMooving);
           
           if(isTurn) {
                canMoove = !stop;

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
                        CheckPath(false);
                        agent.SetPath(path);
                        
                        Quaternion targetRotation = Quaternion.LookRotation(nextStep.position - transform.position);
                        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, 1.5f * Time.deltaTime);
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
               if(stepBack) {
                   if (actualStep.TryGetComponent<Step>(out Step step)) { // Only stepback on player who is not in step stack 
                       if (step.playerInStep.Contains(transform.gameObject)) {
                           stepBack = false;
                           return;
                       }
                   }

//                   Debug.Log("need to stepback");
                   agent.CalculatePath(point, path);
                   agent.SetPath(path); 
                   ShowPath(Color.yellow,path);
               }
               else 
                   point = Vector3.zero;
           }
        }

        isMooving = lastPosition.x != transform.position.x || lastPosition.z != transform.position.z;
        lastPosition = transform.position;
    }


    private void OnCollisionEnter(Collision hit) {
       if(isTurn) {
           if(hit.gameObject.CompareTag("Chest")) {
               gameController.endAnimationController.checkCode = true;
           }

           if(hit.gameObject.CompareTag("Dice")) { 
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

               diceResult = isPlayer ? 6 : 76;

                
               beginResult = diceResult; 
               stepPaths = new GameObject[beginResult]; 
               hasCollideDice = true;  
                
               actualColor = tripleDice ? new Color(1f,0.74f,0f) : doubleDice ? new Color(0.32f,0.74f,0.08f,1.0f) : reverseDice ? new Color(0.41f,0.13f,0.78f,1.0f) : new Color(0f,0.35f,1f,1.0f);
               ui.RefreshDiceResult(diceResult, actualColor);

               GameObject hitObj = hit.gameObject;
               hitObj.GetComponent<MeshRenderer>().enabled = false;

               CheckPath(true);
               ChooseNextStep(gameController.firstStep.GetComponent<Step>().type);

               RunDelayed(0.1f,() => {  hitObj.SetActive(false); });
           }

           if(hit.gameObject.CompareTag("Sol")) { 
               if(isJumping) 
                   isJumping = false;
               
               if(constantJump) 
                Jump();
                

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

                        gameController.UpdateSubPath(this,false);
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
                        if(nextStep.GetComponent<Direction>() != null)
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

                if(bypassDirection && reverseCount) 
                    reverseCount = false;

                if (lastStep.TryGetComponent<Direction>(out Direction direction)) { // IL Y A DEUX DIRECTIONS QUI SE SUCCEDE
                    Transform stepParent = actualStep != null ? actualStep.transform.parent : beginStep.transform.parent;
                    int directionStepIndex = actualStep != null ? gameController.FindIndexInParent(stepParent.gameObject,actualStep) : gameController.FindIndexInParent(stepParent.gameObject,beginStep);

                    nextStep = reverseDice ? stepParent.GetChild(Mathf.Abs(directionStepIndex - (stepParent.childCount - 1))) : actualStep.GetComponent<Direction>().directionsStep[1].transform;
                }
            }
        }
        else {
            RunDelayed(0.35f, () => {
                if (stepBack) {
                    stepBack = false;
                    gameController.UpdateSubPath(this,false);
                    agent.speed /= 2f;
                    agent.angularSpeed /= 2f;
                    agent.acceleration /= 2f;
                }

            });
        }
    }

    private void OnTriggerStay(Collider hit) {
        if(isTurn) {
            StepType type = hit.gameObject.GetComponent<Step>() != null ? hit.gameObject.GetComponent<Step>().type : hit.gameObject.GetComponent<Direction>().type;
            if(type == StepType.BONUS || type == StepType.MALUS || type == StepType.SHOP || type == StepType.BONUS_END || type == StepType.MALUS_END || type == StepType.STEP_END) {

                if(actualStep != null && nextStep != null && actualStep == nextStep.gameObject) 
                  ChooseNextStep(StepType.NONE);
               

                if(type == StepType.STEP_END) {
                        if(diceResult == 0) {
                            if(!gameController.endAnimationController.isInEndAnimation) {
                                Debug.Log("play anim");
                                stop = true;
                                gameController.blackScreenAnim.Play();
                                gameController.endAnimationController.isInEndAnimation = true;
                            }
                        } 
                        else 
                            reverseCount = true;
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

            if(type == StepType.FIX_DIRECTION && bypassDirection) {
                nextStep =  actualStep.GetComponent<Direction>().directionsStep[1].transform;
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
                            nextStep = reverseDice ?  ui.direction.directionsStep[2].transform : ui.direction.directionsStep[0].transform;
                        if(front) {
                            nextStep = ui.direction.directionsStep[1].transform;
                        // front = false;
                        }
                        if(right) 
                            nextStep = reverseDice ? ui.direction.directionsStep[0].transform : ui.direction.directionsStep[2].transform;

                        
                        stop = !(left || front || right);
                    }
                }
                
                else { // Bot Facile = 50 ; Moyen = 70 ; Difficile  = 90 de chance d'aller vers le coffre
                    if(!hasFindChest) {
                        stop = true;
                        iaDirectionPath = new List<GameObject>();
                        Dictionary<GameObject,int> iaPathDirections = new Dictionary<GameObject, int>();
                        int percentageGoToChest = 0;

                        switch(gameController.difficulty) {
                            case GameController.Difficulty.EASY: // Facile
                                percentageGoToChest = 50;
                                break;

                            case GameController.Difficulty.MEDIUM: // Moyen
                                percentageGoToChest = 70;
                                break;

                            case GameController.Difficulty.HARD: // Difficile
                                percentageGoToChest = 90;
                                break;
                        }

                        int randomGoToChest = Random.Range(0,100);

                        // a modifier
                        percentageGoToChest = 100;
                        
                        Debug.Log("direction " + ui.direction);
                        
                        GenerateIAPaths(ui.direction,gameController.stepChest,new List<GameObject>(),new List<Direction>());

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

                        Debug.Log("go to " + (goToSmallest ? " smallest " : " far"));

                        foreach (GameObject step in iaDirectionPath) {
                            if (step.transform.childCount >= 2) {
                                for (int i = 0; i < step.transform.childCount; i++) {
                                    if (step.transform.GetChild(i).gameObject.TryGetComponent<MeshRenderer>(out MeshRenderer meshRenderer)) {
                                        meshRenderer.material.color = Color.magenta;
                                    }
                                }
                            }
                        }
                        
                        RunDelayed(0.1f,() => {
                            nextStep = gameController.GetKeyByValue<GameObject,int>(lastSize,iaPathDirections).transform;

                         //   nextStep = actualStep.GetComponent<Direction>().directionsStep[1].transform;
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
                    
                    Debug.Log("hit obj " + hit.gameObject);
                    
                    userMovement.point = hit.gameObject.transform.position;
                   // userMovement.point.y = user.transform.position.y;
                    
                    Debug.Log("left conflict");
                    
                    gameController.playerConflict.Remove(user);
                }
            }

            if(type == StepType.FIX_DIRECTION || type == StepType.FLEX_DIRECTION) {

                if(bypassDirection) 
                    bypassDirection = false;

                hasCheckPath = false;
                ui.direction = null;

                if(!isPlayer && hasFindChest)
                    hasFindChest = false;
            }

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

    public void InitDice() {
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
    }

    private void ChooseNextStep(StepType type) {
        if(type != StepType.FIX_DIRECTION && type != StepType.FLEX_DIRECTION && type != StepType.NONE && nextStep != null) 
            diceResult--;
        

        GetNextStep();

        if(ui != null) 
            ui.RefreshDiceResult(diceResult,actualColor);
        
        finishMovement = (diceResult == 0);
    }

    private void CheckPath(bool atStart) {
        if(!hasCheckPath && stepPaths != null) { 

            if(beginStep == null)
                beginStep = nextStep != null ? nextStep.gameObject : gameController.firstStep;

            Transform stepParent = actualStep != null ? actualStep.transform.parent : beginStep.transform.parent;
            int stepIndex = actualStep != null ? gameController.FindIndexInParent(stepParent.gameObject,actualStep) : gameController.FindIndexInParent(stepParent.gameObject,beginStep);
            int result = actualStep != null ? diceResult : beginResult;

            if(stepIndex == -1) 
                stepIndex = 0;

            for (int i = 0; i < result; i++) {
                if (!reverseDice) {
                    if (stepIndex + i < stepParent.childCount) // On vérifie que le calcul du prochain index de la prochaine step est bien inférieur au nombre de step max
                        stepPaths[i] = stepParent.GetChild(stepIndex + i).gameObject;
                    else  
                        stepPaths[i] = stepParent.GetChild(stepIndex + i - stepParent.childCount).gameObject;
                }
                else {
                    if (stepIndex - i >= 0)
                        stepPaths[i] = stepParent.GetChild(stepIndex - i).gameObject;
                    else
                        stepPaths[i] = stepParent.GetChild(stepIndex - i + stepParent.childCount).gameObject;
                }
            }

            hasCheckPath = true;
           
            if (stepPaths[stepPaths.Length - 1].TryGetComponent<Step>(out Step step)) { // Si il y a deja 3 joueurs sur une mm case on retire pour eviter qu'il y en ai 4 
                if (step.playerInStep.Count == 2) {
                    GameObject[] stepsCopy = new GameObject[stepPaths.Length - 1];

                    for (int i = 0; i < stepsCopy.Length; i++)
                        stepsCopy[i] = stepPaths[i];

                    stepPaths = stepsCopy;
                    int amplifier = doubleDice ? 2 : tripleDice ? 3 : 1;
                
                    diceResult -= 1 * amplifier;

                    if (atStart && diceResult == 0)
                        diceResult = 1;
                }

                
            }
            
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
            if (stepIndex - 1 >= 0) {
                nextStep = actualParent.transform.GetChild(stepIndex - 1);
            }
            else {
                if (reverseCount) { // Si on est a la fin et qu'on veut revenir
                    Direction targetDirection = FindObjectsOfType<Direction>().Where(dir => dir.directionsStep.Contains(actualStep)).ToList()[0];

                    if (targetDirection != null) {
                        nextStep = targetDirection.directionsStep.Where(step => step != null && step != actualStep).ToList()[0].transform;
                        bypassDirection = true;
                        Debug.Log("entered");
                        return;
                    }
                }
                
                nextStep = actualParent.transform.GetChild(actualParent.transform.childCount - 1);
                // ERROR A TESTER
            }
        }
    }

   // private List<Direction> checkedDirections = new List<Direction>();
    private  List<Direction.AIPath> pendingPaths = new List<Direction.AIPath>();
    
    public void GenerateIAPaths(Direction targetDirection,GameObject target,List<GameObject> allPaths,List<Direction> checkedDirections) {

        foreach (Direction.AIPath aiPath in targetDirection.aiPaths) {
            pendingPaths.Add(aiPath);

            foreach (Direction.AIPath p_Path in pendingPaths) {

                if (p_Path.end.TryGetComponent<Direction>(out Direction direction)) {
                    //foreach()
                }
            }
            
            
            pendingPaths.Clear();
        }

        
        
        
        
        // Une fois que tt les aiPath de la directionStep sont activés  ==> on va check si la target ou on veut aller est sur le path
          
          // Si il est sur le path alors on va calculer la distance et la stocker
          // sinon on passe au directionStep Suivant 






    }


    private bool IsOnPath(GameObject path, GameObject target) {
        foreach (MeshRenderer meshRenderer in path.GetComponentsInChildren<MeshRenderer>()) {
            if (meshRenderer.bounds.Contains(target.transform.position))
                return true;
        }
        
        return false;
    }
    
    
    
   /* private bool FindSmallestChestPath(GameObject begin,GameObject end,List<GameObject> iaDirectionSteps,bool decrement,bool sameParent) {
        int indexEnd = gameController.FindIndexInParent(end.transform.parent.gameObject,end);

        if(end.GetComponent<Direction>() != null && !sameParent)
            indexEnd++;

        if (end.GetComponent<Step>() != null && end.GetComponent<Step>().type == StepType.STEP_END) // Error on board isle at the end the indexEnd is 1 should be 2 
            indexEnd++;

        for(int i = gameController.FindIndexInParent(begin.transform.parent.gameObject,begin); i != indexEnd;) {
            if (i >= begin.transform.parent.childCount) 
                i -= begin.transform.parent.childCount;

            GameObject actualObj = begin.transform.parent.GetChild(i).gameObject;


            if (!iaDirectionPath.Contains(actualObj)) 
                iaDirectionPath.Add(actualObj);

            if (inventory.cards < gameController.secretCode.Length) {
                if(actualObj == gameController.stepChest) {
                    Debug.Log("find chest begin " + begin + " end " + end);
                    hasFindChest = true;
                    return true;
                }
            }
            else {
                GameObject stepEnd = FindObjectsOfType<Step>().Where(step => step.type == StepType.STEP_END).ToList()[0].gameObject;
                if (actualObj == stepEnd) {
                    Debug.Log("find end");
                    hasFindChest = true;
                    return true;
                }
            }
            

            if(actualObj.GetComponent<Direction>() != null) {
                for(int j = 0;j<actualObj.GetComponent<Direction>().directionsStep.Length;j++) {
                    GameObject beginDirection = actualObj.GetComponent<Direction>().directionsStep[j];
                    if(beginDirection != null) { 
                        Direction nextDir = actualObj.GetComponent<Direction>().directionsStep[j].GetComponent<Direction>();

                        GameObject beginObj = beginDirection;
                        GameObject endObj;
 
                        if(nextDir != null) {
                            beginObj = nextDir.directionsStep[1].gameObject;
                            endObj = nextDir.directionsStep[1].gameObject.transform.parent.GetChild(nextDir.directionsStep[1].gameObject.transform.parent.childCount- 1).gameObject;
                        }
                        else {
                            
                            if (beginObj == beginObj.transform.parent.GetChild(beginDirection.transform.parent.childCount - 2).gameObject) { // -2 ici car on ne veut pas prendre en compte la direction
                                endObj = beginObj.transform.parent.GetChild(0).gameObject;
                                decrement = true;
                            }
                            else {
                                endObj = beginDirection.transform.parent.GetChild(beginDirection.transform.parent.childCount - 1).gameObject;
                                decrement = false;
                            }

                            if (beginObj == endObj) { // FOr Isle Board At The 3 ISle BeginObj = EndObj
                                Debug.Log("end is same than begin");
                                endObj = beginObj.transform.parent.GetChild(beginObj.transform.parent.childCount - 1).gameObject;
                                decrement = false;
                            }

                           
                        }
                        
                        int beginSize = iaDirectionPath.Count;
                        bool result = FindSmallestChestPath(beginObj,endObj,iaDirectionSteps,decrement,beginObj.transform.parent == endObj.transform.parent);
                        int size = iaDirectionPath.Count - beginSize;
                        
                        Debug.Log("result " + result + " begin " + beginObj + " end " + endObj);

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
    
    */

    private void StepBackMovement(GameObject[] steps) { // Cette fonction sera a faire en sorte que le joueur recule si un autre joueur passe devant lui 
        if(stepPaths != null) {
            foreach(GameObject step in stepPaths) {
                foreach(GameObject user in gameController.players) {
                    UserMovement userMovement = user.GetComponent<UserMovement>();
              
                    if(step != null && !userMovement.isTurn && userMovement.actualStep == step) { // Si un des joueurs est sur l'un des step du chemin
                        if (nextStep != null && nextStep.gameObject == step) { // Si la prochaine step du user dont c'est le tour est la step target
                            Step targetStep = step.GetComponent<Step>();

                            if (diceResult > 1) { // Les joueurs vont que se croiser
                                userMovement.stepBack = true;
                                agent.enabled = true;
                                gameController.UpdateSubPath(userMovement,true);
                                if (userMovement.point == Vector3.zero) {
                                    userMovement.point = targetStep.avoidPos;
                                    userMovement.point.y = step.transform.position.y;
                                    userMovement.agent.speed *= 2f; // remake /= 3f at the end of stepback
                                    userMovement.agent.angularSpeed *= 2f;
                                    userMovement.agent.acceleration *= 2f;
                                    gameController.playerConflict.Add(user, step);
                                }
                            }
                            else // Il va y avoir plusieurs joueurs sur la même case
                                step.GetComponent<Step>().AddPlayerInStep(user);
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

      //  isMooving = false;
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
        
        
      //  isMooving = false;
        if(inventory.coins > 0) {
            audio.CoinsLoose();
            inventory.CoinLoose(3);
            ui.DisplayReward(false,3,stepReward);
            gameController.ActualizePlayerClassement();
        }   
        else 
            gameController.EndUserTurn(); // A Test lors d'une step shop ou chest 

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
        
        
       // isMooving = false;
        if(inventory.coins > 0) {
            audio.CoinsLoose();
            inventory.CoinLoose(amount);
            ui.DisplayReward(false,amount,stepReward);
            gameController.ActualizePlayerClassement();
        }
        else
            gameController.EndUserTurn(); // A Test lors d'une step shop ou chest 
        

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

    #endregion
     
}
