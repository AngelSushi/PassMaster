using System.Collections;
using System.Collections.Generic;
using UnityEngine;



[CreateAssetMenu(fileName = "Ingredient", menuName = "Cuisine/Ingredient", order = 1)]
public class Ingredient : ScriptableObject {

    public bool isCuttable;
    public bool isCut;
    public bool isCookable;
    public bool isCook;
    public Sprite sprite;
    public string cookTag;
    public string name;
    public GameObject ingredientPrefab;
    public GameObject ingredientCutPrefab;
    public GameObject ingredientCookPrefab;

}
