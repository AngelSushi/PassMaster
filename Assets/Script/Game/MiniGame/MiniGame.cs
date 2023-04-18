using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public abstract class MiniGame : CoroutineSystem {

    public bool finish,begin;
    public bool isTraining;
    public bool runSinceMenu;
    public float gameTime;

    public bool useTimer,useChrono;
    
    public Text chronoText,timer;
    public Text endText;
    public AudioSource startChrono,runChrono,endChrono;
    public AudioSource mainAudio;
    
    
    public List<GameObject> winners = new List<GameObject>();
    public AudioSource win;
    public GameObject[] classementPanels;
    public GameObject confetti;

    
    public Player[] players;
    
    private bool _hasPlayedSfx;
    private float _beginTimer = 4f;
    private string _lastBeginText,_lastTimeText;
    private GameController _gameController;

    public static MiniGame instance;
    
    
    public InputActionAsset inputs;
    

    public virtual void Awake() {
        instance = this;
    }

    public virtual void Start() {
        
        _gameController = GameObject.FindGameObjectsWithTag("Game").Length > 0 ?  GameObject.FindGameObjectsWithTag("Game")[0].GetComponent<GameController>() : null;
        
        // Tools --> If Board Scene not Loaded -> AutoLoad
    }



    public virtual void Update() {
        if(!finish) {
            if(begin) 
                BeginTimer();            
            else 
                MiniGameTimer();     
        }
        else {
            OnFinish();

            mainAudio.Stop();

            if(!_hasPlayedSfx) {
                win.Play();
                _hasPlayedSfx = true;
                endText.gameObject.SetActive(true);
                confetti.SetActive(true);
                confetti.transform.position = winners[0].transform.position;
                confetti.GetComponent<ParticleSystem>().enableEmission = true;
                confetti.GetComponent<ParticleSystem>().Play();
            }


            if(isTraining) {
                SceneManager.LoadScene("MiniGameLabel",LoadSceneMode.Additive);
                SceneManager.UnloadSceneAsync(SceneManager.GetActiveScene().name); // Attention avec plusieurs scenes ouvertes
            }
            else if(runSinceMenu) {
                SceneManager.LoadScene("MainMenu",LoadSceneMode.Single);
                SceneManager.UnloadSceneAsync(SceneManager.GetActiveScene().name);
            }
            else 
                _gameController.mgController.EndMiniGame(classementPanels,winners,endText.gameObject); 
 
        }
    }

    private void BeginTimer() {
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

    public abstract void OnFinish();
}
