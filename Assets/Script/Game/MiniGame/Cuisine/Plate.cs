using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Plate : MonoBehaviour {

    public List<Ingredient> ingredients;

    public Recipe currentRecipe { get; private set; }
    public bool displayUI;

    private GameObject ui;

    private int lastSize = 0;

    void Start() {
        ui = transform.GetChild(0).GetChild(0).gameObject;
    }

    void Update() {
        displayUI = ingredients.Count > 0;

        Vector3 uiPos = transform.position;
        uiPos.y += 2;
        uiPos.x -= 0.5f;

        ui.transform.position = Camera.main.WorldToScreenPoint(uiPos);

        ui.SetActive(displayUI);

        if(ingredients.Count > lastSize) 
            AddIngredientToUI();
        

        lastSize = ingredients.Count;

    }

    private void AddIngredientToUI() {
        Recipe ingredientsRecipe = FindRecipeWithIngredients(ingredients);

        if (ingredientsRecipe != null) {
            for (int i = ui.transform.GetChild(0).childCount - 1; i >= 1; i--)
                Destroy(ui.transform.GetChild(0).GetChild(i).gameObject);

            if (ui.transform.GetChild(0).GetChild(0).gameObject.TryGetComponent<Image>(out Image slot)) {
                slot.sprite = ingredientsRecipe.recipeSprite;
                currentRecipe = ingredientsRecipe;
            }
                
            return;
        }

        if (ingredients.Count <= 3) {
            GameObject slotObj = ui.transform.GetChild(0).GetChild(ingredients.Count - 1).gameObject;

            if (slotObj.TryGetComponent<Image>(out Image slot)) {
                slot.enabled = true;
                slot.sprite = ingredients[ingredients.Count - 1].ingredientModel.sprite;
            }
        }
        else {
            GameObject slotObj = ui.transform.GetChild(0).GetChild(ui.transform.GetChild(0).childCount - 1).gameObject;

            Vector3 newPosition = slotObj.transform.position;
            newPosition.x += 31;
            GameObject newSlot = Instantiate(slotObj, newPosition,Quaternion.identity,slotObj.transform.parent);

            if (newSlot.TryGetComponent<Image>(out Image slot)) {
                slot.enabled = true;
                slot.sprite = ingredients[ingredients.Count - 1].ingredientModel.sprite;
            }

        }
    }

    private Recipe FindRecipeWithIngredients(List<Ingredient> ingredients) {
        Recipe findRecipe = null;
        
        for(int i = 0;i<CookController.instance.recipeController.recipes.Count;i++) {
            Recipe r = CookController.instance.recipeController.recipes[i];

            for(int j = 0;j<r.ingredients.Count;j++) {
                if (r.ingredients.Count != ingredients.Count)
                    continue;

                if (r.ingredients[j] == ingredients[j].ingredientModel)
                    findRecipe = r;
                else {
                    findRecipe = null;
                    break;
                }
            }
        }

        return findRecipe;
    }

    public void ManagePlateUI(bool active) {
        for(int i = 0;i < ui.transform.childCount;i++)
            ui.transform.GetChild(i).gameObject.SetActive(active);   
    }



}
