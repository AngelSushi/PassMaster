using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ammo : CoroutineSystem {

    public GameObject player;
    public ArcheryController controller;

    private void OnCollisionEnter(Collision hit) {
            if(hit.gameObject.tag == "Limit") {
                Destroy(transform.gameObject);
                Splash(hit);
            }

            if(hit.gameObject.tag == "Target") {
                Debug.Log("enter");

                Vector3 center = hit.gameObject.transform.GetChild(0).position;
                float distance = hit.GetContact(0).point.x - center.x;
                float secondDistance = transform.position.x - center.x;
                float thirdDistance = Vector2.Distance(new Vector2(transform.position.x,transform.position.y),new Vector2(center.x,center.y));
                if(distance < 0) 
                    distance *= -1;
                
                if(player.GetComponent<ArcheryIA>() != null)
                    player.GetComponent<ArcheryIA>().stopAmmo = true;

                if(thirdDistance >= 0.8f && thirdDistance <= 1.0f) {
                    Debug.Log("blanc");
                    controller.AddPoints(player,1);
                }
                else if(thirdDistance >= 0.6 && thirdDistance < 0.8) {
                    Debug.Log("noir");
                    controller.AddPoints(player,2);
                }
                else if(thirdDistance >= 0.4 && thirdDistance < 0.6) {
                    Debug.Log("bleu");
                    controller.AddPoints(player,3);
                }
                else if(thirdDistance >= 0.2 && thirdDistance < 0.4) {
                    Debug.Log("rouge");
                    controller.AddPoints(player,4);
                }
                else if(thirdDistance >= 0 && thirdDistance < 0.2) {
                    Debug.Log("jaune");
                    controller.AddPoints(player,5);
                }
                else {
                    Debug.Log("en dehors");
                    if(player.GetComponent<ArcheryIA>() != null)
                        player.GetComponent<ArcheryIA>().stopAmmo = false;
                    return;
                }        
             
                Splash(hit);
            }
    }

    private void Splash(Collision hit) {
        GameObject splash = Instantiate(controller.splashsPainting[ConvertPlayerIndex()]);
        splash.transform.position = new Vector3(transform.position.x,transform.position.y,hit.gameObject.transform.position.z + 0.16f);
        splash.transform.parent = hit.gameObject.transform;
        splash.AddComponent<SplashPainting>();

        transform.gameObject.GetComponent<MeshRenderer>().enabled = false;

        if(player.GetComponent<ArcheryIA>() != null) {
            RunDelayed(0.7f,() => {
                player.GetComponent<ArcheryIA>().shoot = false;
                player.GetComponent<ArcheryIA>().hasGenArea =  false;
                player.GetComponent<ArcheryIA>().chooseShifting =  false;
                player.GetComponent<ArcheryIA>().stopAmmo = false;
                player.GetComponent<ArcheryIA>().lastAngle = 0;
                Destroy(transform.gameObject);
            });
        }
        else {
            player.GetComponent<PaintballShoot>().shoot = false;
            player.GetComponent<PaintballShoot>().check = false;
            Destroy(transform.gameObject);

        }
    }

    private int ConvertPlayerIndex() {
        switch(player.name) {
            case "User":
                return 0;
            
            case "Bot_001":
                return 1;
            
            case "Bot_002":
                return 2;
            
            case "Bot_003":
                return 3;
        }

        return -1;  
    }
    
}
