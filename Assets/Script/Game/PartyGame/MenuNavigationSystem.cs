using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class MenuNavigationSystem : MonoBehaviour
{
    

    [SerializeField] private List<Button> navigationElements;

    private List<Button> _currentNavigationElements = new List<Button>();

    private int _navigationIndex = -1;
        
    public int NavigationIndex
    {
        get => _navigationIndex;

        set
        {
            _navigationIndex = value;
            _navigationIndex = Mathf.Clamp(_navigationIndex, 0, _currentNavigationElements.Count - 1);
            
            _currentNavigationElements[_navigationIndex].Select();
        }
    }


    private void Start()
    {

        _currentNavigationElements = navigationElements;
        _navigationIndex = -1;
        
        /*  foreach (Button navigationElement in _currentNavigationElements.ToList())
          {
              if (!navigationElement.gameObject.activeSelf)
              {
                  _currentNavigationElements.Remove(navigationElement);
              }
          }
          */
    }

    public void OnRight(InputAction.CallbackContext e)
    {
        NavigationIndex = NavigationIndex + 1;
    }

    public void OnLeft(InputAction.CallbackContext e)
    {
        NavigationIndex = NavigationIndex - 1;
    }
}
