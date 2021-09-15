using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class DoorController : MonoBehaviour {
    
    public HSController controller;
    public bool isOpen;
    public bool isCollide;
    public bool mustBigger;
    public bool isInRoom;

    private void OnTriggerEnter(Collider hit) {
        if(hit.gameObject.tag == "Player") {
            controller.interactUI.SetActive(true);
            controller.interactUI.transform.GetChild(1).GetComponent<Text>().text = "Ouvrir";
            isCollide = true;

            if(mustBigger && !isOpen) {
                if(hit.gameObject.transform.position.x < transform.position.x && hit.gameObject.transform.position.z < transform.position.z)
                    isInRoom = true;
                else
                    isInRoom = false;
            }
            else if(!mustBigger && !isOpen) {
                if(hit.gameObject.transform.position.x > transform.position.x && hit.gameObject.transform.position.z > transform.position.z)
                    isInRoom = true;
                else
                    isInRoom = false;
            }
        }
    }

    private void OnTriggerExit(Collider hit) {
        if(hit.gameObject.tag == "Player" && controller.interactUI.activeSelf) {
            controller.interactUI.SetActive(false);
            isCollide = false;
        }
    }

    public void OnOpenDoor(InputAction.CallbackContext e) {
        Debug.Log("openDoor");
        if(e.started && isCollide) {
            if(!isInRoom) {
                if(!isOpen) {
                    transform.Rotate(0,90,0);
                    isOpen = true;
                }
                else {
                    transform.Rotate(0,-90,0);
                    isOpen = false;
                }
            }
            else {
                if(!isOpen) {
                    transform.Rotate(0,-90,0);
                    isOpen = true;
                }
                else {
                    Debug.Log("hunter");
                    transform.Rotate(0,90,0);
                    isOpen = false;
                }
            }
        }
    }
}
