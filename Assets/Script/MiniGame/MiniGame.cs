using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public abstract class MiniGame : CoroutineSystem {

    public bool finish,begin;
    private float beginTimer = 4f;

    public Text beginText,timer;
    private string lastBeginText,lastTimeText;
    public AudioSource startSound,timerSound,timeSound;
    public float gameTime;

    void Update() {
        if(!finish) {
            if(begin) 
                BeginTimer();            
            else 
                MiniGameTimer();     
        }
        else
            OnFinish();
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
