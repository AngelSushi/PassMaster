using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DayController : MonoBehaviour {

    private Light light;

    public Material dayMat;
    public Material duskMat;
    public Material nightMat;
    public Material rainMat;

    public AudioSource mainAudio;
    public AudioClip dayAudio;
    public AudioClip duskAudio;

    public int dayPeriod;

    void Start() {
        light = GetComponent<Light>();

    }

    void Update() {

        switch(dayPeriod) {
            case 0: // Day
                RenderSettings.skybox = dayMat;
                light.intensity = 1.5f;
                mainAudio.clip = dayAudio;
                if(!mainAudio.isPlaying) mainAudio.Play();
                break;

            case 1: // Crepuscule
                RenderSettings.skybox = duskMat;
                light.intensity = 1f;
                mainAudio.clip = duskAudio;
                if(!mainAudio.isPlaying) mainAudio.Play();
                break;

            case 2: // Night
                light.intensity = 0.3f;
                RenderSettings.skybox = nightMat;
                mainAudio.clip = null;
                break;  

            case -1: // Pluie
                RenderSettings.skybox = rainMat;     
                break;          
        }
    }
}
