    using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlackScreenEvents : CoroutineSystem {

    private GameController controller;


    void Start() {
        controller = GameController.Instance;
    }

    public void ChangeDayPeriodEvent() {
        if(!controller.players[controller.actualPlayer].GetComponent<UserMovement>().isTurn || !controller.players[controller.actualPlayer].GetComponent<UserMovement>().useHourglass)
            return;

        controller.dayController.ChangeDayPeriodWithHourglass();
    }

    public void TriggerLightningEvent() {
        if(!controller.players[controller.actualPlayer].GetComponent<UserMovement>().isTurn || !controller.players[controller.actualPlayer].GetComponent<UserMovement>().useLightning)
            return;

        controller.players[controller.actualPlayer].GetComponent<UserMovement>().useLightning = false;
        controller.players[controller.actualPlayer].GetComponent<UserUI>().infoLabel.SetActive(false);
        controller.dayController.ChangeManualDayPeriod(DayController.DayPeriod.RAIN);
        GameObject lightningStep =  controller.players[controller.actualPlayer].GetComponent<UserMovement>().targetLightningStep;
        controller.mainCamera.transform.position = new Vector3(lightningStep.transform.position.x,lightningStep.transform.position.y + 15,lightningStep.transform.position.z) - (controller.GetDirection(lightningStep,lightningStep.GetComponent<Step>(),25f) * 2.25f);
        
        controller.mainCamera.transform.LookAt(new Vector3(lightningStep.transform.position.x,lightningStep.transform.position.y + 5f,lightningStep.transform.position.z));

        Vector3 lightningStepPos = lightningStep.transform.position;
        lightningStepPos.y += 20f;
        List<GameObject> playersInStep = new List<GameObject>();

        foreach(GameObject player in controller.players) {
            if(player.GetComponent<UserMovement>().actualStep == lightningStep) 
                playersInStep.Add(player);
            
        }

<<<<<<< HEAD
<<<<<<< Updated upstream
        Debug.Log("Count: " + playersInStep.Count);
=======


>>>>>>> Stashed changes
=======


>>>>>>> main
        
        RunDelayed(1f,() => {
            GameObject newLightning = Instantiate(controller.itemController.lightningEffect,lightningStepPos,Quaternion.identity);
            newLightning.transform.GetChild(1).LookAt(controller.mainCamera.transform);
            newLightning.transform.eulerAngles = new Vector3(newLightning.transform.eulerAngles.x,newLightning.transform.eulerAngles.y - 180,newLightning.transform.eulerAngles.z);

            AudioController.Instance.ambiantSource.clip = AudioController.Instance.lightning;
            AudioController.Instance.ambiantSource.Play();  

<<<<<<< HEAD
<<<<<<< Updated upstream
            RunDelayed(0.2f,() => {
                if(playersInStep.Count > 0) {
                    foreach(GameObject player in playersInStep) {
                        player.GetComponent<UserMovement>().isElectrocuted = true;
                        controller.itemController.DropCoins(player,player.GetComponent<UserInventory>());
                
                    }
                }

                
            });
=======
            controller.itemController.DropCoins(controller.players[controller.actualPlayer],controller.players[controller.actualPlayer].GetComponent<UserInventory>());
>>>>>>> Stashed changes
=======
            controller.itemController.DropCoins(controller.players[controller.actualPlayer],controller.players[controller.actualPlayer].GetComponent<UserInventory>());
>>>>>>> main

            RunDelayed(2f,() => {
                Destroy(newLightning);
                AudioController.Instance.ambiantSource.Stop();
<<<<<<< HEAD
<<<<<<< Updated upstream

                foreach(GameObject player in playersInStep) {
                    player.GetComponent<UserMovement>().isElectrocuted = false;
                }
=======
>>>>>>> Stashed changes
=======
>>>>>>> main
            });
        });
    }
}
