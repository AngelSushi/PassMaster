using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

[System.Serializable]
public class Dialog {

    public int id;
    public string Author;
    public string Name;
    public string[] Content;
    public bool NeedAnswer;
    public string[] Answers;
    public bool isFinish;
}

[System.Serializable]
 public class DialogArray {
    public Dialog[] dialogs;
}

public class DialogController : MonoBehaviour {

    public float speed;    
    public bool isInDialog;
    public Text text;
    public GameObject nextObj;
    public GameObject dialogObj;
    public DialogArray dialogArray;
    public bool finish = false;
    public GameObject answerObj;
    public Text textAnswer;
    public GameObject arrowObj;
    public Dialog currentDialog;

    private bool nextPage;
    private int index = 0;
    private GameController gController;

    private int answer = 0;

    private bool hasReturnToMainMenu;

    // Events
    public event EventHandler<OnDialogEndArgs> OnDialogEnd;
    public class OnDialogEndArgs : EventArgs {
        public Dialog dialog;
        public GameObject actualPlayer;
        public Vector3 position;
        public GameObject obj;
        public int answerIndex;
    }

    void Start() {
        gController = GameObject.FindGameObjectsWithTag("Game")[0].GetComponent<GameController>();
    }

    public void OnInteract(InputAction.CallbackContext e) { // APPELEZ QUAND LE JOUEUR APPUIE SUR E

        if(e.started) {
            if(isInDialog) 
                AccelerateDialog(0.01f); // Faire en sorte de décelerrer   

            if(nextPage && isInDialog && !finish) {
                nextPage = false;
                nextObj.SetActive(false);
                StartCoroutine(ShowText(currentDialog.Content[index],currentDialog.Content.Length));
            }

            if(finish) {
                if(answer >= 0) { // LE JOUEUR A UN CHOIX A FAIRE
                    OnDialogEndArgs args = new OnDialogEndArgs{ dialog = null, actualPlayer = null,position = Vector3.zero, obj = null,answerIndex = -1};
                    switch(answer) {
                        case 0:
                            
                            if(gController.part  == GameController.GamePart.DIALOG_TUTORIAL) {
                                // LANCER LE TUTORIAL
                                gController.part = GameController.GamePart.TUTORIAL;

                                EndDialog();

                                isInDialog = true;
                                currentDialog = GetDialogByName("StartTextTutorial");
                                StartCoroutine(ShowText(currentDialog.Content[0],currentDialog.Content.Length));
                            }

                            if(currentDialog.Name == "QuitGame" && !hasReturnToMainMenu) {
                                SceneManager.LoadScene("MainMenu",LoadSceneMode.Additive);
                                gController.ChangeStateScene(false,"NewMain");
                                ButtonController.relaunch = true;
                                EndDialog();
                                hasReturnToMainMenu = true; // Faire en sorte que quand on relance ca nous met la var en false
                            }

                            if(currentDialog.id == 0 || currentDialog.id == 7) {// Dialogue du shop
                                Vector3 shopVector =  gController.players[gController.actualPlayer].GetComponent<UserMovement>().actualStep.GetComponent<Step>().shop.transform.position;
                                args = new OnDialogEndArgs { dialog = currentDialog, actualPlayer = gController.players[gController.actualPlayer], position = shopVector,obj = gController.players[gController.actualPlayer].GetComponent<UserMovement>().actualStep.GetComponent<Step>().shop, answerIndex = answer};                               
                            }

                            if(currentDialog.id == 10 || currentDialog.id == 11 || currentDialog.id == 12 || currentDialog.id == 13) {
                                Vector3 chestVector = gController.players[gController.actualPlayer].GetComponent<UserMovement>().actualStep.GetComponent<Step>().chest.transform.position;
                                Debug.Log("chest: " + chestVector);
                                args = new OnDialogEndArgs { dialog = currentDialog, actualPlayer = gController.players[gController.actualPlayer], position = chestVector,obj = gController.players[gController.actualPlayer].GetComponent<UserMovement>().actualStep.GetComponent<Step>().chest, answerIndex = answer};                               
                            }
                            
                            break;
                        case 1:
                            if(gController.part == GameController.GamePart.DIALOG_TUTORIAL) {
                                // SKIP LE TUTO
                                gController.mainCamera.transform.position = new Vector3(gController.players[0].transform.position.x,5747.6f,gController.players[0].transform.position.z);
                                gController.mainCamera.transform.rotation = Quaternion.Euler(90f,265.791f,0f); 
                                // SEND DIRECTEMENT A LORDRE DE PASSAGE

                            }

                            break;
                    }
                    EndDialog(); 

                    OnDialogEnd?.Invoke(this,args); // Call only if OnDialogEnd is null ; you should write if(OnDialogEnd != null) ....
                                              
                }
                else { // LE JOUEUR NA PAS DE CHOIX A FAIRE
                    if(!answerObj.activeSelf) {

                        EndDialog();

                        if(gController.part  == GameController.GamePart.DIALOG_START_ALPHA) {
                            gController.part = GameController.GamePart.DIALOG_TUTORIAL;
                            isInDialog = true;

                            currentDialog = GetDialogByName("AskTextTutorial");
                            StartCoroutine(ShowText(currentDialog.Content[0],currentDialog.Content.Length));
                        }    

                        if(currentDialog.Name == "EndMoveTutorial") {
                            
                            gController.mainCamera.SetActive(false);
                            gController.players[0].transform.GetChild(1).gameObject.SetActive(true);

                            gController.mainCamera.GetComponent<CameraMovement>().zoomBox = false;
                        }

                        if(currentDialog.Name == "FindAllSecretCode" || currentDialog.Name == "FindNewCode") {
                            gController.players[gController.actualPlayer].transform.GetChild(1).gameObject.SetActive(false);
                            gController.mainCamera.transform.position = new Vector3(gController.players[gController.actualPlayer].transform.position.x,5747.6f,gController.players[gController.actualPlayer].transform.position.z);
                            gController.mainCamera.transform.rotation = Quaternion.Euler(90f,265.791f,0f);
                            gController.hasGenChest = false;

                        }
                    }
                }
                
            }
        }

    }

    public void OnNext(InputAction.CallbackContext e) { // Appelé quand le joueur change vers le bas de choix de réponse
       if(e.started) {
            if(answerObj.activeSelf && isInDialog) {
                if(answer < currentDialog.Answers[0].Split('\n').Length ) {
                    arrowObj.SetActive(true);
                    arrowObj.transform.localPosition = new Vector3(arrowObj.transform.localPosition.x,15 ,0);
                    answer++;
                }
            }
        }
    }

    public void OnPrevious(InputAction.CallbackContext e) { // Appelé quand le joueur change vers le haut de choix de réponse 
        if(e.started) {
            if(answerObj.activeSelf && isInDialog) {
                if(answer >= 1){
                    arrowObj.SetActive(true);
                    arrowObj.transform.localPosition = new Vector3(arrowObj.transform.localPosition.x,66,0);
                    answer--;
                }
            }
        }
    }

    void AccelerateDialog(float accelerate) {
        this.speed = accelerate;
    }

    public IEnumerator ShowText(string displayText,int length) {
        nextPage = false;
        dialogObj.SetActive(true);
        text.gameObject.SetActive(true);
        answerObj.SetActive(false);
        textAnswer.gameObject.SetActive(false);
        arrowObj.SetActive(false);

        for(int i = 1;i<displayText.Length;i++) {
           yield return new WaitForSeconds(speed);
           text.text = displayText.Substring(0,i);
       }

        if(length > 1 && index < length) {
            nextPage = true;
            index += 1;
            nextObj.SetActive(true);
        }

        if((length > 1 && index == length) || (length == 1)) {
            if(currentDialog.NeedAnswer) {
                answerObj.SetActive(true);
                textAnswer.gameObject.SetActive(true);
                arrowObj.SetActive(true);

                textAnswer.text = currentDialog.Answers[0];
            }

             finish = true;

        }

    }

    public void EndDialog() {
        dialogObj.SetActive(false);
        if(nextObj != null)
            nextObj.SetActive(false);
        
        text.gameObject.SetActive(false);
        answerObj.SetActive(false);
        textAnswer.gameObject.SetActive(false);
        arrowObj.SetActive(false);
        nextPage = false;
        index = 0;
        answer = 0;
        text.text = "";
        isInDialog = false;
        finish = false;
        currentDialog.isFinish = true;

    }

    public Dialog GetDialogByName(string name) {
         foreach (var dialog in dialogArray.dialogs) {
            if(dialog.Name == name) 
                return dialog;    
        }       

        return null;  
    }

}


