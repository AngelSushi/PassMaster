using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIAction {

    public class AITask
    {
        private Grid.Tile _start;

        public Grid.Tile Start
        {
            get => _start;
            private set => _start = value;
        }
        
        private Grid.Tile _end;

        public Grid.Tile End
        {
            get => _end;
            private set => _end = value;
        }

        public AITask(Grid.Tile start, Grid.Tile end)
        {
            _start = start;
            _end = end;
        }
    }

    private List<AITask> _tasks;

    public List<AITask> Tasks
    {
        get => _tasks;
        private set => _tasks = value;
    }
    
    
    private bool _isSucceed;

    public bool IsSucceed
    {
        get => _isSucceed;
        private set => _isSucceed = value;
    }

    private IngredientData _actionOn;

    public IngredientData ActionOn
    {
        get => _actionOn;
        private set => _actionOn = value;
    }

    private float _priority;

    public float Priority
    {
        get => _priority;
        private set => _priority = value;
    }
    
    
    public AIAction(IngredientData actionOn)
    {
        _actionOn = actionOn;
        _tasks = new List<AITask>();
        _isSucceed = false;
    }
}
