using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class User : CoroutineSystem {

    public int id;
    public bool isTurn;
    public bool isPlayer;
    public UserMovement movement;
    public UserUI ui;
    public UserInventory inventory;
    public UserAudio audio;
    public GameController gameController;

    private bool lastIsTurn;

    public virtual void Start() { }

    public virtual void Update() { 

        if(isTurn && !lastIsTurn)
            OnBeginTurn();
        else
            OnFinishTurn();

        lastIsTurn = isTurn;
    }

    public abstract void OnBeginTurn();

    public abstract void OnFinishTurn();
}
