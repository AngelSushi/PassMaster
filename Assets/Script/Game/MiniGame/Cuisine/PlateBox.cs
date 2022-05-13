using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlateBox : Box {

    public override void Interact(ChefController playerController) {
        if (playerController.actualIngredient != null || playerController.plate != null)
            return;

        Transform plate = transform.GetChild(0);

        Vector3 position = plate.position;
        Quaternion rotation = plate.rotation;

        GameObject newPlate = Instantiate(plate.gameObject,position,rotation,transform);

        plate.parent = playerController.gameObject.transform;

        
        Vector3 localPos = plate.localPosition;
        localPos.z = 0.8f;

        plate.localPosition = localPos;
        playerController.plate = plate.gameObject.GetComponent<Plate>();

    }

}
