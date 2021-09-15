using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class FurnitureController : MonoBehaviour {
    
    public HSController controller;
    public bool isCollide;
    public GameObject hitObj;

    private void OnTriggerEnter(Collider hit) {
        if(hit.gameObject.tag == "Player") {
            if(hit.gameObject.GetComponent<HS_Movement>().role == HSController.Roles.SEEKER) {          
                controller.interactUI.SetActive(true);
                controller.interactUI.transform.GetChild(1).GetComponent<Text>().text = "Se cacher";
            }
            else if(hit.gameObject.GetComponent<HS_Movement>().role == HSController.Roles.HUNTER) {
                controller.interactUI.SetActive(true);
                controller.interactUI.transform.GetChild(1).GetComponent<Text>().text = "Fouiller";
            }

            isCollide = true;
            hitObj = hit.gameObject;
        }
    }

    private void OnTriggerExit(Collider hit) {
        if(hit.gameObject.tag == "Player") {
            controller.interactUI.SetActive(false);
            hitObj = null;
        }
    }

    public void OnActionFurniture(InputAction.CallbackContext e) {
        if(e.started && isCollide) {
            if(hitObj != null && hitObj.tag == "Player") {
                if(hitObj.GetComponent<HS_Movement>().role == HSController.Roles.SEEKER) {
                    controller.hunterWait.SetActive(true);
                    controller.hunterWait.transform.GetChild(0).gameObject.SetActive(false);
                    hitObj.GetComponent<MeshRenderer>().enabled = false;
                    hitObj.GetComponent<HS_Movement>().isHidden = true;
                    controller.interactUI.SetActive(false);
                }
                else if(hitObj.GetComponent<HS_Movement>().role == HSController.Roles.HUNTER) {
                    controller.interactUI.SetActive(false);
                    hitObj.GetComponent<HS_Movement>().isSearching = true;
                    hitObj.GetComponent<HS_Movement>().sliderSearch.gameObject.SetActive(true);
                    hitObj.GetComponent<HS_Movement>().sliderSearch.value = 0;
                    Debug.Log("enterHunter");

                }
            }
        }
    }
}
