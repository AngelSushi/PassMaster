using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AlphaController : MonoBehaviour {

    public bool manageAlpha;
    public bool mainMenu;
    private float alpha = 0;
    private bool positive = true;

    void Start() { 
        if(manageAlpha) StartCoroutine(AlphaCoroutine());
    }

    private IEnumerator AlphaCoroutine() {
        while(manageAlpha) {
            
            if(positive) alpha += 5;
            else alpha -= 5;

            if(alpha == 255 || alpha == 0) positive = !positive;
            

            Color newColor = new Color(1.0f,1.0f,1.0f,alpha / 255f);
            GetComponent<Text>().color = newColor;
            
            if(!mainMenu)
                GetComponent<Text>().text = "Appuyez sur Echap pour quitter le mode";
            else
                GetComponent<Text>().text = "Appuyez sur E pour commencer";

            yield return new WaitForSeconds(0.02f);
        }
    }

    
}
