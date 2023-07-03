using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlateBox : Box {

    private GameObject _plate;

    protected override void Start() {
        base.Start();
        _plate = transform.GetChild(0).gameObject;
    }
    
    public override void BoxInteract(GameObject current, ChiefController controller) {
        currentController = controller;
        Take();
    }
    
    protected override void Put() {}

    protected override void Take() {
        if (currentController.ActualPlate == null) {
            GameObject plate = Instantiate(_plate);
            plate.transform.parent = currentController.transform;
            plate.transform.localPosition = currentController.IngredientSpawn.localPosition;
            plate.transform.eulerAngles = plate.GetComponent<Plate>().plateRotation;
            plate.SetActive(true);
            currentController.ActualPlate = plate;
        }
    }
}
