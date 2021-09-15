using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class NavMeshTest : MonoBehaviour {
    
    public Transform awardLocation;
    public NavMeshAgent agent;
    public bool canJump;
    public bool isJumping;
    public float jumpSpeed;
    public Transform warpPosition;
    public CharacterController controller;
    public GameObject wallLeft;
    public GameObject wallRight;
    public GameObject testObj;
    public GameObject obstacle;
    public int pendingDifficulty; // Variable non définitive
    public bool drawPath;

    private NavMeshPath originalPath;
    private NavMeshPath actualPath;
    private float verticalVelocity = 0;
    public bool jump;
    private int count = 0;
    private int maxCount = 1;
    private float gravity = 300.0f;
    private float jumpHeight = 50f;
    private float speed;
    private Transform firstWall = null;
    private Vector3 loc = Vector3.zero;
    private bool canMoove = true;
    private Vector3 vec = Vector3.zero;
    private bool impulse;
    private int knockbackLeft = -23;
    private int knockbackForward = 0;
    private bool isWallJumping;
    private bool followDestination = true;
    private int numberObstacle = -1;
    private bool choosePath;
    private GameObject collideObj;

    void Start() {
        originalPath = new NavMeshPath();
        actualPath = new NavMeshPath();

        speed = agent.speed;

        if(pendingDifficulty == 1) numberObstacle = Random.Range(3,6);
        if(pendingDifficulty == 2) numberObstacle = Random.Range(2,4);
        if(pendingDifficulty == 3) numberObstacle = Random.Range(0,2);

        NavMesh.CalculatePath(transform.position,awardLocation.position,NavMesh.AllAreas,originalPath);
        Debug.Log("Length: " + originalPath.corners.Length);
        ShowPath(Color.red,originalPath,true);

    }

    void Update() {
   //     if(followDestination) agent.SetDestination(awardLocation.position);


     /*   if(choosePath) {
            NavMesh.CalculatePath(transform.position,awardLocation.position,NavMesh.AllAreas,actualPath);
            ShowPath(Color.blue,actualPath,false);
        }

        if(!choosePath && pendingDifficulty >= 1 ) {
            for(int i = 0;i<numberObstacle;i++) {
                int index = Random.Range(0,originalPath.corners.Length - 1);
                Vector3 corner = originalPath.corners[index];
                Vector3 nextCorner = Vector3.zero;

                if(index % 2 == 0) nextCorner = originalPath.corners[index + 1];
                else nextCorner = originalPath.corners[index - 1];
                
                if(corner != Vector3.zero && nextCorner != Vector3.zero) {
                    // On sait que : 
                    //  Xm = Xa + k(Xb-Xa)
                    // Avec cette équation on en déduit que : k = (Xm - Xa) / (Xb - Xa)
                    // On génère un nombre aléatoire entre Xa et Xb et on vérifie que k est compris dans l'intervalle [0;1]
                    // On fait la même chose pour Z

                    float Xm = Random.Range(corner.x,nextCorner.x);

                    float numérateurX = Xm - corner.x;
                    float dénominateurX = nextCorner.x - corner.x;

                    if(numérateurX / dénominateurX >= 0 && numérateurX / dénominateurX <= 1) {
                        float Zm = Random.Range(corner.z,nextCorner.z);

                        float numérateurZ = Zm - corner.z;
                        float dénominateurZ = nextCorner.z - corner.z;

                        if(numérateurZ / dénominateurZ >= 0 && numérateurZ / dénominateurZ <= 1) {
                            Vector3 obstacleVector = new Vector3(Xm,corner.y,Zm);

                            Instantiate(obstacle,obstacleVector,testObj.transform.rotation);
                            
                        }
                        else return;                     
                    }
                    else return;
                }
            }

            choosePath = true;
            
        }

       

        

     /*   if(agent.enabled) {            

            if(agent.isOnOffMeshLink && collideObj != null && !collideObj.GetComponent<LinkAction>().wallJump) Jump();
            
            else if(collideObj != null && collideObj.GetComponent<LinkAction>().wallJump) CheckWallJump();

        }

        else {

            if(controller.isGrounded && isWallJumping) {
                if(isWallJumping) {
                    isWallJumping = false;
                    agent.enabled = true;
                    followDestination = true;
                }
            }

            if(isJumping) {
                if(impulse) vec.x =  knockbackLeft * jumpSpeed;

                    verticalVelocity -= gravity * Time.deltaTime;

                    if(verticalVelocity < 0 && knockbackForward == 0) { 
                        knockbackLeft *= -1;
                        verticalVelocity = jumpHeight /2;
                        isJumping = true;
                        impulse = true;
                    }

                    // A PATCH SAUT PAS PARFAIT CHECK EN FRAME PAR FRAME
                    
                    vec.y = verticalVelocity * jumpSpeed;
                    vec.z = knockbackForward * jumpSpeed;
                
                    controller.Move(vec * Time.deltaTime);
            }
        }
        */
    }



    private void CheckWallJump() {
        int random = Random.Range(0,3);

        if((random == 1 || random == 0) &&  firstWall == null) {
            firstWall = collideObj.GetComponent<LinkAction>().wall_left;
            loc = new Vector3(firstWall.position.x,agent.gameObject.transform.position.y,firstWall.position.z);
        }

        if((random == 2 || random == 3 ) && firstWall == null) {
            firstWall = collideObj.GetComponent<LinkAction>().wall_right;
            loc = new Vector3(firstWall.position.x,agent.gameObject.transform.position.y,firstWall.position.z);
        }

        Vector3 agentLocation = new Vector3(transform.position.x,0,transform.position.z);
        Vector3 sphereLocation = new Vector3(loc.x,0,loc.z);

        float distanceToTarget = Vector3.Distance(agentLocation, sphereLocation);

        if(distanceToTarget > 0.32) {
            agent.SetDestination(loc); 
        }
    }

    private void Jump() {
        Vector3 moveVector = Vector3.zero;
        
        moveVector.z = agent.speed;

        if(controller.isGrounded) {
            verticalVelocity = -gravity * Time.deltaTime * 3f;

            if(canJump && !isJumping) {
                verticalVelocity = jumpHeight;
                isJumping = true;
            }          
        }

        else {
            moveVector.z = agent.speed * 4;

           if(canJump && !isJumping) {
                verticalVelocity = jumpHeight;
                isJumping = true;
            } 

            if(verticalVelocity < 0 && collideObj.GetComponent<LinkAction>().distance >= 6 && collideObj.GetComponent<LinkAction>().distance <= 12 && count < maxCount) {
                verticalVelocity = jumpHeight;
                count++;
            }

            verticalVelocity -= gravity * Time.deltaTime * 3f;
        }
            moveVector.y = verticalVelocity * jumpSpeed;

            controller.Move(moveVector * Time.deltaTime);      
    }


    private void OnControllerColliderHit(ControllerColliderHit hit) {
        if(hit.gameObject.tag == "Water") {
            isJumping = false;
            canJump = false;
            count = 0;
        }

        if(hit.gameObject.tag == "Sol") {
            if(isJumping && !isWallJumping) {
                isJumping = false;
                agent.Warp(transform.position);
                count = 0;
            }
        }
    }

    private void OnCollisionEnter(Collision other) {
        if(other.gameObject != wallLeft && other.gameObject != wallRight) {
            collideObj = other.gameObject;  
        }

    }

    private void OnCollisionStay(Collision hit) {  

        if(hit.gameObject == wallLeft || hit.gameObject == wallRight) {
            if(!impulse) {
                agent.enabled = false;
                followDestination = false;
                isWallJumping = true;

                if(hit.gameObject == wallRight) knockbackLeft *= -1;

                verticalVelocity -= gravity * Time.deltaTime;

                if(canJump && !isJumping) {
                    verticalVelocity = jumpHeight / 2;
                    isJumping = true;
                } 

                if(verticalVelocity < 0) { // Jump max
                    verticalVelocity = jumpHeight / 2;
                    isJumping = true;
                    impulse = true;
                }
            }
           

            
            
        }
    }

    private void OnTriggerExit(Collider hit) {
        if(hit.gameObject.tag == "JumpWall_Stop") {
            knockbackForward = 10;
            knockbackLeft = 0;
        }
    }

    private void ShowPath(Color color,NavMeshPath path,bool original) {
        for (int i = 0; i < path.corners.Length - 1; i++) {
            if(drawPath) {

                Debug.DrawLine(path.corners[i], path.corners[i + 1], color);
                if(original) Instantiate(testObj,path.corners[i],testObj.transform.rotation);
            }
        }
    }

    
}
