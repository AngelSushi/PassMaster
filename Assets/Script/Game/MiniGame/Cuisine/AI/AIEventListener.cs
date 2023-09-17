using System.Collections.Generic;
using System.Linq;
using Grid;
using UnityEngine;
using Recipes;

public class AIEventListener : CoroutineSystem
{
    private ChiefAIController _aiController;
    private CookController _controller;
    private GridManager _gridManager;

    private void Start()
    {
        _aiController = GetComponent<ChiefAIController>();
        _controller = (CookController)CookController.instance;
        _gridManager = GridManager.Instance;

        _controller.CookEvents.OnUpdateBoxStock += OnUpdateBoxStock;
        _controller.CookEvents.OnFinishedCutIngredient += OnFinishedCutIngredient;
        _controller.CookEvents.OnFinishedCookIngredient += OnFinishedCookIngredient;
        _controller.CookEvents.OnPutIngredientInPlate += OnPutIngredientInPlate;
        _controller.AiEvents.OnTaskStarted += OnTaskStarted;
        _controller.AiEvents.OnTaskFinished += OnTaskFinished;
        _controller.AiEvents.OnTaskReachStart += OnTaskReachStart;
        _controller.AiEvents.OnTaskReachEnd += OnTaskReachEnd;
        _controller.AiEvents.OnActionFinished += OnActionFinished;
        _controller.AiEvents.OnChooseRecipe += OnChooseRecipe;

        _controller.RecipeEvents.OnRecipeEndTimer += OnRecipeEndTimer;
        _controller.RecipeEvents.OnRecipeDelivered += OnRecipeDelivered;

    }

    private void OnDestroy()
    {
        _controller.CookEvents.OnUpdateBoxStock -= OnUpdateBoxStock;
        _controller.CookEvents.OnFinishedCutIngredient -= OnFinishedCutIngredient;
        _controller.CookEvents.OnFinishedCookIngredient -= OnFinishedCookIngredient;
        _controller.CookEvents.OnPutIngredientInPlate -= OnPutIngredientInPlate;
        _controller.AiEvents.OnTaskStarted -= OnTaskStarted;
        _controller.AiEvents.OnTaskFinished -= OnTaskFinished;
        _controller.AiEvents.OnTaskReachStart -= OnTaskReachStart;
        _controller.AiEvents.OnTaskReachEnd -= OnTaskReachEnd;
        _controller.AiEvents.OnActionFinished -= OnActionFinished;
        _controller.AiEvents.OnChooseRecipe -= OnChooseRecipe;

        _controller.RecipeEvents.OnRecipeEndTimer -= OnRecipeEndTimer;
        _controller.RecipeEvents.OnRecipeDelivered -= OnRecipeDelivered;
    }

    private void OnRecipeEndTimer(object sender, RecipeEvents.OnRecipeEndTimerArgs e)
    {
        if (e.Team == _aiController.Team)
        {
            GenerateBinActions();
        }
    }

    private void OnRecipeDelivered(object sender, RecipeEvents.OnRecipeDeliveredArgs e)
    {
        if (e.Player == transform.gameObject)
        {
            _aiController.CurrentRecipeActions.Clear();
            _aiController.StartNewRecipe(_aiController.Team.recipes);
        }
    }

    private void OnChooseRecipe(object sender, AIEvents.OnChooseRecipeArgs e)
    {
        if (e.AI == transform.gameObject)
        {
            e.IsCanceled = !_aiController.CanTakeRecipe(_aiController.CalculateRecipeDistance(e.RecipeActions), e.Recipe);
        }
    }

    private void OnUpdateBoxStock(object sender,CookEvents.OnUpdateBoxStockArgs e)
    {
        if (e.Box.currentController.gameObject == transform.gameObject)
        {
            if (e.Stock == null)
            { // Lorsqu'on prend de la box

                if (!_aiController.EmptyBoxes.Contains(e.Box))
                {
                    _aiController.EmptyBoxes.Add(e.Box);
                }

                if (e.Box is StoveBox || e.Box is PanBox)
                {
                    if (_aiController.ActualIngredient != null)
                    {
                        Ingredient ingredient = _aiController.ActualIngredient.GetComponent<Ingredient>();

                        if (ingredient.Data.CanBeBurn && ingredient.IsBurn)
                        {
                            _aiController.CurrentAction.Tasks.First(task => !task.IsFinished).End = FindObjectOfType<BinBox>().Tile;

                            Debug.Log("change is Finished ");
                            
                            foreach (AIAction action in _aiController.CurrentRecipeActions.Where(aiAction => aiAction.ActionOn == ingredient.Data))
                            {
                                action.IsFinished = false;
                                action.Tasks.ForEach(task => task.IsFinished = false);
                            }
                            
                        }
                    }
                }

            }
            else
            { // Lorsqu'on pose sur la box
                _aiController.EmptyBoxes.Remove(e.Box);
                CreateStockToPlateTask(e);
            }
        }
    }

    private void OnPutIngredientInPlate(object sender, CookEvents.OnPutIngredientInPlateArgs e)
    {
        if (e.From.gameObject == transform.gameObject)
        {
            if (e.IsFull)
            {
                AIAction deliverAction = _aiController.CurrentRecipeActions.Where(aiAction => aiAction.Name.Equals("Deliver") && aiAction.Tasks[0].Start.Coords == _aiController.PlateCoords).FirstOrDefault();

                if (deliverAction != null)
                {
                    Plate plate = e.Box.Stock.GetComponent<Plate>();
                    
                    if (plate != null)
                    {
                        deliverAction.Condition[0] = plate.fullRecipe  != null && plate.fullRecipe.Name.Equals(_aiController.CurrentWorkRecipe.Name);
                    }
                }
            }
        } 
    }
    

    private void OnTaskReachStart(object sender, AIEvents.OnTaskReachStartArgs e)
    {
        if (e.AI == transform.gameObject)
        {
            _aiController.GoToStart = false;
            
            if (e.Tile.AttachedBox != null)
            {
                e.Tile.AttachedBox.BoxInteract(_aiController.ActualIngredient != null ? _aiController.ActualIngredient : _aiController.ActualPlate != null ? _aiController.ActualPlate : null, _aiController);   
            }

            if (_aiController.PlateBox.Stock != null)
            {
                if (_aiController.PlateBox.Stock.GetComponent<Plate>() != null)
                {
                    if (_aiController.CurrentTask.End.Coords != _aiController.PlateCoords && _aiController.CurrentTask.End.Coords != FindObjectOfType<BinBox>().Tile.Coords)
                    {
                        _aiController.CurrentTask.End = _aiController.PlateBox.Tile;
                    } 
                }
                else
                {
                    BasicBox attachedBasicBox = (BasicBox)_aiController.CurrentTask.End.AttachedBox;
                    
                    if (attachedBasicBox.Stock != null)
                    {
                        _aiController.CurrentTask.End = _gridManager.FromCoords(_aiController.ApplyDifficultyOffset(attachedBasicBox).TileCoords);
                    }
                }
                
            }
            
        }
    }

    private void OnTaskReachEnd(object sender, AIEvents.OnTaskReachEndArgs e)
    {
        if (e.AI == transform.gameObject)
        {
            _aiController.IsMooving = false;

            RunDelayed(0.3f, () =>
            {
                e.Tile.AttachedBox.BoxInteract(_aiController.ActualIngredient != null ? _aiController.ActualIngredient : _aiController.ActualPlate != null ? _aiController.ActualPlate : null, _aiController);

                if (e.Tile.AttachedBox is not CutBox && e.Tile.AttachedBox is not PanBox && e.Tile.AttachedBox is not StoveBox && e.Tile.AttachedBox is not OvenBox) 
                {
                    _controller.AiEvents.OnTaskFinished?.Invoke(this, new AIEvents.OnTaskFinishedArgs(e.Task, e.Action, e.AI, e.Team));
                }
                else if (e.Tile.AttachedBox is CutBox)
                {
                    e.Tile.AttachedBox.BoxInteract(_aiController.ActualIngredient != null ? _aiController.ActualIngredient : _aiController.ActualPlate != null ? _aiController.ActualPlate : null, _aiController);
                }
                else if (e.Tile.AttachedBox is OvenBox || e.Tile.AttachedBox is StoveBox || e.Tile.AttachedBox is PanBox)
                {
                    _aiController.ActionDuringTimer();
                }
            });


        }
    }

    private void OnTaskStarted(object sender, AIEvents.OnTaskStartedArgs e)
    {
        if (e.AI == transform.gameObject)
        {
            Box attachedEndBox = e.Task.End.AttachedBox;
            Box attachedStartBox = e.Task.Start.AttachedBox;
            
            if (attachedEndBox is DeliveryBox)
            {
                if (attachedStartBox == null)
                {
                    attachedStartBox = _aiController.PlateBox;
                    e.Task.Start = _aiController.PlateBox.Tile;
                }
                
                attachedStartBox.BoxInteract(_aiController.ActualIngredient != null ? _aiController.ActualIngredient : _aiController.ActualPlate != null ? _aiController.ActualPlate : null,_aiController);
            }
        }
    }

    private void OnTaskFinished(object sender, AIEvents.OnTaskFinishedArgs e)
    {
        if (e.AI == transform.gameObject)
        {
            AIAction.AITask currentTask = _aiController.CurrentTask;
            
            
            RunDelayed(0.3f, () =>
            {
                currentTask.IsFinished = true;
                _aiController.CurrentTask = _aiController.CurrentAction.Tasks.FirstOrDefault(aiTask => !aiTask.IsFinished);
                
                if (_aiController.CurrentTask == null)
                {
                    _controller.AiEvents.OnActionFinished?.Invoke(this,new AIEvents.OnActionFinishedArgs(e.AI,e.Action,_aiController.CurrentWorkRecipe));
                }
                else
                {
                    _controller.AiEvents.OnTaskStarted?.Invoke(this,new AIEvents.OnTaskStartedArgs(_aiController.CurrentTask,_aiController.CurrentAction,transform.gameObject,_aiController.Team));
                    CheckAndChangeTaskEnd();
                }

                _aiController.IsMooving = true;
            });
        }
    }

    private void OnFinishedCutIngredient(object sender, CookEvents.OnFinishedCutIngredientArgs e)
    {
        if (e.Cutter == transform.gameObject && e.Box == _aiController.ActualTile.AttachedBox)
        {
            e.Box.BoxInteract(_aiController.ActualIngredient != null ? _aiController.ActualIngredient : _aiController.ActualPlate != null ? _aiController.ActualPlate : null, _aiController);
            _controller.AiEvents.OnTaskFinished?.Invoke(this,new AIEvents.OnTaskFinishedArgs(_aiController.CurrentTask,_aiController.CurrentAction,transform.gameObject,_aiController.Team));
        }
    }

    private void OnFinishedCookIngredient(object sender, CookEvents.OnFinishedCookIngredientArgs e)
    {
        if (e.Cooker == transform.gameObject)
        {
            AIAction cookToStock = _aiController.CurrentRecipeActions.Where(aiAction => aiAction.Tasks.Contains(aiAction.Tasks.Where(task => task.Start == e.Box.Tile).FirstOrDefault())).FirstOrDefault();

            if (cookToStock != null)
            {
                cookToStock.Condition[0] = true;
            }
        }
    }

    private void OnActionFinished(object sender, AIEvents.OnActionFinishedArgs e)
    {
        if (e.AI == transform.gameObject)
        {
            _aiController.CurrentAction.IsFinished = true;
            AssignNewAction();
        }
    }

    private void CheckAndChangeTaskEnd()
    {
        Box attachedBox = _aiController.CurrentTask.End.AttachedBox;

        if (attachedBox is not CutBox && attachedBox is not OvenBox && attachedBox is not PanBox && attachedBox is not IngredientBox && attachedBox is not PlateBox && attachedBox is not StoveBox && attachedBox is not DeliveryBox)
        { // Box is a stock box 
            BasicBox box = (BasicBox)attachedBox;
            BasicBox plate = _aiController.PlateBox;
            
            if (plate.Stock != null)
            {
                if (plate.Stock.GetComponent<Plate>() == null)
                {
                    _aiController.CurrentTask.End = _gridManager.FromCoords(_aiController.ApplyDifficultyOffset((BasicBox)attachedBox).TileCoords); // Attention a vérifier si la box est vide    
                }
                else
                {
                    _aiController.CurrentTask.End = _aiController.PlateBox.Tile;
                }
                            
                            
            }
        }
    }

    private void CreateStockToPlateTask(CookEvents.OnUpdateBoxStockArgs e)
    {
        if (e.Stock.TryGetComponent(out Plate plate))
        { // Is a plate

            _aiController.PlateBox = e.Box;
            _aiController.PlateCoords = _aiController.PlateBox.TileCoords;

            AIAction deliverAction = _aiController.CurrentRecipeActions.Where(aiAction => aiAction.Name.Equals("Deliver")).FirstOrDefault(); // Ajouter le nom de la recipe pour diférienier les différents Deliver

            if (deliverAction != null)
            {
                deliverAction.Tasks[0].Start = _aiController.PlateBox.Tile;
            }
                
            if (plate.AvailableRecipes.Contains(_aiController.CurrentWorkRecipe) || plate.AvailableRecipes.Count == 0)
            {
                List<Box> fullBoxes = _aiController.BasicBoxes.Where(box => ((BasicBox)box).Stock != null).ToList();

                foreach (Box box in fullBoxes)
                {
                    BasicBox bBox = (BasicBox)box;

                    if (bBox.Stock.TryGetComponent(out Ingredient ingredient) && bBox.TileCoords != _aiController.PlateCoords)
                    {
                        bool isActionExist = _aiController.CurrentRecipeActions.Where(aiAction => aiAction.Name.Equals("StockToBoxPlate") && aiAction.ActionOn == ingredient.Data).FirstOrDefault() != null;

                        if (_aiController.CurrentWorkRecipe.AllIngredients.Contains(ingredient.Data) && !plate.ingredientsInPlate.Contains(ingredient) && !isActionExist) // erreur prise de plate non finie 
                        {
                            AIAction stockToBoxPlate = new AIAction(ingredient.Data);
                            stockToBoxPlate.Name = "StockToBoxPlate";
                            stockToBoxPlate.Priority = 1;

                            AIAction.AITask task = new AIAction.AITask(bBox.Tile,e.Box.Tile);
                            stockToBoxPlate.Tasks.Add(task);

                            _aiController.CurrentRecipeActions.Add(stockToBoxPlate);
                                
                            _aiController.OrderTasks(_aiController.CurrentRecipeActions);
                        }
                    }
                }

            }
        }
    }


    public void GenerateBinActions()
    {
        List<AIAction> binActions = new List<AIAction>();
            
        foreach (AIAction action in _aiController.CurrentRecipeActions)
        {
            Tile endTask = action.Tasks.Last().End;

            if (endTask.AttachedBox is BasicBox)
            {
                
                BasicBox attachedEndBox = (BasicBox)endTask.AttachedBox;

                if (attachedEndBox.Stock == null)
                {
                    continue;
                }
                
                AIAction boxToBin = new AIAction(action.ActionOn);
                boxToBin.Priority = 5; // Faire en sorte que les priorités 5 ne sont pas réorganisables
                boxToBin.Name = "BoxToBin";
                boxToBin.Tasks.Add(new AIAction.AITask(endTask,FindObjectOfType<BinBox>().Tile));
                binActions.Add(boxToBin);
            }
        } 
        
        Debug.Log("add bin actions " + _aiController.CurrentRecipeActions.Count);

        _aiController.CurrentRecipeActions.RemoveAll(aiAction => !binActions.Contains(aiAction));
        _aiController.CurrentRecipeActions.AddRange(binActions);
        
        Debug.Log("new length " + _aiController.CurrentRecipeActions.Count);
        
        _aiController.OrderTasks(_aiController.CurrentRecipeActions);
        AssignNewAction();
    }

    private void AssignNewAction()
    {
        _aiController.CurrentAction = _aiController.CurrentRecipeActions.FirstOrDefault(action => !action.IsFinished && action.Condition.All(val => val));

        if (_aiController.CurrentAction == null)
        {
            //Debug.Log("recipe delivered");
            //_controller.RecipeEvents.OnRecipeDelivered?.Invoke(this,new RecipeEvents.OnRecipeDeliveredArgs(_aiController.CurrentWorkRecipe,_aiController.Team,transform.gameObject));
            return;
        }
            
        _aiController.CurrentTask = _aiController.CurrentAction.Tasks.First();
        _controller.AiEvents.OnTaskStarted?.Invoke(this,new AIEvents.OnTaskStartedArgs(_aiController.CurrentTask,_aiController.CurrentAction,transform.gameObject,_aiController.Team));

        Debug.Log("new Action " + _aiController.CurrentAction.Name + " on " + _aiController.CurrentAction.ActionOn);

        _aiController.CheckGoToStart();
    }

}