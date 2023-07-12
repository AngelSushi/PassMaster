using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class AIAction {

    [Serializable]
    public class AITask
    {
        [SerializeField] private Grid.Tile start;

        public Grid.Tile Start
        {
            get => start;
            set => start = value;
        }
        
        [SerializeField] private Grid.Tile end;

        public Grid.Tile End
        {
            get => end;
            set => end = value;
            
        }

        [SerializeField] private bool isFinished;

        public bool IsFinished
        {
            get => isFinished;
            set => isFinished = value;
        }

        public AITask(Grid.Tile start, Grid.Tile end)
        {
            this.start = start;
            this.end = end;
        }
    }

    [SerializeField] private List<AITask> tasks;

    public List<AITask> Tasks
    {
        get => tasks;
        private set => tasks = value;
    }
    
    
    [SerializeField] private bool isFinished;

    public bool IsFinished
    {
        get => isFinished;
        set => isFinished = value;
    }

    [SerializeField] private IngredientData actionOn;

    public IngredientData ActionOn
    {
        get => actionOn;
        private set => actionOn = value;
    }

    [SerializeField] private int priority;

    public int Priority
    {
        get => priority;
        set => priority = value;
    }

    [SerializeField,Tooltip("The AIAction that must be finished to unlock this action")] private bool[] condition;

    public bool[] Condition
    {
        get => condition;
        private set => condition = value;
    }
    
    [SerializeField] private string name; // Just for debug

    public string Name
    {
        get => name;
        set => name = value;
    }


    public AIAction(IngredientData actionOn)
    {
        this.actionOn = actionOn;
        tasks = new List<AITask>();
        isFinished = false;

        condition = new bool[] { true };
    }

    public AIAction(IngredientData actionOn, bool[] condition)
    {
        this.actionOn = actionOn;
        tasks = new List<AITask>();
        isFinished = false;

        this.condition = condition;
    }
    
}
