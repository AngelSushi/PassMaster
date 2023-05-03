using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public enum UserType {
    PLAYER,
    BOT_001,
    BOT_002,
    BOT_003
}

public enum UserAction {
    WAIT,
    MENU,
    DICE,
    MOVEMENT,
    MALUS,
    BONUS,
    SHOP,
    CHEST
    
}

public abstract class User : CoroutineSystem {
    public bool isTurn;
    public bool isPlayer;
    public UserMovement movement;
    public UserUI ui;
    public UserInventory inventory;
    public UserAudio audio;
    public GameController gameController;


    private bool lastIsTurn;
    private bool lastShowHUD;

    public virtual void Start() {
        gameController = GameController.Instance;
    }

    public virtual void Update() { 

        if(isTurn && !lastIsTurn) 
            OnBeginTurn();
        else if(!isTurn && lastIsTurn)
            OnFinishTurn();
        
        if(!ui.showHUD && lastShowHUD)
            OnDiceAction();

        lastIsTurn = isTurn;
        lastShowHUD = ui.showHUD;
    }

    public abstract void OnBeginTurn();
    public abstract void OnFinishTurn();
    public abstract void OnDiceAction(); // Call when player choose dice button on hud

}
