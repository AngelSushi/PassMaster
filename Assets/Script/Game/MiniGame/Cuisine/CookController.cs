using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
<<<<<<< HEAD
using UnityEngine.UI;

public class CookController : MiniGame {

    public static CookController instance { get; private set; }

    [HideInInspector]
    public RecipeController recipeController;
    public Text scoreText;

    public Dictionary<GameObject,int> playerScore;
    

    /*
     * 
     * 
     * 
     * 
     */

    public void Awake() {
 
        instance = this;

        recipeController = GetComponent<RecipeController>();
        playerScore = new Dictionary<GameObject, int>();


        // Ajouter les joueurs dès le lancement
    }

    public override void Update() {
        //scoreText.text = "" + score;
        foreach(GameObject player in playerScore.Keys)
        {
            scoreText.text = "" + playerScore[player];
        }


        base.Update();
    }

    public override void OnFinish() { }

=======
using UnityEngine.InputSystem;

public class CookController : MiniGame {

    [System.Serializable]
    public class Team {
        public string name;
        public GameObject player;
        public int point;
        public List<RecipeController.Recipe> recipes;
        public GameObject instance;
        public Canvas canvas;
    }

    [System.Serializable]
    public class Instance {
        public Camera instanceCamera;
        public List<Canvas> allCanvas;
        public GameObject instance;
    }
    
    [HideInInspector] public RecipeController recipeController;
    
    public List<Instance> instances;
    
    // UI
    public GameObject sliderPrefab;
    public GameObject slotPrefab;
    public GameObject recipePrefab;
    public GameObject recipeParent;
    public GameObject platePrefab;
    
    
    public Team[] teams;
    
    public override void Start() {
        base.Start();

        recipeController = GetComponent<RecipeController>();

        
        foreach (Team team in teams) {
            GameObject instanceCanvas = new GameObject("Instance Canvas");
            instanceCanvas.transform.parent = team.instance.transform;
            Canvas canvas = instanceCanvas.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;

            team.canvas = canvas;
            Instance targetInstance = instances.Where(instance => instance.instance == team.instance).ToList()[0];
            targetInstance.allCanvas.Add(canvas);
            
            
            Instantiate(recipeParent, instanceCanvas.transform);
        }
    }
    
    public override void OnStartCinematicEnd() {}
    public override void OnFinish() { }
    public override void OnSwitchCamera() { }
    public override void OnTransitionEnd() { }
>>>>>>> main
}
