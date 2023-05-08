using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeliveryBox : Box {
    void Start() {
        
    }

    public override void BoxInteract(GameObject current,ChefController controller) {
        currentController = controller;

        if (current != null) {
            if (current.TryGetComponent<Plate>(out Plate plate)) {
                if (plate.fullRecipe != null) {
                    if (!plate.fullRecipe.needToBeCook || (plate.fullRecipe.needToBeCook && plate.fullRecipe.isCooked)) {
                        Put();
                    }
                }
            }
        }        
    }

    protected override void Put() {
        Debug.Log("put box");
    }

    protected override void Take() { }

}
