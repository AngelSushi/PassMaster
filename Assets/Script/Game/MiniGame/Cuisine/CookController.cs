using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class CookController : MiniGame {

    [System.Serializable]
    public class Team {
        public string name;
        public GameObject player;
        public int point;
        public List<RecipeController.Recipe> recipes;
        private int _deliveredRecipes;
        public GameObject instance;
        public Canvas canvas;

        private float _reputation = 1;
        public float reputation
        {
            get => _reputation;
            set {
                _reputation = Mathf.Clamp(value, 0.5f, 2);
            }
            
        }

        private float _timer;
        private CookController _cookController;

        public bool HasRecipe(string recipeName) {
            foreach (RecipeController.Recipe recipe in recipes) {
                if (recipe.name.Equals(recipeName))
                    return true;
            }

            return false;
        }

        public void DeliverRecipe(RecipeController.Recipe removedRecipe) {
            removedRecipe.ticker.End();
            recipes.Remove(removedRecipe);
            _deliveredRecipes++;
        }

        public void Tick() {
            if (_cookController == null)
                _cookController = (CookController)CookController.instance;
            
            if (_deliveredRecipes >= 1) {
                _timer += Time.deltaTime;

                if (_timer >= (_cookController.recipeController.waitNextRecipeTimer + (_cookController.recipeController.waitNextRecipeTimer * (1 - reputation)))) {
                    _timer = 0;
                    Debug.Log("generate new recipe");
                    _cookController.recipeController.AutoGenerateRecipe(this);
                }
                
            }
        }
        
        
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
    public Transform[] playersUI;
    
    public Team[] teams;

    public int maxPointPerRecipe;
    
    /* TODO-LIST
     *
     * Gestion de livraison des commandes - Fait 
     * Système de recherche de recette/client (avec le système de réputation etc)
     * AI
     * Ajout de difficulté ( viande qui se crame , plat etc)
     * 
     */
    
    
    
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

    public void AddPoint(int point, GameObject player) {
        Team currentTeam = teams.Where(team => team.player == player).ToList()[0];
        currentTeam.point += point;
        
        Debug.Log("team " + currentTeam.name + " has " + currentTeam.point + " points ");

        List<int> allTeamsPoint = new List<int>();
        
        foreach(Team team in teams)
            allTeamsPoint.Add(team.point);

        allTeamsPoint.Sort();
        allTeamsPoint.Reverse();

        List<Player> classedPlayers = new List<Player>();

        foreach (int teamPoint in allTeamsPoint) {
            foreach (Team team in teams) {
                if (team.point == teamPoint) {
                    Player cPlayer = players.Where(p => p.gameObject.name == team.player.name).ToList()[0];
                
                    if(!classedPlayers.Contains(cPlayer))
                        classedPlayers.Add(cPlayer);    
                }
                
            }
        }

        for (int i = 0; i < classedPlayers.Count; i++) {
            playersUI[i].GetChild(2).gameObject.GetComponent<Text>().text = allTeamsPoint[i].ToString("D3");
            playersUI[i].GetChild(1).gameObject.GetComponent<Image>().sprite = classedPlayers[i].uiIcon;
        }
        
    }
    
    public override void OnStartCinematicEnd() {}
    public override void OnFinish() { }
    public override void OnSwitchCamera() { }
    public override void OnTransitionEnd() { }
}
