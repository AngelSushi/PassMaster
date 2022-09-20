using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugController : MonoBehaviour {


    /**
     *
     *  BoardGame : 
        * Pouvoir set le diceResult d'un joueur
        * Pouvoir Give de l'argent
        * Pouvoir Give des Items
        * Pouvoir se tp sur une case précise
        * Pouvoir se give du code secret 
     
     
     
     *
     * 
     * 
     */

    private int currentMenuID = 0;

    private int lastMenuID = 0;

    public GameObject[] menus;

    private void Start() {
        if(menus == null || menus.Length == 0)
            Debug.Log("Aucun menu n'est défini pour le mode debug");
    }

    public void SwitchMenu(int menuID) {
        Debug.Log("switch menu");
        
        if (menuID >= menus.Length) {
            Debug.LogError("Erreur lors du switch de menu");
            return;
        }
        else if (menuID == -1)
        {
            //Fermer le menu
            return;
        }
 
        menus[currentMenuID].SetActive(false);
        menus[menuID].SetActive(true);

        lastMenuID = currentMenuID;
        currentMenuID = menuID;
        
        Debug.Log("switch success");
    }

    public void ValidateAction(int actionID) {
        switch (actionID) {
            
        }
    }
    
}
