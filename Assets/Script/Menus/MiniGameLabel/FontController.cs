using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FontController : MonoBehaviour {
    
    public Sprite[] images;
    private int index = 0;

    private bool active = true;

    void Start() {
        StartCoroutine(WaitEndFrame());

        GameController gController = GameObject.FindGameObjectsWithTag("Game")[0].GetComponent<GameController>();
        transform.parent.GetChild(3).GetChild(0).GetChild(4).GetChild(0).gameObject.GetComponent<Text>().text = gController.mgController.actualMiniGame.minigameName;
        transform.parent.GetChild(3).GetChild(0).GetChild(6).gameObject.GetComponent<Text>().text = gController.mgController.actualMiniGame.minigameDesc;
        transform.parent.GetChild(3).GetChild(0).GetChild(7).gameObject.GetComponent<Text>().text = gController.mgController.actualMiniGame.controls; 
        transform.parent.GetChild(3).GetChild(0).GetChild(7).gameObject.GetComponent<Text>().fontStyle = FontStyle.Bold;
    }

    private IEnumerator WaitEndFrame() {
        while(active) {
            yield return new WaitForSeconds(0.1f);

            GetComponent<Image>().sprite = images[index];

            if(index < 15) index++;
            else index = 0;
        }
    }
}
