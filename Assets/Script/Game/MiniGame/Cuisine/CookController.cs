using System.Collections;
using System.Collections.Generic;
using UnityEngine;
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

}
