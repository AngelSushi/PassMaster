using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ChiefController : CoroutineSystem
{
    
    public GameObject actualIngredient;
    public GameObject actualPlate;
    public Transform ingredientSpawn;
 
    
    public bool isMoving;
    public bool canMoove;
    
}
