using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BinBox : Box
{
    public override void BoxInteract(GameObject current,ChiefController controller) {
        currentController = controller;
        Put();
    }

    protected override void Put()
    {
        Destroy(currentController.ActualIngredient != null ? currentController.ActualIngredient : currentController.ActualPlate != null ? currentController.ActualPlate : null);
    }

    protected override void Take() {}
}
