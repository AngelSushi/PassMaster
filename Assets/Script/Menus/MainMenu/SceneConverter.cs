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
    [SerializeField] private InputDispatcher inputDispatcher;

    private GameController _gameInstance;

    private void Update()
    {
        if (GameController.Instance != null && !_startConvert)
        {
            _gameInstance = GameController.Instance;
            _gameInstance.enabled = false;
            _startConvert = true;

            List<LocalMultiSetup.Player> menuPlayers = localSetup.Players;
            
            for (int i = 0; i < menuPlayers.Count; i++)
            {

                GameObject player = FindPlayerObject(menuPlayers[i].Skin);

                if (player == null)
                {
                    Debug.Log("error when getting gameobject for skin " + menuPlayers[i].Skin.name);
                    continue;
                }
                
                _gameInstance.players[_gameInstance.actualPlayer] = player;
                menuPlayers[i].PlayerInstance = player;
                
                
            }

            for (int i = 0; i < _gameInstance.players.Length; i++)
            {
                if (_gameInstance.players[i] != null)
                {
                    continue;
                }

                _gameInstance.players[i] = FindAIObject(_gameInstance); 

                // Trouver le premier objet dans allPlayers qui n'est pas dans players
            }

            inputDispatcher.EnableDispatch();
            SceneManager.UnloadSceneAsync("MainMenu");
            _gameInstance.enabled = true;
        }
    }

    private GameObject FindPlayerObject(GameObject skin) => GameController.Instance.allPlayers.FirstOrDefault(player => player.name == skin.name);

    private GameObject FindAIObject(GameController gameInstance)
    {
        for (int i = 0; i < gameInstance.allPlayers.Count; i++)
        {
            if (gameInstance.players.ToList().Contains(gameInstance.allPlayers[i]))
            {
                continue;
            }

            return gameInstance.allPlayers[i];
        }

        return null;
    }
}
