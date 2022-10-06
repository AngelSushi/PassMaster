using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadMiniGame : MonoBehaviour {

    private bool hasClick;

    public void RunMiniGame(bool training) {
        Debug.Log("run mg");
        StartCoroutine(LoadScene(training));
    }

    private IEnumerator LoadScene(bool training) {        
        if(!hasClick) {
            AsyncOperation operation = SceneManager.LoadSceneAsync(GameObject.FindGameObjectsWithTag("Game")[0].GetComponent<GameController>().mgController.actualMiniGame.minigameName,LoadSceneMode.Additive);
            hasClick = true;

            while(!operation.isDone) {
                yield return null;
            }

            SceneManager.UnloadSceneAsync("MiniGameLabel");
            
            if(training) {
                switch(GameObject.FindGameObjectsWithTag("Game")[0].GetComponent<GameController>().mgController.actualMiniGame.minigameName) {
                    case "FindPath":
                        GameObject.FindGameObjectsWithTag("ControllerMG")[0].GetComponent<FP_Controller>().isTraining = true;
                        break;

                    case "Archery":
                        GameObject.FindGameObjectsWithTag("ControllerMG")[0].GetComponent<ArcheryController>().isTraining = true;
                        break;

                    case "KeyBall":
                        GameObject.FindGameObjectsWithTag("ControllerMG")[0].GetComponent<KBController>().isTraining = true;
                        break;

                    case "HideAndSeek":
                        GameObject.FindGameObjectsWithTag("ControllerMG")[0].GetComponent<HSController>().isTraining = true;
                        break;
                }
            }

            
        }

    }
}
