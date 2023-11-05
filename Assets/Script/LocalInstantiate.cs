using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;
using UnityEngine.UI;

public class LocalInstantiate : MonoBehaviour
{

    [SerializeField] private GameObject localPrefabKeyboard;
    [SerializeField] private GameObject localPrefabGamepad;
    private Button _readyButton;

    private GameObject _refLocal;
    
    private void Awake()
    {
        GameObject parent = GameObject.Find("Players");

        if (parent == null)
        {
            return;
        }


        PlayerInput input = transform.GetComponent<PlayerInput>() ?? transform.GetComponentInChildren<PlayerInput>();

        if (input.currentControlScheme.Equals("Keyboard & Mouse"))
        {
            _refLocal = Instantiate(localPrefabKeyboard,localPrefabKeyboard.transform.position, Quaternion.identity,parent.transform);   
            _readyButton = _refLocal.transform.GetChild(1).GetComponent<Button>();
            int playerIndex = LocalMultiSetup.Instance.PlayerCount - 1;
        
            _readyButton.onClick.AddListener(delegate { LocalMultiSetup.Instance.OnReady(playerIndex); });
        }
        else
        {
            _refLocal = Instantiate(localPrefabGamepad,localPrefabGamepad.transform.position, Quaternion.identity,parent.transform);   
        }

        _refLocal.GetComponent<RectTransform>().localPosition = Vector3.zero;
        _refLocal.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
        _refLocal.GetComponent<RectTransform>().rotation = Quaternion.Euler(0, 0, 0);


        input.uiInputModule = _refLocal.GetComponentInChildren<InputSystemUIInputModule>();
        _refLocal.GetComponentInChildren<CustomSlider>().Input = input;

        DontDestroyOnLoad(transform.gameObject);
    }
}
