using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicBox : Box {

    protected Vector3 stockPosition;
    
    [SerializeField]
    protected GameObject stock;

    public GameObject Stock
    {
        get => stock;
        private set => stock = value;
    }

    protected override void Start() {
        base.Start();
        stockPosition = transform.GetChild(0).localPosition;
    }

    protected override void Put() {
        
        if (currentController.ActualIngredient != null || currentController.ActualPlate != null)
        {

            if (currentController.ActualIngredient != null) {
                if (stock != null && stock.GetComponent<Plate>() != null) {
                    Plate plate = stock.GetComponent<Plate>();
                    plate.AddIngredient(currentController.ActualIngredient.GetComponent<Ingredient>(),this,currentController);
                }
                else {
                    GameObject ingredientObj = currentController.ActualIngredient;
                    ingredientObj.transform.parent = transform;
                    ingredientObj.transform.localPosition = stockPosition;
            
                    currentController.ActualIngredient = null;
                    stock = ingredientObj;
                }
            }
            else {
                GameObject plate = currentController.ActualPlate;
                plate.transform.parent = transform;
                plate.transform.localPosition = stockPosition;
            
                currentController.ActualPlate = null;
                stock = plate;
            }

            
            audioSource.Play();
            
            
            CookEvents.OnUpdateBoxStockArgs e = new CookEvents.OnUpdateBoxStockArgs(Stock, this);
            _cookController.CookEvents.OnUpdateBoxStock?.Invoke(this, e);
        }
    }

    protected override void Take() {
        if (stock != null && (currentController.ActualIngredient == null && currentController.ActualPlate == null))
        {
            stock.transform.parent = currentController.transform;
            stock.transform.localPosition = currentController.IngredientSpawn.localPosition;
            
            if(stock.GetComponent<Ingredient>() != null)
                currentController.ActualIngredient = stock;
            else if (stock.GetComponent<Plate>() != null)
                currentController.ActualPlate = stock;
            
            stock = null;
            
            
            audioSource.Play();
            
            
            CookEvents.OnUpdateBoxStockArgs e = new CookEvents.OnUpdateBoxStockArgs(Stock, this);
            _cookController.CookEvents.OnUpdateBoxStock?.Invoke(this,e);
        }
    }
}
