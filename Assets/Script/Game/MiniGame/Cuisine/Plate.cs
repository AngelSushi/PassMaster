using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Plate : MonoBehaviour {

    public List<Ingredient> ingredients;
    public bool displayUI;

    private GameObject ui;

    private int lastSize = 0;

    void Start() {
        ui = transform.GetChild(0).GetChild(0).gameObject;
    }

    void Update() {
        displayUI = ingredients.Count > 0;

        Vector3 uiPos = transform.position;
        uiPos.y += 2;
        uiPos.x -= 0.5f;

        ui.transform.position = Camera.main.WorldToScreenPoint(uiPos);

        ui.SetActive(displayUI);

        if(ingredients.Count > lastSize) 
            AddIngredientToUI();
        

        lastSize = ingredients.Count;

    }

    private void AddIngredientToUI() {
        if (ingredients.Count <= 3) {
            GameObject slotObj = ui.transform.GetChild(0).GetChild(ingredients.Count - 1).gameObject;

            if (slotObj.TryGetComponent<Image>(out Image slot)) {
                slot.enabled = true;
                slot.sprite = ingredients[ingredients.Count - 1].ingredientModel.sprite;
            }
        }
        else {
            GameObject slotObj = ui.transform.GetChild(0).GetChild(ui.transform.GetChild(0).childCount - 1).gameObject;

            Vector3 newPosition = slotObj.transform.position;
            newPosition.x += 31;
            GameObject newSlot = Instantiate(slotObj, newPosition,Quaternion.identity,slotObj.transform.parent);

            if (newSlot.TryGetComponent<Image>(out Image slot)) {
                slot.enabled = true;
                slot.sprite = ingredients[ingredients.Count - 1].ingredientModel.sprite;
            }

        }
    }

}
