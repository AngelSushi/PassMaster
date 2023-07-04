using System;
using System.Collections;
using System.Collections.Generic;
using Grid;
using UnityEngine;

public class AIEvents : MonoBehaviour
{

    public EventHandler<OnTaskStartedArgs> OnTaskStarted;

    public class OnTaskStartedArgs : EventArgs
    {
        private AIAction.AITask _task;
        private AIAction _action;
        private GameObject _ai;
        private CookController.Team _team;

        public AIAction.AITask Task
        {
            get => _task;
            private set => _task = value;
        }

        public AIAction Action
        {
            get => _action;
            private set => _action = value;
        }

        public GameObject AI
        {
            get => _ai;
            private set => _ai = value;
        }

        public CookController.Team Team
        {
            get => _team;
            private set => _team = value;
        }

        public OnTaskStartedArgs(AIAction.AITask task, AIAction action, GameObject ai, CookController.Team team)
        {
            _task = task;
            _action = action;
            _ai = ai;
            _team = team;
        }
    }
    
    public EventHandler<OnTaskFinishedArgs> OnTaskFinished;

    public class OnTaskFinishedArgs : EventArgs
    {
        private AIAction.AITask _task;
        private AIAction _action;
        private GameObject _ai;
        private CookController.Team _team;

        public AIAction.AITask Task
        {
            get => _task;
            private set => _task = value;
        }

        public AIAction Action
        {
            get => _action;
            private set => _action = value;
        }

        public GameObject AI
        {
            get => _ai;
            private set => _ai = value;
        }

        public CookController.Team Team
        {
            get => _team;
            private set => _team = value;
        }

        public OnTaskFinishedArgs(AIAction.AITask task, AIAction action, GameObject ai, CookController.Team team)
        {
            _task = task;
            _action = action;
            _ai = ai;
            _team = team;
        }
    }


    public EventHandler<OnTaskReachStartArgs> OnTaskReachStart;

    public class OnTaskReachStartArgs : EventArgs
    {
        private AIAction.AITask _task;
        private AIAction _action;
        private GameObject _ai;
        private CookController.Team _team;
        private Tile _tile;

        public AIAction.AITask Task
        {
            get => _task;
            private set => _task = value;
        }

        public AIAction Action
        {
            get => _action;
            private set => _action = value;
        }

        public GameObject AI
        {
            get => _ai;
            private set => _ai = value;
        }

        public CookController.Team Team
        {
            get => _team;
            private set => _team = value;
        }

        public Tile Tile
        {
            get => _tile;
            private set => _tile = value;
        }

        public OnTaskReachStartArgs(AIAction.AITask task, AIAction action, GameObject ai, CookController.Team team, Tile tile)
        {
            _task = task;
            _action = action;
            _ai = ai;
            _team = team;
            _tile = tile;
        }
    }

    public EventHandler<OnTaskReachEndArgs> OnTaskReachEnd;
    
    public class OnTaskReachEndArgs : EventArgs
    {
        private AIAction.AITask _task;
        private AIAction _action;
        private GameObject _ai;
        private CookController.Team _team;
        private Tile _tile;

        public AIAction.AITask Task
        {
            get => _task;
            private set => _task = value;
        }

        public AIAction Action
        {
            get => _action;
            private set => _action = value;
        }

        public GameObject AI
        {
            get => _ai;
            private set => _ai = value;
        }

        public CookController.Team Team
        {
            get => _team;
            private set => _team = value;
        }

        public Tile Tile
        {
            get => _tile;
            private set => _tile = value;
        }

        public OnTaskReachEndArgs(AIAction.AITask task, AIAction action, GameObject ai, CookController.Team team, Tile tile)
        {
            _task = task;
            _action = action;
            _ai = ai;
            _team = team;
            _tile = tile;
        }
    }


    public EventHandler<OnActionFinishedArgs> OnActionFinished;

    public class OnActionFinishedArgs : EventArgs
    {
        private GameObject _ai;
        private AIAction _action;
        private RecipeController.Recipe _recipe;

        public GameObject AI
        {
            get => _ai;
            set => _ai = value;
        }

        public AIAction Action
        {
            get => _action;
            set => _action = value;
        }

        public RecipeController.Recipe Recipe
        {
            get => _recipe;
            set => _recipe = value;
        }

        public OnActionFinishedArgs(GameObject ai, AIAction action, RecipeController.Recipe recipe)
        {
            _ai = ai;
            _action = action;
            _recipe = recipe;
        }
    }


    public EventHandler<OnChooseRecipeArgs> OnChooseRecipe;

    public class OnChooseRecipeArgs : EventArgs
    {
        private RecipeController.Recipe _recipe;
        private GameObject _ai;
        private bool _isCanceled;
        private List<AIAction> _recipeActions;

        public RecipeController.Recipe Recipe
        {
            get => _recipe;
            private set => _recipe = value;
        }

        public GameObject AI
        {
            get => _ai;
            private set => _ai = value;
        }

        public bool IsCanceled
        {
            get => _isCanceled;
            set => _isCanceled = value;
        }

        public List<AIAction> RecipeActions
        {
            get => _recipeActions;
            set => _recipeActions = value;
        }

        public OnChooseRecipeArgs(RecipeController.Recipe recipe, GameObject ai,List<AIAction> recipeActions)
        {
            _recipe = recipe;
            _ai = ai;
            _isCanceled = false;
            _recipeActions = recipeActions;
        }
    }
}
