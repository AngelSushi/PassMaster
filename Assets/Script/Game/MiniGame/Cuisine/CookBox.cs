using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CookBox : BoxUI {

   public override void Interact(ChefController playerController) {
       Debug.Log("entered interact");

       if(playerController.actualIngredient != null) {

           
       }
   } 
   
}
