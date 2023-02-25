using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugController  : MonoBehaviour
{


    /**
     *
     *  BoardGame : 
        * Pouvoir set le diceResult d'un joueur
        * Pouvoir Give de l'argent
        * Pouvoir Give des Items
        * Pouvoir se tp sur une case précise
        * Pouvoir se give du code secret 
     
        * Pouvoir skip un nombre de tour précis
     
        Pouvoir set le skin
     * 
     */

    public bool skipMG;

    public void ShowMenu() {
        transform.GetChild(0).gameObject.SetActive(true);
        Debug.Log("showMenu");
    }

}
