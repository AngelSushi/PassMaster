using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "DataIngredient", menuName = "Cuisine/DataIngredient", order = 1)]
public class DataIngredient : ScriptableObject {

    public bool isCuttable;
    public bool isCookable;
    public Sprite sprite;
    public string cookTag;
    public string name;
    public GameObject ingredientPrefab;
    public GameObject ingredientCutPrefab;
    public GameObject ingredientCookPrefab;

}
