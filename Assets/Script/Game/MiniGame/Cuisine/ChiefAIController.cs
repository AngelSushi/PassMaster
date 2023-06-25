using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Grid;
using Microsoft.VisualStudio.OLE.Interop;
using Random = UnityEngine.Random;
using UnityEngine.UI;

public class ChiefAIController : MonoBehaviour
{
    private CookController.Team _team;

    public CookController.Team Team
    {
        get => _team;
        set => _team = value;
    }
    
    
    private GridManager _gridManager;

    private Tile _actualTile; 
    
    public Tile ActualTile
    {
        get
        {
            RaycastHit tileRaycast = Physics.RaycastAll(transform.position, Vector3.down, 5, LayerMask.GetMask("Grid"))[0];

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

    
    private RecipeController.Recipe _currentWorkRecipe;
    private CookController _controller;

    private BasicBox _plateBox;

    [SerializeField] private List<BasicBox> basicBoxes;

    private List<BasicBox> _emptyBoxes = new List<BasicBox>();

    [SerializeField] private List<AIAction> currentRecipeActions = new List<AIAction>();

    private void Start()
    {
        _gridManager = GridManager.Instance;
        _controller = (CookController)CookController.instance;
        
        _controller.Events.OnUpdateBoxStock += OnUpdateBoxStock;

        foreach (BasicBox box in FindObjectsOfType<BasicBox>())
        {
            _emptyBoxes.Add(box);
        }
        
      
        ChooseRecipe(_team.recipes);
        FindBoxForPlate();
        DispatchTasks();
    }

    private void OnDestroy() => _controller.Events.OnUpdateBoxStock -= OnUpdateBoxStock;

    #region "Events"
    
    private void OnUpdateBoxStock(object sender,CookEvents.OnUpdateBoxStockArgs e)
    {
        if (e.stock == null)
        {
            if (!_emptyBoxes.Contains(e.box))
            {
                _emptyBoxes.Add(e.box);
            }
        }
        else
        {
            _emptyBoxes.Remove(e.box);
        }
    }
    
    #endregion

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
    
    #endregion

    #region "Choose Recipe"
    private void ChooseRecipe(List<RecipeController.Recipe> recipes)
    { // Get the "brut" distance between each element to know which recipe AI is going to choose 
        List<int> recipesDistance = new List<int>();
        
        foreach (RecipeController.Recipe recipe in recipes)
        {
            int distance;

            CalculateDistance(recipe,out distance,recipesDistance);

            recipesDistance.Add(distance);
        }

        int minIndex;
        FindShortestRecipe(recipesDistance,out minIndex);
        
        _currentWorkRecipe = _team.recipes[minIndex];    
        
        Debug.Log("work recipe " + _currentWorkRecipe.name);
        
         // + Ajouter par difficulté si difficile plus de chance de prendre la bonne sinon non 
        ManageUIRecipe();

    }

    private void CalculateDistance(RecipeController.Recipe recipe, out int distance,List<int> recipesDistance)
    {
        distance = 0;
        
        for (int i = 0; i < recipe.allIngredients.Count; i++)
        {
            IngredientBox targetIngredientBox = FindObjectsOfType<IngredientBox>().Where(ingredientBox => ingredientBox.Ingredient.GetComponent<Ingredient>().data == recipe.allIngredients[i]).ToList()[0];
            distance += _gridManager.GeneratePath(ActualTile, targetIngredientBox.Tile).Count;

            if (recipe.allIngredients[i].isCookable)
            {
                if (recipe.allIngredients[i].cookIndex == 0)
                {
                    distance += _gridManager.GeneratePath(targetIngredientBox.Tile, GetBoxWithType(typeof(StoveBox)).Tile).Count;
                }
                else if (recipe.allIngredients[i].cookIndex == 1)
                {
                    distance += _gridManager.GeneratePath(targetIngredientBox.Tile, GetBoxWithType(typeof(PanBox)).Tile).Count;
                }

            }

            if (recipe.allIngredients[i].isCuttable)
            {
                distance += _gridManager.GeneratePath(targetIngredientBox.Tile, GetBoxWithType(typeof(CutBox)).Tile).Count;
            }
        }



        if (recipe.needToBeCook)
        {
            distance += _gridManager.GeneratePath(ActualTile, GetBoxWithType(typeof(OvenBox)).Tile).Count;

        }
    }

    private void FindShortestRecipe(List<int> recipesDistance,out int shortestIndex)
    { // Ajouter le facteur temps
        int minDistance = recipesDistance[0];
        shortestIndex = 0;
        
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

    private void ManageUIRecipe()
    {
        GameObject targetPlayer =  _controller.teams.Where(team => team.players.Contains(transform.gameObject)).ToList()[0].players.Where(player => player.name.Equals(transform.gameObject.name.Replace("CPU",""))).ToList()[0];
        Player player = _controller.players.Where(player => player.gameObject.name.Equals(targetPlayer.name.Replace("CPU",""))).ToList()[0];
        
        GameObject slot = _currentWorkRecipe.recipeUI.transform.GetChild(3).gameObject;
        
        if(slot.GetComponent<Image>().sprite != null)
        {
            slot = _currentWorkRecipe.recipeUI.transform.GetChild(4).gameObject;
        }

        slot.GetComponent<Image>().sprite = player.uiIcon;
        slot.transform.GetChild(0).gameObject.SetActive(true);
        slot.SetActive(true);
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

        if (choose <= targetChoose)
        {
            List<RecipeController.Recipe> newTeamRecipe = _team.recipes;
            _team.recipes.Remove(_currentWorkRecipe);
            ChooseRecipe(newTeamRecipe);
            
            _currentWorkRecipe = null;
        }
    }
    
    #endregion

    #region "Find Plate Box"
    private void FindBoxForPlate()
    {
        int averageX = ActualTile.X;
        int averageY = ActualTile.Y;
        
        foreach (IngredientData ingredient in _currentWorkRecipe.allIngredients)
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
        
        if (_currentWorkRecipe.needToBeCook)
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

        BasicBox nearestBox = FindNearestBox(brutCoords);
        
        _plateBox = ApplyDifficultyOffset(nearestBox);
    }

    private BasicBox FindNearestBox(Vector2 brutCoords)
    {
        BasicBox minBox = basicBoxes[0];
        float minDistance = Vector2.Distance(brutCoords, minBox.Tile.Coords);

        foreach (BasicBox basicBox in basicBoxes)
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

    private BasicBox ApplyDifficultyOffset(BasicBox basicBox)
    {
        int offset;
        
        switch (_controller.Difficulty)
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

        int randomOffset = Random.Range(-offset, offset);
        
        int basicIndex = basicBoxes.IndexOf(basicBox);
        int newIndex = basicIndex + randomOffset;
        
        newIndex = Mathf.Clamp(newIndex, 0, basicBoxes.Count);
        
        return basicBoxes[newIndex];
    }
    
    #endregion
    
    #region "Tasks"
    
    private void DispatchTasks()
    {
        foreach (IngredientData ingredient in _currentWorkRecipe.allIngredients)
        {
            AIAction ingredientAction = new AIAction(ingredient);
            
            IngredientBox targetIngredientBox = FindObjectsOfType<IngredientBox>().Where(ingredientBox => ingredientBox.Ingredient.GetComponent<Ingredient>().data == ingredient).ToList()[0];
            
            AIAction.AITask movementToIngredientBox = new AIAction.AITask(ActualTile,targetIngredientBox.Tile);
            ingredientAction.Tasks.Add(movementToIngredientBox);
            Tile startStockTile = targetIngredientBox.Tile;
            
            if (!ingredient.isCookable && !ingredient.isCuttable)
            {
                ingredientAction.Priority = 1;
            }
            else if (ingredient.isCookable)
            {
                AIAction.AITask movementToCook =  new AIAction.AITask(targetIngredientBox.Tile,GetBoxWithType(ingredient.IsPan ? typeof(PanBox) : typeof(StoveBox)).Tile);
                startStockTile = movementToCook.End;
                ingredientAction.Priority = 3;
                ingredientAction.Tasks.Add(movementToCook);
            }
            else if (ingredient.isCuttable)
            {
                AIAction.AITask movementToCutBox = new AIAction.AITask(targetIngredientBox.Tile, GetBoxWithType(typeof(CutBox)).Tile);
                startStockTile = movementToCutBox.End;
                ingredientAction.Priority = 2;
                ingredientAction.Tasks.Add(movementToCutBox);
            }

            GenerateActionToStockBox(ingredientAction, startStockTile);
            currentRecipeActions.Add(ingredientAction);
            
        }

        AIAction plateAction = new AIAction(null);

        PlateBox plateBox = FindObjectOfType<PlateBox>();
        
        AIAction.AITask movementToPlateBox = new AIAction.AITask(ActualTile, plateBox.Tile);
        plateAction.Tasks.Add(movementToPlateBox);

        AIAction.AITask plateBoxToPlate = new AIAction.AITask(plateBox.Tile, _plateBox.Tile);
        plateAction.Tasks.Add(plateBoxToPlate);
        plateAction.Priority = 4 /* Pending ; replace to 1 */;
        
        currentRecipeActions.Add(plateAction);

        if (_currentWorkRecipe.needToBeCook)
        {
            AIAction cookAction = new AIAction(null);

            AIAction.AITask movementToOven = new AIAction.AITask(ActualTile, GetBoxWithType(typeof(OvenBox)).Tile);
            
            cookAction.Tasks.Add(movementToOven);
            cookAction.Priority = 0;
            
            currentRecipeActions.Add(cookAction);
        }

        AIAction deliverAction = new AIAction(null);
        AIAction.AITask movementToDeliver = new AIAction.AITask(ActualTile, FindObjectOfType<DeliveryBox>().Tile);
        
        deliverAction.Tasks.Add(movementToDeliver);
        deliverAction.Priority = -1;
        
        currentRecipeActions.Add(deliverAction);

        OrderTasks();
        // Oven ; Deliver
    }

    private void OrderTasks()
    {
        currentRecipeActions = currentRecipeActions.OrderBy(action => action.Priority).ToList();
        currentRecipeActions.Reverse();

        OrderByHeuristicDistance();
        DeOrderByDifficulty();
    }

    private void OrderByHeuristicDistance()
    {
        List<AIAction> actionPriority = new List<AIAction>();
        
        foreach (AIAction action in currentRecipeActions)
        {
            if (actionPriority.Contains(action))
            {
                continue;
            }

            actionPriority = currentRecipeActions.Where(aiAction => aiAction.Priority == action.Priority).ToList();

            if (actionPriority.Count <= 1)
            {
                continue;
            }
            
            Debug.Log("Priority " + action.Priority);

            Dictionary<AIAction,int> actionsDistance = new Dictionary<AIAction,int>();
            
            for (int i = 0; i < actionPriority.Count; i++)
            {
                AIAction aiAction = actionPriority[i];
                AIAction.AITask aiTask = aiAction.Tasks.First();

                int distance = _gridManager.GeneratePath(aiTask.Start, aiTask.End).Count;
                actionsDistance[aiAction] = distance;
                // Distance entre mon joueur et l'ingredientbox
                
                Debug.Log("start " + aiTask.Start.Coords +  " value " + aiTask.End.Coords + " distance " + distance);
                

                // break;

            }
            Debug.Log("nearest tile " + actionsDistance.Keys.ToList()[0].Tasks[0].End.Coords);

            var actionsDistanceList = actionsDistance.ToList();
            actionsDistanceList.Sort((pair1,pair2) => pair1.Value.CompareTo(pair2.Value));
            
            
            
            Debug.Log("nearest tile 2 " + actionsDistanceList[0].Key.Tasks[0].End.Coords);
            
            


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

        foreach (AIAction action in currentRecipeActions)
        {
           // Debug.Log("Priority " + action.Priority);
        }
        
        
        int actionDeorder = Mathf.CeilToInt(currentRecipeActions.Count * randomDeorder);
        actionDeorder = Mathf.Clamp(actionDeorder,0, currentRecipeActions.Count - 1);
        //Debug.Log("----------------------------------------");
        
        
        
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

        //Debug.Log("------------------- FINAL VERSION ------------------------");
        ReOrderOvenAndDeliver();
    }

    private void ReOrderOvenAndDeliver()
    {
        AIAction deliverAction = currentRecipeActions.Where(action => action.Priority == -1).ToList()[0];
        
        if (_currentWorkRecipe.needToBeCook)
        {
            AIAction ovenAction = currentRecipeActions.Where(action => action.Priority == 0).ToList()[0];
            currentRecipeActions.Remove(ovenAction);
            currentRecipeActions.Insert(currentRecipeActions.Count,ovenAction);    
        }

        currentRecipeActions.Remove(deliverAction);
        currentRecipeActions.Insert(currentRecipeActions.Count,deliverAction);
        
        foreach (AIAction action in currentRecipeActions)
        {
           // Debug.Log("Priority " + action.Priority);
        }
    }

    private void GenerateActionToStockBox(AIAction action,Tile startTile)
    {
        if (_plateBox.Stock != null && _plateBox.Stock.GetComponent<Plate>() == null)
        {
            int amplifier = 1;
            int randomIndex = Random.Range(-amplifier, amplifier);

            int currentIndex = basicBoxes.IndexOf(_plateBox);
            int newIndex = currentIndex + randomIndex;
            newIndex = (int)Mathf.Lerp(newIndex, 0, basicBoxes.Count);

            while (basicBoxes[newIndex].Stock != null)
            {
                amplifier++;
                randomIndex = Random.Range(-amplifier, amplifier);
                        
                newIndex = currentIndex + randomIndex;
                newIndex = (int)Mathf.Lerp(newIndex, 0, basicBoxes.Count);
            }

            BasicBox stockBox = basicBoxes[newIndex]; // Va avoir une erreur si deux ia genere en mm temps la même basicbox ==> faire un check au moment ou on pose
            AIAction.AITask movementToStockBox = new AIAction.AITask(startTile, stockBox.Tile);
            action.Priority = 1;
            action.Tasks.Add(movementToStockBox);

            // Au moment ou l'ia va faire l'action pr aller chercher la plate que nous allors ajouter tt les actions pour ramener les ingrédients sur les basic box vers la plate
                    
                    
        }
        else
        {
            AIAction.AITask movementToPlate = new AIAction.AITask(startTile, _plateBox.Tile);
            action.Tasks.Add(movementToPlate);    
        }
    }
    
    #endregion
}
