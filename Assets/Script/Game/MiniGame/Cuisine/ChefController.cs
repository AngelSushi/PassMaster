using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
public class ChefController : MonoBehaviour {

    public float speed;
    public Rigidbody rb;
    public GameObject actualIngredient;
    private Vector2 movement;
    private Vector3 move;
    public bool isMoving;
    public bool canMoove;
    private GameObject cutBox,ingredientBox,panBox,stoveBox,box,groundIngredient,plateBox;
    private CookAction action;
    private Quaternion rotation;

    void Update() {
        
        if(isMoving && canMoove)
            Movement();
        else 
            transform.rotation = rotation;

        if(action != null) {
            if(cutBox != null && cutBox.transform.childCount > 1 &&  cutBox.transform.GetChild(1).gameObject.GetComponent<Ingredient>().cookTag == "CutBox") {
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

    private void OnCollisionStay(Collision hit) {
        if(hit.gameObject.tag == "Ingredient") 
            groundIngredient = hit.gameObject;
    }

    private void OnCollisionExit(Collision hit) {
        if(hit.gameObject.tag == "Ingredient") 
            groundIngredient = hit.gameObject;     
    }

    private void OnTriggerStay(Collider hit) {
        if(hit.gameObject.tag == "IngredientBox") 
            ingredientBox = hit.gameObject;
        if(hit.gameObject.tag == "CutBox") 
            cutBox = hit.gameObject; 
        if(hit.gameObject.tag == "PanBox")
            panBox = hit.gameObject;
        if(hit.gameObject.tag == "StoveBox")
            stoveBox = hit.gameObject;
        if(hit.gameObject.tag == "Box") 
            box = hit.gameObject;
        if(hit.gameObject.tag == "PlateBox") 
            plateBox = hit.gameObject;
    }

    private void OnTriggerExit(Collider hit) {
        if(hit.gameObject.tag == "IngredientBox" && ingredientBox != null) 
            ingredientBox = null;     
        if(hit.gameObject.tag == "CutBox" && cutBox != null) 
            cutBox = null;  
        if(hit.gameObject.tag == "PanBox" && panBox != null)
            panBox = null;
        if(hit.gameObject.tag == "StoveBox" && stoveBox != null)
            stoveBox = null;
        if(hit.gameObject.tag == "Box" && box != null) 
            box = null;
        if(hit.gameObject.tag == "PlateBox" && plateBox != null)
            plateBox = null;
    }

    public void OnMove(InputAction.CallbackContext e) {
        movement = e.ReadValue<Vector2>();

        if(e.started) isMoving = true;
        if(e.canceled)  isMoving = false;    
    }

    public void OnInteract(InputAction.CallbackContext e) {
        if(e.started) {
            if(ingredientBox == null && cutBox == null && box == null && panBox == null && actualIngredient != null) { // On check si le commis peut lacher l'ingrédient
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

                
            }        
        }
    }

    private void CutAction() {
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

    private void CookAction(GameObject box) {
        action = box.transform.GetChild(0).gameObject.GetComponent<CookAction>();
        if(!action.isDoingAction && action.ingredients.Count == 0) {
            actualIngredient.GetComponent<MeshRenderer>().enabled = false;
            action.ingredients.Add(actualIngredient);
            PutIngredient(box);
            action.isDoingAction = true;
            action.RefreshUI();
        }        
    }

    private void PutIngredient(GameObject box) {
        actualIngredient.GetComponent<SphereCollider>().isTrigger = false;
        Vector3 actionBoxPos = box.transform.position;
        actionBoxPos.y = 4.74f; 
        actualIngredient.transform.position = actionBoxPos;
        actualIngredient.transform.parent = box.transform;
        actualIngredient = null;
    }
    private void CutIngredient(GameObject cutIngredient) {
        cutIngredient.transform.localScale = new Vector3(2.05f,0.8f,2.05f);
        Vector3 ingredientPos = cutIngredient.transform.position;
        ingredientPos.y = 4.21f;
        cutIngredient.transform.position = ingredientPos;
        cutIngredient.GetComponent<Ingredient>().isCut = true;
    }

    private void DropIngredient() {
        actualIngredient.transform.parent = CookController.instance.gameObject.transform.parent;
        Vector3 position = actualIngredient.transform.position;
        position.y = 1.3f;
        actualIngredient.transform.position = position;
        actualIngredient.GetComponent<SphereCollider>().isTrigger = false;
        actualIngredient = null;
    }

    private void TakeIngredient() {
        if(action != null && action.isDoingAction)
            return;

        GameObject ingredient = null;

        if(ingredientBox != null)
            ingredient = ingredientBox.transform.GetChild(0).gameObject;
        else if(cutBox != null && cutBox.transform.childCount > 1) // Ajouter condition pour four,poele etc
            ingredient = cutBox.transform.GetChild(cutBox.transform.childCount - 1).gameObject;
        else if(box != null)
            ingredient = box.transform.GetChild(0).gameObject;
        else
            ingredient = groundIngredient;

        if(ingredient != null) {            
            ingredient.transform.parent = transform;
            ingredient.GetComponent<SphereCollider>().isTrigger = true;
            ingredient.transform.localPosition = new Vector3(-0.07f,0.03f,0.8f);
            actualIngredient = ingredient;
        }
    }

    private void TakePlate() {
        GameObject plate = plateBox.transform.GetChild(0).gameObject;

        if(plate != null) {            
            plate.transform.parent = transform;
            plate.transform.localPosition = new Vector3(-0.07f,0.03f,0.8f);
        }
    }

    private void PutIngredientInPlate(GameObject ingredient) {
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



    private List<CookController.Recipes> GetListContaining(List<GameObject> plate) {
        List<CookController.Recipes> plateRecipes = new List<CookController.Recipes>();

        for(int i = CookController.instance.recipes.Count - 1;i > 0;i--) {
            CookController.Recipes recipe = CookController.instance.recipes[i];
            plateRecipes.Add(recipe);

            foreach(GameObject ingredient in plate) {
                if(!recipe.ingredients.Contains(ingredient.GetComponent<Ingredient>().name)) {
                    plateRecipes.Remove(recipe);
                    break;
                }
            }

        }

        return plateRecipes;
    }

}
