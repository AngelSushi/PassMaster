using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "Recipe", menuName = "Cuisine/Recipe", order = 1)]
public class Recipe : ScriptableObject {

    public List<DataIngredient> ingredients;
    public Sprite recipeSprite;
    public float disapearTime;
    public int reward;
    public GameController.Difficulty difficulty;
    public bool isFurnacable;
}
