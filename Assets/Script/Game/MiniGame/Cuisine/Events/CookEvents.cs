using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CookEvents : MonoBehaviour
{

    public EventHandler<OnUpdateBoxStockArgs> OnUpdateBoxStock;
    public class OnUpdateBoxStockArgs : EventArgs
    {
        private GameObject _stock;
        private BasicBox _box;
        private bool _isCanceled;

        public GameObject Stock
        {
            get => _stock;
            set => _stock = value;
        }

        public BasicBox Box
        {
            get => _box;
            set => _box = value;
        }

        public bool IsCanceled
        {
            get => _isCanceled;
            set => _isCanceled = value;
        }
        
        public OnUpdateBoxStockArgs(GameObject stock, BasicBox box)
        {
            _stock = stock;
            _box = box;
            _isCanceled = false;
        }
    }

    public EventHandler<OnFinishedCutIngredientArgs> OnFinishedCutIngredient;

    public class OnFinishedCutIngredientArgs : EventArgs
    {
        private CutBox _box;
        private Ingredient _ingredient;
        private GameObject _cutter;

        public CutBox Box
        {
            get => _box;
            set => _box = value;
        }

        public Ingredient Ingredient
        {
            get => _ingredient;
            set => _ingredient = value;
        }

        public GameObject Cutter
        {
            get => _cutter;
            set => _cutter = value;
        }

        public OnFinishedCutIngredientArgs(CutBox box, Ingredient ingredient, GameObject cutter)
        {
            _box = box;
            _ingredient = ingredient;
            _cutter = cutter;
        }
    }

    public EventHandler<OnFinishedCookIngredientArgs> OnFinishedCookIngredient;

    public class OnFinishedCookIngredientArgs : EventArgs
    {
        private GameObject _cooker;
        private MakeBox _box;
        private Ingredient _ingredient;
        private AIAction _action;

        public GameObject Cooker
        {
            get => _cooker;
            set => _cooker = value;
        }

        public MakeBox Box
        {
            get => _box;
            set => _box = value;
        }

        public Ingredient Ingredient
        {
            get => _ingredient;
            set => _ingredient = value;
        }

        public AIAction Action
        {
            get => _action;
            private set => _action = value;
        }

        public OnFinishedCookIngredientArgs(GameObject cooker, MakeBox box, Ingredient ingredient,AIAction action)
        {
            _cooker = cooker;
            _box = box;
            _ingredient = ingredient;
            _action = action;
        }
    }

    public EventHandler<OnPutIngredientInPlateArgs> OnPutIngredientInPlate;

    public class OnPutIngredientInPlateArgs : EventArgs
    {
        
        private BasicBox _box;
        private Ingredient _ingredient;
        private List<RecipeController.Recipe> _allRecipes;
        private bool _isFull;
        private ChiefController _from;

        public BasicBox Box
        {
            get => _box;
            private set => _box = value;
        }

        public Ingredient Ingredient
        {
            get => _ingredient;
            private set => _ingredient = value;
        }

        public List<RecipeController.Recipe> AllRecipes
        {
            get => _allRecipes;
            private set => _allRecipes = value;
        }

        public bool IsFull
        {
            get => _isFull;
            private set => _isFull = value;
        }

        public ChiefController From
        {
            get => _from;
            private set => _from = value;
        }

        public OnPutIngredientInPlateArgs(BasicBox box, Ingredient ingredient, List<RecipeController.Recipe> allRecipes,bool isFull,ChiefController from)
        {
            _box = box;
            _ingredient = ingredient;
            _allRecipes = allRecipes;
            _isFull = isFull;
            _from = from;
        }
    }
}
