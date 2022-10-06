using System;
using System.Collections;
using System.Collections.Generic;
using ExcelDna.Integration;
using UnityEngine;
using UnityEngine.UI;
using Image = UnityEngine.UI.Image;

public class SliderItem : MonoBehaviour {

    public List<Sprite> items = new List<Sprite>();

    private Image sliderImage;
    private int actualItem;

    public Sprite actualSprite;

    private void Start() {
        sliderImage = GetComponent<Image>();
        actualSprite = items[0];
    }

    public void Next() {

        actualItem++;
        
        if (actualItem >= items.Count)
            actualItem = 0;
        
        sliderImage.sprite = items[actualItem];
        actualSprite = items[actualItem];
    }

    public void Previous() {

        actualItem--;

        if (actualItem < 0)
            actualItem = items.Count - 1;

        sliderImage.sprite = items[actualItem];
        actualSprite = items[actualItem];
    }

    
}
