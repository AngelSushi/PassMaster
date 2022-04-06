using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CutBox : Box {

    public Ingredient actualIngredient;

    public override void Interact(ChefController playerController) {

        if(playerController.actualIngredient != null)
            actualIngredient = playerController.actualIngredient.GetComponent<Ingredient>();

        if(actualIngredient != null && actualIngredient.ingredientModel.isCuttable && !actualIngredient.isCook) {
            Debug.Log("COUPER");
        }

        base.Interact(playerController);


    }
}
