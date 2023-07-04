using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Grid;
using Microsoft.VisualStudio.OLE.Interop;
using UnityEditor;
using UnityEngine.AI;
using Random = UnityEngine.Random;
using UnityEngine.UI;

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
    
    
    [SerializeField] private RecipeController.Recipe _currentWorkRecipe;

    public RecipeController.Recipe CurrentWorkRecipe
    {
        get => _currentWorkRecipe;
        private set => _currentWorkRecipe = value;
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
    
    
    private bool _isDoingNothing;

    public bool IsDoingNothing
    {
        get => _isDoingNothing;
        set => _isDoingNothing = value;
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

    /**
     *
     * AJouter le fait que si on a plus de task ou que si deliver on peut commencer une nouvelle recette
     *
     *
     * Vérifier lorsqu'une tache est supprimée de force ( recette disparaiit) , bien refaire les actions autour
     *
     * Repenser tt le systeme de cramage
     *
     * Régler les soucis de path
     * Il se peut que le joueur se retrouve sans task a faire alors que le steak est toujours en train de se cook
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

        for (int i = 0; i < tryCount; i++)
        {
            StartNewRecipe(_team.recipes);   
        }


        _agent = GetComponent<NavMeshAgent>();
        _path = new NavMeshPath();

    }


    private void Update()
    {

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
          else if (CurrentAction == null)
          {
              Debug.Log("wait action");
              
              CurrentAction = CurrentRecipeActions.FirstOrDefault(action => !action.IsFinished && action.Condition.All(val => val));

              if (CurrentAction != null)
              {
                  _controller.AiEvents.OnTaskStarted?.Invoke(this,new AIEvents.OnTaskStartedArgs(CurrentTask,CurrentAction,transform.gameObject,Team));
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
        float doNothing;

        switch (_controller.Difficulty)
        {
            case GameController.Difficulty.EASY:
                doNothing = 0.75f;
                break;
                        
            case GameController.Difficulty.MEDIUM:
                doNothing = 0.55f;
                break;
                        
            case GameController.Difficulty.HARD:
                doNothing = 0.25f;
                break;
                        
            default:
                doNothing = 0.85f;
                break;
        }

        float random = Random.Range(0f, 1f);
        
        Debug.Log("random " + random + " doNoting " + doNothing);
                    
       // _isDoingNothing =  random > doNothing || currentRecipeActions.Where(action => !action.IsFinished).ToList().Count == 1;

       _isDoingNothing = false;
       
        if (!_isDoingNothing)
        { // Ajouter la task 
            Tile end = GetPlateBoxEndTile();
            

            AIAction cookToStock = new AIAction(_currentAction.ActionOn,new bool[] {false}); // Il faut que l'ingrédient soit cuit 
            cookToStock.Name = "CookToStock";
            cookToStock.Priority = 3;
            
            cookToStock.Tasks.Add(new AIAction.AITask(ActualTile,end)); // Faire en sorte de gérer si le steak crame que fait il
            
            currentRecipeActions.Add(cookToStock);
            _controller.AiEvents.OnTaskFinished?.Invoke(this, new AIEvents.OnTaskFinishedArgs(_currentTask,_currentAction,transform.gameObject,_team));
            
            Debug.Log("task finished " + _controller.AiEvents.OnTaskFinished);
            
        }
                    
     
    }

    private void StartNewRecipe(List<RecipeController.Recipe> recipes)
    {
        ChooseRecipe(_team.recipes);
        FindBoxForPlate();
        DispatchTasks();

        Debug.Log("actionName " + currentRecipeActions[0].Name);

        AIEvents.OnChooseRecipeArgs e = new AIEvents.OnChooseRecipeArgs(_currentWorkRecipe, transform.gameObject, currentRecipeActions);

        _controller.AiEvents.OnChooseRecipe?.Invoke(this,e);
        
        if (e.IsCanceled && recipes.Count > 1)
        {
            Debug.Log("canceled choose recipe event");
            List<RecipeController.Recipe> newRecipes = recipes;
            
            if (recipes.Contains(_currentWorkRecipe))
            {
                recipes.Remove(_currentWorkRecipe);
            }
            
            currentRecipeActions.Clear();
            StartNewRecipe(newRecipes);    
        }
        
        Debug.Log("final recipe is " + _currentWorkRecipe.Name);

        _currentAction = currentRecipeActions[0];

        _currentTask = _currentAction.Tasks.First();
        IsMooving = true;
        CanMoove = true;
    }

    #region "Choose Recipe"
    private void ChooseRecipe(List<RecipeController.Recipe> recipes)
    { // Get the "brut" distance between each element to know which recipe AI is going to choose 
        List<int> recipesDistance = new List<int>();
        
        foreach (RecipeController.Recipe recipe in recipes)
        {
            int distance;

            CalculateTheoricDistance(recipe,out distance);

            recipesDistance.Add(distance);
        }

        int minIndex;
        FindShortestRecipe(recipesDistance,out minIndex);

        minIndex = Mathf.Clamp(minIndex, 0, _team.recipes.Count);
        
        _currentWorkRecipe = _team.recipes[minIndex];    
        
        ChooseRecipeDifficulty();
    }

    private void CalculateTheoricDistance(RecipeController.Recipe recipe, out int distance)
    {
        distance = 0;
        
        for (int i = 0; i < recipe.AllIngredients.Count; i++)
        {
            IngredientBox targetIngredientBox = FindObjectsOfType<IngredientBox>().Where(ingredientBox => ingredientBox.Ingredient.GetComponent<Ingredient>().data == recipe.AllIngredients[i]).ToList()[0];
            distance += _gridManager.GeneratePath(ActualTile, targetIngredientBox.Tile).Count;

            if (recipe.AllIngredients[i].isCookable)
            {
                if (recipe.AllIngredients[i].cookIndex == 0)
                {
                    distance += _gridManager.GeneratePath(targetIngredientBox.Tile, GetBoxWithType(typeof(StoveBox)).Tile).Count;
                }
                else if (recipe.AllIngredients[i].cookIndex == 1)
                {
                    distance += _gridManager.GeneratePath(targetIngredientBox.Tile, GetBoxWithType(typeof(PanBox)).Tile).Count;
                }

            }

            if (recipe.AllIngredients[i].isCuttable)
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
        
        Debug.Log("distance for " + _currentWorkRecipe.Name + " " + distance);
        
        return distance;
    }

    public bool CanTakeRecipe(float distance, RecipeController.Recipe currentRecipe)
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
        
        Debug.Log("random " + choose);
        Debug.Log("succeed " + succeed);
        Debug.Log("distance " + (distance * 0.37f));
        
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

    private void ChooseRecipeDifficulty()
    {
        int choose = Random.Range(0, 100);
        int targetChoose = 0;

        switch (_controller.Difficulty)
        {
            case GameController.Difficulty.EASY:
                targetChoose = 25;
                break;
            
            case GameController.Difficulty.MEDIUM:
                targetChoose = 55;
                break;
            
            case GameController.Difficulty.HARD:
                targetChoose = 80;
                break;
            
            default:
                targetChoose = 25;
                break;
        }

        bool succeed = choose > targetChoose;

        succeed = false;
        
        if (succeed)
        {
            if (_team.recipes.Count == 1)
            {
                return;
            }
            
            List<RecipeController.Recipe> newTeamRecipe = _team.recipes;
            _team.recipes.Remove(_currentWorkRecipe);

            ChooseRecipe(newTeamRecipe);
            
        }
    }
    
    #endregion

    #region "Find Plate Box"
    private void FindBoxForPlate()
    {
        int averageX = ActualTile.X;
        int averageY = ActualTile.Y;
        
        foreach (IngredientData ingredient in _currentWorkRecipe.AllIngredients)
        {
            IngredientBox targetIngredientBox = FindObjectsOfType<IngredientBox>().Where(ingredientBox => ingredientBox.Ingredient.GetComponent<Ingredient>().data == ingredient).ToList()[0];
            averageX += targetIngredientBox.Tile.X;
            averageY += targetIngredientBox.Tile.Y;

            if (ingredient.isCuttable)
            {
                CutBox targetCutBox = (CutBox)GetBoxWithType(typeof(CutBox));
                averageX += targetCutBox.Tile.X;
                averageY += targetCutBox.Tile.Y;
            }

            if (ingredient.isCookable)
            {
                if (ingredient.cookIndex == 0)
                {
                    StoveBox targetStoveBox = (StoveBox)GetBoxWithType(typeof(StoveBox));
                    averageX += targetStoveBox.Tile.X;
                    averageY += targetStoveBox.Tile.Y;
                }
                else if (ingredient.cookIndex == 1)
                {
                    PanBox targetPanBox = (PanBox)GetBoxWithType(typeof(PanBox));
                    averageX += targetPanBox.Tile.X;
                    averageY += targetPanBox.Tile.Y;
                }
            }
        }
        
        if (_currentWorkRecipe.NeedToBeCook)
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
        foreach (IngredientData ingredient in _currentWorkRecipe.AllIngredients)
        {
            AIAction ingredientAction = new AIAction(ingredient);
            
            IngredientBox targetIngredientBox = FindObjectsOfType<IngredientBox>().Where(ingredientBox => ingredientBox.Ingredient.GetComponent<Ingredient>().data == ingredient).ToList()[0];
            AIAction.AITask movementToIngredientBox = new AIAction.AITask(ActualTile,targetIngredientBox.Tile);
            
            ingredientAction.Tasks.Add(movementToIngredientBox);
            
            if (!ingredient.isCookable && !ingredient.isCuttable)
            {
                AIAction.AITask ingredientBoxToStock = new AIAction.AITask(targetIngredientBox.Tile, _plateBox.Tile);
                ingredientAction.Name = " Ingredient " + ingredient.name + "To Stock";
                ingredientAction.Tasks.Add(ingredientBoxToStock);
                ingredientAction.Priority = 1;
            }
            else if (ingredient.isCookable)
            {
                Tile cookTile = GetBoxWithType(ingredient.IsPan ? typeof(PanBox) : typeof(StoveBox)).Tile;
                AIAction.AITask movementToCook =  new AIAction.AITask(targetIngredientBox.Tile,cookTile);
                ingredientAction.Name = " Ingredient " + ingredient.name + "To Cook";
                ingredientAction.Priority = 3;
                ingredientAction.Tasks.Add(movementToCook);
            }
            else if (ingredient.isCuttable)
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

        if (_currentWorkRecipe.NeedToBeCook)
        {
            AIAction cookAction = new AIAction(null);

            AIAction.AITask movementToOven = new AIAction.AITask(ActualTile, GetBoxWithType(typeof(OvenBox)).Tile);

            cookAction.Name = "Oven Recipe " + _currentWorkRecipe.Name;
            
            cookAction.Tasks.Add(movementToOven);
            cookAction.Priority = 0;
            
            currentRecipeActions.Add(cookAction);
        }

        AIAction deliverAction = new AIAction(null, new bool[] {false});
        AIAction.AITask movementToDeliver = new AIAction.AITask(_plateBox.Tile, FindObjectOfType<DeliveryBox>().Tile);

        deliverAction.Name = "Deliver";
        deliverAction.Tasks.Add(movementToDeliver);
        deliverAction.Priority = -1;
        
        currentRecipeActions.Add(deliverAction);

        OrderTasks();
    }

    public void OrderTasks()
    {
        currentRecipeActions = currentRecipeActions.OrderBy(action => action.Priority).ToList();
        currentRecipeActions.Reverse();

      //  OrderByHeuristicDistance();
      //  DeOrderByDifficulty();
    }

    private void OrderByHeuristicDistance()
    {
        List<AIAction> actionPriority = new List<AIAction>();
        List<AIAction> correctOrder = new List<AIAction>();
        bool result = false;
        
        foreach (AIAction action in currentRecipeActions)
        {
            if (actionPriority.Contains(action))
            {
                continue;
            }

            actionPriority = currentRecipeActions.Where(aiAction => aiAction.Priority == action.Priority && !aiAction.IsFinished).ToList();

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
            SwitchTasks(correctOrder);   
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

    private void SwitchTasks(List<AIAction> correctOrder)
    {
        int smallestIndex = currentRecipeActions.IndexOf(currentRecipeActions.Where(aiAction => aiAction == correctOrder[0]).ToList()[0]);

        foreach (AIAction correctAction in correctOrder)
        {
            int newIndex = currentRecipeActions.IndexOf(correctAction);
            
            if (newIndex < smallestIndex)
            {
                smallestIndex = newIndex;
            }
        }

        for (int i = 0; i < correctOrder.Count; i++)
        {
            currentRecipeActions[smallestIndex + i] = correctOrder[i];
        }
    }

    private void DeOrderByDifficulty()
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

        
        int actionDeorder = Mathf.CeilToInt(currentRecipeActions.Count * randomDeorder);
        actionDeorder = Mathf.Clamp(actionDeorder,0, currentRecipeActions.Count - 1);
 
        for (int i = actionDeorder; i > 0; i--)
        {
            AIAction temp = currentRecipeActions[i];
            int j = Random.Range(0, i);

            while (currentRecipeActions[j] == temp)
            {
                j = Random.Range(0, i);
            }

            currentRecipeActions[i] = currentRecipeActions[j];
            currentRecipeActions[j] = temp;
        }

        ReOrderOvenAndDeliver();
    }

    private void ReOrderOvenAndDeliver()
    {
        AIAction deliverAction = currentRecipeActions.Where(action => action.Priority == -1).ToList()[0];
        
        if (_currentWorkRecipe.NeedToBeCook)
        {
            AIAction ovenAction = currentRecipeActions.Where(action => action.Priority == 0).ToList()[0];
            currentRecipeActions.Remove(ovenAction);
            currentRecipeActions.Insert(currentRecipeActions.Count,ovenAction);    
        }

        currentRecipeActions.Remove(deliverAction);
        currentRecipeActions.Insert(currentRecipeActions.Count,deliverAction);
        
        foreach (AIAction action in currentRecipeActions)
        {
            //Debug.Log("Priority " + action.Priority + " name " + action.ActionOn?.name + " end " + action.Tasks[0].End + " Finished " + action.IsFinished);
        }
    }

    #endregion
}
