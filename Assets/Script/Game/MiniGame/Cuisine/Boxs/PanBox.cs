using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PanBox : MakeBox {
    
    #region Variables
    
    [SerializeField] private Vector2 slotOffset;
    
    
    [SerializeField] private Color mainColor,cookedColor,burnedColor;
    private MeshRenderer _waterMesh;

    private Ingredient _stockIngredient;
    private Color _mainIngredientColor,_mainCookedColor;

    private MeshRenderer _meshIngredient;
    private MeshRenderer _cookedMesh;

    private Image _slot;
    
    [SerializeField] protected float timeToBurn;
    #endregion
    
    #region Unity Functions
    
    protected override void Start() {
        base.Start();
        
        _waterMesh = transform.GetChild(transform.childCount - 2).GetComponent<MeshRenderer>();
        _waterMesh.material.color = mainColor;
        
        InitUI();
    }
    
    #endregion
    
    #region Make Box Function
    
    protected override void StartMake() {
        
        boxSlider.gameObject.SetActive(true);
        
        _stockIngredient = stock.GetComponent<Ingredient>();
        _meshIngredient = _stockIngredient.basic.GetComponent<MeshRenderer>();
        _cookedMesh = _stockIngredient.modified.GetComponent<MeshRenderer>();
        _mainIngredientColor = _meshIngredient.material.color;
        _mainCookedColor = _cookedMesh.material.color;

        _slot.gameObject.SetActive(true);
        _slot.sprite = _stockIngredient.Data.Sprite;

        StartCoroutine(LerpColor(_waterMesh,mainColor,cookedColor,timeToSucceed));
        StartCoroutine(LerpColor(_meshIngredient, _mainIngredientColor, _mainCookedColor,timeToSucceed));
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
        RunDelayed(2f, () =>
        {
            if (stock == null || _stockIngredient.IsBurn)
            {
                isBurning = false;
                return;
            }

            isBurning = true;
            burnTimer = 0f;
            StartCoroutine(LerpColor(_waterMesh, cookedColor, burnedColor, timeToBurn));
        });
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
            
        if (burnTimer >= timeToBurn)
        {
            FinishBurn();
        }
    }

    protected override void FinishBurn()
    {
        isBurning = false;
        _stockIngredient.IsBurn = true;
    }

    #endregion
    
    #region Basic Box Function
    protected override void Put() {
        if (stock == null && currentController.ActualIngredient != null && currentController.ActualIngredient.TryGetComponent(out Ingredient ingredient)) {
            if (ingredient.Data.IsPan) { // detect if ingredient is cooked  on pan
                if (ingredient.Data.CanBeCook && !ingredient.IsCook) 
                    base.Put();
            }
        }
        
    }

    protected override void Take() {
        if (stock != null) {
            if (stock.TryGetComponent<Ingredient>(out Ingredient ingredient) && ingredient.IsCook) {
                _slot.gameObject.SetActive(false);
                base.Take();
            }
        }
    }
    
    #endregion

    private void InitUI() {
        GameObject slot = Instantiate(_cookController.slotPrefab);
        slot.transform.parent = canvas.transform;

        Vector3 screenPosition = transform.parent.GetComponentInChildren<Camera>().WorldToScreenPoint(transform.position);
        screenPosition += (Vector3)slotOffset;
        slot.transform.position = screenPosition;
        _slot = slot.GetComponent<Image>();
        slot.gameObject.SetActive(false);
        
    }

    private IEnumerator LerpColor(MeshRenderer renderer,Color start,Color end,float time) {
        for (float f = 0; f <= time; f += Time.deltaTime) {
            renderer.material.color = Color.Lerp(start,end,f / time);
            yield return null;
        }
    }
    
}
