using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using System.Linq;
using UnityEngine.Events;

public class FindKey_IAController : MonoBehaviour {

    // Path
    public Transform awardLocation;
    public NavMeshAgent agent;
    public GameObject wallLeft;
    public GameObject wallRight;
    public GameObject corners;
    public GameObject obstacle;
    public bool drawPath;
    public GameObject[] fantoms = new GameObject[2];
    public GameObject[] areas = new GameObject[8];
    private NavMeshPath originalPath;
    private NavMeshPath actualPath;
    private NavMeshPath wallPath;
    public bool choosePath;

    // Relative Jump
    public bool canJump;
    public bool isJumping;
    public float jumpSpeed;
    private float verticalVelocity = 0;
    private bool jump;
    private int count = 0;
    private int maxCount = 1;
    private float gravity = 300.0f;
    private float jumpHeight = 175f;
    private JumpAction jumpAction;

    // Relative WallJump
    private Vector3 vec = Vector3.zero;
    private bool impulse;
    private int knockbackLeft = -35;
    private int knockbackForward = 0;
    private bool isWallJumping;

    // Relative Difficulty
    public int pendingDifficulty; // Variable non définitive 

    // Attention au wallpath quand il faut faire un walljump 
    private NavMeshPath easyPath;
    private NavMeshPath mediumPath;
    private NavMeshPath hardPath;
    private NavMeshPath choosenPath;

    // Other
    public CharacterController controller;
    private GameObject collideObj;
    private Transform targetWall = null;
    private bool hasGenObstacle;
    private int numberObstacle = -1;
    private int remainingObstacles;

    private bool[] hasGenObstacles = new bool[3];
    private bool[] hasGenPath = new bool[3];
    private bool waitCalculPath;

    public bool canMoove;
    public bool isDead;
    private bool waitMovablePlateform;
    private GameObject gameController;
    // A SUPPR
    public GameObject cornersTest;
    private bool canJumpOnMovePlatform;
    private Vector3 lastPos = Vector3.zero;


// CLEAN LES VARIABLES - Fait 
// MODIFIER LA SURFACE DE CALCUL DU PATH POUR EVITER QU'IL COLLE LA LIMITE DU BLOC
// FAIRE LES NOUVEAUX SAUTS AVEC LA PLATEFORME QUI BOUGE
// FAIRE LES CHEMINS EN FONCTION DE LA DIFFUCULTE - En cours
// Refaire le double jump
// Rajouter deux link bas sur les côtés de la plateforme qui bouge sur une seule ligne
// Vérifier les différentes path possible notamment quand le bot doit rester quelques temps sur les plateformes mouvantes

// Pour la rotation des obstacles --> Script qui donne la direction sur les sols


// GERER LES PATH
// IL FAUT GENERER LE PATH FACILE ET METTRE SES OBSTACLES
// ENSUITE FAIRE LE MATH MEDIUM EN FAISANT LE PATH FACILE

// RECALCULATE LE PATH AU MOMENT OU IL COLLIDE AVEC LINK POUR MOI SI CA REGLE LE PROBLEME DU PATH BIZARRE
    private bool waitFrame;
    private bool isOnPlatform;
    public Color color;

    void Start() {
        originalPath = new NavMeshPath();
        actualPath = new NavMeshPath();
        wallPath = new NavMeshPath();
        easyPath = new NavMeshPath();
        mediumPath = new NavMeshPath();
        hardPath = new NavMeshPath();
        choosenPath = new NavMeshPath();

    /*    Debug.Log("objMinX: " + bounds.min.x);
        Debug.Log("objMaxX: " + bounds.max.x);
        Debug.Log("objMinZ: " + bounds.min.z);
        Debug.Log("objMaxZ: " + bounds.max.z);

        */

        gameController = GameObject.FindGameObjectsWithTag("Game")[0];

       
    }

    void Update() {      


        // LE PROBLEME EST QUE LORSQUON DOIT GENERER 2 OBSTACLES SI LE PREMIER MARCHE MAIS PAS LE DEUXIEME ALORS CA VA REGENERER DEUX OBSTACLES SANS ENLEVER LE PREMIER QUI A MARCHER
        // FAIRE EN SORTE QUE SI CA MARCHE PAS A CHAQUE FOIS DE GENERER QUE LES OBSTACLES MANQUANTS
        
     //   agent.CalculatePath(awardLocation.position,originalPath);
                
      /*  if(!hasGenObstacles[0]) {
            bool result = CreateDifficultyPath(1,easyPath,originalPath);

            if(!result) return; 
            else { 
                hasGenObstacles[0] = true;
                StartCoroutine(WaitPath());
                CalculatePath(easyPath);   
                
            }
        }

        if(hasGenObstacles[0] && !hasGenObstacles[1]) {
            bool result = CreateDifficultyPath(2,mediumPath,originalPath);

            if(!result) return;
            else {
                hasGenObstacles[1] = true;
                StartCoroutine(WaitPath());
                CalculatePath(mediumPath);  
            }
        }

        if(hasGenObstacles[0] && hasGenObstacles[1] && !hasGenObstacles[2]) {
            bool result = CreateDifficultyPath(3,hardPath,originalPath);

            if(!result) return;
            else {
                hasGenObstacles[2] = true;
                StartCoroutine(WaitPath());
                CalculatePath(hardPath);
            }
        }

        if(hasGenObstacles[0] && hasGenObstacles[1] && hasGenObstacles[2]) {
            if(choosenPath.corners.Length == 0 && wallPath.corners.Length == 0) ChoosePath();        
        }



        if(easyPath.corners.Length > 0) //ActiveFantoms(false);

    /*    Debug.Log("originalPath: " + originalPath.corners.Length);
        Debug.Log("easyPath: " + easyPath.corners.Length + " status: " + easyPath.status);
        Debug.Log("mediumPath: " + mediumPath.corners.Length + " status: " + mediumPath.status);
        Debug.Log("hardPath: " + hardPath.corners.Length + " status: " + hardPath.status);
*/ 

 /*       if(!waitFrame) {
            waitFrame = true;
            return;
        }
*/        
        if(choosePath) {
            if(agent.enabled) {   

                if(!jump && canMoove && !isOnPlatform) {
                    if((int)awardLocation.position.y == 55 ) { // Le bot n'a pas besoin de wall jump
                        agent.CalculatePath(awardLocation.position,actualPath);
                        agent.SetPath(actualPath);
                        ShowPath(color,actualPath,false);
                    }

                    else{ // Le bot a besoin de walljump         
                        if(targetWall == null) {
                            int random = Random.Range(0,4);
                            if(random == 0 || random == 1) targetWall = wallRight.transform;
                            else if(random == 2 || random == 3) targetWall = wallLeft.transform;                       
                        }

                        else {
                            if((int)transform.position.y == 67) {
                                agent.CalculatePath(targetWall.position,wallPath);
                                agent.SetPath(wallPath);
                                ShowPath(Color.yellow,wallPath,false);
                            }
                            else {
                                agent.CalculatePath(awardLocation.position,actualPath);
                                agent.SetPath(actualPath);
                                ShowPath(Color.yellow,actualPath,false);
                            }
                        }
                        
                    }
                    
                }

            }

            else {
                if(jump) {
                    Jump();
                    return;
                }

                if(waitMovablePlateform) { // Gérer le saut entre le sol et une plateforme
                    GameObject platform = jumpAction.platform;
                    float minPlatformZ = platform.transform.position.z - 50f;
                    float maxPlatformZ = platform.transform.position.z + 50f;

                    if(minPlatformZ <= transform.position.z && transform.position.z <= maxPlatformZ && !canJumpOnMovePlatform) canJumpOnMovePlatform = true;
                    
                    if(canJumpOnMovePlatform) { // Attention aux link arrière ca peut créer des problemes de saut
                        Jump();
                    }     
                    return;
                }

                if(controller.isGrounded && isWallJumping) {
                    if(isWallJumping) {
                        isWallJumping = false;
                        agent.enabled = true;
                    }
                }

                if(isWallJumping) { 
                    if(impulse) {
                        vec.z =  knockbackLeft * jumpSpeed;
                    }
                        verticalVelocity -= gravity * Time.deltaTime;

                        if(verticalVelocity < 0 && knockbackForward == 0) { 
                            if(!impulse) impulse = true;
                            knockbackLeft *= -1;
                            verticalVelocity = jumpHeight /2;
                            isJumping = true;
                            impulse = true;
                        }
                        
                        vec.y = verticalVelocity * jumpSpeed;
                        vec.x = knockbackForward * jumpSpeed;
                    
                        controller.Move(vec * Time.deltaTime);
                        
                }
                
            }
        }      
    }

    private void Jump() {
        Vector3 moveVector = Vector3.zero;
        
        if(collideObj != null && collideObj.tag == "Sol" && lastPos == Vector3.zero) {
            lastPos = transform.position;
        }

        if(controller.isGrounded) {
            if(jumpAction.xAxis) {
                if(jumpAction.isNegative) moveVector.x = agent.speed / 1.5f * -1;
                else moveVector.x = agent.speed / 1.5f; 
            }
            if(jumpAction.zAxis) {
                if(jumpAction.isNegative) moveVector.z = agent.speed / 1.5f * -1;
                else moveVector.z = agent.speed / 1.5f;
            }

            verticalVelocity = -gravity * Time.deltaTime * 3f;

            if(canJump && !isJumping) {
                verticalVelocity = jumpHeight;
                isJumping = true;
            }          
        }

        else {
            if(jumpAction.xAxis) {
                if(jumpAction.isNegative) moveVector.x = agent.speed / 1.5f * -1;
                else moveVector.x = agent.speed / 1.5f;
            }
            if(jumpAction.zAxis) {
                if(jumpAction.isNegative) moveVector.z = agent.speed / 1.5f * -1;
                else moveVector.z = agent.speed / 1.5f;
            }

           if(canJump && !isJumping) {
                verticalVelocity = jumpHeight;
                isJumping = true;
            } 

            // Refaire le double jump
          /*  if(verticalVelocity < 0 && collideObj.GetComponent<LinkAction>().distance >= 6 && collideObj.GetComponent<LinkAction>().distance <= 12 && count < maxCount) {
                verticalVelocity = jumpHeight;
                count++;
            }
*/
            verticalVelocity -= gravity * Time.deltaTime * 3f;
        }

        moveVector.y = verticalVelocity * jumpSpeed;

        controller.Move(moveVector * Time.deltaTime);      
    }

    private bool CreateDifficultyPath(int difficulty,NavMeshPath path,NavMeshPath model) {
        
       // if(numberObstacle == -1) numberObstacle = remainingObstacles = Random.Range(0,difficulty + 1); A GARDER !!!

       if(difficulty == 1 && numberObstacle == -1) numberObstacle = remainingObstacles = 3;
       if(difficulty == 2 && numberObstacle == -1) numberObstacle = remainingObstacles = 2;
       if(difficulty == 3 && numberObstacle == -1) numberObstacle = remainingObstacles = 1;

        Debug.Log("difficulty: " + difficulty + " obstacle: " + numberObstacle);

       if(numberObstacle == 0) {
            numberObstacle = -1;
            return true;
        }
     // if(model.corners.Length == 0) return;

        for(int i = 0;i<remainingObstacles;i++) {
            int index = Random.Range(0,model.corners.Length - 1);
            
    //        Debug.Log("index: " + index);
    //        Debug.Log("length: " + model.corners.Length);

            Vector3 corner = model.corners[index]; // Index out of bound 
            Vector3 nextCorner = Vector3.zero;

            if(index % 2 == 0) nextCorner = model.corners[index + 1];
            else nextCorner = model.corners[index - 1];
                
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
                        Vector3 obstacleVector = new Vector3(Xm,corner.y + 5,Zm);

                        if(difficulty == 1) obstacle.name = "Easy_0" + numberObstacle + "";
                        if(difficulty == 2) obstacle.name = "Medium_0" + numberObstacle + "";
                        if(difficulty == 3) obstacle.name = "Hard_0" + numberObstacle + "";
                    
                        return ObstacleConditions((int)Xm,(int)Zm,obstacleVector,i);
                            
                    }
                    else return false;                     
                    
                }

                else return false;
                
                
            }
        }

        return false;
    }

// IL FAUT VERIFIER QUE LE POINT M(Xm;Y;Zm) appartient a un plane
    private bool ObstacleConditions(int Xm,int Zm,Vector3 obstacleVector,int i) { // CHECK CA A PAS LAIR DE FONCTIONNER
        if(!Enumerable.Range((int)transform.position.x - 5,11).Contains(Xm)) {
            if(!Enumerable.Range((int)transform.position.z - 5,11).Contains(Zm)) {     
                for(int j = 0;j<areas.Length;j++) {         
                    GameObject area = areas[j];
                    
                    Vector3 topLeft = cornersTest.transform.GetChild(j).GetChild(0).position;
                    Vector3 topRight = cornersTest.transform.GetChild(j).GetChild(1).position;
                    Vector3 bottomLeft = cornersTest.transform.GetChild(j).GetChild(2).position;
                    Vector3 bottomRight = cornersTest.transform.GetChild(j).GetChild(3).position;

// A TESTER
                    Debug.Log("Zm: " + Zm + "topLeftZ: " + topLeft.z + " topRight: " + topRight.z);
                    Debug.Log("Xm: " + Xm + "topLeftZ: " + topLeft.x + " topRight: " + topRight.x);

                    Bounds bounds = obstacle.GetComponent<Renderer>().bounds;
                    int distanceX = (int)bottomLeft.x - (int)topLeft.x;
                    int distanceZ = (int)topRight.z - (int)topLeft.z;

                    if(Zm >= topLeft.z && Xm <= topRight.z && Enumerable.Range((int)topLeft.z,distanceZ).Contains((int)bounds.min.z) && Enumerable.Range((int)topLeft.z,distanceZ).Contains((int)bounds.max.z)) {
                        if(Xm >= topLeft.x && Xm <= bottomLeft.x && Enumerable.Range((int)topLeft.x,distanceX).Contains((int)bounds.min.x) && Enumerable.Range((int)topLeft.x,distanceX).Contains((int)bounds.max.x)) {

                            Instantiate(obstacle,obstacleVector,corners.transform.rotation);
                            remainingObstacles--;
                            // TROP BIEN MON OBJET EST BIEN APPARU
                            if(i == numberObstacle - 1 || remainingObstacles == 0) {
                                //hasGenObstacle = true;
                                numberObstacle = -1;
                                Debug.Log("finish spawn obstacle");
                                return true;
                            }

                        }
                    }       
                }   

                     
            }
            else return false;
            
                            
        }
        else return false;
        
        return false;
    }

    private void ChoosePath() {
        int random = Random.Range(0,101);

        if(pendingDifficulty == 1) { // Difficulté Facile
            if(random >= 0 && random <= 85) {
                if(awardLocation.position.y == 63) choosenPath = easyPath;
                else wallPath = easyPath;
            }

            if(random > 85 && random <= 95) {
                if(awardLocation.position.y == 63) choosenPath = mediumPath;
                else wallPath = mediumPath;
            }

            if(random > 95 && random <= 100) {
                if(awardLocation.position.y == 63) choosenPath = hardPath;
                else wallPath = hardPath;
            }
        }

        if(pendingDifficulty == 2) { // Diffuculté Intermédiaire
            if(random >= 0 && random <= 85) {
                if(awardLocation.position.y == 63) choosenPath = mediumPath;
                else wallPath = mediumPath;
            }

            if(random > 85 && random <= 95) {
                if(awardLocation.position.y == 63) choosenPath = easyPath;
                else wallPath = easyPath;
            }

            if(random > 95 && random <= 100) {
                if(awardLocation.position.y == 63) choosenPath = hardPath;
                else wallPath = hardPath;
            }
        }

        if(pendingDifficulty == 3) { // Difficulté Difficile
            if(random >= 0 && random <= 85) {
                if(awardLocation.position.y == 63) choosenPath = hardPath;
                else wallPath = hardPath;
            }

            if(random > 85 && random <= 95) {
                if(awardLocation.position.y == 63) choosenPath = mediumPath;
                else wallPath = mediumPath;
            }

            if(random > 95 && random <= 100) {
                if(awardLocation.position.y == 63) choosenPath = easyPath;
                else wallPath = easyPath;
            }
        }

     //   choosePath = true;
    }

    private void CalculatePath(NavMeshPath path) {
        agent.CalculatePath(awardLocation.position,path);
            
            if(path.status != NavMeshPathStatus.PathInvalid) {
                GameObject[] obstacles = GameObject.FindGameObjectsWithTag("Obstacle");

                //foreach(GameObject obj in obstacles) Destroy(obj);   
            }
    }

    private void ActiveFantoms(bool enabled) {
        foreach(GameObject fantom in fantoms) {
            fantom.SetActive(enabled);
        }
    }

    private IEnumerator WaitPath() {
        yield return new WaitForSeconds(0.1f);
    }

    private void OnControllerColliderHit(ControllerColliderHit hit) {
        

        if(hit.gameObject.tag == "Sol" || hit.gameObject.tag == "Platform") {
            if(isJumping && !isWallJumping) {
                agent.enabled = true;
                jump = false;
                isJumping = false;
                agent.Warp(transform.position);
                count = 0;
                lastPos = Vector3.zero;
            }
        }

        if(hit.gameObject.tag == "Water") {
            agent.enabled = true;
         //   agent.Warp(lastPos);
            agent.enabled = false;
         //   isJumping = false;
        }


        if(hit.gameObject.tag == "SolHigh") {
             if(isWallJumping) {
                agent.enabled = true;

                jump = false;
                isWallJumping = false;
                isJumping= false;
                agent.Warp(transform.position);

            }
        }
    }

    private void OnCollisionEnter(Collision other) { 
        if(other.gameObject != wallLeft && other.gameObject != wallRight && other.gameObject.tag != "Link") {
            collideObj = other.gameObject;  
        }

        if(other.gameObject.tag == "Link") {
            Debug.Log("enter link: " + isJumping);

            jumpAction = other.gameObject.GetComponent<JumpAction>();
            agent.enabled = false;
            if(jumpAction.platform == null) jump = true;
            else waitMovablePlateform = true;
        }

        if(other.gameObject.tag == "Platform") {
            agent.enabled = false;
            transform.parent = other.transform;
            waitMovablePlateform = false;
            isOnPlatform = true;

        }

        if(other.gameObject.tag == "Key") {
            canMoove = false;
            agent.enabled = false;
            awardLocation = other.gameObject.transform;
            //gameController.GetComponent<FindKeyController>().Qualify(transform.gameObject,other.gameObject);
            //Destroy(other.gameObject);
            other.gameObject.SetActive(false);
            // Animation
        }

    }

    private void OnCollisionStay(Collision hit) {  

        if(hit.gameObject.tag == "JumpWall_Left" || hit.gameObject.tag == "JumpWall_Right") {
            if(!impulse) {
                agent.enabled = false;
                isWallJumping = true;

                if(hit.gameObject == wallRight) knockbackLeft *= -1;

                verticalVelocity -= gravity * Time.deltaTime;


                if(canJump && !isJumping) {
                    verticalVelocity = jumpHeight / 2;
                    isJumping = true;
                }

                vec.y = verticalVelocity * jumpSpeed;
                controller.Move(vec * Time.deltaTime);

            }      
        }

        if(hit.gameObject.tag == "Platform") {
            agent.enabled = true;
            

            if(transform.position.x != hit.gameObject.transform.position.x && transform.position.z != hit.gameObject.transform.position.z) agent.SetDestination(hit.gameObject.transform.position); 
            else agent.enabled = false;
            
            ShowPath(color,actualPath,true);
        }
    }

    private void OnTriggerExit(Collider hit) {
        if(hit.gameObject.tag == "JumpWall_Stop") {
            knockbackForward = 26;
            knockbackLeft = 0;
        }
    }

    private void ShowPath(Color color,NavMeshPath path,bool original) {

    //    Debug.Log("length: " + path.corners.Length);
    //    Debug.Log("status: " + path.status);

        for (int i = 0; i < path.corners.Length ; i++) {
            if(drawPath) {
               // if(original) Instantiate(corners,path.corners[i],corners.transform.rotation);
            }
        }

        for(int i = 0;i<path.corners.Length - 1;i++) {
            if(drawPath) {
                Debug.DrawLine(path.corners[i], path.corners[i + 1], color);
            }
        }

    }


}
