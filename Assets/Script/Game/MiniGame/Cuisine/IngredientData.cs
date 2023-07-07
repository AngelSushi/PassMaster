using System.Collections;
using System.Collections.Generic;
using Microsoft.VisualStudio.CommandBars;
using UnityEngine;

[CreateAssetMenu(fileName = "Ingredient",menuName = "PassMaster/Ingredient")]
public class IngredientData : ScriptableObject {
  
    
    [SerializeField] private int id;

    public int Id
    {
        get => id;
        private set => id = value;
    }
    
    [SerializeField] private Sprite sprite;

    public Sprite Sprite
    {
        get => sprite;
        private set => sprite = value;
    }
    
    [Range(-1,1)] [Tooltip("-1 : Ignore ; 0 : Stove ; 1 : Pan")] [SerializeField] private int cookIndex;

    public int CookIndex
    {
        get => cookIndex;
        private set => cookIndex = value;
    }
    
    [SerializeField] private bool canBeCut;

    public bool CanBeCut
    {
        get => canBeCut;
        private set => canBeCut = value;
    }
    
    [SerializeField] private bool canBeCook;

    public bool CanBeCook
    {
        get => canBeCook;
        private set => canBeCook = value;
    }

    [SerializeField] private bool canBeBurn;

    public bool CanBeBurn
    {
        get => canBeBurn;
        private set => canBeBurn = value;
    }

    public bool IsPan
    {
        get
        {
            return cookIndex == 1;
        }
    }
}
