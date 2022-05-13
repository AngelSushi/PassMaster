using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CookController : MiniGame {

    public static CookController instance { get; private set; }

    [HideInInspector]
    public RecipeController recipeController;
    public Text scoreText;

    public int score;


    void Start() {
        instance = this;

        recipeController = GetComponent<RecipeController>();
    }

    void Update() {
        scoreText.text = "" + score;
    }

    public override void OnFinish() { }

}
