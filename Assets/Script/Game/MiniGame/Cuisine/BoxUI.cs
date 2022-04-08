using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BoxUI : Box {

    public List<Image> slots;
    //public List<Ingredient> ingredients;
    private int availableSlotId;


    public void AddIngredientsToUI(List<Ingredient> ingredientsToAdd) {
        if(availableSlotId >= ingredientsToAdd.Count) 
            return;

        for(int i = 0;i < ingredientsToAdd.Count;i++) {
            slots[availableSlotId].sprite = ingredientsToAdd[i].ingredientModel.sprite;
            availableSlotId++;
        }

    }

    public void RemoveIngredientsToUI() {
        foreach(Image slot in slots) 
            slot.sprite = null;
    }

}
