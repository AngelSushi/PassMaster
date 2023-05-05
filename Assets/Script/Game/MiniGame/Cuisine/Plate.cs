using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Plate : MonoBehaviour {

    private CookController _cookController;
    public List<Ingredient> ingredientsInPlate;
    public List<RecipeController.Recipe> availableRecipes;

    private GameObject _plateUIParent;
    public Vector2 plateOffset;

    [HideInInspector] public Camera currentCameraInstance;

    private void Start() {
        _cookController = (CookController)CookController.instance;
        SetupPlateUI();
    }

    public void SetupPlateUI() {
        GameObject plateCanvasObj = new GameObject("Plate Canvas");
        Canvas plateCanvas = plateCanvasObj.AddComponent<Canvas>();
        plateCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
        plateCanvas.transform.parent = transform;
        
        _plateUIParent = Instantiate(_cookController.platePrefab,plateCanvasObj.transform);
        _plateUIParent.SetActive(false);

    }

    public void Update() {
        Vector3 plateUIPosition = currentCameraInstance.WorldToScreenPoint(transform.position);
        plateUIPosition += (Vector3)plateOffset;
        _plateUIParent.transform.position = plateUIPosition;
    }
    
    public void AddIngredient(Ingredient newIngredient,Box box) {
        if ((newIngredient.data.isCookable && !newIngredient.isCook) || (newIngredient.data.isCuttable && !newIngredient.isCut))
            return;
        
        if(!_plateUIParent.activeSelf)
            _plateUIParent.SetActive(true);
        
        ingredientsInPlate.Add(newIngredient);

        GameObject ingredientImage = new GameObject("Ingredient");
        Image image = ingredientImage.AddComponent<Image>();

        Sprite sprite = newIngredient.data.sprite;

        if (CheckAvailableRecipesWithIngredients() || availableRecipes.Count == 1)
            sprite = availableRecipes[0].recipeSprite;

        
        foreach(RecipeController.Recipe recipe in availableRecipes)
            Debug.Log("recipe " + recipe.name);
        
        image.sprite = sprite;
        
        ingredientImage.transform.parent = _plateUIParent.transform;

        box.currentController.actualIngredient = null;
        Destroy(box.currentController.transform.GetChild(box.currentController.transform.childCount - 1).gameObject);
    }

    private bool CheckAvailableRecipesWithIngredients() {
        foreach (RecipeController.Recipe recipe in _cookController.recipeController.recipes) {
            bool isRecipeAvailable = true;
            bool isFull = false;
            // Si il contient ou qu'il est full

            for (int i = 0; i < recipe.allIngredients.Count; i++) {
                IngredientData ingredient = recipe.allIngredients[i];
                if (!recipe.allIngredients.Contains(ingredient)) {
                    isRecipeAvailable = false;
                    break;
                }

                if (i == recipe.allIngredients.Count)  // FULL RECIPE
                    isFull = true;
            }

            if (isRecipeAvailable)
                availableRecipes.Add(recipe);

            if (isFull)
                return true;

        }

        return false;
    }
    
    
}
