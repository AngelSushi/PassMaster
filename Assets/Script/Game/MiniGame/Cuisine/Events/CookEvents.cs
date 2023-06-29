using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CookEvents : MonoBehaviour
{

    public EventHandler<OnUpdateBoxStockArgs> OnUpdateBoxStock;
    public class OnUpdateBoxStockArgs : EventArgs
    {
        public GameObject stock;
        public BasicBox box;
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
}
