using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightDetection : MonoBehaviour {
    
    public HSController controller;
    private GameObject playerInLight;
    private float timerToKill;

    void Update() {
        if(playerInLight != null) {
            timerToKill += Time.deltaTime;
            Debug.Log("enter killing");
            if(timerToKill >= 3) {
                if(controller.isSeekerBot(playerInLight)) {
                    playerInLight.GetComponent<HSIA>().isDead = true;
                    timerToKill = 0;
                    playerInLight = null;
                }
                else {
                    playerInLight.GetComponent<HS_Movement>().isDead = true;
                    timerToKill = 0;
                    playerInLight = null;
                }

                controller.GetHunter().GetComponent<HSIA>().follow = false;
                controller.GetHunter().GetComponent<HSIA>().searchingFurniture = null;
                
            }
        }
        else 
            timerToKill = 0;
    }

    private void OnTriggerEnter(Collider hit) {
        if(hit.gameObject.tag == "Player" || hit.gameObject.tag == "Bot") {
            if(playerInLight == null) 
                playerInLight = hit.gameObject;
        }
    }

    private void OnTriggerExit(Collider hit) {
        if(hit.gameObject.tag == "Player" || hit.gameObject.tag == "Bot") {
            if(playerInLight != null) {
                playerInLight = null;
            }
        }
    }

    private void CheckIfFinish() {
        bool finish = true;
        foreach(GameObject seeker in controller.GetSeekers()) {
            if(controller.isSeekerBot(seeker)) {
                if(!seeker.GetComponent<HSIA>().isDead)
                    finish = false;
            }
            else {
                if(!seeker.GetComponent<HS_Movement>().isDead) {
                    finish = false;
                }
            }
        }

        controller.finish = finish;
    }
}
