using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CookAction : MonoBehaviour {

    public bool isDoingAction;
    public float seconds;
    public Slider slider;
    public GameObject plate;
    public bool carryPlate;
    public List<GameObject> ingredients;
    public bool finish;
    private float timer,step;

    void Update() {
        if(isDoingAction) {
            slider.gameObject.SetActive(true);

            timer += Time.deltaTime;
            step += Time.deltaTime;

            if(timer < seconds) {
                if(step >= seconds / 20) {
                    step = 0;
                    slider.value += 0.05f;
                }
            }
            else {
                finish = true;
                isDoingAction = false;
            }
        }
        else {
            slider.gameObject.SetActive(false);
            timer = 0;
            step = 0;
            slider.value = 0;
        }
    }

    void OnGUI() {  
        if(plate != null) {
            if(ingredients.Count > 0) {
                plate.SetActive(true);
                plate.transform.position = Camera.main.WorldToScreenPoint(transform.parent.position);
            }
            else 
                plate.SetActive(false);
        }
    }

   /* public void RefreshUI() {
        for(int i = 0;i<ingredients.Count;i++) {
            GameObject ingredient = ingredients[i];
            GameObject actionPanel = slider.gameObject.transform.parent.gameObject;

            actionPanel.transform.GetChild(1 + i).gameObject.GetComponent<Image>().sprite = ingredient.GetComponent<Ingredient>().sprite;
        }
    }

    public void RefreshPlateUI() {
        for(int i = 0;i<ingredients.Count;i++) {
            GameObject ingredient = ingredients[i];

            plate.transform.GetChild(i).gameObject.GetComponent<Image>().sprite = ingredient.GetComponent<Ingredient>().sprite;
        }
    }
    */
}
