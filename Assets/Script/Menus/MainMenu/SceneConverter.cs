using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneConverter : MonoBehaviour
{

    private bool _startConvert;

    [SerializeField] private LocalMultiSetup localSetup;


    private void Update()
    {
        if (GameController.Instance != null && !_startConvert)
        {
            _startConvert = true;

            List<LocalMultiSetup.Player> menuPlayers = localSetup.players;
            List<GameObject> players = GameController.Instance.players.ToList();

            Debug.Log("size of " + menuPlayers.Count + " length " + localSetup.players.Count);
            
            for (int i = 0; i < menuPlayers.Count; i++)
            {
                Instantiate(menuPlayers[i].Skin, players[i].transform);
            }
            
            // Set the Index for ui
                                    

            SceneManager.UnloadSceneAsync("MainMenu");
            
            
            Debug.Log("enter board part");
        }
    }
}
