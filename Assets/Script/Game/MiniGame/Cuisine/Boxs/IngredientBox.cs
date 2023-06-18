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

    public override void BoxInteract(GameObject current,ChefController controller) {
        currentController = controller;
        Take();
    }
   
    protected override void Put() {}

    protected override void Take() {
        if (currentController.actualIngredient == null) {
            GameObject ingredient = Instantiate(Ingredient);
            ingredient.transform.parent = currentController.transform;
            ingredient.transform.localPosition = currentController.ingredientSpawn.localPosition;
            
            Debug.Log("rotation " + Ingredient.GetComponent<Ingredient>().ingredientRotation);
            
            Vector3 euler = Ingredient.GetComponent<Ingredient>().ingredientRotation;
            ingredient.transform.rotation = Quaternion.Euler(euler.x, euler.y, euler.z);

            
            
            Debug.Log("euler " + ingredient.transform.eulerAngles);
            Debug.Log("rotation " + ingredient.transform.rotation);
            
            ingredient.SetActive(true);
            currentController.actualIngredient = ingredient;
        }

    }
}
