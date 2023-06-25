using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class AIAction {

    [Serializable]
    public class AITask
    {
        [SerializeField] private Grid.Tile _start;

        public Grid.Tile Start
        {
            get => _start;
            private set => _start = value;
        }
        
        [SerializeField] private Grid.Tile _end;

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

    [SerializeField] private List<AITask> _tasks;

    public List<AITask> Tasks
    {
        get => _tasks;
        private set => _tasks = value;
    }
    
    
    [SerializeField] private bool _isSucceed;

    public bool IsSucceed
    {
        get => _isSucceed;
        private set => _isSucceed = value;
    }

    [SerializeField] private IngredientData _actionOn;

    public IngredientData ActionOn
    {
        get => _actionOn;
        private set => _actionOn = value;
    }

    [SerializeField] private int _priority;

    public int Priority
    {
        get => _priority;
        set => _priority = value;
    }
    
    
    public AIAction(IngredientData actionOn)
    {
        _actionOn = actionOn;
        _tasks = new List<AITask>();
        _isSucceed = false;
    }
}
