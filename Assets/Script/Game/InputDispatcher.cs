using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputDispatcher : MonoBehaviour
{

    private PlayerInputActions _playerInputActions;

    private LocalMultiSetup _instanceMulti;
    private GameController _instance;

    private GameObject ActualPlayer
    {
        get
        {
            return _instance.players[_instance.actualPlayer];
        }
    }

    private void Awake()
    {
        _playerInputActions = new PlayerInputActions();
        _instanceMulti = LocalMultiSetup.Instance;
        DontDestroyOnLoad(transform.gameObject);
    }

    public void EnableDispatch()
    {
        foreach (LocalMultiSetup.Player player in _instanceMulti.Players)
        {
            player.Input.onActionTriggered += OnActionTriggered;
        }
        
        _instance = GameController.Instance;
    }

    private void OnActionTriggered(InputAction.CallbackContext e)
    {
        if (ActualPlayer == null)
        {
            return;
        }
        
        UserMovement movement = ActualPlayer?.GetComponent<UserMovement>();

        if (movement == null)
        {
            return;
        }
        
        LocalMultiSetup.Player targetInputPlayer = _instanceMulti.Players.FirstOrDefault(player => player.PlayerInstance == ActualPlayer);
        
        if (targetInputPlayer == null || !targetInputPlayer.Input.devices.Contains(e.control.device))
        {
            return;
        }
        
        if (e.action.name == _playerInputActions.Menus.Right.name && e.started)
        {
            if (movement.currentAction == UserAction.MENU)
            {
                movement.ui.CurrentNavigationSystem.OnRight(e);
            }    
        }

        if (e.action.name == _playerInputActions.Menus.Left.name && e.started)
        {
            if (movement.currentAction == UserAction.MENU)
            {
                movement.ui.CurrentNavigationSystem.OnLeft(e);
            }
        }

        if (e.action.name == _playerInputActions.Player.Jump.name)
        {
            if (movement.currentAction == UserAction.DICE)
            {
                movement.OnJump(e);
            }
        }
    }
}
