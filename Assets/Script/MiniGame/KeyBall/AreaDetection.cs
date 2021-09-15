using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AreaDetection : MonoBehaviour {

    private void OnTriggerEnter(Collider hit) {
        if(hit.gameObject.tag == "BlueBall" || hit.gameObject.tag == "GreenBall" || hit.gameObject.tag == "YellowBall") {
            
            switch(transform.gameObject.tag) {
                case "Area_01":
                    hit.gameObject.transform.parent.GetComponent<KBIA>().area = KBIA.Area.ONE;
                    RestorePercentage(hit);                   
                    break;

                case "Area_02":
                    hit.gameObject.transform.parent.GetComponent<KBIA>().area = KBIA.Area.TWO;
                    RestorePercentage(hit);  
                    break;                

                case "Area_03":
                    hit.gameObject.transform.parent.GetComponent<KBIA>().area = KBIA.Area.THREE;
                    RestorePercentage(hit);  
                    break;

                case "Area_04":
                    hit.gameObject.transform.parent.GetComponent<KBIA>().area = KBIA.Area.FOUR;
                    RestorePercentage(hit);  
                    break;

                case "Area_05":
                    hit.gameObject.transform.parent.GetComponent<KBIA>().area = KBIA.Area.FIVE;
                    RestorePercentage(hit);  
                    break;    

                case "Area_06":
                    hit.gameObject.transform.parent.GetComponent<KBIA>().area = KBIA.Area.SIX;
                    RestorePercentage(hit);  
                    break;  

                case "Area_07":
                    hit.gameObject.transform.parent.GetComponent<KBIA>().area = KBIA.Area.SEVEN;
                    RestorePercentage(hit);  
                    break;  

                case "Area_08":
                    hit.gameObject.transform.parent.GetComponent<KBIA>().area = KBIA.Area.EIGHT;
                    RestorePercentage(hit);  
                    break;  

                case "Area_09":
                    hit.gameObject.transform.parent.GetComponent<KBIA>().area = KBIA.Area.NINE;
                    RestorePercentage(hit);  
                    break;  

                case "Area_010":
                    hit.gameObject.transform.parent.GetComponent<KBIA>().area = KBIA.Area.TEN;
                    RestorePercentage(hit);  
                    break;  

                case "Area_011":
                    hit.gameObject.transform.parent.GetComponent<KBIA>().area = KBIA.Area.ELEVEN;
                    RestorePercentage(hit);  
                    break;  

                case "Area_012":
                    hit.gameObject.transform.parent.GetComponent<KBIA>().area = KBIA.Area.TWELVE;
                    RestorePercentage(hit);  
                    break;  

                case "Area_013":
                    hit.gameObject.transform.parent.GetComponent<KBIA>().area = KBIA.Area.THIRTEEN;
                    RestorePercentage(hit);  
                    break; 

                case "Area_014":
                    hit.gameObject.transform.parent.GetComponent<KBIA>().area = KBIA.Area.FOURTEEN;
                    RestorePercentage(hit);  
                    break; 

                case "EndSpeedBoost":
                    if(hit.gameObject.transform.parent.gameObject.tag == "Player") {
                        if(hit.gameObject.transform.parent.GetComponent<KB_PlayerMovement>().speed > 75)
                            hit.gameObject.transform.parent.GetComponent<KB_PlayerMovement>().speed = 75;
                    }
                    else if(hit.gameObject.transform.parent.gameObject.tag == "Bot") {
                        if(hit.gameObject.transform.parent.GetComponent<KBIA>().speed > 70)
                            hit.gameObject.transform.parent.GetComponent<KBIA>().speed = 70;
                    } 

                    
                    
                    break;              
            }

        }
    }

    private void OnTriggerExit(Collider hit) {
        if(transform.gameObject.tag == "Area_04") { // Check si c pas un joueur
            if(hit.gameObject.transform.parent.gameObject.GetComponent<KBIA>() != null) {
                hit.gameObject.transform.parent.gameObject.GetComponent<KBIA>().speed += 10;
            }
            if(hit.gameObject.transform.parent.gameObject.GetComponent<KB_PlayerMovement>() != null) {
                hit.gameObject.transform.parent.gameObject.GetComponent<KB_PlayerMovement>().speed += 10;
            }
        }

        if(transform.gameObject.tag  == "BeginSpeedBoost") {
            Debug.Log("enterSpeedBoost: " + hit.gameObject.transform.parent.gameObject.tag);
            Debug.Log("enterSpeedBoost2: " + hit.gameObject);
            
            if(hit.gameObject.transform.parent.gameObject.GetComponent<KB_PlayerMovement>() != null)
                hit.gameObject.transform.parent.gameObject.GetComponent<KB_PlayerMovement>().speed += 10;
            if(hit.gameObject.transform.parent.gameObject.GetComponent<KBIA>() != null)
                hit.gameObject.transform.parent.gameObject.GetComponent<KBIA>().speed += 10;         
        }

        if(transform.gameObject.tag == "Area_01" || transform.gameObject.tag == "Area_02" || transform.gameObject.tag == "Area_03" || transform.gameObject.tag == "Area_04" || transform.gameObject.tag == "Area_013" ) 
            if(hit.gameObject.transform.parent.gameObject.GetComponent<KBIA>() != null)
                hit.gameObject.transform.parent.gameObject.GetComponent<KBIA>().area = KBIA.Area.NONE;

    }

    private void RestorePercentage(Collider hit) {
        if(hit.gameObject.transform.parent.GetComponent<KBIA>().percentage <= 0) 
            hit.gameObject.transform.parent.GetComponent<KBIA>().percentage = hit.gameObject.transform.parent.GetComponent<KBIA>().beginPercentage / 2;
    }
}
