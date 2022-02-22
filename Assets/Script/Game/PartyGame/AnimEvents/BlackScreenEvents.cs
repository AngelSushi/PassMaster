using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlackScreenEvents : MonoBehaviour {

    private GameController controller;


    void Start() {
        controller = GameController.Instance;

    }
    public void ChangeDayPeriodEvent() {
        if(!controller.players[controller.actualPlayer].GetComponent<UserMovement>().isTurn || !controller.players[controller.actualPlayer].GetComponent<UserMovement>().useHourglass)
            return;

        DayController.DayPeriod actualPeriod = controller.dayController.dayPeriod;

        if(controller.players[controller.actualPlayer].GetComponent<UserMovement>().useHourglass) 
            controller.dayController.ChangeDayPeriodWithHourglass();
        
    }
}
