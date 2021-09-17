using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class HS_Movement : CoroutineSystem {

    public float speed;
    public Rigidbody rb;
    public Vector2 movement;
    public HSController controller;
    public HSController.Roles role;
    public bool isHidden;
    public bool isSearching;
    public bool isDead;
    public bool isReady;
    public bool isMoving;
    public Slider sliderSearch;
    public GameObject deadCam;
    public GameObject deadUI;
    public GameObject floor02;
    public Text detectorText;
    public Material[] activeMats;
    public int floor;
    public int actualRoom;
    public int adjacentRoom;
    public GameObject searchingFurniture;
    private RaycastHit hit;
    private float readyTimer = 13f;

    private float timer;
    private bool runNextValue;

    public bool isActive;
    public bool isCarry;

    private bool isCollide;
    private GameObject collideObj;
    public bool isDizzy;
    private float dizzyTimer;

    void Update() {
        if(!controller.begin && !controller.finish) {
            if(role == HSController.Roles.HUNTER && !isReady) {
                readyTimer -= Time.deltaTime;

                if(readyTimer > 0) {
                    controller.hunterWait.SetActive(true);
                    float seconds = Mathf.FloorToInt(readyTimer % 60);
                    controller.hunterWait.transform.GetChild(0).gameObject.GetComponent<Text>().text = seconds.ToString();

                }
                else {
                    isReady = true;
                    controller.hunterWait.SetActive(false);
                }
            }

            if(role == HSController.Roles.SEEKER && !isReady)
                isReady = true;
            
            if(isReady) {
                if(isDizzy) {
                    dizzyTimer += Time.deltaTime;

                    if(dizzyTimer >= 5f) {
                        isDizzy = false;
                        dizzyTimer = 0;
                    }
                    return;
                }
               if(!isDead && !isHidden && !isSearching) {
                    Movement();
               }

               if(isSearching) {
                   sliderSearch.gameObject.SetActive(true);
                   timer += Time.deltaTime;

                    if(!runNextValue) {
                        runNextValue = true;
                        RunDelayed(0.1f,() => {
                            sliderSearch.value += 0.03f;
                            runNextValue = false;
                        });

                    }

                    RunDelayed(3.8f,() => {
                        isSearching = false;
                        sliderSearch.gameObject.SetActive(false);
                    });
                    
               }

               if(isActive && role == HSController.Roles.SEEKER && !isDead) {
                    GameObject hunter = controller.GetHunter();
                    Vector3 hunterPos = hunter.transform.position;

                    float distance = Vector3.Distance(hunterPos,transform.position);
                    float percentage;

                    if(distance > 255)
                        percentage = 0;
                    else 
                        percentage =(distance - 255) / 255 * 100; // ici 255 représente la distance max

                    if(percentage < 0)
                        percentage *= -1;
                    
                    if(percentage > 100)
                        percentage = 100;

                    detectorText.text = (int)percentage + "%";
               }

               if(isDead) {
                    detectorText.text = "";
                    transform.gameObject.GetComponent<MeshRenderer>().enabled = false;
                    transform.GetChild(0).gameObject.GetComponent<MeshRenderer>().enabled = false;
                    transform.GetChild(1).gameObject.SetActive(false);
                    deadCam.SetActive(true);
                    deadUI.SetActive(true);     
                    transform.gameObject.GetComponent<HS_Movement>().enabled = false;
                    transform.GetChild(2).gameObject.SetActive(false);
                    transform.GetChild(3).gameObject.SetActive(false);
                    transform.position = new Vector3(0,0,0);
               }
            }
        }
    }

    void FixedUpdate() {
        if(!controller.begin && !controller.finish && !controller.randomRoles && !isDead) {
            Ray rayOrigin = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());

            if(Physics.Raycast(rayOrigin,out hit) && hit.collider != null) {
                Vector3 direction = hit.point - transform.GetChild(2).position;
                transform.GetChild(2).LookAt(hit.point,Vector3.up);
            }
        }
    }

    private void Movement() {
        float moveX = movement.y * speed * -1;
        float moveZ = movement.x * speed * -1;
        
        rb.velocity = new Vector3(moveZ, rb.velocity.y,moveX);
        //transform.rotation = Quaternion.LookRotation(new Vector3(moveZ,0,moveX));
    }
    
    public void OnMove(InputAction.CallbackContext e) {
        if(!isDead) {
            movement = e.ReadValue<Vector2>();

            if(e.started) isMoving = true;
            if(e.canceled)  isMoving = false;
        }
        else {
            if(e.started) {
                movement = e.ReadValue<Vector2>();
                float moveZ = movement.y * speed * -1;

                if(moveZ > 0) {
                    // Etage 1
                    for(int i = 0;i<floor02.transform.childCount;i++) {
                        floor02.transform.GetChild(i).GetComponent<MeshRenderer>().enabled = false;
                    }
                }
                else if(moveZ < 0) {
                    // Etage 2
                    for(int i = 0;i<floor02.transform.childCount;i++) {
                        floor02.transform.GetChild(i).GetComponent<MeshRenderer>().enabled = true;
                    }
                }
            }
        }
    }

    public void OnAction(InputAction.CallbackContext e) {
        if(e.started && isReady && (role == HSController.Roles.SEEKER || role == HSController.Roles.HUNTER)) {
            if(collideObj != null && collideObj.tag != "Pot")
                return;

            isActive = !isActive;

            if(role ==  HSController.Roles.SEEKER ) {
                if(isActive) 
                    transform.GetChild(3).GetChild(1).gameObject.GetComponent<MeshRenderer>().material = activeMats[0];    
                else {
                    detectorText.text = "";
                    transform.GetChild(3).GetChild(1).gameObject.GetComponent<MeshRenderer>().material = activeMats[1];              
                }
            }
        }
    }

    public void OnSprint(InputAction.CallbackContext e) {
        Debug.Log("sprintMagic");
    }

    private void OnTriggerEnter(Collider hit) {
        if(hit.gameObject.tag == "Room") {
            actualRoom = hit.gameObject.GetComponent<Room>().index;
            adjacentRoom = hit.gameObject.GetComponent<Room>().adjacentRoom;
            floor = hit.gameObject.GetComponent<Room>().floor;
        }
    }

    private void OnTriggerExit(Collider hit) {
        if(hit.gameObject.tag == "Room") {
            actualRoom = 0;
            adjacentRoom = 0;
        }
    }

    private void OnCollisionEnter(Collision hit) {
        if(hit.gameObject.tag == "Pot") {     
            controller.interactUI.SetActive(true);
            controller.interactUI.transform.GetChild(1).GetComponent<Text>().text = "Prendre";
            isCollide = true;
            collideObj = hit.gameObject;
        }
    }

    private void OnCollisionExit(Collision hit) {
        if(hit.gameObject.tag == "Pot" && hit.gameObject.transform.parent != transform) {
            controller.interactUI.SetActive(false);
            controller.interactUI.transform.GetChild(1).GetComponent<Text>().text = "";
            isCollide = false;
            collideObj = null;
        }
    }

    public void OnTake(InputAction.CallbackContext e) {
        if(e.started && isCollide && collideObj.tag == "Pot") {
            collideObj.transform.parent = transform;
            collideObj.GetComponent<BoxCollider>().isTrigger = true;
            controller.interactUI.SetActive(false);
            controller.interactUI.transform.GetChild(1).GetComponent<Text>().text = "";
            collideObj.transform.localPosition = new Vector3(0.008f,1.732f,-0.030f);
        }
    }

    public void OnThrow(InputAction.CallbackContext e) {
        if(e.started && collideObj != null && collideObj.tag == "Pot") {
            collideObj.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationZ;
            collideObj.GetComponent<PotController>().throwObject = true;
            collideObj.GetComponent<PotController>().thrower = transform.gameObject;
        }
    }

}
