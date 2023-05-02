using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Ingredient",menuName = "PassMaster/Ingredient")]
public class IngredientData : ScriptableObject {
  
    
    public int id;
    public Sprite sprite;
    [Range(-1,1)] [Tooltip("-1 : Ignore ; 0 : Stove ; 1 : Pan")] public int cookIndex; 
    
    
    public bool isCuttable;
    public bool isCookable;
}
