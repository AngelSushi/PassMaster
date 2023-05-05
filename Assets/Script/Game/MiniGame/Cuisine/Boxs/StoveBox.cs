using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoveBox : MakeBox {
    
    #region Variables
    
    private Color _mainColor,_cookedColor;
    private Ingredient _stockIngredient;
    private MeshRenderer _meshIngredient;
    private MeshRenderer _cookedMesh;

    #endregion
    
    #region Make Box FUnctions
    
    protected override void StartMake() {
        boxSlider.gameObject.SetActive(true);
        _stockIngredient = stock.GetComponent<Ingredient>();
        
        _meshIngredient = _stockIngredient.basic.GetComponent<MeshRenderer>();
        _cookedMesh = _stockIngredient.modified.GetComponent<MeshRenderer>();
        _mainColor = _meshIngredient.material.color;
        _cookedColor = _cookedMesh.material.color;
        
        StartCoroutine(LerpColor());
    }

    protected override void Make() {
        timer += Time.deltaTime;
        boxSlider.value = timer / timeToSucceed;

        if (timer >= timeToSucceed) 
            FinishMake();
    }

    protected override void FinishMake() {
        boxSlider.gameObject.SetActive(false);
        timer = 0f;
        _stockIngredient.isCook = true;
    }
    
    #endregion
    
    #region Basic Box Functions

    protected override void Put() {
        if (stock == null && currentController.actualIngredient != null && currentController.actualIngredient.TryGetComponent<Ingredient>(out Ingredient ingredient)) {
            if (ingredient.data.cookIndex == 0) { // detect if ingredient is cooked  on stove
                if (ingredient.data.isCookable && !ingredient.isCook) 
                    base.Put();
            }
        }
    }

    protected override void Take() {
        if (stock != null) {
            if (stock.TryGetComponent<Ingredient>(out Ingredient ingredient) && ingredient.isCook) {
                base.Take();
            }
        }
    }
    
    #endregion

    private IEnumerator LerpColor() {
        for (float f = 0; f <= timeToSucceed; f += Time.deltaTime) {
            _meshIngredient.material.color = Color.Lerp(_mainColor,_cookedColor,f / timeToSucceed);
            yield return null;
        }
    }
}
