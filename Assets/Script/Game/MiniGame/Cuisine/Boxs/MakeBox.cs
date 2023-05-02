using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public abstract class MakeBox : BasicBox {

    protected float timer;
    protected Slider boxSlider;
    
    public float timeToSucceed;

    public Vector2 sliderOffset;

    private GameObject _lastStock;

    protected GameObject canvas;

    protected virtual void Start() {
        base.Start();
        canvas = new GameObject("Box Canvas");
        canvas.transform.parent = transform;
        canvas.AddComponent<Canvas>();
        canvas.AddComponent<CanvasScaler>();
        canvas.AddComponent<GraphicRaycaster>();
        canvas.GetComponent<Canvas>().renderMode = RenderMode.ScreenSpaceOverlay;

        CookController.Instance targetInstance = _cookController.instances.Where(instance => instance.instance == transform.parent.gameObject).ToList()[0];
        targetInstance.allCanvas.Add(canvas.GetComponent<Canvas>());

        GameObject slider = Instantiate(_cookController.sliderPrefab);
        slider.transform.parent = canvas.transform;
        Vector3 screenPosition = transform.parent.GetComponentInChildren<Camera>().WorldToScreenPoint(transform.position);
        screenPosition += (Vector3)sliderOffset;
        
        slider.transform.position = screenPosition;
        slider.SetActive(false);
        boxSlider = slider.GetComponent<Slider>();
    }

    protected void Update() {
        if(stock != null && _lastStock == null)
            StartMake();
        
        if (stock != null && timer < timeToSucceed) 
            Make();

        _lastStock = stock;
    }

    protected abstract void StartMake();
    protected abstract void Make(); // Called all the time when a player is doing an action on the box 
    protected abstract void FinishMake();
}
