using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public class ButtonController : CoroutineSystem {

    [SerializeField] private int menuId;

    public int MenuId
    {
        get => menuId;
        set
        {
            
            Debug.Log("change value");
            
            menus[menuId].SetActive(false);
            menuId = value;
            menus[menuId].SetActive(true);
            
        }
    }

    [SerializeField] private List<GameObject> menus;

    

    private string _menuName;

    public static bool relaunch;

    private bool _hasShowMenu;


    private int _index = -1;
    
    private Sprite _normal;
    

    [SerializeField] private PlayerInputManager inputManager;


    public void SetupLocal()
    {
        FindObjectsOfType<PlayerInput>().ToList().ForEach(input => input.enabled = false);
        FindObjectOfType<EventSystem>().enabled = false;
                        
        RunDelayed(0.3f, () =>
        {
            inputManager.enabled = true;
        });
    }

    public void RelaunchGame() {
        GameObject.FindGameObjectsWithTag("Game")[0].GetComponent<GameController>().ChangeStateScene(true,"NewMain");
        SceneManager.UnloadSceneAsync("MainMenu");
    }
}
