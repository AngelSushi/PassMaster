using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotativeForce : MonoBehaviour
{


    private void OnTriggerEnter(Collider hit) {
        if(hit.gameObject.tag == "Player") {
            if(transform.parent.gameObject.GetComponent<RotativeObject>().left && transform.gameObject.tag == "Left") {
                hit.gameObject.GetComponent<KB_PlayerMovement>().thrusterForce = -10000; 
                hit.gameObject.GetComponent<KB_PlayerMovement>().thruster = true;   
            }
            else if(!transform.parent.gameObject.GetComponent<RotativeObject>().left && transform.gameObject.tag == "Right") {
                hit.gameObject.GetComponent<KB_PlayerMovement>().thrusterForce = 10000; 
                hit.gameObject.GetComponent<KB_PlayerMovement>().thruster = true; 
            }
            return;
        }

        if(hit.gameObject.tag == "RedBall") {
            if(transform.parent.gameObject.GetComponent<RotativeObject>().left && transform.gameObject.tag == "Left") {
                hit.gameObject.transform.parent.gameObject.GetComponent<KB_PlayerMovement>().thrusterForce = -10000; 
                hit.gameObject.transform.parent.gameObject.GetComponent<KB_PlayerMovement>().thruster = true;   
            }
            else if(!transform.parent.gameObject.GetComponent<RotativeObject>().left && transform.gameObject.tag == "Right") {
                hit.gameObject.transform.parent.gameObject.GetComponent<KB_PlayerMovement>().thrusterForce = 10000; 
                hit.gameObject.transform.parent.gameObject.GetComponent<KB_PlayerMovement>().thruster = true; 
            }
            return;
        }

        if(hit.gameObject.tag == "Bot") {
            if(transform.parent.gameObject.GetComponent<RotativeObject>().left && transform.gameObject.tag == "Left") {
                hit.gameObject.GetComponent<KBIA>().thrusterForce = -10000; 
                hit.gameObject.GetComponent<KBIA>().thruster = true;   
            }
            else if(!transform.parent.gameObject.GetComponent<RotativeObject>().left && transform.gameObject.tag == "Right") {
                hit.gameObject.GetComponent<KBIA>().thrusterForce = 10000; 
                hit.gameObject.GetComponent<KBIA>().thruster = true; 
            }
            return;
        }

        if(hit.gameObject.tag == "BlueBall" || hit.gameObject.tag == "GreenBall" || hit.gameObject.tag == "YellowBall") {
            if(transform.parent.gameObject.GetComponent<RotativeObject>().left && transform.gameObject.tag == "Left") {
                hit.gameObject.transform.parent.gameObject.GetComponent<KBIA>().thrusterForce = -10000; 
                hit.gameObject.transform.parent.gameObject.GetComponent<KBIA>().thruster = true;   
            }
            else if(!transform.parent.gameObject.GetComponent<RotativeObject>().left && transform.gameObject.tag == "Right") {
                hit.gameObject.transform.parent.gameObject.GetComponent<KBIA>().thrusterForce = 10000; 
                hit.gameObject.transform.parent.gameObject.GetComponent<KBIA>().thruster = true; 
            }
            return;
        }

        
    }



}
