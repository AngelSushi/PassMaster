using System;
using UnityEngine;
using Recipes;

public class RecipeEvents : MonoBehaviour
{
    public EventHandler<OnRecipeEndTimerArgs> OnRecipeEndTimer;

    public class OnRecipeEndTimerArgs : EventArgs
    {
        private Recipe _recipe;
        private CookController.Team _team;

        public Recipe Recipe
        {
            get => _recipe;
            set => _recipe = value;
        }

        public CookController.Team Team
        {
            get => _team;
            set => _team = value;
        }

        public OnRecipeEndTimerArgs(Recipe recipe, CookController.Team team)
        {
            _recipe = recipe;
            _team = team;
        }
    }


    public EventHandler<OnRecipeDeliveredArgs> OnRecipeDelivered;

    public class OnRecipeDeliveredArgs : EventArgs
    {
        private Recipe _recipe;
        private CookController.Team _team;
        private GameObject _player;

        public Recipe Recipe
        {
            get => _recipe;
            private set => _recipe = value;
        }

        public CookController.Team Team
        {
            get => _team;
            private set => _team = value;
        }

        public GameObject Player
        {
            get => _player;
            private set => _player = value;
        }


        public OnRecipeDeliveredArgs(Recipe recipe, CookController.Team team, GameObject player)
        {
            _recipe = recipe;
            _team = team;
            _player = player;
        }
    }
}
