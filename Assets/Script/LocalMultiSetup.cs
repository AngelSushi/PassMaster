using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;
using UnityEngine.SceneManagement;
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

        [SerializeField] private GameObject skin;

        private GameObject _playerInstance;
        
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

        public GameObject Skin
        {
            get => skin;
            set => skin = value;
        }

        public GameObject PlayerInstance
        {
            get => _playerInstance;
            set => _playerInstance = value;
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


    private static LocalMultiSetup _instance;

    public static LocalMultiSetup Instance
    {
        get => _instance;
        private set => _instance = value;
    }

    [SerializeField] private List<Player> players = new List<Player>();
    [SerializeField] private GameObject playersParent;
    [SerializeField] private List<GameObject> waitingScreens;

    private bool _launchGame;
    private float _launchTimer;

    [SerializeField] private Text launchText;

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

        _launchTimer = 4f;
        
        DontDestroyOnLoad(transform.gameObject);
    }

    private void Update()
    {
        if (_launchGame)
        {
            _launchTimer -= Time.deltaTime;
            launchText.text = "Lancement dans " + (int)_launchTimer + " seconde(s)";

            if (_launchTimer <= 0)
            {
                _launchGame = false;
                SceneManager.LoadScene("NewMain",LoadSceneMode.Additive);
            }
        }
    }

    

    private void OnDestroy()
    {
        foreach (Player player in players)
        {
            player.Ready.started -= OnReady;
        }
    }

    public void OnPlayerJoin(PlayerInput input)
    {
        if (players.Where(player => player.Input == input).ToList().Count == 0)
        {
            InputAction readyAction = input.actions.FindAction("Menus/Quit");
            readyAction.started += OnReady;
            waitingScreens[PlayerCount].SetActive(false);
            Players.Add(new Player(input,PlayerCount,readyAction));
        }
    }



    public void OnPlayerLeave(PlayerInput input)
    {
        Player targetPlayer = Players.FirstOrDefault(player => player.Input == input);
        
        waitingScreens[PlayerCount - 1].SetActive(true);
        
        if (targetPlayer != null)
        {
            Players.Remove(targetPlayer);
        }
    }

    private void OnReady(InputAction.CallbackContext e)
    {
        Player player = players.First(player => player.Ready == e.action);
        player.Input.uiInputModule.GetComponent<MultiplayerEventSystem>().playerRoot.transform.GetChild(2).GetComponent<TextMeshProUGUI>().text = "<color=green> Pret";

        
        OnReady(player);
        
    } 
    public void OnReady(int playerIndex) => OnReady(players.First(player => player.PlayerIndex == playerIndex));
    
    private void OnReady(Player targetPlayer)
    {
        targetPlayer.IsReady = true;
        
        if (players.All(player => player.IsReady))
        {
            _launchGame = true;
            launchText.gameObject.SetActive(true);
            FindObjectOfType<PlayerInputManager>().enabled = false;
        }
    }
}
