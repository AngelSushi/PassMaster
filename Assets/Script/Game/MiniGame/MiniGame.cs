using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public abstract class MiniGame : CoroutineSystem {

    public bool finish,begin;
    public bool isTraining;
    public bool runSinceMenu;
    public float gameTime;
    public List<GameObject> winners = new List<GameObject>();
    public Text beginText,timer;
    public Text endText;
    public AudioSource startSound,timerSound,timeSound;
    public AudioSource mainAudio;
    public AudioSource win;
    public GameObject[] classementPanels;
    public GameObject confetti;

    public GameObject[] players;
    
    private bool hasPlayedSFX;
    private float beginTimer = 4f;
    private string lastBeginText,lastTimeText;
    private GameController gameController;

    public static MiniGame Instance;

    public virtual void Awake() {
        Instance = this;
    }

    public virtual void Start() {
        
        gameController = GameObject.FindGameObjectsWithTag("Game").Length > 0 ?  GameObject.FindGameObjectsWithTag("Game")[0].GetComponent<GameController>() : null;
        
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

            if(!hasPlayedSFX) {
                win.Play();
                hasPlayedSFX = true;
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
                gameController.mgController.EndMiniGame(classementPanels,winners,endText.gameObject); 
 
        }
    }

    private void BeginTimer() {
        beginTimer -= Time.deltaTime;
        float seconds = Mathf.FloorToInt(beginTimer % 60);
                    
        if(seconds > 0)
             beginText.text = "" + seconds;
        else
            beginText.text = "GO";

        if(lastBeginText == null || beginText.text != lastBeginText) {
            if(beginText.text == "GO")
                startSound.Play();
            else 
                timerSound.Play();
        }

        if(beginTimer < 0) {
            beginText.text = "";
            begin = false;
        }

        lastBeginText = beginText.text;
    }

    private void MiniGameTimer() {
        gameTime -= Time.deltaTime;

        float minutes = Mathf.FloorToInt(gameTime / 60);
        float seconds = Mathf.FloorToInt(gameTime % 60);

        if(gameTime > 0) {
            if(seconds >= 10)
                timer.text = minutes + ":" + seconds;
            else 
                timer.text = minutes + ":0" + seconds;

            if(gameTime <= 10) {
                timer.gameObject.GetComponent<Outline>().enabled = true;
            
                if(timer.text != lastTimeText) 
                    timeSound.Play();                        
            }

            lastTimeText = timer.text;
        }
        else 
            finish = true;
    }

    public abstract void OnFinish();
}
