using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ChiefController : CoroutineSystem
{
    
    [Header("Movement")]
    [SerializeField] private bool isMooving;

    public bool IsMooving
    {
        get => isMooving;
        set => isMooving = value;
    }
    
    [SerializeField] private bool canMoove;

    public bool CanMoove
    {
        get => canMoove;
        protected set => canMoove = value;
    }
    
    [Header("Inventory")]
    [SerializeField] private GameObject actualIngredient;

    public GameObject ActualIngredient
    {
        get => actualIngredient;
        set => actualIngredient = value;
    }
    
    [SerializeField] private GameObject actualPlate;

    public GameObject ActualPlate
    {
        get => actualPlate;
        set => actualPlate = value;
    }
    
    [SerializeField] private Transform ingredientSpawn;

    public Transform IngredientSpawn
    {
        get => ingredientSpawn;
        private set => ingredientSpawn = value;
    }
    
}
