using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Playables;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Timeline;

public abstract class MiniGame : CoroutineSystem {

    public enum GameState {
        START,
        PLAYING,
        END
    }
    
    public bool finish,begin;
    private bool _lastFinish;
    public bool isTraining;
    public bool runSinceMenu;
    public float gameTime;
    public GameState actualState;

    public bool useTimer,useChrono;
    
    public Text chronoText,timer;
    public Text endText;
    public AudioSource startChrono,runChrono,endChrono;
    public AudioSource mainAudio;
    public Camera mainCamera;
    
    
    public List<GameObject> winners = new List<GameObject>();
    public AudioSource win;
    public GameObject[] classementPanels;
    public GameObject confetti;

    
    public Player[] players;
    
    protected bool _hasPlayedSfx;
    private float _beginTimer = 4f;
    private string _lastBeginText,_lastTimeText;
    private GameController _gameController;

    public static MiniGame instance;

    public PlayableDirector startCinematic;
    public Camera[] startCinematicCameras;
    public PlayableDirector endCinematic;
    public Camera[] endCinematicCameras;
    public InputActionAsset inputs;

    public GameObject circleTransition;

    private bool _hasGenerateBoard;

    
    public virtual void Awake() => instance = this;
    
    public virtual void Start() {
        _gameController = GameObject.FindGameObjectsWithTag("Game").Length > 0 ?  GameObject.FindGameObjectsWithTag("Game")[0].GetComponent<GameController>() : null;

        switch (actualState) {
            case GameState.START:
                if(startCinematic != null)
                    startCinematic.Play();
                break;
            case GameState.END:
                finish = true;
                break;
        }
    }



    public virtual void Update() {
        if(!finish) {
            
            if (startCinematic.state == PlayState.Playing) 
                return;
            
            if (startCinematicCameras[0].gameObject.activeSelf) {
                foreach(Camera camera in startCinematicCameras)
                    camera.gameObject.SetActive(false);
            }
            
            if(begin) 
                BeginTimer();            
            else 
                MiniGameTimer();     
        }
        else {
            if (!_lastFinish) {
                mainAudio.Stop();
                _lastFinish = true;
                OnFinish();
            }
        }

    }

    public void OnStartCinematicEndListener() { 
        actualState = GameState.PLAYING;
        OnStartCinematicEnd();
    }

    protected void FinishMiniGame() {
        if(isTraining) {
            SceneManager.LoadScene("MiniGameLabel",LoadSceneMode.Additive);
            SceneManager.UnloadSceneAsync(SceneManager.GetActiveScene().name); // Attention avec plusieurs scenes ouvertes
        }
        else if(runSinceMenu) {
            SceneManager.LoadScene("MainMenu",LoadSceneMode.Single);
            SceneManager.UnloadSceneAsync(SceneManager.GetActiveScene().name);
        }
        else 
            _gameController.mgController.EndMiniGame(classementPanels,winners); 
        
    }

    private void BeginTimer() {

        if (!useChrono && begin) {
            begin = false;
            return;
        }

        _beginTimer -= Time.deltaTime;
        float seconds = Mathf.FloorToInt(_beginTimer % 60);
                    
        if(seconds > 0)
             chronoText.text = "" + seconds;
        else
            chronoText.text = "GO";

        if(_lastBeginText == null || chronoText.text != _lastBeginText) {
            if(chronoText.text == "GO")
                startChrono.Play();
            else 
                runChrono.Play();
        }

        if(_beginTimer < 0) {
            chronoText.text = "";
            begin = false;
        }

        _lastBeginText = chronoText.text;
    }

    private void MiniGameTimer() {
        gameTime -= Time.deltaTime;

        float minutes = Mathf.FloorToInt(gameTime / 60);
        float seconds = Mathf.FloorToInt(gameTime % 60);

        if(gameTime > 0) {
            if (timer == null)
                return;
            
            if(seconds >= 10)
                timer.text = minutes + ":" + seconds;
            else 
                timer.text = minutes + ":0" + seconds;

            if(gameTime <= 10) {
                timer.gameObject.GetComponent<Outline>().enabled = true;
            
                if(timer.text != _lastTimeText) 
                    endChrono.Play();                        
            }

            _lastTimeText = timer.text;
        }
        else 
            finish = true;
    }

    public abstract void OnStartCinematicEnd();
    public abstract void OnFinish();
    public abstract void OnSwitchCamera();
    public abstract void OnTransitionEnd();
}
