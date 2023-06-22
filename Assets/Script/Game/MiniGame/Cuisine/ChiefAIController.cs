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

    private void Start()
    {
        _gridManager = GridManager.Instance;
        _controller = (CookController)CookController.instance;
         ChooseRecipe(_team.recipes);

         FindBoxForPlate();
         
         // Choose in function of difficulty the priority of action 
    }
    
    #region "Choose Recipe"
    private void ChooseRecipe(List<RecipeController.Recipe> recipes)
    { // Get the "brut" distance between each element to know which recipe AI is going to choose 
        List<int> recipesDistance = new List<int>();
        
        foreach (RecipeController.Recipe recipe in recipes)
        {
            int distance = 0;

            CalculateDistance(recipe,distance,recipesDistance);
            recipesDistance.Add(distance);
        }

        int minIndex;
        FindShortestRecipe(recipesDistance,out minIndex);


        _currentWorkRecipe = _team.recipes[minIndex];
        ManageUIRecipe();

    }

    private void CalculateDistance(RecipeController.Recipe recipe, int distance,List<int> recipesDistance)
    {

        for (int i = 0; i < recipe.allIngredients.Count; i++)
        {
            IngredientBox targetIngredientBox = FindObjectsOfType<IngredientBox>().Where(ingredientBox => ingredientBox.Ingredient.GetComponent<Ingredient>().data == recipe.allIngredients[i]).ToList()[0];
            distance += _gridManager.GeneratePath(ActualTile, targetIngredientBox.Tile).Count;
            
            if (recipe.allIngredients[i].isCookable)
            {
                if (recipe.allIngredients[i].cookIndex == 0)
                {
                    StoveBox targetStoveBox =
                        FindObjectsOfType<StoveBox>().Where(stoveBox => stoveBox.Stock == null).ToList()[0];

                    distance += _gridManager.GeneratePath(targetIngredientBox.Tile, targetStoveBox.Tile).Count;
                }
                else if (recipe.allIngredients[i].cookIndex == 1)
                {
                    PanBox targetPanBox = FindObjectsOfType<PanBox>().Where(panBox => panBox.Stock == null).ToList()[0];
                    distance += _gridManager.GeneratePath(targetIngredientBox.Tile, targetPanBox.Tile).Count;
                }

            }

            if (recipe.allIngredients[i].isCuttable)
            {
                CutBox targetCutBox = FindObjectsOfType<CutBox>().Where(cutBox => cutBox.Stock == null).ToList()[0];
                distance += _gridManager.GeneratePath(targetIngredientBox.Tile, targetCutBox.Tile).Count;
            }
        }



        if (recipe.needToBeCook)
        {
            OvenBox targetOvenBox = FindObjectsOfType<OvenBox>().Where(ovenBox => ovenBox.Stock == null).ToList()[0];
            distance += _gridManager.GeneratePath(ActualTile, targetOvenBox.Tile).Count;

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
                CutBox targetCutBox = FindObjectsOfType<CutBox>().Where(cutBox => cutBox.Stock == null).ToList()[0];
                averageX += targetCutBox.Tile.X;
                averageY += targetCutBox.Tile.Y;
            }

            if (ingredient.isCookable)
            {
                if (ingredient.cookIndex == 0)
                {
                    StoveBox targetStoveBox = FindObjectsOfType<StoveBox>().Where(stoveBox => stoveBox.Stock == null).ToList()[0];
                    averageX += targetStoveBox.Tile.X;
                    averageY += targetStoveBox.Tile.Y;
                }
                else if (ingredient.cookIndex == 1)
                {
                    PanBox targetPanBox = FindObjectsOfType<PanBox>().Where(panBox => panBox.Stock == null).ToList()[0];
                    averageX += targetPanBox.Tile.X;
                    averageY += targetPanBox.Tile.Y;
                }
            }
        }
        
        if (_currentWorkRecipe.needToBeCook)
        {
            OvenBox targetOvenBox = FindObjectsOfType<OvenBox>().Where(ovenBox => ovenBox.Stock == null).ToList()[0];
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
        
        Debug.Log("brut coords : [" + averageX + "," + averageY + "]");
        Vector2 brutCoords = new Vector2(averageX, averageY);

        BasicBox nearestBox = FindNearestBox(brutCoords);
        
        _plateBox = ApplyDifficultyOffset(nearestBox);
        
        
        Debug.Log("nearest box " + nearestBox);
        Debug.Log("final plate box " + _plateBox);
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
        
        Debug.Log("offset " + randomOffset);
        Debug.Log("newIndex " + newIndex);
        
        return basicBoxes[newIndex];
    }
    
    #endregion
    
    private void DispatchTask()
    {
        foreach (IngredientData ingredient in _currentWorkRecipe.allIngredients)
        {
            AIAction ingredientAction = new AIAction(ingredient);
            
            // Movement to ingredient box 
            IngredientBox targetIngredientBox = FindObjectsOfType<IngredientBox>().Where(ingredientBox => ingredientBox.Ingredient.GetComponent<Ingredient>().data == ingredient).ToList()[0];
            
            AIAction.AITask movementToIngredientBox = new AIAction.AITask(ActualTile,targetIngredientBox.Tile);

            if (!ingredient.isCookable && !ingredient.isCuttable)
            {
                
            }
            else if (ingredient.isCookable)
            {
                
            }
            else if (ingredient.isCuttable)
            {
                
            }


            ingredientAction.Tasks.Add(movementToIngredientBox);
        }
    }
}
