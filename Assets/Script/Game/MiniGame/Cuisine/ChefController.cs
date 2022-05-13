﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
public class ChefController : MonoBehaviour {

    public float speed;
    public Rigidbody rb;
    public GameObject actualIngredient;
    public Plate plate;

    private Vector2 movement;
    private Vector3 move;
    public bool isMoving;
    public bool canMoove;
    private Box actualBox;
    private CookAction action;
    private Quaternion rotation;

    void Update() {
        
        if(isMoving && canMoove)
            Movement();
        else 
            transform.rotation = rotation;

        if(action != null) {
        /*    if(cutBox != null && cutBox.transform.childCount > 1 &&  cutBox.transform.GetChild(1).gameObject.GetComponent<Ingredient>().cookTag == "CutBox") {
                if(action.isDoingAction)
                    canMoove = false;

                if(action.finish) {
                    CutIngredient(cutBox.transform.GetChild(1).gameObject);
                    actualIngredient = null;
                    action.finish = false;
                    action.ingredients.Remove(action.ingredients[0]);
                    canMoove = true;
                }
            }
          */  
        }

    }

      private void Movement() {
        float moveX = movement.x * speed * -1;
        float moveZ = movement.y * speed * -1;

        move = new Vector3(moveX,rb.velocity.y,moveZ);
        rb.velocity = move;

        rotation = Quaternion.LookRotation(move, Vector3.up);
        transform.rotation = rotation;
        
    }



  /*  private void OnCollisionStay(Collision hit) {
        if(hit.gameObject.tag == "Ingredient") 
            groundIngredient = hit.gameObject;
    }

    private void OnCollisionExit(Collision hit) {
        if(hit.gameObject.tag == "Ingredient") 
            groundIngredient = hit.gameObject;     
    }
*/



    private void OnTriggerStay(Collider hit) {
        if(hit.gameObject.GetComponent<Box>() != null)
            actualBox = hit.gameObject.GetComponent<Box>();
    }

    private void OnTriggerExit(Collider hit) {
        if(hit.gameObject.GetComponent<Box>() != null && actualBox != null)
            actualBox = null;
    }

    public void OnMove(InputAction.CallbackContext e) {
        movement = e.ReadValue<Vector2>();

        if(e.started) isMoving = true;
        if(e.canceled)  isMoving = false;    
    }
    

    public void OnInteract(InputAction.CallbackContext e) {
        if(e.started) {
            if(actualBox != null) {
                Box box = (Box)actualBox.GetComponent<Box>();

                box.Interact(this);
            }


            
           /* if(ingredientBox == null && cutBox == null && box == null && panBox == null && actualIngredient != null) { // On check si le commis peut lacher l'ingrédient
                DropIngredient();
                return;
            }
            
            if(actualIngredient == null && (ingredientBox != null  || (cutBox != null && cutBox.transform.childCount > 1 && cutBox.transform.GetChild(1).gameObject.GetComponent<Ingredient>().isCut) || (box != null && box.transform.childCount > 0) || groundIngredient != null)) { // On vérifie si le commis peut prendre l'ingrédient
                TakeIngredient();
                return;
            }
            
            if(plateBox != null && plateBox.transform.childCount > 0) {
                GameObject plate = plateBox.transform.GetChild(0).gameObject;
                Debug.Log("plateBox");

                if(actualIngredient != null) {
                    Ingredient ingredientInfo = actualIngredient.GetComponent<Ingredient>();
                    if((ingredientInfo.isCuttable && ingredientInfo.isCut && !ingredientInfo.isCookable) || (ingredientInfo.isCookable && ingredientInfo.isCook)) {
                        PutIngredientInPlate(actualIngredient);
                        Debug.Log("enter putting");
                    }
                }
                else 
                    TakePlate();

                return; 
            }

            if(actualIngredient != null) { // On vérifie que le commis a bien un ingrédient en main
                Ingredient ingredientInfo = actualIngredient.GetComponent<Ingredient>();
                if(cutBox != null && ingredientInfo.isCuttable && !ingredientInfo.isCut)  // Le commis est devant une plaque pour couper les ingrédients
                    CutAction();
                if(panBox != null && ingredientInfo.isCookable && !ingredientInfo.isCook && ingredientInfo.cookTag == "PanBox") // Le commis est devant la casserole
                    CookAction(panBox);
                if(stoveBox != null && ingredientInfo.isCut && ingredientInfo.isCookable && !ingredientInfo.isCook && ingredientInfo.cookTag == "StoveBox") // Le commis est devant la poele
                    CookAction(stoveBox);
                if(box != null && cutBox == null && panBox == null) 
                    PutIngredient(box);         
            }   
            else {
                if(cutBox != null && cutBox.transform.childCount > 1) {
                    Ingredient ingredientInfo = cutBox.transform.GetChild(1).gameObject.GetComponent<Ingredient>();
                    if(cutBox != null && ingredientInfo.isCuttable && !ingredientInfo.isCut)  // Le commis est devant une plaque pour couper les ingrédients
                        CutAction();
                }

                
            }    */     
        }
        
    }

    

   /* private void CutAction() {
        action = cutBox.transform.GetChild(0).gameObject.GetComponent<CookAction>();
        if(!action.isDoingAction) {
            if(action.ingredients.Count == 0) {         
                action.ingredients.Add(actualIngredient);
                PutIngredient(cutBox);
            }
            else 
                action.isDoingAction = true;
        }
    }
    */

    private void CookAction(GameObject box) {
        action = box.transform.GetChild(0).gameObject.GetComponent<CookAction>();
        if(!action.isStarted && action.ingredients.Count == 0) {
            actualIngredient.GetComponent<MeshRenderer>().enabled = false;
            action.ingredients.Add(actualIngredient);
           // PutIngredient(box);
            action.isStarted = true;
            action.RefreshUI();
        }        
    }

  /*  private void PutIngredient(GameObject box) {
        actualIngredient.GetComponent<SphereCollider>().isTrigger = false;
        Vector3 actionBoxPos = box.transform.position;
        actionBoxPos.y = 4.74f; 
        actualIngredient.transform.position = actionBoxPos;
        actualIngredient.transform.parent = box.transform;
        actualIngredient = null;
    }
    */


   

   /* private void TakePlate() {
        GameObject plate = plateBox.transform.GetChild(0).gameObject;

        if(plate != null) {            
            plate.transform.parent = transform;
            plate.transform.localPosition = new Vector3(-0.07f,0.03f,0.8f);
        }
    }
    */

 /*   private void PutIngredientInPlate(GameObject ingredient) {
        GameObject plate = plateBox.transform.GetChild(0).gameObject;
        CookAction plateAction = plate.GetComponent<CookAction>();

        Debug.Log("enter plate");

        if(plateAction.ingredients.Count == 0) // L'assiette n'a pas d'ingrédient on peut donc lui ajouter n'importe quel ingrédient
            plateAction.ingredients.Add(ingredient);
        else { 
            List<CookController.Recipes> possibleRecipes = GetListContaining(plateAction.ingredients); // On récupère les recettes présentes avec les ingrédients de l'assiette

            foreach(CookController.Recipes recipe in possibleRecipes) {
                if(recipe.ingredients.Contains(actualIngredient.GetComponent<Ingredient>().name)) { // Si l'ingrédient porté par le commis fait partie d'une des recettes de l'assiette
                    plateAction.ingredients.Add(ingredient);
                    plateAction.RefreshPlateUI();
                    Destroy(actualIngredient);
                    break;
                }
            }
        }

        actualIngredient = null;
    }

*/

}
