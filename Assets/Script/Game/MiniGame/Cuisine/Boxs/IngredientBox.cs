using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IngredientBox : Box {

    private GameObject _ingredient;

    protected override void Start() {
        base.Start();
        _ingredient = transform.GetChild(0).gameObject;
    }

    public override void BoxInteract(GameObject current,ChefController controller) {
        currentController = controller;
        Take();
    }
   
    protected override void Put() {}

    protected override void Take() {
        if (currentController.actualIngredient == null) {
            GameObject ingredient = Instantiate(_ingredient);
            ingredient.transform.parent = currentController.transform;
            ingredient.transform.localPosition = currentController.ingredientSpawn.localPosition;
            ingredient.SetActive(true);
            currentController.actualIngredient = ingredient;
        }

    }
}
