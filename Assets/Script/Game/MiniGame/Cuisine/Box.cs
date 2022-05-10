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

    public virtual void Interact(ChefController playerController) {
        if(playerController.actualIngredient == null) {
           // if(boxType == BoxType.NORMAL || boxType == BoxType.INGREDIENT)
                Debug.Log("take my ingredient");
                TakeIngredient(playerController);
        }
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

        if(ingredient != null) {
            ingredient.SetActive(true);
            ingredient.transform.parent = playerController.gameObject.transform;
            ingredient.GetComponent<SphereCollider>().isTrigger = true;
            ingredient.transform.localPosition = new Vector3(-0.07f,0.03f,0.8f);
            playerController.actualIngredient = ingredient;
        }
    }

    protected void PutIngredient(ChefController playerController,GameObject box) {
        playerController.actualIngredient.GetComponent<SphereCollider>().isTrigger = false;
        Vector3 actionBoxPos = box.transform.position;
        actionBoxPos.y = 4.74f; 
        playerController.actualIngredient.transform.position = actionBoxPos;
        playerController.actualIngredient.transform.parent = box.transform;
        playerController.actualIngredient = null;
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
