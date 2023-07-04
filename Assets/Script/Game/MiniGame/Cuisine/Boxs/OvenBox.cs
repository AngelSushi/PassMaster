using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OvenBox : MakeBox {

    private GameObject _furnaceDoor;
    private RecipeController.Recipe _targetRecipe;

    protected override void Start() {
        base.Start();
        _furnaceDoor = transform.GetChild(1).GetChild(0).gameObject;
    }

    protected override void Put() {
        if (currentController.ActualPlate != null) {
            Plate plate = currentController.ActualPlate.GetComponent<Plate>();
            GameObject plateObj = plate.gameObject;
            if (plate.fullRecipe != null) {
                _targetRecipe = plate.fullRecipe;
                
                if (_targetRecipe.NeedToBeCook && !_targetRecipe.IsCooked) {
                    plateObj.transform.parent = transform;
                    plateObj.transform.localPosition = stockPosition;
                   // plate.plateCanvas.gameObject.SetActive(false);
                    _furnaceDoor.transform.eulerAngles = Vector3.zero; // x = 90 ; y = 0 ; z = 0
                    currentController.ActualPlate = null;
                    stock = plateObj;
                }
            }
        }
    }

    protected override void Take() {
        if (_targetRecipe != null && _targetRecipe.IsCooked) {
            stock.transform.parent = currentController.transform;
            stock.transform.localPosition = currentController.IngredientSpawn.localPosition;
            currentController.ActualPlate = stock;
            _targetRecipe = null;
            stock = null;
        }
        
    }

    protected override void StartMake() {
        boxSlider.gameObject.SetActive(true);
    }

    protected override void Make() {
        timer += Time.deltaTime;
        boxSlider.value = timer / timeToSucceed;

        if (timer >= timeToSucceed) 
            FinishMake();
    }

    protected override void FinishMake() {
        boxSlider.gameObject.SetActive(false);
        _targetRecipe.IsCooked = true;
        _furnaceDoor.transform.eulerAngles = new Vector3(90, 0, 0);
    }
}
