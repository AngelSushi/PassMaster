using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Grid;
using Microsoft.VisualStudio.OLE.Interop;
using Random = UnityEngine.Random;

public class ChiefAIController : MonoBehaviour
{
    
    
    /** AI LIST
     *
     *  Begin :
     * At the begin of the begin game and when the ai must choose a new recipe it check the less costable recipe per distance 
     *
     * Générer un chemin qui selon la difficulté priorise les bonnes choses ou non
     *
     * Comment générer un chemin ?
     * Selon la difficulté il priorise ou non l'action
     * + ca prend les éléments le plus proche de lui en fonction de sa position
     * Prendre aussi en compte la distance a laquelle il est ?
     *
     * + Faire un mélange entre tt les positions ou faut aller et faire une moyenne pr voir la position ou faut mettre l'assiette
     */

    
    
    
    public enum ChiefState
    {
        MOVEMENT,
        CUT,
        COOK,
        BRING
    }


    private ChiefState _actualState;

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
    private CookController _cookController;

    private BasicBox _plateBox;

    private void Start()
    {
        _gridManager = GridManager.Instance;
        _cookController = (CookController)CookController.instance;
        
        ChooseRecipe(_team.recipes);
    }

    private void ChooseRecipe(List<RecipeController.Recipe> recipes)
    { // Get the "brut" distance between each element to know which recipe AI is going to choose 
        List<int> recipesDistance = new List<int>();
        
        foreach (RecipeController.Recipe recipe in recipes)
        {
            int distance = 0;

            for (int i = 0; i < recipe.allIngredients.Count; i++)
            {
                IngredientBox targetIngredientBox = FindObjectsOfType<IngredientBox>().Where(ingredientBox => ingredientBox.Ingredient.GetComponent<Ingredient>().data == recipe.allIngredients[i]).ToList()[0];
                distance += _gridManager.GeneratePath(ActualTile, targetIngredientBox.Tile).Count;
                
                if (recipe.allIngredients[i].isCookable)
                {
                    if (recipe.allIngredients[i].cookIndex == 0)
                    {
                        StoveBox targetStoveBox = FindObjectsOfType<StoveBox>().Where(stoveBox => stoveBox.Stock == null).ToList()[0];

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
            
            recipesDistance.Add(distance);
            
            
        }

        int minDistance = recipesDistance[0];
        int minIndex = 0;
        
        for (int i = 0;i < recipesDistance.Count;i++)
        {
            int distance = recipesDistance[i];
            
            if (distance < minDistance)
            {
                minDistance = distance;
                minIndex = i;
            }
        }


        _currentWorkRecipe = _team.recipes[minIndex];
        
        Debug.Log("work on "+ _currentWorkRecipe.name);
        ApplyDifficultyOffset();

    }

    private void ChooseRecipeDifficulty()
    {
        int choose = Random.Range(0, 100);
        int targetChoose = 0;

        switch (_cookController.Difficulty)
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

    private void FindBoxForPlate()
    {
        int averageX = ActualTile.X;
        int averageY = ActualTile.Y;
        
        foreach (IngredientData ingredient in _currentWorkRecipe.allIngredients)
        {
            IngredientBox targetIngredientBox = FindObjectsOfType<IngredientBox>().Where(ingredientBox => ingredientBox.Ingredient.GetComponent<Ingredient>().data == ingredient).ToList()[0];
            ApplyAddition(averageX,averageY,targetIngredientBox);

            if (ingredient.isCuttable)
            {
                CutBox targetCutBox = FindObjectsOfType<CutBox>().Where(cutBox => cutBox.Stock == null).ToList()[0];
                ApplyAddition(averageX,averageY,targetCutBox);
            }

            if (ingredient.isCookable)
            {
                if (ingredient.cookIndex == 0)
                {
                    StoveBox targetStoveBox = FindObjectsOfType<StoveBox>().Where(stoveBox => stoveBox.Stock == null).ToList()[0];
                    ApplyAddition(averageX,averageY,targetStoveBox);
                }
                else if (ingredient.cookIndex == 1)
                {
                    PanBox targetPanBox = FindObjectsOfType<PanBox>().Where(panBox => panBox.Stock == null).ToList()[0];
                    ApplyAddition(averageX,averageY,targetPanBox);
                }
            }
        }
        
        if (_currentWorkRecipe.needToBeCook)
        {
            OvenBox targetOvenBox = FindObjectsOfType<OvenBox>().Where(ovenBox => ovenBox.Stock == null).ToList()[0];
            ApplyAddition(averageX,averageY,targetOvenBox);
        }

        PlateBox plateBox = FindObjectOfType<PlateBox>();
        ApplyAddition(averageX,averageY,plateBox);

        DeliveryBox deliveryBox = FindObjectOfType<DeliveryBox>();
        ApplyAddition(averageX, averageY,deliveryBox);
        
        averageX /= 7;
        averageY /= 7;
        
        Debug.Log("brut coords : [" + averageX + "," + averageY + "]");
        Vector2 brutCoords = new Vector2(averageX, averageY);

       // FindNearestBox(brutCoords);
        
        
        // Apply difficulty offset 
    }

    private void ApplyAddition(int averageX,int averageY,Box targetBox)
    {
        averageX += targetBox.Tile.X;
        averageY += targetBox.Tile.Y;
    }
    
    private BasicBox FindNearestBox(Vector2 brutCoords)
    {
        List<BasicBox> basicsBoxes = FindObjectsOfType<BasicBox>().ToList();

        BasicBox minBox = basicsBoxes[0];
        float minDistance = Vector2.Distance(brutCoords, minBox.Tile.Coords);

        foreach (BasicBox basicBox in basicsBoxes)
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

    private void ApplyDifficultyOffset()
    {
        int offset = 0;
        
        switch (_cookController.Difficulty)
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

        for (int i = -offset; i <= offset; i++)
        {
            Debug.Log("offset " + i);
        }

        int randomOffset = Random.Range(-offset, offset);
    }
    
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
