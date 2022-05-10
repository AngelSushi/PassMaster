using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CookBox : BoxUI {

    public Ingredient actualIngredient;
    private CookAction action;

    protected override void Start() {
        base.Start();

        action = GetComponent<CookAction>();
        action.OnActionFinished += OnActionFinished;
    }


    public override void Interact(ChefController playerController) {

        if (playerController.actualIngredient != null) {
            actualIngredient = playerController.actualIngredient.GetComponent<Ingredient>();

            if (boxType == actualIngredient.ingredientModel.boxActionType) {
                if (!actualIngredient.isCook && actualIngredient.ingredientModel.isCookable && !action.isStarted) {
                    if(boxType == BoxType.STOVE)
                        actualIngredient.gameObject.SetActive(false);
                    else {
                        
                        if (transform.childCount < 1) {
                            Debug.LogWarning("Error when getting cooked ingredient point");
                            return;
                        }

                        PutIngredient(playerController,transform.gameObject);

                        actualIngredient.gameObject.transform.position = transform.GetChild(1).position;
                        actualIngredient.gameObject.transform.localScale = new Vector3(48.9f, 35.4f, 33.8f);

                    }

                    playerController.actualIngredient = null;
                    AddIngredientsToUI(new List<Ingredient> { actualIngredient });
                    action.StartAction();
                }
            }
        }
        else {
            if (actualIngredient != null && actualIngredient.isCook) {
                TakeIngredient(playerController);
                ClearUI();
            }
        }
    }


    public void OnActionFinished(object sender, CookAction.OnActionFinishArgs e) {
        actualIngredient.isCook = true;

        GameObject cookedIngredient = Instantiate(actualIngredient.ingredientModel.ingredientCookPrefab, actualIngredient.gameObject.transform.position, actualIngredient.ingredientModel.ingredientCookPrefab.transform.rotation);

        UnityEditorInternal.ComponentUtility.CopyComponent(actualIngredient);
        UnityEditorInternal.ComponentUtility.PasteComponentAsNew(cookedIngredient);

        cookedIngredient.transform.parent = transform;

        cookedIngredient.SetActive(slots.Count == 0);

        Destroy(actualIngredient.gameObject);

        actualIngredient = cookedIngredient.GetComponent<Ingredient>();
    }
}
