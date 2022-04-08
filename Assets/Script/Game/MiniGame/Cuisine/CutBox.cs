using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CutBox : Box {

    public Ingredient actualIngredient;

    private CookAction action;

    void Start() {
        action = GetComponent<CookAction>();
    }

    public override void Interact(ChefController playerController) {

        Debug.Log("cutted");

        if(actualIngredient != null && actualIngredient.ingredientModel.isCuttable && !actualIngredient.isCut) {
            action.StartAction();
            action.OnActionFinished += OnActionFinished;
            return;
        }

        if(playerController.actualIngredient != null) // On pose l'ingrédient pour le couper
            actualIngredient = playerController.actualIngredient.GetComponent<Ingredient>();

        
        base.Interact(playerController);
    }

    public void OnActionFinished(object sender,CookAction.OnActionFinishArgs e) {
        actualIngredient.isCut = true;

        GameObject cutIngredient = Instantiate(actualIngredient.ingredientModel.ingredientCutPrefab,actualIngredient.gameObject.transform.position,actualIngredient.ingredientModel.ingredientCutPrefab.transform.rotation);

        UnityEditorInternal.ComponentUtility.CopyComponent(actualIngredient); 
        UnityEditorInternal.ComponentUtility.PasteComponentAsNew(cutIngredient);

        cutIngredient.transform.parent = transform;

        Destroy(actualIngredient.gameObject);
        
        actualIngredient = cutIngredient.GetComponent<Ingredient>();
        
    }
}
