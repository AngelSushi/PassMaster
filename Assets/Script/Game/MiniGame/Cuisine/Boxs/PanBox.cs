using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PanBox : MakeBox {
    
    #region Variables
    
    public Vector2 slotOffset;
    public Color mainColor,cookedColor;
    private MeshRenderer _waterMesh;

    private Ingredient _stockIngredient;
    private Color _mainIngredientColor,_mainCookedColor;
    private MeshRenderer _meshIngredient;
    private MeshRenderer _cookedMesh;

    private bool _isDirtyWater;
    private Image _slot;
    
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
        if (_isDirtyWater)
            return;
        
        boxSlider.gameObject.SetActive(true);
        
        _stockIngredient = stock.GetComponent<Ingredient>();
        _meshIngredient = _stockIngredient.basic.GetComponent<MeshRenderer>();
        _cookedMesh = _stockIngredient.modified.GetComponent<MeshRenderer>();
        _mainIngredientColor = _meshIngredient.material.color;
        _mainCookedColor = _cookedMesh.material.color;
        _isDirtyWater = true;

        _slot.gameObject.SetActive(true);
        _slot.sprite = _stockIngredient.sprite;

        StartCoroutine(LerpColor(_waterMesh,mainColor,cookedColor));
        StartCoroutine(LerpColor(_meshIngredient, _mainIngredientColor, _mainCookedColor));
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
    
    #region Basic Box Function
    protected override void Put() {
        if (stock == null && currentController.actualIngredient != null && currentController.actualIngredient.TryGetComponent<Ingredient>(out Ingredient ingredient)) {
            if (ingredient.cookIndex == 1) { // detect if ingredient is cooked  on pan
                if (ingredient.isCookable && !ingredient.isCook && !_isDirtyWater) 
                    base.Put();
            }
        }
        
    }

    protected override void Take() {
        if (stock != null) {
            if (stock.TryGetComponent<Ingredient>(out Ingredient ingredient) && ingredient.isCook) {
                _slot.gameObject.SetActive(false);
                base.Take();
            }
        }
        else if (stock == null && currentController.actualIngredient == null) {
            if (_isDirtyWater) {
                _waterMesh.material.color = mainColor;
                _isDirtyWater = false;
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

    private IEnumerator LerpColor(MeshRenderer renderer,Color start,Color end) {
        for (float f = 0; f <= timeToSucceed; f += Time.deltaTime) {
            renderer.material.color = Color.Lerp(start,end,f / timeToSucceed);
            yield return null;
        }
    }
    
}