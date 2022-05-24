using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BoxUI : Box {

    public List<Image> slots;

    private int availableSlotId;

    protected virtual void Start() => ClearUI();
    

    public void AddIngredientsToUI(List<DataIngredient> ingredientsToAdd) {
        if(availableSlotId >= ingredientsToAdd.Count || slots.Count == 0) 
            return;

        for(int i = 0;i < ingredientsToAdd.Count;i++) {
            slots[availableSlotId].sprite = ingredientsToAdd[i].sprite;
            slots[availableSlotId].gameObject.SetActive(true);
            availableSlotId++;
        }

    }

    public void AddRecipeToUI(Recipe recipe) {
        if (recipe == null)
            return;

        slots[0].sprite = recipe.recipeSprite;
        slots[0].gameObject.SetActive(true);
    }

    public void ClearUI() {
        foreach (Image slot in slots)
            slot.gameObject.SetActive(false);
    }

}
