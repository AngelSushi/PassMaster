using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyBallDetection : CoroutineSystem {
    
    private void OnCollisionEnter(Collision hit) {
        if(hit.gameObject.tag == "Speed") {
            if(transform.parent.gameObject.tag == "Player") {
                transform.parent.gameObject.GetComponent<KB_PlayerMovement>().speed += 10;
                Destroy(hit.gameObject);
            }
            else 
                Destroy(hit.gameObject);          
        }

        if(hit.gameObject.tag == "Destroy") {
            if(transform.parent.gameObject.tag == "Player") {
                transform.parent.gameObject.GetComponent<KB_PlayerMovement>().timeToDestroy -= 1;
                Destroy(hit.gameObject);
            }
            else {
                transform.parent.gameObject.GetComponent<KBIA>().timeToDestroy -= 1;
                Destroy(hit.gameObject);
            }
        }  

        if(transform.gameObject.tag == "Death") {
            if(hit.gameObject.tag == "Player") {
                Check(hit.gameObject.GetComponent<KB_PlayerMovement>().respawnPos,hit.gameObject.GetComponent<KB_PlayerMovement>().lastFloor,hit.gameObject);
                hit.gameObject.transform.position = hit.gameObject.GetComponent<KB_PlayerMovement>().respawnPos;
                hit.gameObject.GetComponent<KB_PlayerMovement>().dead = true;
            }
            else if(hit.gameObject.tag == "Bot") {
                Check(hit.gameObject.GetComponent<KBIA>().respawnPos,hit.gameObject.GetComponent<KBIA>().lastFloor,hit.gameObject);
                hit.gameObject.transform.position = hit.gameObject.GetComponent<KBIA>().respawnPos;
                hit.gameObject.GetComponent<KBIA>().dead = true;
                hit.gameObject.GetComponent<KBIA>().percentage -= 10;
            }
        }

        if(transform.gameObject.tag == "DeathArea") {
            if(hit.gameObject.tag == "Player") {
                Vector3 deathAreaPos = hit.gameObject.GetComponent<KB_PlayerMovement>().controller.areaDeath.position;
                deathAreaPos.y = 59.83f;
                hit.gameObject.transform.position = deathAreaPos;
                hit.gameObject.GetComponent<KB_PlayerMovement>().dead = true;
            }
            else if(hit.gameObject.tag == "Bot") {
                Vector3 deathAreaPos = hit.gameObject.GetComponent<KBIA>().controller.areaDeath.position;
                deathAreaPos.y = 59.83f;
                hit.gameObject.transform.position = deathAreaPos;
                hit.gameObject.GetComponent<KBIA>().percentage -= 10;
            }
        }
    }

    private void OnTriggerEnter(Collider hit) {
        if(transform.gameObject.tag == "Booster") {
            if(hit.gameObject.transform.parent.gameObject.tag == "Player") 
                hit.gameObject.transform.parent.gameObject.GetComponent<KB_PlayerMovement>().speed = 300;
            else if(hit.gameObject.transform.parent.gameObject.tag == "Bot") {   
                hit.gameObject.transform.parent.gameObject.GetComponent<KBIA>().speed = 300;
                hit.gameObject.transform.parent.gameObject.GetComponent<KBIA>().moveZ = 0;
            }
        }

        if(transform.gameObject.tag == "Jump" && (hit.gameObject.tag == "BlueBall" || hit.gameObject.tag == "GreenBall" || hit.gameObject.tag == "YellowBall" || hit.gameObject.tag == "RedBall")) {
            if(hit.gameObject.transform.parent.gameObject.GetComponent<KBIA>() != null) {
                if(hit.gameObject.transform.parent.gameObject.GetComponent<KBIA>().speed == 70) {
                    hit.gameObject.transform.parent.gameObject.GetComponent<KBIA>().speed += 12;
                }
                
                hit.gameObject.transform.parent.gameObject.GetComponent<KBIA>().goToJump = true;
            }
            if(hit.gameObject.transform.parent.gameObject.GetComponent<KB_PlayerMovement>() != null) {
                if(hit.gameObject.transform.parent.gameObject.GetComponent<KB_PlayerMovement>().speed == 75)
                    hit.gameObject.transform.parent.gameObject.GetComponent<KB_PlayerMovement>().speed += 12;
                
            }
            
        }

        if(transform.gameObject.tag == "DestroyBall" && hit.gameObject.tag == "RedBall" && hit.gameObject.transform.parent.gameObject.GetComponent<KB_PlayerMovement>().isOnBall) {
            hit.gameObject.transform.parent.gameObject.GetComponent<KB_PlayerMovement>().freeze = true;
            hit.gameObject.GetComponent<DestroyTimer>().maxTime = hit.gameObject.transform.parent.gameObject.GetComponent<KB_PlayerMovement>().timeToDestroy;
            hit.gameObject.GetComponent<DestroyTimer>().run = true;
        }
        else if(transform.gameObject.tag == "DestroyBall" && hit.gameObject.transform.parent.gameObject.tag == "Bot" && hit.gameObject.transform.parent.gameObject.GetComponent<KBIA>().isOnBall) {
            hit.gameObject.transform.parent.gameObject.GetComponent<KBIA>().freeze = true;
            hit.gameObject.GetComponent<DestroyTimer>().maxTime = hit.gameObject.transform.parent.gameObject.GetComponent<KBIA>().timeToDestroy;
            hit.gameObject.GetComponent<DestroyTimer>().run = true;
        }

    }

    private void OnTriggerExit(Collider hit) {
        if(transform.gameObject.tag == "Booster") {
            if(hit.gameObject.transform.parent.gameObject.tag == "Player") 
                hit.gameObject.transform.parent.gameObject.GetComponent<KB_PlayerMovement>().speed = 75;           
            else 
                hit.gameObject.transform.parent.gameObject.GetComponent<KBIA>().speed = 70;
            
        }
    }

    private void OnCollisionExit(Collision hit) {
        if(transform.gameObject.tag == "Socle" && (hit.gameObject.tag == "Bot" || hit.gameObject.tag == "Player")) {
            Vector3 soclePosition = transform.position;
            GameObject hitObj = hit.gameObject;

            RunDelayed(7f,() => {
                Vector3 ballPosition = Vector3.zero;
                int index = 0;

                if(hitObj.transform.childCount == 3)
                    index = 2;
                else if(hitObj.transform.childCount == 2)
                    index = 1;

                ballPosition = hitObj.transform.GetChild(index).position;
                Debug.Log("obj: " + hitObj);
                if(hitObj.tag == "Player") {
                    if(GameObject.FindGameObjectsWithTag("RedBall").Length > 2)
                        return;
                }
                else if(hitObj.tag == "Bot") {
                    if(GameObject.FindGameObjectsWithTag(hitObj.transform.GetChild(index).gameObject.tag).Length > 2)
                        return;
                }

                if(ballPosition.x != soclePosition.x && ballPosition.z != soclePosition.z) {
                    GameObject newBall = Instantiate(hitObj.transform.GetChild(index).gameObject,new Vector3(soclePosition.x,51.89f,soclePosition.z),Quaternion.Euler(270,270,0));   
                    newBall.transform.localScale = new Vector3(0.2752542f,0.2752542f,0.2752542f);             
                }
            }); 
        }
    }

    private void Check(Vector3 respawnPos,GameObject lastFloor,GameObject player) {
        Vector3 topLeft = lastFloor.GetComponent<FloorBounds>().topLeft;
        Vector3 topRight = lastFloor.GetComponent<FloorBounds>().topRight;
        Vector3 bottomLeft = lastFloor.GetComponent<FloorBounds>().bottomLeft;

        if((int)respawnPos.x >= (int)topLeft.x && respawnPos.x <= (int)bottomLeft.x) {
            if((int)respawnPos.z <=  (int)topRight.z && (int)respawnPos.z >= (int)topLeft.z ) {}
            else { // le z n'est pas bon
                RandomPosInBounds(topLeft,bottomLeft,topRight,topLeft.y,player);
            }
        }
        else { // le x n'est pas bon
            RandomPosInBounds(topLeft,bottomLeft,topRight,topLeft.y,player);
        }


    }

    private void RandomPosInBounds(Vector3 topLeft,Vector3 bottomLeft,Vector3 topRight,float y,GameObject player) {
        float x = Random.Range(topLeft.x,bottomLeft.x);
        float z = Random.Range(topLeft.z,topRight.z);

        Vector3 newRespawnPos = new Vector3(x,y,z);

        if(player.GetComponent<KBIA>() != null)
            player.GetComponent<KBIA>().respawnPos = newRespawnPos;
        else if(player.GetComponent<KB_PlayerMovement>() != null) 
            player.GetComponent<KB_PlayerMovement>().respawnPos = newRespawnPos;
    }
}
