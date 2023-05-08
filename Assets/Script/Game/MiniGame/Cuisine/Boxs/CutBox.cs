using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CutBox : MakeBox {
    
    #region Variables
    
    private bool _startCutting,_lastStartCutting;
    private ParticleSystem _effect;

    #endregion
    
    #region Unity Functions
    protected override void Start() {
        base.Start();
        _effect = GetComponentInChildren<ParticleSystem>();
    }
    
    protected override void Update() {
        if(_startCutting && !_lastStartCutting)
            StartMake();
        
        if (_startCutting && timer < timeToSucceed)
            Make();

        _lastStartCutting = _startCutting;
    }
    #endregion


    #region Make Box Functions

    protected override void StartMake() {
        boxSlider.gameObject.SetActive(true);
        _effect.Play();
    }
    
    protected override void Make() {
        timer += Time.deltaTime;
        boxSlider.value = timer / timeToSucceed;
        
        
         if (timer >= timeToSucceed) 
             FinishMake();
    }

    protected override void FinishMake() {
        boxSlider.gameObject.SetActive(false);
        Ingredient stockIngredient = stock.GetComponent<Ingredient>();
        _startCutting = false;
        timer = 0f;
        _effect.Stop();
        stockIngredient.isCut = true;
        stockIngredient.basic.SetActive(false);
        stockIngredient.modified.SetActive(true);
    }
    #endregion
    
    #region Basic Box Functions
    public override void BoxInteract(GameObject current, ChefController controller) {
        if (stock != null && stock.TryGetComponent<Ingredient>(out Ingredient ingredient)) {
            if (ingredient.data.isCuttable) {
                if (!ingredient.isCut)
                    _startCutting = true;
                else
                    base.BoxInteract(current, controller);
            }
        }
        else 
            base.BoxInteract(current, controller);
    }
    
    protected override void Take() {
        base.Take();
    }

    protected override void Put() {
        if (stock == null && currentController.actualIngredient != null && currentController.actualIngredient.TryGetComponent<Ingredient>(out Ingredient ingredient)) {
            if (!ingredient.data.isCuttable || ingredient.isCut)
                return;
        }
        
        base.Put();
    }
    #endregion
}
