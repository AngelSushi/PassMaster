using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IngredientBox : Box {

    public DataIngredient ingredient;

    public override void Interact(ChefController playerController) {
        base.Interact(playerController);
    }

}
