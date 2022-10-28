using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class KBIA : CoroutineSystem {
    
    #region IA Enum 
    public enum Area {
        NONE,
        ONE,
        TWO,
        THREE,
        FOUR,
        FIVE,
        SIX,
        SEVEN,
        EIGHT,
        NINE,
        TEN,
        ELEVEN,
        TWELVE,
        THIRTEEN,
        FOURTEEN
    }

    public enum IADirection {
        FRONT,
        LEFT,
        RIGHT
    }

    #endregion

    #region Variables

    public KBController controller;
    public IADirection direction;
    public Area area;
    public Rigidbody rb;
    public Vector3 respawnPos;
    public bool goToJump;
    public bool dead;
    public bool gotoDestroy;
    public bool goToPortal;
    public bool freeze;
    public bool isOnBall;
    public bool thruster;
    public float thrusterForce;
    public float speed;
    public float jumpSpeed;
    public float timeToDestroy;
    public float moveX;
    public float moveZ;
    public float percentage;
    public float beginPercentage;

    private Area lastArea;
    private Vector3 newPos;
    private Vector3 beginPos;
    private Vector3 positionToGo;
    private Vector3 destroyPos;
    private GameObject lastHit;
    private bool hasGenPosition;
    private bool isJumping;
    private bool finishMove;
    private bool hasGenObstacle;
    private bool hasCheckPos;
    private int stopWatchPercentage;
    private float wallTimeWait;
    private float seconds;
    private float time;
    private float thrusterTime;

    public GameObject lastFloor;

    private bool checkPosition = true;
    
    #endregion
    // IA BOT - Vérifier le movetowards sur le destroy et faire la direction vers le portail
    // FAIRE LES COLLISIONS AVEC LES TRUCS ROTATIFS - Dem
    // SET LES BONNES POSITIONS POUR LE PORTAIL - juste a mettre dans le code
    // AJUSTER LE PLAYER - Fait a verif
    // TOUT CHECK
    // + Faire les collisions entre les boules 
    // + Faire le système de temps entre les murs
        // FAIRE LES CHRONOMETRES - Enlever , a mettre ???

    #region Unity's Function
    void Start() {
        switch(GameController.difficulty) {
            case 0:
                percentage = 65;
                stopWatchPercentage = 45;
                wallTimeWait = 1.25f;
                break;

            case 1:
                percentage = 40;
                stopWatchPercentage = 65;
                wallTimeWait = 0.75f;
                break;

            case 2:
                percentage = 30;
                stopWatchPercentage = 80;
                break;
        }

        beginPercentage = percentage;


    }

    void Update() {

        if(!controller.begin && !controller.finish) {
            if(!isOnBall) {
                GameObject ball = null;

                if(transform.gameObject.name == "Bot_001") 
                    ball = GameObject.FindGameObjectsWithTag("BlueBall")[0]; 
                else if(transform.gameObject.name == "Bot_002") 
                    ball = GameObject.FindGameObjectsWithTag("GreenBall")[0];  
                else if(transform.gameObject.name == "Bot_003") 
                    ball = GameObject.FindGameObjectsWithTag("YellowBall")[0]; 
                
                
                if(!hasGenPosition) {
                    positionToGo = ball.transform.position;
                    positionToGo.x += 20;
                    positionToGo.y = transform.position.y;
                    jumpSpeed = 95;
                    hasGenPosition = true; 
                }

                if(transform.position != positionToGo && !finishMove && !dead)
                    transform.position = Vector3.MoveTowards(transform.position,positionToGo,speed * Time.deltaTime);
                else {
                    finishMove = true;
                    if(!isJumping) 
                        Jump(-0.5f,0);
                }
            }   

            else {

                if(!dead && !freeze) {
                    if(jumpSpeed > 75) 
                        jumpSpeed = 75;                    

                    if(thruster) {
                        thrusterTime += Time.deltaTime;
                        rb.AddForce(new Vector3(0,0,thrusterForce));
                        if(thrusterTime >= 0.05f) {
                            thrusterTime = 0;
                            thruster = false;
                        }
                    }

                    moveX = -speed;

                    if(isOnBall) {
                        if( (rb.velocity.x > 0.1 || rb.velocity.x < -0.1) || (rb.velocity.z > 0.1 || rb.velocity.z < -0.1)) {
                            transform.GetChild(1).Rotate(2,0,0);
                        }
                    }

                    if(rb.velocity.magnitude > speed){
                        rb.velocity = Vector3.ClampMagnitude(rb.velocity, speed);
                    }
                    if(area == Area.NONE) {
                        if(lastArea != Area.THIRTEEN || area == Area.THIRTEEN) {
                            int random = Random.Range(0,100);
                        
                            if(random < percentage && !hasGenObstacle) {    
                                beginPos = transform.position;

                                int r = Random.Range(0,2);
                                if(r == 0)
                                    moveZ = 25;
                                else
                                    moveZ = -25;


                                hasGenObstacle = true;

                                RunDelayed(4f,() => {
                                    hasGenObstacle = false;
                                });
                            }
                            
                            if(moveZ > 0) {
                                if((int)transform.position.z >= (int)beginPos.z + 10) 
                                    moveZ = 0;                                
                            }
                            else {
                                if((int)transform.position.z <= (int)beginPos.z - 10) 
                                    moveZ = 0;                                
                            }
                        }
                    }
                    else if(area == Area.ONE) {
                     int random = Random.Range(0,100);

                        if(lastArea == Area.NONE) {
                            if(random < stopWatchPercentage ) { // IL VA SUR LE SAUT
                                if(speed == 70)
                                    speed += 10;
                            }

                            else { 
                                if(direction != IADirection.RIGHT)  
                                    direction = IADirection.RIGHT;
                            }
                        }
                    }
                    else if(area == Area.TWO && direction != IADirection.FRONT) 
                        direction = IADirection.FRONT;                      
                    else if(area == Area.THREE && direction != IADirection.LEFT) 
                        direction = IADirection.LEFT;
                    else if(area == Area.FOUR && direction != IADirection.FRONT) 
                        direction = IADirection.FRONT; 

                    else if(area == Area.FIVE && lastArea == Area.NONE) {
                        if(moveZ != 30) {
                            int random = Random.Range(0,100);
                            
                            if(random >= percentage) {
                                int side = Random.Range(0,2);

                                if(side == 0) // Droite
                                    moveZ = 40;
                                else 
                                    moveZ = -40;
                            }
                        }

                        if(speed > 70)
                            speed = 70;
                    }
                    else if(area == Area.SIX && lastArea == Area.FIVE) { // On fait revenir le bot vers le centre
                        int random = Random.Range(0,100);

                        if(random < percentage) 
                            moveZ = 50;                             
                    }
                    else if(area == Area.SEVEN && lastArea == Area.FIVE) {
                        int random = Random.Range(0,100);

                        if(random < percentage)
                            moveZ = -50;
                    }
                    else if(area == Area.EIGHT && (lastArea == Area.SIX || lastArea == Area.SEVEN)) {
                        int random = Random.Range(0,100);

                        if(random >= percentage) {
                            int side = Random.Range(0,2);

                            if(side == 0)
                                moveZ = 40;
                            else
                                moveZ = -40;
                        }
                    }
                    else if(area == Area.NINE && lastArea == Area.SIX) {
                        int random = Random.Range(0,100);

                        if(random < percentage) 
                            moveZ = 50;                   
                    }
                    else if(area == Area.TEN && lastArea == Area.SEVEN) {
                        int random = Random.Range(0,100);

                        if(random < percentage)
                            moveZ = -50;
                    }
                    else if(area == Area.ELEVEN && (lastArea == Area.EIGHT || lastArea == Area.NINE)) 
                        moveZ = 50;
                    else if(area == Area.TWELVE && (lastArea == Area.EIGHT || lastArea == Area.TEN))
                        moveZ = -50;
                    else if(area == Area.THIRTEEN) {
                        if(CheckPosWall() >= 0) {
                            GameObject nextWall = controller.walls[CheckPosWall()];

                            if(!nextWall.GetComponent<MeshRenderer>().enabled) {
                                RunDelayed(wallTimeWait,() => {
                                    moveX = -speed;
                                });
                            }
                            else {
                                if(CheckPosWall() > 0) { // 0.1 fait aller le bot jusqu'au prochain mur
                                    //RunDelayed(0.01f,() => {
                                        moveX = 0;
                                    //});

                                    // A rajouter plus tard random un temps pour pas qu'il s'arrete a chaque fois au même endroit
                                }
                                else 
                                    moveX = 0;
                            }
                        }
                        else {
                            moveX = -speed;       
                        }
                    }

                    else if(area == Area.FOURTEEN) {
                        if(!gotoDestroy && !goToPortal) {
                            moveX = 0;
                            moveZ = 0;
                            gotoDestroy = true;
                            destroyPos = controller.destroyPoints[controller.ConvertPlayerInt(transform.gameObject)];
                            destroyPos.y = transform.position.y;
                        }                        
                    }

                    CheckArea();

                    if(!gotoDestroy && !goToPortal) {
                        if(direction == IADirection.FRONT) {
                            rb.AddForce(new Vector3(moveX,0,moveZ));
                        }
                        else if(direction == IADirection.RIGHT) {
                            rb.AddForce(new Vector3(moveZ,0,-moveX));
                        }
                        else if(direction == IADirection.LEFT) {
                            rb.AddForce(new Vector3(moveZ,0,moveX));
                        }
                    }
                    else if(gotoDestroy) {
                            Debug.Log("mooove");
                            transform.position = Vector3.MoveTowards(transform.position,destroyPos,speed * Time.deltaTime);
                    }
                    else if(goToPortal) {
                        Vector3 portalPos = controller.portal.transform.position;
                        transform.position = Vector3.MoveTowards(transform.position,new Vector3(portalPos.x + 10,portalPos.y,portalPos.z),speed * Time.deltaTime);

                        if (transform.position.x < controller.tpPos.x && isOnBall) {
                            controller.AddPoint(transform.gameObject);
                            percentage = beginPercentage;
                            transform.position = controller.portalPoints[controller.ConvertPlayerInt(transform.gameObject)];
                            hasCheckPos = false;
                            area = Area.NONE;  
                            gotoDestroy = false;
                            hasGenPosition = false;
                            finishMove = false;
                            isOnBall = false;
                            goToPortal = false;
                        }
                    }


                    if(goToJump) {
                        Jump(0,106);
                        goToJump = false;
                    }

                    if(checkPosition && !dead) {
                        Vector3 position = transform.position;
                        checkPosition = false;
                        RunDelayed(1.5f,() => {
                            Vector3 newPosition = transform.position;

                            if(!dead) {
                                if((int)position.x == newPosition.x && (int)position.z == (int)newPosition.z) {
                                    moveZ = speed;
                                }
                            }

                            checkPosition = true;
                        });
                    }

                    
                }
                else if(dead) { // le joueur est mort
                    time += Time.deltaTime;

                    if(time >= 0.2) {  
                        transform.gameObject.GetComponent<MeshRenderer>().enabled = !transform.gameObject.GetComponent<MeshRenderer>().enabled;
                        transform.GetChild(0).gameObject.GetComponent<MeshRenderer>().enabled = !transform.GetChild(0).gameObject.GetComponent<MeshRenderer>().enabled;
                        time = 0;
                        seconds++;
                    }

                    if(seconds == 15) {
                        dead = false;
                        transform.gameObject.GetComponent<MeshRenderer>().enabled = true;
                        transform.GetChild(0).gameObject.GetComponent<MeshRenderer>().enabled = true;
                        seconds = 0;
                        time = 0;
                        moveZ = 0;
                        speed = 70;
                        moveX = -speed;
                        if(hasCheckPos)
                            hasCheckPos = false;
                        if(gotoDestroy)
                            gotoDestroy = false;
                    }
                }
            }
        }
        
        lastArea = area;
    }

    #endregion

    #region Customs Functions
    private void Jump(float moveForce,float amplifier) {
        rb.AddForce(new Vector3(moveForce * speed,jumpSpeed + amplifier,0),ForceMode.Impulse);
        isJumping = true;
    }

    private void CheckArea() {
        if((area == Area.SIX || area == Area.NINE) && moveZ == 50 && transform.position.z > -5) 
            moveZ = 0;
        if((area == Area.SEVEN || area == Area.TEN) && moveZ == -50 && transform.position.z < -5) 
            moveZ = 0;
    }

    private int CheckPosWall() {
        Vector3 pos = transform.position;

        if((int)pos.x > (int)controller.walls[0].transform.position.x) 
            return 0;
        if((int)pos.x > (int)controller.walls[1].transform.position.x) 
            return 1;
        if((int)pos.x > (int)controller.walls[2].transform.position.x) 
            return 2;
        if((int)pos.x > (int)controller.walls[3].transform.position.x) 
            return 3;

        return -1;
    }

    #endregion

    #region Collision's Functions
    private void OnCollisionEnter(Collision hit) {
        if(hit.gameObject.tag == "Sol") {
            isJumping = false;
            lastHit = hit.gameObject;
        }

        if(hit.gameObject.tag == "BlueBall" || hit.gameObject.tag == "GreenBall" || hit.gameObject.tag == "YellowBall") {
            isOnBall = true;
            Vector3 centerPos = hit.gameObject.GetComponent<SphereCollider>().bounds.center;
            transform.position = new Vector3(centerPos.x,centerPos.y + 10.5f,centerPos.z);
            hit.gameObject.transform.parent = transform; 
        }
    }

    private void OnCollisionExit(Collision hit) {
        if(hit.gameObject.tag == "Sol") {
            respawnPos = transform.position;
            if(area == KBIA.Area.ELEVEN ||area == KBIA.Area.TWELVE || area == KBIA.Area.THIRTEEN)
                respawnPos.y = 69f;
            else 
                respawnPos.y = 61f;

            lastFloor = hit.gameObject;
            moveX = 0;
            moveZ = 0;
        }
    }

    #endregion
}
