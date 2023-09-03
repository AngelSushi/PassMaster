using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CustomSlider : MonoBehaviour {

    public Transform listParent;

    public int actualElement;
    private int listSize;

    public TextMeshProUGUI elementName;

    private void Awake() => listSize = listParent.childCount;

    public void OnNext() {

        if (actualElement < listSize - 1)
            actualElement++;
        
        listParent.GetChild(actualElement - 1).gameObject.SetActive(false);
        listParent.GetChild(actualElement).gameObject.SetActive(true);

        elementName.text = listParent.GetChild(actualElement).gameObject.name;
    }

    public void OnPrevious() {
        
        if (actualElement > 0)
            actualElement--;
        
        listParent.GetChild(actualElement + 1).gameObject.SetActive(false);
        listParent.GetChild(actualElement).gameObject.SetActive(true);
        
        elementName.text = listParent.GetChild(actualElement).gameObject.name;
    }
}
