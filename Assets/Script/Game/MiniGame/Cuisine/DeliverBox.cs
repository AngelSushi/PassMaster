using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeliverBox : Box {

    private CookController cookController;

     void Start() {
        cookController = CookController.instance;

        Debug.Log("cookInstance " + cookController);
     }

    public override void Interact(ChefController playerController) {
        if(playerController.plate != null && playerController.plate.currentRecipe != null) {
            GameObject deliverPlayer = playerController.gameObject;

            if(cookController.playerScore.ContainsKey(deliverPlayer)) {
                int actualScore = cookController.playerScore[deliverPlayer];
                actualScore += playerController.plate.currentRecipe.reward;

                cookController.playerScore[deliverPlayer] = actualScore;
            }
            else {
                Debug.Log("deliver me ");
                cookController.playerScore.Add(deliverPlayer,playerController.plate.currentRecipe.reward);
                
            }

            Destroy(playerController.plate.gameObject);
        }

    }


}
