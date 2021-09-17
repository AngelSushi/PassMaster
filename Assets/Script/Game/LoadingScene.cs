using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LoadingScene : MonoBehaviour {
    
    public bool loadScene;
    public string mgSceneName;
    public Text loadingText;
    public GameObject bs;

    void Update() {
        if(loadScene) {
            bs.SetActive(true);
            StartCoroutine(LoadScene(mgSceneName));
            loadScene = false;
        }
    }

    private IEnumerator LoadScene(string name) {
        AsyncOperation operation = SceneManager.LoadSceneAsync("MiniGameLabel",LoadSceneMode.Additive);
        Debug.Log("enumerator");


        while(!operation.isDone) {
            
            float progress = Mathf.Clamp01(operation.progress / 0.9f);
            loadingText.text = "Loading... " + " (" + progress * 100f + "%)";
            yield return null;
        }

        Debug.Log("isDone");
        GameObject.FindGameObjectsWithTag("Game")[0].GetComponent<GameController>().ChangeStateScene("Main",false);
        GameObject.FindGameObjectsWithTag("Game")[0].GetComponent<GameController>().timer = 0;
        GameObject.FindGameObjectsWithTag("Game")[0].GetComponent<GameController>().index = 0;
        GameObject.FindGameObjectsWithTag("Game")[0].GetComponent<GameController>().maxTimer = 0;
        GameObject.FindGameObjectsWithTag("Game")[0].GetComponent<GameController>().step = 0;
        bs.SetActive(false);
    }
}
