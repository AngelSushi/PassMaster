using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IngredientBox : Box {

    private GameObject _ingredient;

    public GameObject Ingredient
    {
        get => _ingredient;
        protected set => _ingredient = value;
    }

    protected override void Start() {
        base.Start();
        Ingredient = transform.GetChild(0).gameObject;
    }

    public override void BoxInteract(GameObject current,ChiefController controller) {
        currentController = controller;
        Take();
    }
   
    protected override void Put() {}

    protected override void Take() {
        if (currentController.ActualIngredient == null) {
            GameObject ingredient = Instantiate(Ingredient);
            ingredient.transform.parent = currentController.transform;
            ingredient.transform.localPosition = currentController.IngredientSpawn.localPosition;

            Vector3 euler = Ingredient.GetComponent<Ingredient>().ingredientRotation;
            ingredient.transform.rotation = Quaternion.Euler(euler.x, euler.y, euler.z);

            
            
            
            ingredient.SetActive(true);
            currentController.ActualIngredient = ingredient;
        }

    }
}
