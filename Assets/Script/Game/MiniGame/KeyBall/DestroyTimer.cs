using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DestroyTimer : MonoBehaviour {
    
    public float maxTime;
    public bool run;
    public Image timerUI;
    public KBController controller;

    private int spriteCount = 48;
    private float timeToChangeSprite;
    private float timer;
    private bool check;
    private float step;
    private int index;

    void Update() {

        if(run) {
            if(!check) {
                timeToChangeSprite = maxTime / spriteCount;
                check = true;
            }
            timer += Time.deltaTime;
            step += Time.deltaTime;

            if(timer < maxTime) {
                if(step >= timeToChangeSprite) {
                    timerUI.gameObject.SetActive(true);
                    // Changer le sprite
                    timerUI.sprite = controller.destroySprite[index];

                    index++;

                    if(index > 48)
                        index = 0;
                    
                    step = 0;
                }
            }
            else {
                controller.AddPoint(transform.parent.gameObject,1);
                timerUI.gameObject.SetActive(false);
                index = 0;
                timer = 0;
                step = 0;
                check = false;
                run = false;

                if(transform.parent.gameObject.tag == "Bot") {
                    transform.parent.gameObject.GetComponent<KBIA>().gotoDestroy = false;
                    transform.parent.gameObject.GetComponent<KBIA>().goToJump = true;
                    transform.parent.gameObject.GetComponent<KBIA>().goToPortal = true;
                    transform.parent.gameObject.GetComponent<KBIA>().freeze = false;
                    transform.parent.gameObject.GetComponent<KBIA>().isOnBall = false;
                }
                else if(transform.parent.gameObject.tag == "Player") {
                    transform.parent.gameObject.GetComponent<KB_PlayerMovement>().freeze = false;   
                    transform.parent.gameObject.GetComponent<KB_PlayerMovement>().isOnBall = false;
                }

                Destroy(transform.gameObject);
            }
        }
        
        

    }
}
