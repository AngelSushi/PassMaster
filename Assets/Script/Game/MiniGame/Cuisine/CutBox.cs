using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CutBox : Box {

    public Ingredient actualIngredient;

    public override void Interact(ChefController playerController) {
        base.Interact(playerController);
    }
}
