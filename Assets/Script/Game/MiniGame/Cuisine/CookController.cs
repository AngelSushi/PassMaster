using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CookController : MiniGame {

    [System.Serializable]
    public class Team {
        public string name;
        public GameObject player;
        public int point;
        public List<RecipeController.Recipe> recipes;
        public GameObject instance;
    }
    
    [HideInInspector] public RecipeController recipeController;
    public List<Camera> camerasInstance;
    public GameObject sliderPrefab;
    public GameObject slotPrefab;
    public Team[] teams;
    
    public override void Start() {
        base.Start();

        recipeController = GetComponent<RecipeController>();
    }
    
    public override void OnStartCinematicEnd() {}
    public override void OnFinish() { }
    public override void OnSwitchCamera() { }
    public override void OnTransitionEnd() { }
}
