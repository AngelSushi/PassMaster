using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UserAudio : MonoBehaviour {

    public AudioSource userSource;

    public void CoinsGain() {
        userSource.clip = AudioController.Instance.coinsGain;
        userSource.Play();
    }

    public void CoinsLoose() {
        userSource.clip = AudioController.Instance.coinsLoose;
        userSource.Play();
    }

    public void BuyLoose() {
        userSource.clip = AudioController.Instance.buyLoose;
        userSource.Play();
    }  

    public void CardGain() { // Don't use userSource because it's already use by CoinsLoose while the chest opens
        AudioController.Instance.ambiantSource.clip = AudioController.Instance.cardGain;
        AudioController.Instance.ambiantSource.Play();
    } 

    public void FindSecretCode() {
        AudioController.Instance.ambiantSource.clip = AudioController.Instance.findSecretCode;
        AudioController.Instance.ambiantSource.Play();
    }

    public void ButtonHover() {
        userSource.clip = AudioController.Instance.buttonHover;
        userSource.Play();
    }

    public void ButtonClick() {
        userSource.clip = AudioController.Instance.buttonClick;
        userSource.Play();
    }

    public void Footstep() {
        userSource.clip = AudioController.Instance.footstep;
        userSource.Play();
    }
}
