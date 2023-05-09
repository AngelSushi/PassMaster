using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class RecipeController : MonoBehaviour {

    [System.Serializable]
    public class Recipe {
        public string name;
        public List<IngredientData> allIngredients;
        public Sprite recipeSprite;
        public bool needToBeCook;
        public bool isCooked;
        public GameObject recipeMesh;
        public RecipeTicker ticker;
        public float recipeTime;
    }

    public class RecipeTicker {

        private CookController.Team _currentTeam;
        private Recipe _currentRecipe;
        public GameObject _recipeUI;
        private Slider _slider;
        public float _currentTime;
        public RecipeController _recipeController;

        public RecipeTicker(CookController.Team currentTeam,Recipe currentRecipe,GameObject recipeUI, RecipeController recipeController) {
            _currentTeam = currentTeam;
            _recipeController = recipeController;
            _currentRecipe = currentRecipe;
            _recipeUI = recipeUI;

            _slider = recipeUI.transform.GetChild(0).GetComponent<Slider>();
        }
        
        public void Start() {
            _currentTime = _currentRecipe.recipeTime;
        }
        
        public void Tick() {
            _currentTime -= Time.deltaTime;
            _slider.value = _currentTime / _currentRecipe.recipeTime;
            _slider.transform.GetChild(1).GetChild(0).gameObject.GetComponent<Image>().color = _recipeController.sliderColor.Evaluate(_slider.value);

            if (_currentTime < 0)
                End();
        }

        public void End() {
            _currentTeam.recipes.Remove(_currentRecipe);
            Destroy(_recipeUI);
            _recipeController._recipeTickers.Remove(this);
        }
        
    }
    
    public List<Recipe> recipes;
    public int maxRecipesPerTeam;
    public float waitNextRecipeTimer;
    
    private CookController _cookController;
    private List<RecipeTicker> _recipeTickers = new List<RecipeTicker>();

    public Gradient sliderColor;

    
    
    
    private void Start() {
        _cookController = (CookController)CookController.instance;

        foreach (CookController.Team team in _cookController.teams) {
            GenerateRecipe(team);
            GenerateRecipeUI(team);
            DrawRecipe(team);   
        }
    }

    private void Update() {
        foreach(RecipeTicker ticker in _recipeTickers)
            ticker.Tick();
        foreach (CookController.Team team in _cookController.teams) 
            team.Tick();
        
    }

    
    private void GenerateRecipe(CookController.Team team) {
        for (int i = team.recipes.Count; i < maxRecipesPerTeam; i++) {
            Recipe lastRecipe = null;
            
            if (i - 1 >= 0) 
                 lastRecipe = team.recipes[i - 1];

            int randomRecipe = Random.Range(0, recipes.Count);
            Recipe recipe = recipes[randomRecipe];

            while (recipe == lastRecipe) {
                randomRecipe = Random.Range(0, recipes.Count);
                recipe = recipes[randomRecipe];
            }
            
            team.recipes.Add(recipe);
        }
        
        
    }

    private void GenerateRecipeUI(CookController.Team team) {
        for (int i = 0; i < maxRecipesPerTeam; i++) {
            Transform recipeParent = team.canvas.transform.GetChild(0);
            GameObject recipeUI = Instantiate(_cookController.recipePrefab,recipeParent);

            Recipe currentRecipe = team.recipes[i];
            RecipeTicker ticker = new RecipeTicker(team,currentRecipe,recipeUI,this);
            currentRecipe.ticker = ticker;
            _recipeTickers.Add(ticker);
            ticker.Start();
        }
        
    }

    private void DrawRecipe(CookController.Team team) {
        for (int i = 0; i < maxRecipesPerTeam; i++) {
            Recipe currentRecipe = team.recipes[i];
            Transform recipe = team.canvas.transform.GetChild(0).GetChild(i);
            
            Transform ingredientParent = team.canvas.transform.GetChild(0).GetChild(i).GetChild(2);
            
            Image recipeSpriteSlot = recipe.GetChild(1).GetComponent<Image>();
            recipeSpriteSlot.sprite = currentRecipe.recipeSprite;

            for (int j = 0; j < currentRecipe.allIngredients.Count; j++) {
                IngredientData ingredientData = currentRecipe.allIngredients[j];

                GameObject ingredient = new GameObject("Ingredient");
                ingredient.transform.parent = ingredientParent;
                Image ingredientImage = ingredient.AddComponent<Image>();
                ingredientImage.sprite = ingredientData.sprite;
            }
        }
    }

    public void AutoGenerateRecipe(CookController.Team team) { // Est ce qu'on veut que le changement de reputation affecte le timer actuel ou le prochain timer
        GenerateRecipe(team);
        GenerateRecipeUI(team);
        DrawRecipe(team);
    }

}
