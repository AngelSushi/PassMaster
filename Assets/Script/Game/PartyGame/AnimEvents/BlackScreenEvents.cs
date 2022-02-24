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
        GameObject lightingStep =  controller.players[controller.actualPlayer].GetComponent<UserMovement>().targetLightningStep;
        controller.mainCamera.transform.position = new Vector3(lightingStep.transform.position.x,lightingStep.transform.position.y + 15,lightingStep.transform.position.z) - (controller.GetDirection(lightingStep,lightingStep.GetComponent<Step>(),25f) * 2.25f);
        
        controller.mainCamera.transform.LookAt(new Vector3(lightingStep.transform.position.x,lightingStep.transform.position.y + 5f,lightingStep.transform.position.z));

        Vector3 lightningStepPos = lightingStep.transform.position;
        lightningStepPos.y += 20f;
        
        RunDelayed(1f,() => {
            GameObject newLightning = Instantiate(controller.itemController.lightningEffect,lightningStepPos,Quaternion.identity);
            newLightning.transform.GetChild(1).LookAt(controller.mainCamera.transform);
            newLightning.transform.eulerAngles = new Vector3(newLightning.transform.eulerAngles.x,newLightning.transform.eulerAngles.y - 180,newLightning.transform.eulerAngles.z);

            AudioController.Instance.ambiantSource.clip = AudioController.Instance.lightning;
            AudioController.Instance.ambiantSource.Play();  

            RunDelayed(2f,() => {
                Destroy(newLightning);
                AudioController.Instance.ambiantSource.Stop();
            });
        });
    }
}
