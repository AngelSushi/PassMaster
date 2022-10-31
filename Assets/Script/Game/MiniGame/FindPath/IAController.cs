using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class IAController : CoroutineSystem {
    
    #region Variables

    public NavMeshAgent agent;
    public FP_Controller gameController;

    public Vector3 actualPosition;

    private Vector3 lookPosition;
    private float time;
    private float seconds;
    private int index = 1;
    private int percentage;
    private bool notInPath;
    private bool hasReachFirstStep;
    private bool isInLava;

    private GameObject actualStep;
    #endregion

    #region Todo-List

    #endregion

    #region Unity's Functions
    void Start() {

        switch(GameController.Instance.difficulty) {
            case GameController.Difficulty.EASY:
                percentage = 55;
                break;

            case GameController.Difficulty.MEDIUM:
                percentage = 70;
                break;

            case GameController.Difficulty.HARD:
                percentage = 85;
                break;        
        }
    }

    void Update() {
        if(gameController.hasGenPath && !gameController.finish && !gameController.begin) {
            if(!isInLava) {
                if(!hasReachFirstStep) {
                    agent.enabled = true;
                    actualPosition = gameController.path[0].transform.position;
                    agent.SetDestination(actualPosition);
                }
                else {
                    if(!notInPath) {
                        actualPosition.y = transform.position.y;
                        transform.position = Vector3.MoveTowards(transform.position,actualPosition,100 * Time.deltaTime);
                        if(gameController.path.Contains(gameController.GetObjectByCoords(actualPosition)))
                            transform.position = new Vector3(transform.position.x,47f,transform.position.z);

                        if(agent != null) agent.enabled = false;
                    } 
                }

                if(actualPosition != null) {
                    lookPosition = new Vector3(actualPosition.x,transform.position.y,actualPosition.z);
                    transform.LookAt(lookPosition);

                    if(transform.eulerAngles.y <= 0.5 || transform.eulerAngles.y >= 359) 
                        transform.eulerAngles = new Vector3(transform.eulerAngles.x,180,transform.eulerAngles.z);
                }
            }
            else {
                time += Time.deltaTime;

                if(time >= 0.2) {
                    transform.gameObject.GetComponent<MeshRenderer>().enabled = !transform.gameObject.GetComponent<MeshRenderer>().enabled;
                    transform.GetChild(0).gameObject.GetComponent<MeshRenderer>().enabled = !transform.GetChild(0).gameObject.GetComponent<MeshRenderer>().enabled;
                    time = 0;
                    seconds++;
                }

                if(seconds >= 15) {
                    isInLava = false;
                    transform.gameObject.GetComponent<MeshRenderer>().enabled = true;
                    transform.GetChild(0).gameObject.GetComponent<MeshRenderer>().enabled = true;
                    seconds = 0;
                    time = 0;
                }
            }
        }
    }

    private void OnTriggerEnter(Collider hit) {
        if(hit.gameObject.tag == "Lava") {
            isInLava = true;
            agent.enabled = true;
            agent.Warp(gameController.spawnPoints[GetBotInArray(transform.gameObject)].transform.position);
            hasReachFirstStep = false;
            actualStep = null;
        }
        
        if(hit.gameObject.tag == "End") {
            gameController.winPlayer = transform.gameObject;
            gameController.finish = true;
        }
    }

    private void OnCollisionEnter(Collision hit) {
       if(hit.gameObject.tag == "Step" || hit.gameObject.tag == "LimitRight" || hit.gameObject.tag == "LimitRightEnd" || hit.gameObject.tag == "LimitLeft" || hit.gameObject.tag == "LimitLeftEnd" || hit.gameObject.tag == "EndPath") {
            if(!gameController.path.Contains(hit.gameObject)) {
                notInPath = true;
                return;
            }

            else {
                GameObject beginHit = hit.gameObject;
                if(hit.gameObject.name != "platform_01" && hit.gameObject.tag != "Bot" && hit.gameObject.tag != "Player")
                    actualStep = hit.gameObject;

                RunDelayed(0.8f,() => { // On attend que le bot arrive a sa destination
                        hasReachFirstStep = true;
                        notInPath = false;

                        int random = Random.Range(0,100);

                        if(random <= percentage) { // Le bot va vers la prochaine roche du chemin
                            Debug.Log("new new tag: " + beginHit.tag);
                            Debug.Log("my super object: " + actualStep);
                            Debug.Log("tag: " + hit.gameObject.tag);
                            AssignNewPosition();
                        }

                        else { 
                            GameObject frontObj = gameController.GetObjectByCoords(beginHit.gameObject.transform.localPosition + beginHit.gameObject.transform.forward * 1383);
                            GameObject leftObj = gameController.GetObjectByCoords(beginHit.gameObject.transform.localPosition + beginHit.gameObject.transform.up * 1251 * -1);
                            GameObject rightObj = gameController.GetObjectByCoords(beginHit.gameObject.transform.localPosition + beginHit.gameObject.transform.up * 1251);

                            // Mettre plus de conditions
                            if(gameController.path.Contains(frontObj) && !gameController.path.Contains(leftObj) && !gameController.path.Contains(rightObj)) 
                                ChangeRandomPosition(leftObj,rightObj);                        
                            else if(gameController.path.Contains(leftObj)) 
                                ChangeRandomPosition(frontObj,rightObj);   
                            else if(gameController.path.Contains(rightObj)) 
                                ChangeRandomPosition(frontObj,leftObj);
                            else {
                                 AssignNewPosition();
                            }                            
                        }
                });
            }
       }

       if(hit.gameObject.tag == "Sol") {
           hasReachFirstStep = false;
       }
        
    }

    #endregion

    #region Custom Functions

    private void ChangeRandomPosition(GameObject first,GameObject second) {
        int newRandom = Random.Range(0,2);
        if(newRandom == 0) {
            if(first != null) {
                actualPosition = first.transform.position;
            }
            else {
                if(second != null) 
                    actualPosition = second.transform.position;         
                else 
                    AssignNewPosition();             
            }
        }
        else if(newRandom == 1) {
           if(second != null) {
                actualPosition = second.transform.position;
           }
           else {
               if(first != null) {
                   actualPosition = first.transform.position;
               }
               else 
                    AssignNewPosition();           
           }
        }
    }

    private GameObject FindGameObjectWithVector(Vector3 vec) {
        Debug.Log("vector: " + vec);
        foreach(GameObject obj in gameController.path) {
            if((int)obj.transform.position.x == (int)vec.x && (int)obj.transform.position.z == (int)vec.z) 
                return obj;
        }

        Debug.Log("pass verification");
        return null;
    }

    

    private void AssignNewPosition() {
        if(actualStep != null) {
            if((FindIndexByGameObject(actualStep) + 1) < (gameController.path.Count)) {                                 
                Debug.Log("object: " +  gameController.GetObjectByWorldCoords(actualPosition));
                Debug.Log("index: " + FindIndexByGameObject( actualStep));
                Debug.Log("newIndex: " + (FindIndexByGameObject(gameController.GetObjectByWorldCoords(actualPosition)) + 1));
                actualPosition = gameController.path[FindIndexByGameObject(actualStep) + 1].transform.position;                          
            }
            else 
                actualPosition = new Vector3(-1.5f,transform.position.y,-333.5f);
        }
    }

    private int FindIndexByGameObject(GameObject obj) {
        Debug.Log("obj: " + obj);
        for(int i = 0;i<gameController.path.Count;i++) {
            if(gameController.path[i] == obj) {
                return i;
            }
        }

        Debug.Log("isVeryNull");
        return -1;
    }
    private int GetBotInArray(GameObject bot) {
        for(int i = 0;i<gameController.players.Length;i++) {
            if(gameController.players[i] == bot)
                return i;
        }

        return -1;
    }

    private void ManageRotation(){
        GameObject nextStep = gameController.GetObjectByCoords(actualPosition);

        if(nextStep != null) {
            GameObject frontObj = gameController.GetObjectByCoords(transform.localPosition + transform.forward * 1383);
            GameObject leftObj = gameController.GetObjectByCoords(transform.localPosition + transform.up * 1251 * -1);
            GameObject rightObj = gameController.GetObjectByCoords(transform.localPosition + transform.up * 1251);

            if(nextStep == frontObj) {
                transform.rotation = Quaternion.Euler(0,-180,0);
            }
            else if(nextStep == leftObj) {
                transform.rotation = Quaternion.Euler(0,-90,0);
            }
            else if(nextStep == rightObj) {
                transform.rotation = Quaternion.Euler(0,-270,0);
            }
        }
    }
    #endregion
    
}
