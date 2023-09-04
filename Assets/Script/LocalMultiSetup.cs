using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;
using UnityEngine.UI;

public class LocalMultiSetup : MonoBehaviour
{
    [Serializable]
    public class Player
    {
        [SerializeField] private int playerIndex;
        [SerializeField] private PlayerInput input;
        // Skin,Cosmetics etc

        [SerializeField] private bool isReady;

        private InputAction _ready;

        public int PlayerIndex
        {
            get => playerIndex;
            set => playerIndex = value;
        }

        public PlayerInput Input
        {
            get => input;
            set => input = value;
        }

        public bool IsReady
        {
            get => isReady;
            set => isReady = value;
        }

        public InputAction Ready
        {
            get => _ready;
            set => _ready = value;
        }

        public Player(PlayerInput pi,int playerIndex,InputAction ready)
        {
            Input = pi;
            PlayerIndex = playerIndex;
            Ready = ready;
        }
    }

    private int _playerCount;
    public int PlayerCount
    {
        get
        {
            _playerCount = Players.Count;
            return _playerCount;
        }
    }

    [SerializeField] private GameObject localPrefab;


    private static LocalMultiSetup _instance;

    public static LocalMultiSetup Instance
    {
        get => _instance;
        private set => _instance = value;
    }

    [SerializeField] private List<Player> players = new List<Player>();

    public List<Player> Players
    {
        get => players;
    }
    
    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
        }
    }

    public void OnPlayerJoin(PlayerInput input)
    {
        InputAction readyAction = input.actions.FindAction("Menus/Quit");
        readyAction.started += OnReady;

        Players.Add(new Player(input,PlayerCount,readyAction));
    }

    public void OnPlayerLeave(PlayerInput input)
    {
        Player targetPlayer = Players.FirstOrDefault(player => player.Input == input);

        if (targetPlayer != null)
        {
            Players.Remove(targetPlayer);
        }
        
        
    }

    private void OnReady(InputAction.CallbackContext e)
    {
        Player player = players.First(player => player.Ready == e.action);
        player.Input.uiInputModule.GetComponent<MultiplayerEventSystem>().playerRoot.transform.GetChild(2).GetComponent<TextMeshProUGUI>().text = "<color=green>Pret";
        OnReady(player);
        
    } 
    public void OnReady(int playerIndex) => OnReady(players.First(player => player.PlayerIndex == playerIndex));
    
    private void OnReady(Player targetPlayer)
    {
        targetPlayer.IsReady = true;
        Debug.Log("player " + targetPlayer.PlayerIndex + " is ready");
        if (players.All(player => player.IsReady))
        {
            Debug.Log("tout les joueurs sont prêts");
        }
    }
}
