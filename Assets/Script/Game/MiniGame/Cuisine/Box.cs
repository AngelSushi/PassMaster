﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum BoxType {
    NONE,
    NORMAL,
    INGREDIENT,
    CUT,
    PAN,
    STOVE,
    PLATE

}

public class Box : MonoBehaviour {
    
    public BoxType boxType;

    private Plate plate;



    public virtual void Interact(ChefController playerController) {
        if(playerController.actualIngredient == null && playerController.plate == null) 
            TakeIngredient(playerController);      
        else
            PutIngredient(playerController,transform.gameObject);

    }

    protected void TakeIngredient(ChefController playerController) {
        GameObject ingredient = null;
        
        switch(boxType) {
            case BoxType.INGREDIENT:
                ingredient = CreateIngredient((IngredientBox)this);
                break;

            case BoxType.CUT: // Meme code pour stove et pan
                if(transform.childCount <= 1)
                    return;
                
                ingredient = transform.GetChild(transform.childCount - 1).gameObject;
                break;

            case BoxType.NORMAL:
                ingredient = transform.GetChild(0).gameObject;
                break;

            case BoxType.STOVE:
                if (transform.childCount <= 1)
                    return;

                ingredient = transform.GetChild(transform.childCount - 1).gameObject;
                break;

            case BoxType.PAN:
                if (transform.childCount <= 1)
                    return;

                ingredient = transform.GetChild(transform.childCount - 1).gameObject;
                break;
        }

        if (ingredient != null) {
            ingredient.SetActive(true);
            ingredient.transform.parent = playerController.gameObject.transform;

            if (ingredient.TryGetComponent<SphereCollider>(out SphereCollider col)) {
                col.isTrigger = true;
                playerController.actualIngredient = ingredient;
            }
            else {
                playerController.plate = ingredient.GetComponent<Plate>();
                plate = null;
            }

            ingredient.transform.localPosition = new Vector3(-0.07f,0.03f,0.8f);
        }
    }

    protected void PutIngredient(ChefController playerController,GameObject box) {

        if(plate == null) {
            Debug.Log("put ingredient on box");
            Vector3 actionBoxPos = box.transform.position;
            actionBoxPos.y = 4.74f;

            GameObject target = playerController.actualIngredient != null ? playerController.actualIngredient : playerController.plate.gameObject;
            target.transform.position = actionBoxPos;
            target.transform.parent = box.transform;

            if (playerController.actualIngredient != null) {
                playerController.actualIngredient.GetComponent<SphereCollider>().isTrigger = false;
                playerController.actualIngredient = null;
            }
            else {
                playerController.plate = null;

                // Vérifier que c une box simple
                plate = box.transform.GetChild(0).gameObject.GetComponent<Plate>();
            }
        }
        else {
            Debug.Log("put ingredient in plate");

            if (playerController.actualIngredient == null || playerController.actualIngredient.GetComponent<Ingredient>() == null)
                return;

            plate.ingredients.Add(playerController.actualIngredient.GetComponent<Ingredient>());
            
            Destroy(playerController.actualIngredient);
            playerController.actualIngredient = null;

        }
    }

    private GameObject CreateIngredient(IngredientBox ingredientBox) {
        Vector3 spawnPosition = transform.position;
        spawnPosition.y += 0.5f;

        GameObject ingredient = Instantiate(ingredientBox.ingredient.ingredientPrefab,spawnPosition,ingredientBox.ingredient.ingredientPrefab.transform.rotation);

        ingredient.AddComponent<Ingredient>();
        ingredient.GetComponent<Ingredient>().ingredientModel = ingredientBox.ingredient;

        return ingredient;
    }

}
