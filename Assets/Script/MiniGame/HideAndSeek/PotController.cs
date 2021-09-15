using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PotController : MonoBehaviour
{
    public bool throwObject;
    public HSController controller;
    public bool isBot;
    private Vector3 direction;
    public Vector3 potDirection;
    public GameObject thrower;
    private RaycastHit hit;

    void Update() {

        if(throwObject) {
            if(!isBot)
                transform.gameObject.GetComponent<Rigidbody>().AddForce(direction * 50);
            else 
                transform.gameObject.GetComponent<Rigidbody>().AddForce(potDirection * 50);      
            
            Debug.Log("potDirection: " + potDirection);
        }
    }

    void FixedUpdate() {
        Ray rayOrigin = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());

        if(Physics.Raycast(rayOrigin,out hit) && hit.collider != null) 
            direction = hit.point - transform.position;
    }

    private void OnCollisionEnter(Collision hit) {
        if(hit.gameObject.tag != transform.parent.tag && transform.parent.gameObject.name != "Pots") {
            if(throwObject) {
                Debug.Log("destroy");
                Destroy(transform.gameObject);       
            }
        }
    }

    private void OnTriggerEnter(Collider hit) {
        if(thrower != null && (thrower.tag == "Bot" || thrower.tag == "Player")) {
            if((hit.gameObject.tag == "Bot" || hit.gameObject.tag == "Player") && throwObject) {
                Debug.Log("wallahEnter");
                Debug.Log("etourdi");
                Vector3 dizzyPos = hit.gameObject.transform.position;
                dizzyPos.y += 10;
                
                GameObject prefab = Instantiate(controller.prefabDizzy,dizzyPos,Quaternion.Euler(0,0,0));
                prefab.transform.parent = hit.gameObject.transform;

                if(controller.IsHunterBot(hit.gameObject)) {
                    controller.GetHunter().GetComponent<HSIA>().isDizzy = true; 
                    if(controller.isSeekerBot(controller.GetHunter().GetComponent<HSIA>().targetSeeker))
                        controller.GetHunter().GetComponent<HSIA>().targetSeeker.GetComponent<HSIA>().isFinding = false; 
                }
                else 
                    controller.GetHunter().GetComponent<HS_Movement>().isDizzy = true;
                
                Destroy(transform.gameObject);     
            }
            return;
        }
        if(hit.gameObject.tag != transform.parent.tag && transform.parent.gameObject.name != "Pots" && hit.gameObject.tag != "Room" && hit.gameObject.tag != "VolumetricLight") {
            if(throwObject && hit.gameObject.tag != "Bot" && hit.gameObject.tag != "Player") {        
                Debug.Log("delete");
                Destroy(transform.gameObject);
            }
        }
    }
}
