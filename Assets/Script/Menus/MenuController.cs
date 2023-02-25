using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class MenuController : MonoBehaviour {

    [System.Serializable]
    public class ButtonItem {
        public GameObject subMenu;
        public GameObject button;
        public UnityEvent action;

        public ButtonItem(GameObject subMenu,GameObject button, UnityEvent action) {
            this.subMenu = subMenu;
            this.button = button;
            this.action = action;
        }
    }

    public List<ButtonItem> buttons = new List<ButtonItem>();
    private List<ButtonItem> _currentButtons = new List<ButtonItem>();
    
    private int _buttonIndex = -1;

    private int maxIndex {
        get {
            return _currentButtons.Count;
        } 
    }

    public int subMenus;

    public InputActionAsset inputs;
    public string actionName;
    public UnityEvent startAction;

    private GameObject _currentMenu;

    private void Start() {
        inputs.FindAction("Menus/Next").started += OnNext;
        inputs.FindAction("Menus/Previous").started += OnPrevious;
        inputs.FindAction("Menus/Interact").started += OnSelectButton;
        inputs.FindAction(actionName).started += OnShowMenu;
        
        _currentMenu = transform.GetChild(0).gameObject;
        _currentButtons = buttons.Where(buttonItem => buttonItem.subMenu == _currentMenu).ToList();
    }

    public void OnShowMenu(InputAction.CallbackContext e) {
        if (e.started)
            startAction.Invoke();
    }

    public void OnNext(InputAction.CallbackContext e) {
        if (e.started) {
            if (_buttonIndex < maxIndex - 1) {
                if(_buttonIndex >= 0) 
                    SwitchButton(_buttonIndex,false);
                
                _buttonIndex++;
                SwitchButton(_buttonIndex,true);
            }
        }
    }

    public void OnPrevious(InputAction.CallbackContext e) {
        if (e.started) {
            if (_buttonIndex >= 0) {
                SwitchButton(_buttonIndex,false);
                _buttonIndex--;
                SwitchButton(_buttonIndex,true);
            }
        }
    }

    public void OnSelectButton(InputAction.CallbackContext e) {
        if (e.started && !GameController.Instance.dialog.isInDialog) {
            SwitchButton(_buttonIndex,false);
            _currentButtons[_buttonIndex].action.Invoke();
        }
    }

    private void SwitchButton(int newIndex,bool state) {
        if (newIndex < 0)
            return;
        
        _currentButtons[newIndex].button.transform.GetChild(0).gameObject.SetActive(state);
    }
    
    #region Button's Action

    public void SwitchSubMenu(GameObject subMenu) { 
        _currentMenu.SetActive(false);
        subMenu.SetActive(true);
        _currentMenu = subMenu;
        _buttonIndex = -1;
        _currentButtons = buttons.Where(buttonItem => buttonItem.subMenu == _currentMenu).ToList();
    }

    #endregion

}
