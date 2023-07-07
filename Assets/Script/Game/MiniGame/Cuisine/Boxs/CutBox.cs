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

        originSound = audioSource.clip;
        audioSource.clip = sound;
        audioSource.Play();
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
        stockIngredient.IsCut = true;
        stockIngredient.basic.SetActive(false);
        stockIngredient.modified.SetActive(true);
        audioSource.clip = originSound;
        
        _cookController.CookEvents.OnFinishedCutIngredient?.Invoke(this,new CookEvents.OnFinishedCutIngredientArgs(this,stockIngredient,currentController.gameObject));
    }

    protected override void BeginBurn()
    {
        throw new System.NotImplementedException();
    }

    protected override void Burn()
    {
        throw new System.NotImplementedException();
    }

    protected override void FinishBurn()
    {
        throw new System.NotImplementedException();
    }

    #endregion
    
    #region Basic Box Functions
    public override void BoxInteract(GameObject current, ChiefController controller) {
        if (stock != null && stock.TryGetComponent<Ingredient>(out Ingredient ingredient)) {
            if (ingredient.Data.CanBeCut) {
                if (!ingredient.IsCut)
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
        if (stock == null && currentController.ActualIngredient != null && currentController.ActualIngredient.TryGetComponent(out Ingredient ingredient)) {
            if (!ingredient.Data.CanBeCut || ingredient.IsCut)
                return;
        }
        
        base.Put();
    }
    #endregion
}
