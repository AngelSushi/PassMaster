using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Grid;

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
    
    public class ChiefAction
    {

        private int _priority;
        private ChiefAction[] _needs;
        private bool _succeed;

        public int Priority
        {
            get => _priority;
            private set => _priority = value;
        }

        public ChiefAction[] Needs
        {
            get => _needs;
            private set => _needs = value;
        }

        public bool Succeed
        {
            get => _succeed;
            private set => _succeed = value;
        }

        public ChiefAction(int priority, ChiefAction[] needs, bool succeed)
        {
            _priority = priority;
            _needs = needs;
            _succeed = succeed;
        }
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


    private void Start()
    {
        _gridManager = GridManager.Instance;
       // ChooseRecipe();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            ChooseRecipe();
        }
    }


    private void ChooseRecipe()
    { // Get the "brut" distance between each element to know which recipe AI is going to choose 
        List<int> recipesDistance = new List<int>();
        
        foreach (RecipeController.Recipe recipe in _team.recipes)
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
            
            Debug.Log("distance " + distance + " for " + recipe.name);
            
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

    }
}
