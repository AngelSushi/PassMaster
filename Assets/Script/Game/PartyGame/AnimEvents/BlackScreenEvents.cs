    using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlackScreenEvents : CoroutineSystem { // Use for BoardGame and perfectNote

    private GameController controller;

    public void DestroyNoteState() => Destroy(transform.gameObject);
    


    void Start() {
        controller = GameController.Instance;
    }


    public void SwitchCameraDuringCircleTransition() {
        MusicController.instance.OnSwitchCamera();
    }

    public void OnTransitionEnd() {
        MusicController.instance.OnTransitionEnd();
    }
    
    public void ChangeDayPeriodEvent() {
        if(!controller.players[controller.actualPlayer].GetComponent<UserMovement>().isTurn || !controller.players[controller.actualPlayer].GetComponent<UserMovement>().useHourglass)
            return;

        controller.dayController.ChangeDayPeriodWithHourglass();
    }

}
