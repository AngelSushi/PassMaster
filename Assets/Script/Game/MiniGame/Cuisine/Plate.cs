using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class Plate : MonoBehaviour {

    private CookController _cookController;
    public List<Ingredient> ingredientsInPlate;
    private List<RecipeController.Recipe> _availableRecipes = new List<RecipeController.Recipe>();

    public List<RecipeController.Recipe> AvailableRecipes
    {
        get => _availableRecipes;
    }
    
    public RecipeController.Recipe fullRecipe;

    private GameObject _plateUIParent;
    public Vector2 plateOffset;

    [HideInInspector] public Canvas plateCanvas;

    public Vector3 plateRotation;

    private Vector3[] _localIngredientsPosition;
    
    
    private void Start() {
        _cookController = (CookController)CookController.instance;
        SetupPlateUI();

        _localIngredientsPosition = new Vector3[transform.childCount];
        for (int i = 0; i < _localIngredientsPosition.Length; i++)
            _localIngredientsPosition[i] = transform.GetChild(i).localPosition;
        
    }

    public void SetupPlateUI() {
        GameObject plateCanvasObj = new GameObject("Plate Canvas");
        plateCanvas = plateCanvasObj.AddComponent<Canvas>();
        plateCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
        plateCanvas.transform.parent = transform;
        
        _plateUIParent = Instantiate(_cookController.platePrefab,plateCanvasObj.transform);
        _plateUIParent.SetActive(false);

    }

    public void Update() {
        Vector3 plateUIPosition = Camera.main.WorldToScreenPoint(transform.position);
        plateUIPosition += (Vector3)plateOffset;
        _plateUIParent.transform.position = plateUIPosition;
    }
    
    public void AddIngredient(Ingredient newIngredient,Box box,ChiefController from) {
        if ((newIngredient.data.isCookable && !newIngredient.isCook) || (newIngredient.data.isCuttable && !newIngredient.isCut))
            return;

        if(!_plateUIParent.activeSelf)
            _plateUIParent.SetActive(true);

        ingredientsInPlate.Add(newIngredient); // Vérifier si on peut mettre 2 items qui ont rien a voir 
        
        // Appelle de l'event 
        
        bool drawRecipe = CheckAvailableRecipesWithIngredients(newIngredient) || _availableRecipes.Count == 1;

        _cookController.CookEvents.OnPutIngredientInPlate?.Invoke(this,new CookEvents.OnPutIngredientInPlateArgs((BasicBox)box,newIngredient,_availableRecipes,fullRecipe != null,from));
        
        
        
        Sprite sprite = drawRecipe ? fullRecipe != null ? fullRecipe.RecipeSprite : _availableRecipes[0].RecipeSprite : newIngredient.data.sprite;
        

        if (!ingredientsInPlate.Contains(newIngredient)) // CheckAvailableRecipesWithIngredients() can remove ingredients if its not in the current recipe ==> recheck if plate contains the newIngredient
            return;

        for (int i = 0; i < _plateUIParent.transform.childCount; i++) 
            Destroy(_plateUIParent.transform.GetChild(i).gameObject);

        int maxImage = drawRecipe ? 1 : ingredientsInPlate.Count;

        for (int i = 0; i < maxImage; i++) {
            GameObject ingredientImage = new GameObject("Ingredient");
            Image image = ingredientImage.AddComponent<Image>();

            image.sprite = drawRecipe ? sprite : ingredientsInPlate[i].data.sprite;
            
            ingredientImage.transform.parent = _plateUIParent.transform;
        }

        box.currentController.ActualIngredient = null;
        Destroy(box.currentController.transform.GetChild(box.currentController.transform.childCount - 1).gameObject);
       // DrawRecipeModel(drawRecipe,maxImage);
    }

    private void DrawRecipeModel(bool drawRecipe,int max) { // A refaire lorsque les ingredients ont leur bonne rotation
        for (int i = 0; i < max; i++) {
          
            // visé toujours la version la plus haute ( coupé etc)
            Transform ingredientModel = Instantiate(ingredientsInPlate[i].gameObject.transform.GetChild(0), transform);
            ingredientModel.localPosition = _localIngredientsPosition[i];
            ingredientModel.gameObject.SetActive(true);
        }
    }

    private bool CheckAvailableRecipesWithIngredients(Ingredient newIngredient) {
        foreach (RecipeController.Recipe recipe in _cookController.recipeController.recipes) {
            bool isRecipeAvailable = true;
            bool isFull = false;

            for (int i = 0; i < ingredientsInPlate.Count; i++) {
                IngredientData ingredient = ingredientsInPlate[i].data;   
                
                if (!recipe.AllIngredients.Contains(ingredient)) { 
                    isRecipeAvailable = false;
                    break;
                }

                if (i == recipe.AllIngredients.Count - 1 && ingredientsInPlate.Count == recipe.AllIngredients.Count)  // FULL RECIPE
                    isFull = true;
            }

            if (isRecipeAvailable) {
                _availableRecipes.Add(recipe);
            }

            if (isFull) {
                fullRecipe = recipe;
                return true;
            }
        }

        bool containsInOne = false;
        
        foreach (RecipeController.Recipe recipe in _availableRecipes) { // Part to avoid ingredients that is not in the current recipes
            for (int i = 0; i < ingredientsInPlate.Count; i++) { 
                IngredientData ingredient = ingredientsInPlate[i].data;

                if (recipe.AllIngredients.Contains(ingredient)) {
                    containsInOne = true;
                }
                else {
                    containsInOne = false;
                    break;
                }
            }
        }
        
        if (!containsInOne) 
            ingredientsInPlate.Remove(newIngredient);

        return false;
    }
    
    
}
