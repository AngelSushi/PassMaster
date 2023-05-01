using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RecipeController : MonoBehaviour {

    [System.Serializable]
    public class Recipe {
        public string name;
        public List<int> allIngredientsID;
        public Sprite recipeSprite;
        public bool needToBeCook;
        public bool isCooked;
        public GameObject recipeMesh;
    }
    
    public List<Recipe> recipes;

    public void GenerateRecipe() {
        
    }
    
    
}
