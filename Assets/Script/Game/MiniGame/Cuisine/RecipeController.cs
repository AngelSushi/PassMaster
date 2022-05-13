using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RecipeController : MonoBehaviour {

    public List<Recipe> recipes;
    public Recipe currentRecipe;

    private int difficultyRange = 50;

    private void Start() {
        int randomValue = Random.Range(0,100);
        
        if(randomValue >= difficultyRange) { // On random dans la difficulté
            List<Recipe> difficultyRecipes = recipes.FindAll(recipe => recipe.difficulty == GameController.difficulty);

            Debug.Log("size: " + difficultyRecipes.Count);
        }

    }
}
