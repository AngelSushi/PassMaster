using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SEController : CoroutineSystem {
    
    public GameObject goldPrefab;
    public GameObject ironPrefab;
    private bool hasGenSpike;
    public bool begin;
    private float beginTimer = 4f;
    public Text beginText;
    public AudioSource startSound;
    public AudioSource timerSound;
    public bool finish;
    private float gameTime = 45f;
    public Text timer;
    private string lastTimeText;
    public AudioSource timeSound;

    public static SEController instance;
    private string lastBeginText;
    private int spikeCount;
    private int percentageToChange;
    private float lastY;

    public List<GameObject> players;
    public List<GameObject> deadPlayers = new List<GameObject>();

    void Awake() {
        instance = this;
    }

    void Start() {
<<<<<<< HEAD
        switch(GameController.difficulty) {
=======
        switch(GameController.Instance.difficulty) {
>>>>>>> main
            case GameController.Difficulty.EASY:
                percentageToChange = 35;
                break;

            case GameController.Difficulty.MEDIUM:
                percentageToChange = 60;
                break;

            case GameController.Difficulty.HARD:
                percentageToChange = 85;
                break;
        }
    }

    void Update() {
        if(begin) {
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

        else {
            if(!finish) {
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
                
                GenSpike();
                GetIfFinish();
            }
        }
    }

    private void GenSpike() {
        if(!hasGenSpike) {
            hasGenSpike = true;
            RunDelayed(1f,() => {
                GameObject target;

                float y = 0;
                int random = Random.Range(0,2);
                int side = Random.Range(0,2);

                if(random == 1)
                    target = ironPrefab;
                else 
                    target = goldPrefab;

                if(spikeCount > 0) {
                    int percentage = Random.Range(0,100);

                    if(percentage <= percentageToChange) {
                        if(lastY == 142) 
                            y = 151;
                         else 
                            y = 142;
                    }
                    else 
                        y = lastY;        
                }
                else {
                    if(side == 0)
                        y = 142;
                    else 
                        y = 151;
                }

                Instantiate(target,new Vector3(7,y,-19),Quaternion.Euler(270,90,0));
                spikeCount++;
                hasGenSpike = false;
                lastY = y;
            });
        }
    }

    private void GetIfFinish() {
        if(deadPlayers.Count == players.Count - 1) 
            finish = true;
    }
}
