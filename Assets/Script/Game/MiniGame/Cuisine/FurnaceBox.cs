using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FurnaceBox : BoxUI {

    [HideInInspector]
    public Recipe recipeIn;
    private CookAction action;

    protected override void Start() {
        base.Start();

        action = GetComponent<CookAction>();
    }

    public override void Interact(ChefController playerController) {
        if(playerController.plate != null) {
            if(playerController.plate.currentRecipe != null && playerController.plate.currentRecipe.isFurnacable) {
                recipeIn = playerController.plate.currentRecipe;
                playerController.plate.ManagePlateUI(false);
                PutIngredient(playerController,transform.gameObject);
                AddRecipeToUI(recipeIn);

                action.StartAction();
            }            
        }
        else {
            if (recipeIn != null && !action.isStarted) {
                recipeIn = null;
                TakeIngredient(playerController);
                playerController.plate.ManagePlateUI(true);
                ClearUI();
            }
        }
    }
}
