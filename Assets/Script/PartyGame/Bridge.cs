using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bridge : MonoBehaviour
{
    public GameController controller;
    public GameObject[] bridgesBreak;
    public Transform[] distancePos;
    public bool breakBridge;

    // Changez la couleur de la texture du pont si elle est cassée - Fait (a test)
    // passez breakbridge en true - Fait
    // modifiez le chemin si le pont est cassé
    // mettre isTUrn sur true

    private void OnCollisionEnter(Collision hit) {
        if(hit.gameObject.tag == "Bomb") {
            controller.GetPlayers()[controller.GetActualPlayer()].GetComponent<UserAudio>().BombExplode();
            Destroy(hit.gameObject);

            float x = hit.gameObject.transform.position.x;
            float z = hit.gameObject.transform.position.z;

            Vector3 bridge_pos_01 = Vector3.zero;
            Vector3 bridge_pos_02 = Vector3.zero;
            Vector3 bridge_pos_03 = Vector3.zero;
            Quaternion bridge_rotation = Quaternion.Euler(0,0,0);

            if(transform.parent.gameObject.name == "bridge") {
                bridge_pos_01 = new Vector3(-836.5f,5176.4f,-15413.3f);
                bridge_pos_02 = new Vector3(-742.5f,5175.2f,-15425.1f);
                bridge_pos_03 = new Vector3(-649.8f,5175.2f,-15438.3f);
                bridge_rotation = Quaternion.Euler(0f,8.06f,0f);
            }

            if(transform.parent.gameObject.name == "bridge_01") {
                bridge_pos_01 = new Vector3(-1013.3f,5175.6f,-15458.7f);
                bridge_pos_02 = new Vector3(-1026.4f,5177.6f,-15366.2f);
                bridge_pos_03 = new Vector3(-1039.3f,5175.6f,-15271.9f);
                bridge_rotation = Quaternion.Euler(0,82.08701f,0f);
            }


            float distancePos01 = Vector2.Distance(new Vector2(distancePos[0].position.x,distancePos[0].position.z),new Vector2(x,z));
            float distancePos02 = Vector2.Distance(new Vector2(distancePos[1].position.x,distancePos[1].position.z), new Vector2(x,z));
            float distancePos03 = Vector2.Distance(new Vector2(distancePos[2].position.x,distancePos[2].position.z), new Vector2(x,z));

            if(distancePos01 < 0) distancePos01 *= -1;
            if(distancePos02 < 0) distancePos02 *= -1;
            if(distancePos03 < 0) distancePos03 *= -1;

            if(distancePos01 < distancePos02 && distancePos01 < distancePos03 && bridge_pos_01 != Vector3.zero && bridge_rotation != Quaternion.Euler(0,0,0)) {
                GameObject bridge = Instantiate(bridgesBreak[1],bridge_pos_01,bridge_rotation);
                bridge.transform.SetParent(transform.parent.parent);

                if(transform.parent.gameObject.name == "bridge") {
                    bridge.transform.SetSiblingIndex(2);
                    controller.bridgeRemainingTurn = 1;
                }
                else if(transform.parent.gameObject.name == "bridge_01") {
                    bridge.transform.SetSiblingIndex(3);
                    controller.bridgeRemainingTurn01 = 1;
                }

                transform.parent.SetSiblingIndex(transform.parent.parent.childCount - 1);
                bridge.transform.GetChild(0).gameObject.GetComponent<Bridge>().breakBridge = true;
                transform.parent.gameObject.SetActive(false);
                controller.WaitToBreak();
            }

            if(distancePos02 < distancePos01 && distancePos02 < distancePos03 && bridge_pos_02 != Vector3.zero && bridge_rotation != Quaternion.Euler(0,0,0)) {
                GameObject bridge = Instantiate(bridgesBreak[2],bridge_pos_02,bridge_rotation);
                bridge.transform.SetParent(transform.parent.parent); 
                
                if(transform.parent.gameObject.name == "bridge") {
                    bridge.transform.SetSiblingIndex(2);
                    controller.bridgeRemainingTurn = 1;
                }
                else if(transform.parent.gameObject.name == "bridge_01") {
                    bridge.transform.SetSiblingIndex(3);
                    controller.bridgeRemainingTurn01 = 1;
                }

                transform.parent.SetSiblingIndex(transform.parent.parent.childCount - 1);
                bridge.transform.GetChild(0).gameObject.GetComponent<Bridge>().breakBridge = true;
                transform.parent.gameObject.SetActive(false);
                controller.WaitToBreak();
            }

            if(distancePos03 < distancePos01 && distancePos03 < distancePos02 && bridge_pos_03 != Vector3.zero && bridge_rotation != Quaternion.Euler(0,0,0)) {
                GameObject bridge = Instantiate(bridgesBreak[0],bridge_pos_03,bridge_rotation);
                bridge.transform.SetParent(transform.parent.parent);
                
                if(transform.parent.gameObject.name == "bridge") {
                    bridge.transform.SetSiblingIndex(2);
                    controller.bridgeRemainingTurn = 1;
                }
                else if(transform.parent.gameObject.name == "bridge_01") {
                    bridge.transform.SetSiblingIndex(3);
                    controller.bridgeRemainingTurn01 = 1;
                }

                transform.parent.SetSiblingIndex(transform.parent.parent.childCount - 1);
                bridge.transform.GetChild(0).gameObject.GetComponent<Bridge>().breakBridge = true;
                transform.parent.gameObject.SetActive(false);
                controller.WaitToBreak();  
            }
            
            
            

        }
    }


    
}
