using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CookController : MiniGame {
    
    [HideInInspector] public RecipeController recipeController;
    public List<Camera> camerasInstance;
    public GameObject sliderPrefab;
    public GameObject slotPrefab;

    public override void Start() {
        base.Start();

        recipeController = GetComponent<RecipeController>();
    }
    
    public override void OnStartCinematicEnd() {}
    public override void OnFinish() { }
    public override void OnSwitchCamera() { }
    public override void OnTransitionEnd() { }
}
