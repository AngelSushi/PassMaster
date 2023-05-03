using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public class ButtonController : MonoBehaviour {

    public int menu;

    public GameObject[] buttons = new GameObject[3];
    public GameObject minigame;

    private string menuName;

    public static bool relaunch;
    public GameObject relaunchObj;
    public AudioSource start;

    private bool hasShowMenu;

    public GameObject champSelect;

    private void Start() {
        Debug.Log(FindObjectOfType<PlayerInput>());
    }

    void Update() {
        if(hasShowMenu) 
            ManageMenu();
    }

    public void OnClickButton(int button) {
        switch(menu) {
            case 0: // Premier Menu
                if(button == 0)  {// A cliquer sur le bouton jeu de plateau
                    menu = 1;
                    menuName = "JDP";
                }
            
                if(button == 1) { // A cliquer sur le bouton Mini-Jeux
                    menu = 1;
                    menuName = "MJ";
                }

                if(button == 2)  // A cliquer sur le bouton Boutique
                    menu = 2;
                
                break;

            case 1: // Menu Difficulté
<<<<<<< HEAD
            
                GameController.difficulty = button == 0 ? GameController.Difficulty.EASY : button == 1 ? GameController.Difficulty.MEDIUM : button == 2 ? GameController.Difficulty.HARD : GameController.Difficulty.EASY;
=======
                
                GameController.Instance.difficulty = button == 0 ? GameController.Difficulty.EASY : button == 1 ? GameController.Difficulty.MEDIUM : button == 2 ? GameController.Difficulty.HARD : GameController.Difficulty.EASY;
>>>>>>> main

                if(menuName == "JDP") 
                    menu = 3;
                   // SceneManager.LoadScene("Main",LoadSceneMode.Single);

                if(menuName == "MJ") {
                    foreach(GameObject btn in buttons) 
                        btn.SetActive(false);
                        
                    minigame.SetActive(true);
                }

                break;

            case 3:  // Choix des personnages
                SceneManager.LoadScene("Main",LoadSceneMode.Single);
                break;    
   
        }
    }
    
    private void ManageMenu() {
        if(menu == 0) {
            SetButtonVisible(true);
            
            
            if(relaunch) 
                relaunchObj.SetActive(true);
        }

        if(menu == 1) {
            SetButtonVisible(false);
            
            champSelect.SetActive(true);
        }

        if(menu == 2) {
            SetButtonVisible(false);
            
        }

        if(menu == 3) {
            SetButtonVisible(false);
        }  
    }

    private void SetButtonVisible(bool visible) {
        foreach(GameObject btn in buttons) 
            btn.SetActive(visible);
    }

    public void LaunchMiniGame(int minigame) {
        switch(minigame) {
            case 0: // Archery
                StartCoroutine(LoadMiniGame("Archery"));
                break;

            case 1: // findPath
                StartCoroutine(LoadMiniGame("FindPath"));
                break;

            case 2: // KeyBall
                StartCoroutine(LoadMiniGame("KeyBall"));
                break;

            case 3: // HideAndSeek
                StartCoroutine(LoadMiniGame("HideAndSeek"));
                break;
        }
    }

    private IEnumerator LoadMiniGame(string sceneName) {
        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneName,LoadSceneMode.Additive);

        while(!operation.isDone) {
            yield return null;
        }
        
        switch(sceneName) {
            case "FindPath":
                GameObject.FindGameObjectsWithTag("ControllerMG")[0].GetComponent<FP_Controller>().runSinceMenu = true;
                break;

            case "Archery":
                GameObject.FindGameObjectsWithTag("ControllerMG")[0].GetComponent<ArcheryController>().runSinceMenu = true;
                break;

            case "KeyBall":
                GameObject.FindGameObjectsWithTag("ControllerMG")[0].GetComponent<KBController>().runSinceMenu = true;
                break;
                
            case "HideAndSeek":
                GameObject.FindGameObjectsWithTag("ControllerMG")[0].GetComponent<HSController>().runSinceMenu = true;
                break;
        }

        
        ChangeStateScene("MainMenu",false);
            
    }
    public void BackButton() {
        switch(menu) {
            case 1:
                minigame.SetActive(false);
                menu = 0;
                break;
            case 2:
                menu = 0;
                break;
            case 3:
                minigame.SetActive(false);
                menu = 0;
                break;        
        }
    }

    public void ChangeStateScene(string sceneName,bool state) {
        GameObject[] objects = SceneManager.GetSceneByName(sceneName).GetRootGameObjects();

        foreach(GameObject obj in objects) {       
            obj.SetActive(state);
        }
    }

    public void OnShowMenu(InputAction.CallbackContext e) {
        if(e.started && !hasShowMenu) {
            hasShowMenu = true;
            start.Play();
        }   
    }

    public void RelaunchGame() {
        GameObject.FindGameObjectsWithTag("Game")[0].GetComponent<GameController>().ChangeStateScene(true,"NewMain");
        SceneManager.UnloadSceneAsync("MainMenu");
    }
}
