using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicBox : Box {

    protected Vector3 stockPosition;
    protected GameObject stock;

    protected override void Start() {
        base.Start();
        stockPosition = transform.GetChild(0).localPosition;
    }

    protected override void Put() {
        if (currentController.actualIngredient != null) {
            GameObject ingredient = currentController.actualIngredient;
            ingredient.transform.parent = transform;
            ingredient.transform.localPosition = stockPosition;
            
            currentController.actualIngredient = null;
            stock = ingredient;
        }
    }

    protected override void Take() {
        if (stock != null && currentController.actualIngredient == null) {
            stock.transform.parent = currentController.transform;
            stock.transform.localPosition = currentController.ingredientSpawn.localPosition;
            currentController.actualIngredient = stock;
            stock = null;
        }
    }
}
