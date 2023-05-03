using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class CookAction : MonoBehaviour {

    public bool isStarted;
    public float seconds;
    public Slider slider;
    public GameObject plate;
    public List<GameObject> ingredients;
    private float timer,step;

    public event EventHandler<OnActionFinishArgs> OnActionFinished;

    public class OnActionFinishArgs : EventArgs { }

    private void Start() {
        slider.gameObject.SetActive(false);
    }

    void Update() {
        if(isStarted) {
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
                isStarted = false;
                slider.gameObject.SetActive(false);
                timer = 0;
                step = 0;

                OnActionFinished?.Invoke(this,new OnActionFinishArgs());
           //   slider.value = 0;
            }
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
<<<<<<< HEAD

    public void StartAction() {
        isStarted = true;
        slider.gameObject.SetActive(true);
    }
    
=======
>>>>>>> main

   /* public void RefreshUI() {
        for(int i = 0;i<ingredients.Count;i++) {
            GameObject ingredient = ingredients[i];
            GameObject actionPanel = slider.gameObject.transform.parent.gameObject;

            actionPanel.transform.GetChild(1 + i).gameObject.GetComponent<Image>().sprite = ingredient.GetComponent<Ingredient>().ingredientModel.sprite;
        }
    }

    public void RefreshPlateUI() {
        for(int i = 0;i<ingredients.Count;i++) {
            GameObject ingredient = ingredients[i];

            plate.transform.GetChild(i).gameObject.GetComponent<Image>().sprite = ingredient.GetComponent<Ingredient>().ingredientModel.sprite;
        }
    }
    */
}
