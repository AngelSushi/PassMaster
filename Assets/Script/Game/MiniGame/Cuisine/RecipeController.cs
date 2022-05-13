using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RecipeController : MonoBehaviour {

    public List<Recipe> recipes;
    public Recipe currentRecipe;
    public GameObject recipeUI;

    private int difficultyRange = 50;

    void Start() {
        ChooseRecipe();
    }

    private void ChooseRecipe() {
        int randomValue = Random.Range(0, 100);

        if (randomValue >= difficultyRange) { 
            List<Recipe> difficultyRecipes = recipes.FindAll(recipe => recipe.difficulty == GameController.difficulty);
            currentRecipe = difficultyRecipes[Random.Range(0, difficultyRecipes.Count)];
        }
        else {
            GameController.Difficulty[] remainingDifficulties = FindOtherDifficulties();
            int randomDifficulty = Random.Range(0, remainingDifficulties.Length);

            List<Recipe> difficultyRecipes = recipes.FindAll(recipe => recipe.difficulty == remainingDifficulties[randomDifficulty]);
            currentRecipe = difficultyRecipes[Random.Range(0, difficultyRecipes.Count)];
        }

        DisplayRecipeUI();
    }

    private void DisplayRecipeUI() {
        Transform slotParent = recipeUI.transform.GetChild(0);

        if (currentRecipe.ingredients.Count > slotParent.childCount)
            AddSlot(slotParent);
        else
            RemoveSlot(slotParent);

        for(int i = 0; i < currentRecipe.ingredients.Count; i++)
            slotParent.GetChild(i).GetChild(0).gameObject.GetComponent<Image>().sprite = currentRecipe.ingredients[i].sprite;
        
        recipeUI.transform.GetChild(1).GetChild(0).gameObject.GetComponent<Image>().sprite = currentRecipe.recipeSprite;
    }

    private void AddSlot(Transform slotParent) {
        for(int i = 0;i <= currentRecipe.ingredients.Count - slotParent.childCount;i++) {
            GameObject newSlot = Instantiate(slotParent.GetChild(slotParent.childCount - 1).gameObject);
            newSlot.transform.parent = slotParent;
        }
    }

    private void RemoveSlot(Transform slotParent) {
        for(int i = slotParent.childCount - 1;i >= 0;i--) {
            if (i >= currentRecipe.ingredients.Count)
                Destroy(slotParent.GetChild(i).gameObject);
            else
                break;
        }
    }

    private GameController.Difficulty[] FindOtherDifficulties() {
        switch(GameController.difficulty) {
            case GameController.Difficulty.EASY:
                return new GameController.Difficulty[] { GameController.Difficulty.MEDIUM,GameController.Difficulty.HARD};

            case GameController.Difficulty.MEDIUM:
                return new GameController.Difficulty[] { GameController.Difficulty.EASY, GameController.Difficulty.HARD };

            case GameController.Difficulty.HARD:
                return new GameController.Difficulty[] { GameController.Difficulty.EASY, GameController.Difficulty.MEDIUM };
        }

        return null;
    }
}
