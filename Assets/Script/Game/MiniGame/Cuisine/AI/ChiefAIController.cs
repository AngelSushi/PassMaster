using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Grid;
using UnityEngine.AI;
using Random = UnityEngine.Random;
using Recipes;

public class ChiefAIController : ChiefController
{
    private CookController.Team _team;

    public CookController.Team Team
    {
        get => _team;
        set => _team = value;
    }
    
    private Tile _actualTile; 
    
    public Tile ActualTile
    {
        get
        {
            RaycastHit tileRaycast = Physics.RaycastAll(IngredientSpawn.position, Vector3.down, 5, LayerMask.GetMask("Grid"))[0];

            if (tileRaycast.collider != null)
            {
                foreach (Tile tile in _gridManager.Grid)
                {
                    if (tile.TileObj == tileRaycast.collider.gameObject)
                    {
                        _actualTile = tile;
                        return _actualTile;
                    }
                }
            }

            return null;
        }
    }
    
    
    [SerializeField] private AIAction _currentAction;

    public AIAction CurrentAction
    {
        get => _currentAction;
        set => _currentAction = value;
    }
    
    private AIAction.AITask _currentTask;

    public AIAction.AITask CurrentTask
    {
        get => _currentTask;
        set => _currentTask = value;
    }
    
    
    [SerializeField] private Recipe currentWorkRecipe;

    public Recipe CurrentWorkRecipe
    {
        get => currentWorkRecipe;
        private set => currentWorkRecipe = value;
    }
    
    
    private List<BasicBox> _emptyBoxes = new List<BasicBox>();

    public List<BasicBox> EmptyBoxes
    {
        get => _emptyBoxes;
        private set => _emptyBoxes = value;
    }
    
    
    private BasicBox _plateBox;

    public BasicBox PlateBox
    {
        get => _plateBox;
        set => _plateBox = value;
    }

    private bool _goToStart;

    public bool GoToStart
    {
        get => _goToStart;
        set => _goToStart = value;
    }
    
    [Header("Boxes")]
    [SerializeField] private Vector2Int plateCoords;

    public Vector2Int PlateCoords
    {
        get => plateCoords;
        set => plateCoords = value;
    }
    
    [SerializeField] private List<Box> basicBoxes;

    public List<Box> BasicBoxes
    {
        get => basicBoxes;
        set => basicBoxes = value;
    }
    
    [Header("Actions")]
    [SerializeField] private List<AIAction> currentRecipeActions = new List<AIAction>();

    public List<AIAction> CurrentRecipeActions
    {
        get => currentRecipeActions;
        private set => currentRecipeActions = value;
    }


    [Header("Debug")]
    [SerializeField] private int tryCount;

    private GridManager _gridManager;
    private CookController _controller;
    private NavMeshAgent _agent;
    private NavMeshPath _path;

    private float _actionTimer;

    public float ActionTimer
    {
        get => _actionTimer;
        set => _actionTimer = value;
    }
    

    /**
     *  
     *
     * Régler les soucis de path
     *
     *
     */

    
    private void Start()
    {
        _gridManager = GridManager.Instance;
        _controller = (CookController)CookController.instance;

        foreach (BasicBox box in FindObjectsOfType<BasicBox>())
        {
            _emptyBoxes.Add(box);
        }

        
        StartNewRecipe(_team.recipes);      
        

        
        _agent = GetComponent<NavMeshAgent>();
        _path = new NavMeshPath();
            
        IsMooving = true;
        CanMoove = true;
    }


    private void Update()
    {

        if (Input.GetKeyDown(KeyCode.R))
        {
            GetComponent<AIEventListener>().GenerateBinActions();
        }

        if (_currentAction != null && _currentTask != null)
        {
            if (CanMoove)
            {
                  if (IsMooving)
                  {
                      if (_goToStart)
                      {
                          _agent.CalculatePath(_gridManager.GetWorldPosition(_currentTask.Start), _path);
                          _agent.SetPath(_path);
                          _agent.velocity = _agent.desiredVelocity;
  
                          if (_agent.velocity.normalized != Vector3.zero)
                          {
                              transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(_agent.velocity.normalized), 2f * Time.deltaTime);    
                          }

                          if (_agent.remainingDistance < 0.5f)
                          {
                              _controller.AiEvents.OnTaskReachStart?.Invoke(this,new AIEvents.OnTaskReachStartArgs(_currentTask,_currentAction,transform.gameObject,_team,ActualTile));
                          }
                      }
                      else
                      {
                           _agent.CalculatePath(_gridManager.GetWorldPosition(_currentTask.End), _path);
                          _agent.SetPath(_path);
                          _agent.velocity = _agent.desiredVelocity;
  
                          if (_agent.velocity.normalized != Vector3.zero)
                          {
                              transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(_agent.velocity.normalized), 2f * Time.deltaTime);    
                          }
                      
  
                          if (_agent.remainingDistance < 0.5f)
                          {
                              _controller.AiEvents.OnTaskReachEnd?.Invoke(this, new AIEvents.OnTaskReachEndArgs(_currentTask, _currentAction, transform.gameObject, _team,_currentTask.End));
                          }
                      }
                  }
            }
        }
        else 
        {
            CurrentAction = CurrentRecipeActions.FirstOrDefault(aiAction => !aiAction.IsFinished && aiAction.Condition.All(val => val));

            if (CurrentAction != null)
            {
                CurrentTask = CurrentAction.Tasks.First(task => !task.IsFinished);
                _controller.AiEvents.OnTaskStarted?.Invoke(this,new AIEvents.OnTaskStartedArgs(CurrentTask,CurrentAction,transform.gameObject,Team));
                CheckGoToStart();
            }
        }
    }

    #region "Utils"
    private BasicBox GetBoxWithType(Type boxType)  {
        if (boxType == typeof(CutBox))
        {
            return _emptyBoxes.Where(box => box is CutBox).ToList()[0];
        }
        else if (boxType == typeof(PanBox))
        {
            return _emptyBoxes.Where(box => box is PanBox).ToList()[0];
        }
        else if (boxType == typeof(StoveBox))
        {
            return _emptyBoxes.Where(box => box is StoveBox).ToList()[0];
        }
        else if (boxType == typeof(OvenBox))
        {
            return _emptyBoxes.Where(box => box is OvenBox).ToList()[0];
        }

        return null;
    }

    public Tile GetPlateBoxEndTile()
    {
        Tile end = _plateBox.Tile;

        if (_plateBox.Stock != null && _plateBox.Stock.GetComponent<Plate>() == null)
        {
            end = _gridManager.FromCoords(ApplyDifficultyOffset(_plateBox).TileCoords);
        }

        return end;
    }

    #endregion

    public void ActionDuringTimer()
    {

        float random = Random.Range(0f, 1f);
        float difficultyRandom = 0f;
        
        switch (_controller.Difficulty)
        {
            case GameController.Difficulty.EASY:
                difficultyRandom = 0.60f;
                break;
                        
            case GameController.Difficulty.MEDIUM:
                difficultyRandom = 0.4f;
                break;
                        
            case GameController.Difficulty.HARD:
                difficultyRandom = 0.2f;
                break;
                        
            default:
                difficultyRandom = 0.6f;
                break;
        }
        
        
        
       Tile end = GetPlateBoxEndTile();

       /**
        *
        * Le temps qu'il va mettre pour faire la prochaine action
        *
        *
        * Il est cramé ou non
        *
        * S'il est cramé
        * ==> Générer un temps qui va faire que
        *
        
        */

       AIAction nextAction = currentRecipeActions.FirstOrDefault(aiAction => !aiAction.IsFinished && aiAction.Condition.All(val => val));

       AIAction nextEndToActual = new AIAction(null);
       nextEndToActual.Tasks.Add(new AIAction.AITask(nextAction.Tasks.Last().End, ActualTile));
       
       float distance = CalculateRecipeDistance(new List<AIAction>() { nextAction,nextEndToActual });
       float timeToReach =   20 / (distance / 2);
       
       Debug.Log("distance " + distance);
       Debug.Log("timeToReach " + timeToReach);
       
       // V = d * t

       if (random <= difficultyRandom && _actionTimer == 0)
       {
           Debug.Log("crame toi");
           _actionTimer = Random.Range(timeToReach + 0.1f,5f);
       }
       else
       {
           Debug.Log("ne crame pas");
           _actionTimer = Random.Range(0f, timeToReach);
       }

       _actionTimer = 0f;
       
       AIAction cookToStock = new AIAction(_currentAction.ActionOn,new bool[] {false}); 
       cookToStock.Name = "CookToStock";
       cookToStock.Priority = 3;
            
       cookToStock.Tasks.Add(new AIAction.AITask(ActualTile,end));
            
       currentRecipeActions.Add(cookToStock);
       OrderTasks(CurrentRecipeActions);

       Debug.Log("wait");
       
       RunDelayed(_actionTimer, () =>
       {
           Debug.Log("wait22");
           _controller.AiEvents.OnTaskFinished?.Invoke(this, new AIEvents.OnTaskFinishedArgs(_currentTask,_currentAction,transform.gameObject,_team));
       });

    }

    public void StartNewRecipe(List<Recipe> recipes)
    {
        ChooseRecipe(recipes);
        
        AIEvents.OnChooseRecipeArgs e = new AIEvents.OnChooseRecipeArgs(currentWorkRecipe, transform.gameObject, CurrentRecipeActions);

        _controller.AiEvents.OnChooseRecipe?.Invoke(this,e);
        
        if (e.IsCanceled && recipes.Count > 1)
        {
            List<Recipe> newRecipes = recipes.ToList();
            
            if (newRecipes.Contains(currentWorkRecipe))
            {
                newRecipes.Remove(currentWorkRecipe);
            }
            
            currentRecipeActions.Clear();
            StartNewRecipe(newRecipes);    
        }

        FindBoxForPlate();
        DispatchTasks();
        
        Debug.Log("recipe " + currentWorkRecipe.Name);
        _currentAction = CurrentRecipeActions[0];
        _actionTimer = 0f;

        Debug.Log("actionName " + _currentAction.Name);
        _currentTask = _currentAction.Tasks.First();

        CheckGoToStart();
    }

    public void CheckGoToStart()
    {
        Box startTileBox = CurrentTask.Start.AttachedBox;

        if (startTileBox != null)
        {
            if (startTileBox is BasicBox)
            {
                BasicBox basicStartTileBox = (BasicBox)startTileBox;
                
                if (basicStartTileBox.Stock != null)
                {
                    Ingredient stockIngredient = basicStartTileBox.Stock.GetComponent<Ingredient>();

                    Debug.Log("stockIngredient " + stockIngredient);

                    if (stockIngredient != null)
                    {
                        if (stockIngredient.Data != null &&  (!stockIngredient.Data.CanBeCook && !stockIngredient.Data.CanBeCut) || (stockIngredient.Data.CanBeCook && stockIngredient.IsCook) || (stockIngredient.Data.CanBeCut && stockIngredient.IsCut))
                        {
                            GoToStart = true;
                        } 
                    }
                    else
                    {
                        Plate plate = basicStartTileBox.Stock.GetComponent<Plate>();

                        if (plate != null)
                        {
                            GoToStart = true;
                        }
                    }
                    
                    
                }
            }
        }
    }

    #region "Choose Recipe"
    private void ChooseRecipe(List<Recipe> recipes)
    { // Get the "brut" distance between each element to know which recipe AI is going to choose 
        List<int> recipesDistance = new List<int>();
        
        
        Debug.Log("recipesLength " + recipes.Count);
        
        foreach (Recipe recipe in recipes)
        {
            int distance;

            CalculateTheoricDistance(recipe,out distance);

            recipesDistance.Add(distance);
            
            Debug.Log("choose recipe distance for " + recipe.Name + " is " + distance);
        }

        int minIndex;
        FindShortestRecipe(recipesDistance,out minIndex);

        minIndex = Mathf.Clamp(minIndex, 0, recipes.Count - 1);
        
        currentWorkRecipe = recipes[minIndex];    
        
        ChooseRecipeDifficulty(recipes);
    }

    private void CalculateTheoricDistance(Recipe recipe, out int distance)
    {
        distance = 0;
        
        for (int i = 0; i < recipe.AllIngredients.Count; i++)
        {
            IngredientBox targetIngredientBox = FindObjectsOfType<IngredientBox>().Where(ingredientBox => ingredientBox.Ingredient.GetComponent<Ingredient>().Data == recipe.AllIngredients[i]).ToList()[0];
            distance += _gridManager.GeneratePath(ActualTile, targetIngredientBox.Tile).Count;

            if (recipe.AllIngredients[i].CanBeCook)
            {
                if (!recipe.AllIngredients[i].IsPan)
                {
                    distance += _gridManager.GeneratePath(targetIngredientBox.Tile, GetBoxWithType(typeof(StoveBox)).Tile).Count;
                }
                else
                {
                    distance += _gridManager.GeneratePath(targetIngredientBox.Tile, GetBoxWithType(typeof(PanBox)).Tile).Count;
                }

            }

            if (recipe.AllIngredients[i].CanBeCut)
            {
                distance += _gridManager.GeneratePath(targetIngredientBox.Tile, GetBoxWithType(typeof(CutBox)).Tile).Count;
            }
        }



        if (recipe.NeedToBeCook)
        {
            distance += _gridManager.GeneratePath(ActualTile, GetBoxWithType(typeof(OvenBox)).Tile).Count;

        }
        
        // Add Time Condition
        distance = (int)(distance * recipe.Ticker.CurrentTime / recipe.RecipeTime);

    }

    public int CalculateRecipeDistance(List<AIAction> actions)
    {
        int distance = 0;

        
        for (int i = 0;i < actions.Count;i++)
        {
            AIAction action = actions[i];

            Tile startTile = action.Tasks.First().Start;

            if (i >= 1)
            {
                startTile = actions[i - 1].Tasks.Last().End;
            }
            
            for(int j = 0;j < action.Tasks.Count;j++)
            {
                AIAction.AITask task = action.Tasks[j];

                Tile startTileTask = task.Start;

                if (j == 0)
                {
                    startTileTask = startTile;
                }

                List<Tile> path = _gridManager.GeneratePath(startTileTask, task.End);

                if (path != null)
                {   
                    //Debug.Log("start to " + startTileTask.Coords + " end To " + task.End.Coords + " name " + action.Name);
                    
                    distance += path.Count;
                }
            }
        }
        
        Debug.Log("distance for " + currentWorkRecipe.Name + " " + distance);
        
        return distance;
    }

    public bool CanTakeRecipe(float distance, Recipe currentRecipe)
    {
        float targetChoose = 0;
        
        switch (_controller.Difficulty)
        {
            case GameController.Difficulty.EASY:
                targetChoose = 0.30f;
                break;
            
            case GameController.Difficulty.MEDIUM:
                targetChoose = 0.5f;
                break;
            
            case GameController.Difficulty.HARD:
                targetChoose = 0.75f;
                break;
        }

        float choose = Random.Range(0f, 1f);

        bool succeed = choose <= targetChoose;
        
        if (succeed)
        {
            return currentRecipe.Ticker.CurrentTime >= (int)(distance * 0.37f); // 0.37s = 1 tile move
        }
        
        return true;
    }

    private void FindShortestRecipe(List<int> recipesDistance,out int shortestIndex)
    {
        shortestIndex = 0;
        
        if (recipesDistance.Count == 0)
        {
            Debug.Log("Recipe Distance Count is 0");
            return;
        }
        
        
        int minDistance = recipesDistance[0];
        
        
        for (int i = 0;i < recipesDistance.Count;i++)
        {
            int distance = recipesDistance[i];
            
            if (distance < minDistance)
            {
                minDistance = distance;
                shortestIndex = i;
            }
        }
    }

    private void ChooseRecipeDifficulty(List<Recipe> recipes)
    {
        int choose = Random.Range(0, 100);
        int targetChoose = 0;

        switch (_controller.Difficulty)
        {
            case GameController.Difficulty.EASY:
                targetChoose = 75; 
                break;
            
            case GameController.Difficulty.MEDIUM:
                targetChoose = 45;
                break;
            
            case GameController.Difficulty.HARD:
                targetChoose = 20;
                break;
            
            default:
                targetChoose = 75;
                break;
        }

        bool succeed = choose <= targetChoose;

       // succeed = false;
        
        if (succeed)
        {
            if (recipes.Count == 1)
            {
                return;
            }
            
            Debug.Log("choose no the shortest recipe");
            List<Recipe> newTeamRecipe = _team.recipes.ToList();
            newTeamRecipe.Remove(currentWorkRecipe);
            
            ChooseRecipe(newTeamRecipe);
        }
    }
    
    #endregion

    #region "Find Plate Box"
    private void FindBoxForPlate()
    {
        int averageX = ActualTile.X;
        int averageY = ActualTile.Y;
        
        foreach (IngredientData ingredient in currentWorkRecipe.AllIngredients)
        {
            IngredientBox targetIngredientBox = FindObjectsOfType<IngredientBox>().Where(ingredientBox => ingredientBox.Ingredient.GetComponent<Ingredient>().Data == ingredient).ToList()[0];
            averageX += targetIngredientBox.Tile.X;
            averageY += targetIngredientBox.Tile.Y;

            if (ingredient.CanBeCut)
            {
                CutBox targetCutBox = (CutBox)GetBoxWithType(typeof(CutBox));
                averageX += targetCutBox.Tile.X;
                averageY += targetCutBox.Tile.Y;
            }

            if (ingredient.CanBeCook)
            {
                if (!ingredient.IsPan)
                {
                    StoveBox targetStoveBox = (StoveBox)GetBoxWithType(typeof(StoveBox));
                    averageX += targetStoveBox.Tile.X;
                    averageY += targetStoveBox.Tile.Y;
                }
                else 
                {
                    PanBox targetPanBox = (PanBox)GetBoxWithType(typeof(PanBox));
                    averageX += targetPanBox.Tile.X;
                    averageY += targetPanBox.Tile.Y;
                }
            }
        }
        
        if (currentWorkRecipe.NeedToBeCook)
        {
            OvenBox targetOvenBox = (OvenBox)GetBoxWithType(typeof(OvenBox));
            averageX += targetOvenBox.Tile.X;
            averageY += targetOvenBox.Tile.Y;
        }

        PlateBox plateBox = FindObjectOfType<PlateBox>();
        averageX += plateBox.Tile.X;
        averageY += plateBox.Tile.Y;

        DeliveryBox deliveryBox = FindObjectOfType<DeliveryBox>();
        averageX += deliveryBox.Tile.X;
        averageY += deliveryBox.Tile.Y;
        
        averageX /= 7;
        averageY /= 7;
        
        Vector2 brutCoords = new Vector2(averageX, averageY);

        Box nearestBox = FindNearestBox(brutCoords);
        
        _plateBox = (BasicBox)ApplyDifficultyOffset((BasicBox)nearestBox);
        plateCoords = _plateBox.TileCoords;
    }

    private Box FindNearestBox(Vector2 brutCoords)
    {
        Box minBox = basicBoxes[0];
        float minDistance = Vector2.Distance(brutCoords, minBox.Tile.Coords);
        
        foreach (Box basicBox in basicBoxes)
        {
            float distance = Vector2.Distance(basicBox.Tile.Coords, brutCoords);

            if (distance < minDistance)
            {
                minDistance = distance;
                minBox = basicBox;
            }
        }

        return minBox;
    }

    public Box ApplyDifficultyOffset(BasicBox basicBox)
    {
        int offset = 0;
        
       /* switch (_controller.Difficulty)
        {
            case GameController.Difficulty.EASY:
                offset = 3;
                break;
            
            case GameController.Difficulty.MEDIUM:
                offset = 2;
                break;
            
            case GameController.Difficulty.HARD:
                offset = 1;
                break;
            
            default:
                offset = 3;
                 break;
        }
*/
        
        int randomOffset = Random.Range(-offset, offset);

        int basicIndex = basicBoxes.IndexOf(basicBox);
        int newIndex = basicIndex + randomOffset;
        
        newIndex = Mathf.Clamp(newIndex, 0, basicBoxes.Count - 1);
        
        if (((BasicBox)basicBoxes[newIndex]).Stock == null)
        {
            return basicBoxes[newIndex];    
        }

        return basicBoxes.Select((box,index) => new { Box = box, Index = index}).OrderBy(box => Mathf.Abs(box.Index - basicIndex)).FirstOrDefault(box => ((BasicBox)box.Box).Stock == null).Box;
        
        // Attention boucle infini possible si aucune box est dispo cas très rare
    }
    
    #endregion
    
    #region "Tasks"
    
    private void DispatchTasks()
    {
        foreach (IngredientData ingredient in currentWorkRecipe.AllIngredients)
        {
            AIAction ingredientAction = new AIAction(ingredient);
            
            IngredientBox targetIngredientBox = FindObjectsOfType<IngredientBox>().Where(ingredientBox => ingredientBox.Ingredient.GetComponent<Ingredient>().Data == ingredient).ToList()[0];
            AIAction.AITask movementToIngredientBox = new AIAction.AITask(ActualTile,targetIngredientBox.Tile);
            
            ingredientAction.Tasks.Add(movementToIngredientBox);
            
            if (!ingredient.CanBeCook && !ingredient.CanBeCut)
            {
                AIAction.AITask ingredientBoxToStock = new AIAction.AITask(targetIngredientBox.Tile, _plateBox.Tile);
                ingredientAction.Name = " Ingredient " + ingredient.name + "To Stock";
                ingredientAction.Tasks.Add(ingredientBoxToStock);
                ingredientAction.Priority = 1;
            }
            else if (ingredient.CanBeCook)
            {
                Tile cookTile = GetBoxWithType(ingredient.IsPan ? typeof(PanBox) : typeof(StoveBox)).Tile;
                AIAction.AITask movementToCook =  new AIAction.AITask(targetIngredientBox.Tile,cookTile);
                ingredientAction.Name = " Ingredient " + ingredient.name + "To Cook";
                ingredientAction.Priority = 3;
                ingredientAction.Tasks.Add(movementToCook);
            }
            else if (ingredient.CanBeCut)
            {
                Tile cutTile = GetBoxWithType(typeof(CutBox)).Tile;
                AIAction.AITask movementToCutBox = new AIAction.AITask(targetIngredientBox.Tile, cutTile);
                ingredientAction.Name = " Ingredient " + ingredient.name + "To Cut";
                AIAction.AITask cutToStock = new AIAction.AITask(cutTile, _plateBox.Tile);
                ingredientAction.Priority = 2;
                ingredientAction.Tasks.Add(movementToCutBox);
                ingredientAction.Tasks.Add(cutToStock);
            }

            currentRecipeActions.Add(ingredientAction);
        }

        AIAction plateAction = new AIAction(null);
        plateAction.Name = "Plate";
        
        PlateBox plateBox = FindObjectOfType<PlateBox>();
    
        AIAction.AITask movementToPlateBox = new AIAction.AITask(ActualTile, plateBox.Tile);
        plateAction.Tasks.Add(movementToPlateBox);

        AIAction.AITask plateBoxToPlate = new AIAction.AITask(plateBox.Tile, _plateBox.Tile);
        plateAction.Tasks.Add(plateBoxToPlate);
        plateAction.Priority = 4 /* Pending ; replace to 1 */;
    
        currentRecipeActions.Add(plateAction);

        if (currentWorkRecipe.NeedToBeCook)
        {
            AIAction cookAction = new AIAction(null);

            AIAction.AITask movementToOven = new AIAction.AITask(ActualTile, GetBoxWithType(typeof(OvenBox)).Tile);

            cookAction.Name = "Oven Recipe " + currentWorkRecipe.Name;
        
            cookAction.Tasks.Add(movementToOven);
            cookAction.Priority = 0;
        
            currentRecipeActions.Add(cookAction);
        }

        AIAction deliverAction = new AIAction(null,new bool[] {false});
        AIAction.AITask movementToDeliver = new AIAction.AITask(_plateBox.Tile, FindObjectOfType<DeliveryBox>().Tile);

        deliverAction.Name = "Deliver";
        deliverAction.Tasks.Add(movementToDeliver);
        deliverAction.Priority = -1;
    
        currentRecipeActions.Add(deliverAction);
        

        OrderTasks(currentRecipeActions);
    }

    public void OrderTasks(List<AIAction> targetRecipeActions)
    {
        targetRecipeActions = targetRecipeActions.OrderBy(action => action.Priority).ToList();
        targetRecipeActions.Reverse();

      //  OrderByHeuristicDistance();
      //  DeOrderByDifficulty();

      currentRecipeActions = targetRecipeActions;
    }

    private void OrderByHeuristicDistance(List<AIAction> targetRecipeActions)
    {
        List<AIAction> actionPriority = new List<AIAction>();
        List<AIAction> correctOrder = new List<AIAction>();
        bool result = false;
        
        foreach (AIAction action in targetRecipeActions)
        {
            if (actionPriority.Contains(action))
            {
                continue;
            }

            actionPriority = targetRecipeActions.Where(aiAction => aiAction.Priority == action.Priority && !aiAction.IsFinished).ToList();

            if (actionPriority.Count <= 1)
            {
                continue;
            }
            
            correctOrder.Clear();
            result = FindShortestTaskByHd(actionPriority,correctOrder);

            if (!result)
            {
                break;
            }
        }

        if (result)
        {
            SwitchTasks(correctOrder,targetRecipeActions);   
        }
    }

    private bool FindShortestTaskByHd(List<AIAction> actionsPriority,List<AIAction> correctOrder,Tile startTile = null) // HD means Heuristic Distance
    {
        Dictionary<AIAction,int> actionsDistance = new Dictionary<AIAction,int>();

        for (int i = 0; i < actionsPriority.Count; i++)
        {
            AIAction aiAction = actionsPriority[i];
            AIAction.AITask aiTask = aiAction.Tasks.First();

            // Dut au fait que le cook se fin avant d'avoir posé l'assiette                         


            List<Tile> path = _gridManager.GeneratePath(startTile != null ? startTile : aiTask.Start, aiTask.End);

            if (path == null)
            {
                //Debug.Log("error when path is null skip this order task");
                continue;
            }
            
            int distance = path.Count;
            actionsDistance[aiAction] = distance;
        }


        if (actionsDistance.Count == 0)
        {
            return false;
        }
        
        var actionsDistanceList = actionsDistance.ToList();
        actionsDistanceList.Sort((pair1,pair2) => pair1.Value.CompareTo(pair2.Value));

        Tile endTile = actionsDistanceList[0].Key.Tasks[actionsDistanceList[0].Key.Tasks.Count - 1].End;
        
        correctOrder.Add(actionsDistanceList[0].Key);
        actionsPriority.Remove(actionsDistanceList[0].Key);
        
        if (actionsPriority.Count >= 1)
        {
            FindShortestTaskByHd(actionsPriority,correctOrder,endTile);
        }

        return true;

    }

    private void SwitchTasks(List<AIAction> correctOrder,List<AIAction> targetRecipeActions)
    {
        int smallestIndex = targetRecipeActions.IndexOf(targetRecipeActions.Where(aiAction => aiAction == correctOrder[0]).ToList()[0]);

        foreach (AIAction correctAction in correctOrder)
        {
            int newIndex = targetRecipeActions.IndexOf(correctAction);
            
            if (newIndex < smallestIndex)
            {
                smallestIndex = newIndex;
            }
        }

        for (int i = 0; i < correctOrder.Count; i++)
        {
            targetRecipeActions[smallestIndex + i] = correctOrder[i];
        }
    }

    private void DeOrderByDifficulty(List<AIAction> targetRecipeActions)
    {
        float randomDeorder;

        switch (_controller.Difficulty)
        {
            case GameController.Difficulty.EASY:
                randomDeorder = Random.Range(0.65f, 0.85f);
                break;
            
            case GameController.Difficulty.MEDIUM:
                randomDeorder = Random.Range(0.35f, 0.55f);
                break;
            
            case GameController.Difficulty.HARD:
                randomDeorder = Random.Range(0.05f, 0.25f);
                break;
            
            default:
                randomDeorder = Random.Range(0.65f, 0.85f);
                break;
        }

        
        int actionDeorder = Mathf.CeilToInt(targetRecipeActions.Count * randomDeorder);
        actionDeorder = Mathf.Clamp(actionDeorder,0, targetRecipeActions.Count - 1);
 
        for (int i = actionDeorder; i > 0; i--)
        {
            AIAction temp = targetRecipeActions[i];
            int j = Random.Range(0, i);

            while (targetRecipeActions[j] == temp)
            {
                j = Random.Range(0, i);
            }

            targetRecipeActions[i] = targetRecipeActions[j];
            targetRecipeActions[j] = temp;
        }

        ReOrderOvenAndDeliver(targetRecipeActions);
    }

    private void ReOrderOvenAndDeliver(List<AIAction> targetRecipeActions)
    {
        AIAction deliverAction = targetRecipeActions.Where(action => action.Priority == -1).ToList()[0];
        
        if (currentWorkRecipe.NeedToBeCook)
        {
            AIAction ovenAction = targetRecipeActions.Where(action => action.Priority == 0).ToList()[0];
            targetRecipeActions.Remove(ovenAction);
            targetRecipeActions.Insert(targetRecipeActions.Count,ovenAction);    
        }

        targetRecipeActions.Remove(deliverAction);
        targetRecipeActions.Insert(targetRecipeActions.Count,deliverAction);
        
        foreach (AIAction action in targetRecipeActions)
        {
            //Debug.Log("Priority " + action.Priority + " name " + action.ActionOn?.name + " end " + action.Tasks[0].End + " Finished " + action.IsFinished);
        }
    }

    #endregion
}
