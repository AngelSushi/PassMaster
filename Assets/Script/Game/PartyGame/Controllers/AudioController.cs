using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioController : MonoBehaviour {
    
    public static AudioController Instance { get; private set; }
    
    public AudioClip mainAudioClip;
    public AudioClip duskAudioClip;
    public AudioClip coinsGain;
    public AudioClip coinsLoose;
    public AudioClip buyLoose;
    public AudioClip error;
    public AudioClip cardGain;
    public AudioClip findSecretCode;
    public AudioClip buttonHover;
    public AudioClip buttonClick;
    public AudioClip earthQuake;
    public AudioClip lightning;
    public AudioClip footstep;
    
    public AudioSource mainSource;
    public AudioSource ambiantSource;


    void Awake() {
        Instance = this;
    }
}
