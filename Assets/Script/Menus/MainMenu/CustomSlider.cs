using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class CustomSlider : MonoBehaviour {

    public Transform listParent;

    public int actualElement;
    private int listSize;

    public TextMeshProUGUI elementName;

    private PlayerInput _input;

    public PlayerInput Input
    {
        get => _input;
        set
        {
            _input = value;
            _input.actions.FindAction("Menus/Left").started += OnPrevious;
            _input.actions.FindAction("Menus/Right").started += OnNext;
        }
    }
    
    private void Awake()
    {
        listSize = listParent.childCount;
    }
    

    private void OnNext(InputAction.CallbackContext e) => OnNext();

    public void OnNext() {

        if (actualElement < listSize - 1)
            actualElement++;
        
        listParent.GetChild(actualElement - 1).gameObject.SetActive(false);
        listParent.GetChild(actualElement).gameObject.SetActive(true);

        elementName.text = listParent.GetChild(actualElement).gameObject.name;
    }

    private void OnPrevious(InputAction.CallbackContext e) => OnPrevious();
    
    public void OnPrevious() {
        
        if (actualElement > 0)
            actualElement--;
        
        listParent.GetChild(actualElement + 1).gameObject.SetActive(false);
        listParent.GetChild(actualElement).gameObject.SetActive(true);
        
        elementName.text = listParent.GetChild(actualElement).gameObject.name;
    }
}
