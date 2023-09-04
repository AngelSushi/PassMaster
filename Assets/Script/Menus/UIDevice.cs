using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class UIDevice : MonoBehaviour
{
    [SerializeField] private List<Sprite> keyboardIcons;
    [SerializeField] private List<Sprite> gamepadIcons;
    

    public void SwitchIcons(Transform parent,string scheme)
    {
        foreach (Transform obj in parent)
        {
            if (obj.TryGetComponent(out Image image))
            {
                switch (scheme)
                {
                    case "Keyboard & Mouse":
                        if (gamepadIcons.Contains(image.sprite))
                        {
                            int index = gamepadIcons.IndexOf(image.sprite);
                            image.sprite = keyboardIcons[index];
                        }
                        break;
                    
                    case "Gamepad":
                        if (keyboardIcons.Contains(image.sprite))
                        {
                            int index = keyboardIcons.IndexOf(image.sprite);
                            image.sprite = gamepadIcons[index];
                        }
                        break;
                }
            }

            if (obj.childCount > 0)
            {
                SwitchIcons(obj,scheme);
            }
        }
    }
}
