using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using System;

public class PMController : CoroutineSystem {
    
    public enum Menu {
        OPTIONS,
        MUSIC,
        CONTROLS,
        NONE
    }

    #region Variables
    public AudioSource menuOpen;
    public AudioSource menuClose;
    public AudioSource menuChange;
    public AudioSource menuSelect;
    public Sprite hoverButton;
    public Sprite normalButton;
    public Sprite normalControlsButton;
    public bool isMenuOpen;
    public int index = -1;
    private Menu action;
    private int menuID;
    private GameController controller;

    #endregion

    #region Unity's Functions

    void Start() {
        controller = GameObject.FindGameObjectsWithTag("Game")[0].GetComponent<GameController>();
    }
    

    #endregion

    #region Input Functions
    public void OnOpen(InputAction.CallbackContext e) {      
        if(e.started ) {
            if(!isMenuOpen) {
                menuOpen.Play();
                isMenuOpen = true;
                Time.timeScale = 0;
                action = Menu.OPTIONS;
                ManageMenu();
            }
            else 
                CloseMenu();    
        }
    }

    public void OnNext(InputAction.CallbackContext e) {
        if(e.started && isMenuOpen) {
            if(index < 3 && menuID >= 0 && menuID <= 1) {
                menuChange.Play();
                index++;
                transform.GetChild(menuID).GetChild(2 + index).gameObject.GetComponent<Image>().sprite = hoverButton;
                if(index > 0)
                    transform.GetChild(menuID).GetChild(1 + index).gameObject.GetComponent<Image>().sprite = normalButton;
            }         
        }
    }

    public void OnPrevious(InputAction.CallbackContext e) {
        if(e.started && isMenuOpen) {
            if(index > 0 && menuID >= 0 && menuID <= 1) {
                menuChange.Play();
                index--;
                transform.GetChild(menuID).GetChild(2 + index).gameObject.GetComponent<Image>().sprite = hoverButton;
                if(index < 3)
                    transform.GetChild(menuID).GetChild( 3 + index).gameObject.GetComponent<Image>().sprite = normalButton;
            }
        }
    }

// TOUT CONVERTIR EN BOUTON
    public void OnInteract(InputAction.CallbackContext e) {
        if(e.started && isMenuOpen) {
            if(index >= 0) {
                switch(action) {
                    case Menu.OPTIONS:
                        if(index == 0) {
                            menuClose.Play();
                            isMenuOpen = false;
                            Time.timeScale = 1;
                            index = -1;
                            action = Menu.NONE;
                        }
                        if(index == 1)
                            action = Menu.MUSIC;
                        if(index == 2) {
                            action = Menu.CONTROLS;                       
                            menuSelect.Play();
                        }
                        if(index == 3) { 
                            if(controller.part == GameController.GamePart.MINIGAME) {

                            }
                            else { 
                                CloseMenu();
                                DialogController dialogController = controller.dialog;
                                Dialog currentDialog = dialogController.GetDialogByName("QuitGame");
                                dialogController.currentDialog = currentDialog;
                                dialogController.isInDialog = true;
                                dialogController.finish = false;
                                StartCoroutine(dialogController.ShowText(currentDialog.Content[0],currentDialog.Content.Length));
                            }
                        }
                        break;

                    case Menu.MUSIC:
                        if(index == 2) {
                            action = Menu.OPTIONS;
                            menuSelect.Play();
                        }
                        break;

                }

                ManageMenu();
            }
        }
    }

    #endregion

    #region Custom Functions
    private void ManageMenu() {
        transform.gameObject.SetActive(true);
        index = -1;

        for(int i = 0;i<2;i++) {
            transform.GetChild(i).gameObject.SetActive(false);
            
            for(int j = 0;j<2;j++) {
                transform.GetChild(i).GetChild(2 + j).gameObject.GetComponent<Image>().sprite = normalButton;
            }
        }
        transform.GetChild(1).gameObject.SetActive(false);
        transform.GetChild(1).GetChild(4).gameObject.GetComponent<Image>().sprite = normalButton;

        switch(action) {
            case Menu.OPTIONS:
                for(int i = 0;i<transform.GetChild(0).childCount;i++) {
                    transform.GetChild(0).GetChild(i).gameObject.SetActive(true);
                }
                transform.GetChild(0).gameObject.SetActive(true);
                menuID = 0;
                break;         
            case Menu.MUSIC:
                for(int i = 0;i<transform.GetChild(1).childCount;i++) {
                    transform.GetChild(1).GetChild(i).gameObject.SetActive(true);
                }
                transform.GetChild(1).gameObject.SetActive(true);
                menuID = 1;
                break;
            case Menu.CONTROLS:
                for(int i = 0;i<transform.GetChild(0).childCount;i++) {
                    transform.GetChild(0).GetChild(i).gameObject.SetActive(false);
                }
                for(int i = 0;i<transform.GetChild(1).childCount;i++) {
                    transform.GetChild(1).GetChild(i).gameObject.SetActive(false);
                }
                transform.GetChild(2).gameObject.SetActive(true);
                menuID = 2;
                break;
        }
    }

    private void CloseMenu() {
        menuClose.Play();
        isMenuOpen = false;
        Time.timeScale = 1;
        index = -1;
        action = Menu.NONE;

        for(int i = 0;i<3;i++) {
            transform.GetChild(0).GetChild(2 + i).gameObject.GetComponent<Image>().sprite = normalButton;
            transform.GetChild(1).GetChild(2 + i).gameObject.GetComponent<Image>().sprite = normalButton;
            transform.GetChild(i).gameObject.SetActive(false);
        }

        for(int i = 0;i<14;i++) {
            transform.GetChild(2).GetChild(i).gameObject.GetComponent<Image>().sprite = normalControlsButton;
        }
    }
    
    #endregion

}
