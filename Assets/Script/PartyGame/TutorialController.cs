using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AI;

public class TutorialController : MonoBehaviour {

    private static GameController gController;

    void Start() {
        gController = GameObject.FindGameObjectsWithTag("Game")[0].GetComponent<GameController>();
    }

    public void StartTutorial() {
        
        // Triage des joueurs
        
        gController.GetPlayers()[0].transform.rotation = Quaternion.Euler(0,114.5f,0);
        gController.GetPlayers()[0].transform.GetChild(1).gameObject.SetActive(true);
        gController.GetPlayers()[0].GetComponent<PlayerController>().isInTutorial = true;
        gController.GetPlayers()[0].GetComponent<PlayerController>().canJump = true;
        
        gController.light.transform.rotation = Quaternion.Euler(137.83f,-74.54f,0.3f);
        gController.getMainCamera().GetComponent<Camera>().enabled = false;


    }
}
