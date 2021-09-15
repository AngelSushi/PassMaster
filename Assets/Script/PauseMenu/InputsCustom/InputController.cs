using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using System;

public class InputController : MonoBehaviour {
 
 [Serializable]
    public class KeyPath {
        public string input;
        public string overridePath;
    }
 
 [Serializable]
    public class KeyPathArray {
        public KeyPath[] PC;
    }

    public Sprite hoverControlsButton;
    public Sprite normalControlsButton;
    private bool changeControl;
    private int inputTarget;
    private int actionIndex;
    public InputActionAsset playerControls;
    public List<string> actionsName;
    private List<InputAction> actions; 
    public TextAsset inputFile;
    private KeyPathArray keyArray = new KeyPathArray();

    void Start() {
        foreach(string actionName in actionsName) {
            InputAction action = playerControls.FindAction(actionName);
            actions.Add(action);
        }

        keyArray = JsonUtility.FromJson<KeyPathArray>(inputFile.text);
    }
    
    void OnGUI() {
        if(changeControl) {
            Event e = Event.current;
            transform.GetChild(2).GetChild(inputTarget).gameObject.GetComponent<Image>().sprite = hoverControlsButton;

            if (e.isKey || e.isMouse) {
                transform.GetChild(2).GetChild(inputTarget).gameObject.GetComponent<Image>().sprite = normalControlsButton;
                changeControl = false;
                transform.GetChild(2).GetChild(inputTarget).GetChild(0).gameObject.GetComponent<Text>().text = "" + e.keyCode;

                if(e.isKey)
                    AffectInputs("" + e.keyCode);
                else {
                    // Mouse detection
                }
            }
        }
    }
    public void OnClickInputButton(int input) {
        inputTarget = input;
        changeControl = true;
    }

    public void OnClickActionIndex(int index) {
        actionIndex = index;
    }

    private void AffectInputs(string key) {
        InputActionRebindingExtensions.ApplyBindingOverride(actions[inputTarget],actionIndex,GetOverridePath(key)); 
    }

    private string GetOverridePath(string keyName) {
        foreach(var key in keyArray.PC) {
            if(key.input == keyName) 
                return key.overridePath;
        }

        return "";
    }
}
