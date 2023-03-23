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
    public bool startCoroutine;
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
    public Vector3 point;
    public bool hasCheckPath;
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

    public bool bypassReverse;

    public UserAction currentAction;

    void Start() {
        path = new NavMeshPath();
        lastPosition = transform.position;

        gameController.inputs.FindAction("Player/Jump").started += OnJump;
        userCam = transform.GetChild(1).gameObject;
    }

    public override void OnBeginTurn() {
        stepBack = false;
        point = Vector3.zero;
        ui.enabled = true;

        transform.LookAt(gameController.mainCamera.transform);

        if(waitDiceResult) {
            if(!isPlayer) {

                if (inventory.HasObjects())
                {
                    Debug.Log("use item as bot");
                    inventory.UseItemBot();
                }

                InitDice();
            }
            else
                currentAction = UserAction.MENU;
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
        userCam.SetActive(false);
        hasShowChestHUD = false;
        canMooveToChest = true;
        waitChest = false;
        hasCollideDice = false;
        waitDiceResult = true;
        hasJump = false;
        finishMovement = false;
        startCoroutine = false;

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
                Debug.Log("turn of " + transform.gameObject.name);
                canMoove = !stop;
                
                if(nextStep != null && !jump && !stop) {
                    if(!userCam.activeSelf) {
                        RunDelayed(0.2f,() => {
                            GameObject camera = userCam.gameObject;
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

                    if (canMoove)
                    {
                        agent.CalculatePath(nextStep.position, path);
                        ShowPath(Color.magenta, path);
                        //CheckPath(false);

                        if (!finishMovement)
                            StepBackMovement(stepPaths);

                        agent.SetPath(path);
                        agent.velocity = agent.desiredVelocity;


                        if (nextStep.GetComponent<Step>()?.type != StepType.STEP_END) { // This condition to avoid camera moove when player comeback from end animation controller
                            Quaternion targetRotation = Quaternion.LookRotation(nextStep.position - transform.position);
                            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, 1.5f * Time.deltaTime);
                        }

                        if (!audio.userSource.isPlaying)                
                            audio.Footstep();
                        
                    }
                }

                if(finishMovement && !startCoroutine) {
                    StepType type = actualStep.GetComponent<Step>().type;
                    if (type == StepType.BONUS) {
                        currentAction = UserAction.BONUS;
                        StartCoroutine(WaitBonus());
                    }

                    if(type == StepType.MALUS) 
                        StartCoroutine(WaitMalus());

                    ui.ClearDiceResult();
                    hasCheckPath = false;
                    startCoroutine = true;
                }            

                if(!isPlayer && waitDiceResult) {
                    /*if(inventory.HasObjects() && !checkObjectToUse) {
                        Debug.Log("use my items as bot");
                        checkObjectToUse = true;
                       // inventory.UseItemBot();
                    }
*/
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
               currentAction = UserAction.WAIT;

               if(stepBack) {
                   if (actualStep.TryGetComponent<Step>(out Step step)) { // Only stepback on player who is not in step stack 
                       if (step.playerInStep.Contains(transform.gameObject)) {
                           stepBack = false;
                           return;
                       }
                   }
                   
                   
                   Quaternion targetRotation = Quaternion.LookRotation(point - transform.position);
                   transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, 1.5f * Time.deltaTime);
                   

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

    #region Collisions 
    private void OnCollisionEnter(Collision hit) {
       if(isTurn) {
           if(hit.gameObject.CompareTag("Chest")) {

               if (actualStep.GetComponent<Step>().type == StepType.STEP_END) { // To Check If correct
                   gameController.endAnimationController.checkCode = true;
               }
               else {
                   hit.gameObject.GetComponent<Animation>().clip = GameController.Instance.chestController.chestAnimations[1];
                   hit.gameObject.GetComponent<Animation>().Play();
               }
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

            //   diceResult = 50;

               // diceResult = 26;
            
                //diceResult =  63;

               beginResult = diceResult; 
               stepPaths = new GameObject[beginResult]; 
               hasCollideDice = true;  
                
               actualColor = tripleDice ? new Color(1f,0.74f,0f) : doubleDice ? new Color(0.32f,0.74f,0.08f,1.0f) : reverseDice ? new Color(0.41f,0.13f,0.78f,1.0f) : new Color(0f,0.35f,1f,1.0f);
               ui.RefreshDiceResult(diceResult, actualColor);

               GameObject hitObj = hit.gameObject;
               hitObj.GetComponent<MeshRenderer>().enabled = false;

               CheckPath(true,stepPaths,actualStep != null ? diceResult : beginResult);
               ChooseNextStep(gameController.firstStep.GetComponent<Step>().type);

               currentAction = UserAction.MOVEMENT;

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
            
            if (ui.showDirection.value && type == StepType.FIX_DIRECTION) { // Used to patch a bug sometime the showDirection is showed a long time 
                ui.showDirection.value = false;
                stop = false;
            }
            
            if(type == StepType.NONE) 
                return;
            
            if(type == StepType.BONUS || type == StepType.MALUS || type == StepType.SHOP || type == StepType.BONUS_END || type == StepType.MALUS_END || type == StepType.STEP_END) {

                if (diceResult < 0) { // Called when the player is coming back from the end animation controller
                    if (type == StepType.BONUS || type == StepType.STEP_END)
                        StartCoroutine(WaitBonus());

                    if (type == StepType.MALUS)
                        StartCoroutine(WaitMalus());
                }
                
                if(gameController.shopController.returnToStep || gameController.chestController.returnToStep) {
                    RunDelayed(0.35f,() => {

                        if (userCam.transform.parent != transform)
                            userCam.transform.SetParent(transform, true);

                        if(gameController.shopController.returnToStep) {
                            gameController.shopController.returnToStep = false;
                            gameController.EndUserTurn();
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
                        if (nextStep.GetComponent<Direction>() != null) {
                            
                            if (!reverseDice && !reverseCount) {
                                nextStep = nextStep.GetComponent<Direction>().directionsStep[1].transform;
                            }
                        }
                    }
                }

                if(type == StepType.SHOP && diceResult == 0) { // Faire en sorte qu'on puisse y aller sans que ca soit la dernière step 
                    if(isPlayer) {
                        RunDelayed(0.5f,() => {
                            Dialog shopDialog = gameController.dialog.GetDialogByName("AskShop");
                            if(gameController.dialog.currentDialog != shopDialog) {
                                gameController.dialog.isInDialog.value = true;
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

                if (bypassDirection && reverseCount) 
                    reverseCount = false;
                
                if (lastStep.TryGetComponent<Direction>(out Direction direction)) { // IL Y A DEUX DIRECTIONS QUI SE SUCCEDE
                    Transform stepParent = actualStep != null ? actualStep.transform.parent : beginStep.transform.parent;
                    int directionStepIndex = actualStep != null ? gameController.FindIndexInParent(stepParent.gameObject,actualStep) : gameController.FindIndexInParent(stepParent.gameObject,beginStep);

                    nextStep = reverseDice ? stepParent.GetChild(Mathf.Abs(directionStepIndex - (stepParent.childCount - 1))) : actualStep.GetComponent<Direction>().directionsStep[1].transform;
                }
                
                if(bypassDirection) {
                    if (bypassReverse) {
                        bypassReverse = false;
                        GetNextStep();
                        return;
                    }
                
                    nextStep =  actualStep.GetComponent<Direction>().directionsStep[1].transform;
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

            if(type == StepType.FIX_DIRECTION && (lastStep != null && lastStep.GetComponent<Step>() != null && (lastStep.GetComponent<Step>().type != StepType.BONUS_END || lastStep.GetComponent<Step>().type != StepType.MALUS_END)) && !bypassDirection && isTurn) {
                if(isPlayer) { // Joueur
                    if(!left && !right && !front) {
                        if(!ui.showDirection) 
                            ui.showDirection.value = true;
                        stop = true; 
                        
                        return;
                    }
                    else {
                        agent.enabled = true;
                        if (left) {
                            nextStep = reverseDice ? ui.direction.directionsStep[2].transform : ui.direction.directionsStep[0].transform;
                            bypassReverse = reverseDice ? ui.direction.bypassReverse[0] : ui.direction.bypassReverse[2];
                        }

                        if(front) {
                            nextStep = ui.direction.directionsStep[1].transform;
                            bypassReverse = ui.direction.bypassReverse[1];
                            // front = false;
                        }

                        if (right) {
                            nextStep = reverseDice ? ui.direction.directionsStep[0].transform : ui.direction.directionsStep[2].transform;
                            bypassReverse = reverseDice ? ui.direction.bypassReverse[2] : ui.direction.bypassReverse[0];
                        }

                        stop = !(left || front || right);
                    }
                }
                
                else { // Bot Facile = 50 ; Moyen = 70 ; Difficile  = 90 de chance d'aller vers le coffre
                    if(!hasFindChest) {
                        stop = true;
                        
                        RunDelayed(0.1f,() => {
                            GameObject target = inventory.cards < gameController.secretCode.Length
                                ? gameController.stepChest
                                : FindObjectsOfType<Step>().Where(step => step.type == StepType.STEP_END).ToList()[0].gameObject;

                            Direction direction = ui.direction;

                            if (direction == null || direction.directionInfos == null)
                                return;
                            
                            Direction.DirectionInfos targetInfo = reverseCount || reverseDice ? direction.directionInfos[1] : direction.directionInfos[0];
                            // Prend en compte que les coffres et pas le chest finale 

                            if (inventory.cards == gameController.secretCode.Length) 
                                nextStep = targetInfo.toEnd.transform;
                            else 
                                nextStep = targetInfo.mustContains.Contains(target) ? targetInfo.directionTarget.transform : direction.directionsStep.Where(obj => obj != null && obj != targetInfo.directionTarget).ToList()[0].transform;
                            
                            
                            stop = false;
                            hasFindChest = true;
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
                   // userMovement.point.y = user.transform.position.y;
                    
                    gameController.playerConflict.Remove(user);
                }
            }

            if(type == StepType.FIX_DIRECTION || type == StepType.FLEX_DIRECTION) {

                if (bypassDirection) {
                    bypassDirection = false;
                }

                hasCheckPath = false;
                ui.direction = null;

                if(!isPlayer && hasFindChest)
                    hasFindChest = false;
                
                hasCheckPath = false;
                stepPaths = new GameObject[diceResult];
                CheckPath(false,stepPaths,actualStep != null ? diceResult : beginResult);

            }
            
            
            left = false;
            front = false;
            right = false;
            
            // Code Here

            if(type == StepType.BONUS || type == StepType.MALUS || type == StepType.SHOP || type == StepType.FIX_DIRECTION || type == StepType.FLEX_DIRECTION || type == StepType.BONUS_END  || type == StepType.MALUS_END || type == StepType.STEP_END) 
                lastStep = hit.gameObject;
        }
    }
    #endregion

    #region Inputs Functions

    public void OnJump(InputAction.CallbackContext e) {
        if(e.started && canJump && isPlayer) 
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
        currentAction = UserAction.DICE;
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

    public void CheckPath(bool atStart,GameObject[] stepPaths,int sizeToGo) {
        if(!hasCheckPath && stepPaths != null) { 
            
            if(beginStep == null)
                beginStep = nextStep != null ? nextStep.gameObject : gameController.firstStep;

            Transform stepParent = actualStep != null ? actualStep.transform.parent : beginStep.transform.parent;
            int stepIndex = actualStep != null ? gameController.FindIndexInParent(stepParent.gameObject,actualStep) + 1 : gameController.FindIndexInParent(stepParent.gameObject,beginStep);
            int result = sizeToGo; // actualStep != null ? diceResult : beginResult

            if (!atStart && nextStep != null) { // called when leave direction
                stepParent = nextStep.transform.parent;
                stepIndex = gameController.FindIndexInParent(stepParent.gameObject, nextStep.gameObject);
            }

            if(stepIndex == -1) 
                stepIndex = 0;

            if (stepParent.childCount < result) // Dans le cas ou le nouveau parent est pas aussi grand que le numéro du dé
                result = stepParent.childCount;
            
            
            for (int i = 0; i < result; i++) {
                if (!reverseDice && !reverseCount) {
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
           
            if (atStart && stepPaths[stepPaths.Length - 1].TryGetComponent<Step>(out Step step)) { // Si il y a deja 3 joueurs sur une mm case on retire pour eviter qu'il y en ai 4 
                if (step.playerInStep.Count == 2) {
                    GameObject[] stepsCopy = new GameObject[stepPaths.Length - 1];

                    for (int i = 0; i < stepsCopy.Length; i++)
                        stepsCopy[i] = stepPaths[i];

                    this.stepPaths = stepsCopy;
                    int amplifier = doubleDice ? 2 : tripleDice ? 3 : 1;
                
                    diceResult -= 1 * amplifier;

                    if (diceResult <= 0)
                        diceResult = 1 * amplifier;
                }

                
            }
            
        }

    }

    private void GetNextStep() {
        GameObject actualParent = actualStep != null ? actualStep.transform.parent.gameObject : gameController.firstStep.transform.parent.gameObject;
        int stepIndex = gameController.FindIndexInParent(actualParent,actualStep);

        if (diceResult <= 0)
            return;

        if(!reverseDice && !reverseCount && !bypassReverse) { // Si le joueur n'utilise pas le dé inverse ou qu'il n'est pas en reverseCount
            if(stepIndex + 1 < actualParent.transform.childCount) 
                nextStep = actualParent.transform.GetChild(stepIndex + 1);
            else 
                nextStep = actualParent.transform.GetChild(0);
        }
        else { // Le joueur utilise le dé inverse ou est en reverseCount
            if (bypassReverse) {
                if(stepIndex + 1 < actualParent.transform.childCount) 
                    nextStep = actualParent.transform.GetChild(stepIndex + 1);
                else 
                    nextStep = actualParent.transform.GetChild(0);
            }
            else {
                
                
                Debug.Log("enter else 22");
                if (stepIndex -  1 >= 0) {
                    Debug.Log("next is here");
                    nextStep = actualParent.transform.GetChild(stepIndex - 1);
                }
                else {
                    if (reverseCount) { // Si on est a la fin et qu'on veut revenir
                        Debug.Log("here reverse");
                        Direction targetDirection = FindObjectsOfType<Direction>().Where(dir => dir.directionsStep.Contains(actualStep)).ToList()[0];
                        if (targetDirection != null) {
                            nextStep = targetDirection.directionsStep.Where(step => step != null && step != actualStep).ToList()[0].transform;
                            bypassDirection = true;
                            return;
                        }
                    }
                
                    nextStep = actualParent.transform.GetChild(actualParent.transform.childCount - 1);
                    // ERROR A TESTER
                }
            }
        }
        
//        Debug.Log("nextStep " + nextStep);
    }

    private void StepBackMovement(GameObject[] steps) { // Cette fonction sera a faire en sorte que le joueur recule si un autre joueur passe devant lui 
        if(steps != null) {
            foreach(GameObject step in steps) {
                foreach(GameObject user in gameController.players) {
                    UserMovement userMovement = user.GetComponent<UserMovement>();
                    if(step != null && !userMovement.isTurn && userMovement.actualStep == step && !step.GetComponent<Step>().playerInStep.Contains(userMovement.gameObject)) { // Si un des joueurs est sur l'un des step du chemin
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
        
        Debug.Log("jump my body");

        agent.enabled = false;
        rb.AddForce(jumpSpeed * Vector3.up,ForceMode.Impulse);    
    }

    #endregion

    #region Timers

    public static void ShowPath(Color color,NavMeshPath path) {
        for(int i = 0;i<path.corners.Length - 1;i++) 
            Debug.DrawLine(path.corners[i], path.corners[i + 1], color);
    }

    private IEnumerator WaitBonus() {
        yield return new WaitForSeconds(0.5f);

        //  isMooving = false;
        inventory.CoinGain(3);
        audio.CoinsGain();
        ui.DisplayReward(true,3);
        ui.ClearDiceResult();
        gameController.ActualizePlayerClassement();

        random = -1;
        timer = 0f;
    }

    private IEnumerator WaitMalus() {

        currentAction = UserAction.MALUS;
        
        yield return new WaitForSeconds(0.5f);

        //  isMooving = false;
        if(inventory.coins > 0) {
            audio.CoinsLoose();
            inventory.CoinLoose(3);
            ui.DisplayReward(false,3);
            gameController.ActualizePlayerClassement();
        }
        else
        {
            Debug.Log("end when no coins ");
            gameController.EndUserTurn(); // A Test lors d'une step shop ou chest 
        }

        ui.ClearDiceResult();   

        random = -1;
        timer = 0f;
    }

    public IEnumerator WaitMalus(int amount) { // Impossible de booster les cases

        yield return new WaitForSeconds(0.5f);

        Debug.Log("enter wait malus " + currentAction);
        
       // isMooving = false;
        if(inventory.coins > 0) {
            audio.CoinsLoose();
            inventory.CoinLoose(amount);
            ui.DisplayReward(false,amount);
            gameController.ActualizePlayerClassement();
        }
        //else {
          //  gameController.EndUserTurn(); // A Test lors d'une step shop ou chest 
        //}

        ui.ClearDiceResult();   

        random = -1;
        timer = 0f;
    }
    

    public void DisplayChestDialog() {
        if (inventory.cards >= gameController.secretCode.Length) {
            gameController.EndUserTurn();
            Debug.Log("end cause have full secret code");
            return;
        }

        
        Dialog askChest = gameController.dialog.GetDialogByName("AskChestBuy");
        gameController.dialog.isInDialog.value = true;
        gameController.dialog.currentDialog = askChest;
        StartCoroutine(gameController.dialog.ShowText(askChest.Content[0],askChest.Content.Length));
    }

    #endregion
     
}
