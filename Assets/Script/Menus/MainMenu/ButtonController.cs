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
    
    [Serializable]
    public class Menu
    {
        [SerializeField] private float id;
        [SerializeField] private GameObject menu;
        [SerializeField] private List<Button> buttons;
        [SerializeField] private List<GameObject> uiElements;

        public float Id
        {
            get => id;
        }

        public List<Button> Buttons
        {
            get => buttons;
        }

        public List<GameObject> UiElements
        {
            get => uiElements;
        }

        public void SetVisible(bool visible)
        {
            menu.SetActive(visible);
            
            foreach (Button btn in buttons)
            {
                btn.ButtonObj.SetActive(visible);

                if (!btn.IsActive)
                {
                    btn.ButtonObj.GetComponent<Image>().color = Color.gray;
                }
            }
            
            uiElements.ForEach(uiElement => uiElement.SetActive(visible));
        }
    }
    
    [Serializable]
    public class Button
    {
        [SerializeField] private GameObject buttonObj;
        [SerializeField] private bool isActive;
        [SerializeField] private Sprite font;

        public GameObject ButtonObj
        {
            get => buttonObj;
        }

        public bool IsActive
        {
            get => isActive;
        }

        public Sprite Font
        {
            get => font;
        }
    }
    
    [SerializeField] private float menuId;

    public float MenuId
    {
        get => menuId;
        set
        {
            TargetMenu.SetVisible(false);
            TargetMenu.Buttons[_index].ButtonObj.GetComponent<Image>().sprite = _normal;
            menuId = value;
            TargetMenu.SetVisible(true);
            _index = -1;
        }
    }

    [SerializeField] private List<Menu> menus;

    private Menu TargetMenu
    {
        get => menus.First(menu => menu.Id == menuId);
    }

    public GameObject minigame;

    private string _menuName;

    public static bool relaunch;
    public GameObject relaunchObj;
    public AudioSource start;

    private bool _hasShowMenu;

    public GameObject champSelect;

    [SerializeField] private GameObject logo;
    [SerializeField] private GameObject interactText;
    [SerializeField] private GameObject buttonBackground;


    private int _index = -1;
    
    [SerializeField] private Sprite hover;
    private Sprite _normal;


    [SerializeField] private PlayerInputManager inputManager;
    
    private void Start() {
        _normal = TargetMenu.Buttons.FirstOrDefault()?.ButtonObj.GetComponent<Image>().sprite;
    }

    public void OnNext(InputAction.CallbackContext e)
    {
        if (e.started && _hasShowMenu)
        {
            if (_index < TargetMenu.Buttons.Count - 1)
            {
                if (_index >= 0)
                {
                    TargetMenu.Buttons[_index].ButtonObj.GetComponent<Image>().sprite = _normal;
                }
                
                _index = TargetMenu.Buttons.FindIndex(btn => TargetMenu.Buttons.IndexOf(btn) > _index && btn.IsActive);
                TargetMenu.Buttons[_index].ButtonObj.GetComponent<Image>().sprite = hover;

                if (!buttonBackground.activeSelf)
                {
                    buttonBackground.SetActive(true);
                }

                buttonBackground.GetComponent<Image>().sprite = TargetMenu.Buttons[_index].Font;
            }
        }
    }

    public void OnPrevious(InputAction.CallbackContext e)
    {
        if (e.started && _hasShowMenu)
        {
            if (_index > 0)
            {
                TargetMenu.Buttons[_index].ButtonObj.GetComponent<Image>().sprite = _normal;

                _index = TargetMenu.Buttons.FindLastIndex(TargetMenu.Buttons.Count - 1,btn => TargetMenu.Buttons.IndexOf(btn) < _index && btn.IsActive);


                TargetMenu.Buttons[_index].ButtonObj.GetComponent<Image>().sprite = hover;
                buttonBackground.GetComponent<Image>().sprite = TargetMenu.Buttons[_index].Font;
            }
        }
    }
    
    public void BackButton() {
        switch(menuId) {
            case 1:
                minigame.SetActive(false);
                menuId = 0;
                break;
            case 2:
                menuId = 0;
                break;
            case 3:
                minigame.SetActive(false);
                menuId = 0;
                break;        
        }
    }

    public void OnShowMenu(InputAction.CallbackContext e) { // Called when player hit E or "X" 
        if(e.started) {
            if (!_hasShowMenu)
            {
                _hasShowMenu = true;
                start.Play();
                logo.SetActive(false);
                interactText.SetActive(false);   
                buttonBackground.SetActive(false);
                TargetMenu.SetVisible(true);
            }
            else
            {
                Debug.Log("menuID " + MenuId);
                SwitchMenu();
            }
        }
    }

    private void SwitchMenu()
    {
        switch (menuId)
        {
            case 0:
                switch (_index)
                {

                    case 0: // Button Board
                        MenuId = 1f;
                        break;
                    case 6: // Button Quit
                        Application.Quit();
                        break;
                }
                break;
            
            case 1:
                switch (_index)
                {
                    case 0: // Local
                        MenuId = 1.1f;
                        FindObjectsOfType<PlayerInput>().ToList().ForEach(input => input.enabled = false);
                        FindObjectOfType<EventSystem>().enabled = false;
                        
                        RunDelayed(0.3f, () =>
                        {
                            inputManager.enabled = true;
                        });
                        
                        break;
                    case 1: // Multi
                        break;
                    case 2: // Return
                        MenuId = 0f;
                        break;
                }
                break;
        }
    }

    public void RelaunchGame() {
        GameObject.FindGameObjectsWithTag("Game")[0].GetComponent<GameController>().ChangeStateScene(true,"NewMain");
        SceneManager.UnloadSceneAsync("MainMenu");
    }
}
