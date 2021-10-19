using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UserAudio : MonoBehaviour {
    
    public AudioSource coinsGain;
    public AudioSource coinsLoose;
    public AudioSource spawnChest;
    public AudioSource buyLoose;
    public AudioSource cardGain;
    public AudioSource findSecretCode;
    public AudioSource buttonHover;
    public AudioSource buttonClick;
    public AudioSource bombFall;
    public AudioSource bombExplode;
    public AudioSource invicibility;
    public AudioSource lightning;

    public void CoinsGain() {
        coinsGain.Play();
    }

    public void CoinsLoose() {
        coinsLoose.Play();
    }

    public void SpawnChest() {
        spawnChest.Play();
    }

    public void BuyLoose() {
        buyLoose.Play();
    }  

    public void CardGain() {
        cardGain.Play();
    } 

    public void FindSecretCode() {
        findSecretCode.Play();
    }

    public void ButtonHover() {
        buttonHover.Play();
    }

    public void ButtonClick() {
        buttonClick.Play();
    }

    public void BombFall(){
        bombFall.Play();
    }

    public void BombExplode() {
        bombFall.Stop();
        bombExplode.Play();
    }

    public void Invicibility() {
        invicibility.Play();
    }

    public void Lightning() {
        lightning.Play();
    }

}
