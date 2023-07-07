using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoveBox : MakeBox {
    
    #region Variables
    
    private Color _mainColor,_cookedColor;

    [SerializeField] private Color burnedColor;
    
    private Ingredient _stockIngredient;
    private MeshRenderer _meshIngredient;
    private MeshRenderer _cookedMesh;
    [SerializeField] private float timeToBurn;

    #endregion
    
    #region Make Box Functions
    
    protected override void StartMake() {
        boxSlider.gameObject.SetActive(true);
        _stockIngredient = stock.GetComponent<Ingredient>();
        
        _meshIngredient = _stockIngredient.basic.GetComponent<MeshRenderer>();
        _cookedMesh = _stockIngredient.modified.GetComponent<MeshRenderer>();
        _mainColor = _meshIngredient.material.color;
        _cookedColor = _cookedMesh.material.color;
        
        StartCoroutine(LerpColor(_mainColor,_cookedColor,timeToSucceed));
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
        _stockIngredient.IsCook = true;

        AIAction action = currentController is ChiefAIController ? ((ChiefAIController)currentController).CurrentAction : null;
        
        _cookController.CookEvents.OnFinishedCookIngredient?.Invoke(this,new CookEvents.OnFinishedCookIngredientArgs(currentController.gameObject,this,_stockIngredient,action));
        
        BeginBurn();
    }

    protected override void BeginBurn()
    {
        if (stock == null || _stockIngredient.IsBurn)
        {
            isBurning = false;
            return;
        }
        
        isBurning = true;
        burnTimer = 0f;
        StartCoroutine(LerpColor(_cookedColor,burnedColor,timeToBurn));
        //Start anim 
        // PlaySound                                                
    }

    protected override void Burn()
    {
        if (stock == null || _stockIngredient.IsBurn)
        {
            burnTimer = 0f;
            isBurning = false;
            return;
        }
        
        
        burnTimer += Time.deltaTime;
        Debug.Log("is burning " + burnTimer);
            
        if (burnTimer >= timeToBurn)
        {
            FinishBurn();
        }
    }

    protected override void FinishBurn()
    {
        Debug.Log("stop burn");
        isBurning = false;
        _stockIngredient.IsBurn = true;
    }
    
    #endregion
    
    #region Basic Box Functions

    protected override void Put() {
        if (stock == null && currentController.ActualIngredient != null && currentController.ActualIngredient.TryGetComponent<Ingredient>(out Ingredient ingredient)) {
            if (!ingredient.Data.IsPan) { 
                if (ingredient.Data.CanBeCook && !ingredient.IsCook) 
                    base.Put();
            }
        }
    }

    protected override void Take() {
        if (stock != null) {
            if (stock.TryGetComponent<Ingredient>(out Ingredient ingredient) && ingredient.IsCook) {
                base.Take();
            }
        }
    }
    
    #endregion

    private IEnumerator LerpColor(Color mainColor,Color lerpColor,float time) {
        for (float f = 0; f <= time; f += Time.deltaTime) {
            _meshIngredient.material.color = Color.Lerp(mainColor,lerpColor,f / time);
            yield return null;
        }
    }
}
