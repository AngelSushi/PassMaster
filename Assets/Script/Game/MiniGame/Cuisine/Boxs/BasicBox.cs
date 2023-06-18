using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicBox : Box {

    protected Vector3 stockPosition;
    protected GameObject stock;

    public GameObject Stock
    {
        get => stock;
        private set => stock = value;
    }

    protected override void Start() {
        base.Start();
        stockPosition = transform.GetChild(0).localPosition;
    }

    protected override void Put() {
        
        if (currentController.actualIngredient != null || currentController.actualPlate != null) {
            if (currentController.actualIngredient != null) {
                if (stock != null && stock.GetComponent<Plate>() != null) {
                    Plate plate = stock.GetComponent<Plate>();
                    plate.currentCameraInstance = _cookController.instances[currentController.GetComponent<ZoneSwapper>().areaIndex].instanceCamera;
                    plate.AddIngredient(currentController.actualIngredient.GetComponent<Ingredient>(),this);
                }
                else {
                    GameObject ingredient = currentController.actualIngredient;
                    ingredient.transform.parent = transform;
                    ingredient.transform.localPosition = stockPosition;
            
                    currentController.actualIngredient = null;
                    stock = ingredient;
                }
            }
            else {
                GameObject plate = currentController.actualPlate;
                plate.transform.parent = transform;
                plate.transform.localPosition = stockPosition;
            
                currentController.actualPlate = null;
                stock = plate;
            }
        }
    }

    protected override void Take() {
        if (stock != null && (currentController.actualIngredient == null && currentController.actualPlate == null)) {
            stock.transform.parent = currentController.transform;
            stock.transform.localPosition = currentController.ingredientSpawn.localPosition;
            
            if(stock.GetComponent<Ingredient>() != null)
                currentController.actualIngredient = stock;
            else if (stock.GetComponent<Plate>() != null)
                currentController.actualPlate = stock;
            
            stock = null;
        }
    }
}
