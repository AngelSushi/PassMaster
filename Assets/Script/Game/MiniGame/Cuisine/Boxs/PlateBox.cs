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
        if (currentController.actualPlate == null) {
            GameObject plate = Instantiate(_plate);
            plate.transform.parent = currentController.transform;
            plate.transform.localPosition = currentController.ingredientSpawn.localPosition;
            plate.transform.eulerAngles = plate.GetComponent<Plate>().plateRotation;
            plate.GetComponent<Plate>().currentCameraInstance = _cookController.instances[currentController.GetComponent<ZoneSwapper>().areaIndex].instanceCamera;
            plate.SetActive(true);
            currentController.actualPlate = plate;
        }
    }
}
