using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonPush : CoroutineSystem {

    public FP_Controller controller;
    public GameObject mainCamera;

    private Vector3 newPosition = Vector3.zero;
    private Vector3 camDirection;
    private Vector3 lastCamDirection;
    private int index;
    private bool returnPos;
    public bool isPushing;

    // Faire en sorte que le bouton marque le segment sur lequel se trouvait le joueur pour la dernière fois

    void Update() {
        if(isPushing) {
            foreach(GameObject obj in controller.path) {
                obj.GetComponent<Animation>().enabled = true;
                obj.GetComponent<Animation>().Play();
            }

            if((int)mainCamera.transform.position.z != -203) {
                Debug.Log("enterr");
                mainCamera.transform.position = Vector3.MoveTowards(mainCamera.transform.position,camDirection,150 * Time.deltaTime);
            }
            else 
                returnPos = true;

            RunDelayed(4f,() => {
                foreach(GameObject obj in controller.path) {
                    obj.GetComponent<Animation>().enabled = false;
                    obj.transform.rotation = Quaternion.Euler(0,-180,-90);
                }
            });
        }
        if(returnPos) {
            if(lastCamDirection != Vector3.zero) {
                Debug.Log("panel");
                mainCamera.transform.position = Vector3.MoveTowards(mainCamera.transform.position,lastCamDirection,250 * Time.deltaTime);   

                if((int) mainCamera.transform.position.z == (int)lastCamDirection.z) {
                    isPushing = false;
                    Debug.Log("enterEnd");
                    returnPos = false;
                }
            }         
        }
    }
    private void OnCollisionEnter(Collision hit) {
        if(hit.gameObject.tag == "Player") {
            isPushing = true;    
            camDirection = new Vector3(mainCamera.transform.position.x,mainCamera.transform.position.y,-266f);
            lastCamDirection = mainCamera.transform.position;
        }
    }

    private void OnCollisionStay(Collision hit) {
        if(hit.gameObject.tag == "Player") {
            if(newPosition == Vector3.zero)
                newPosition = new Vector3(transform.GetChild(1).position.x,transform.GetChild(1).position.y - 7,transform.GetChild(1).position.z);

            transform.GetChild(1).position = Vector3.MoveTowards(transform.GetChild(1).position,newPosition,150 * Time.deltaTime);
        }
    }

    private void OnCollisionExit(Collision hit) {
        if(hit.gameObject.tag == "Player") {
            
            Vector3 newPosition = new Vector3(transform.GetChild(1).position.x,transform.GetChild(1).position.y + 12,transform.GetChild(1).position.z);
            transform.GetChild(1).position = Vector3.MoveTowards(transform.GetChild(1).position,newPosition,150 * Time.deltaTime);

        }
    }
}
