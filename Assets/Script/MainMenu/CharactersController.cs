using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharactersController : MonoBehaviour {

    private string[] typesName = {"Player","Bot"};
    private string[] charactersName = {"Mario","Luigi","Link","Zelda","Luffy","Zoro","Kirito","Asuna"};
    private string[] cosmeticsName = {"Chapeau Mario","Chapeau Luffy","Toque"};

    public void ChangeValue(int value) {
        if(value == 0 || value == 1)  // Type
            ManageSlide(0,value,1,0,typesName);
        else if(value == 2 || value == 3) // Character
            ManageSlide(2,value,3,2,charactersName);
        else if(value == 4 || value == 5)  // Cosmétiques
            ManageSlide(3,value,5,4,cosmeticsName);
        else if(value == 6 || value == 7)
            ManageSlideSprite(4,value,7,6);
    }

    private void ManageSlide(int child,int value,int leftValue,int rightValue,string[] array) {
            Text uiText = transform.GetChild(child).GetChild(0).gameObject.GetComponent<Text>();
            int index = GetIndexByText(uiText.text,array);

            if(value == rightValue && index < array.Length - 1) 
                index++;
            if(value == leftValue && index > 0)
                index--;

            uiText.text = array[index];
    }

    private void ManageSlideSprite(int child,int value,int leftValue,int rightValue) {
            Image img = transform.GetChild(child).GetChild(0).gameObject.GetComponent<Image>();
            Image img02 = transform.GetChild(child).GetChild(1).gameObject.GetComponent<Image>();

            if(value == rightValue) {
                img02.gameObject.SetActive(true);
                img.gameObject.SetActive(false);
            }
            if(value == leftValue) {
                img02.gameObject.SetActive(false);
                img.gameObject.SetActive(true);
            }

    }

    public int GetIndexByText(string text,string[] array) {
        for(int i = 0;i<array.Length;i++) {
            if(array[i] == text) 
                return i;
        }

        return -1;
    }

    public int GetIndexBySprite(Sprite sprite,Sprite[] array) {
        for(int i = 0;i<array.Length;i++) {
            if(array[i] == sprite)
                return i;
        }

        return -1;
    }
}
