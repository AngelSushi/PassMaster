using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DayController : MonoBehaviour {

    private Light light;

    public Material dayMat;
    public Material duskMat;
    public Material nightMat;
    public Material rainMat;

    public int dayPeriod;

    private AudioSource mainAudio;

    void Start() {
        light = GetComponent<Light>();

        mainAudio = AudioController.Instance.mainSource;

    }

    void Update() {

        switch(dayPeriod) {
            case DayPeriod.DAY: // Day
                RenderSettings.skybox = nightMat;
                light.intensity = 1.5f;
                mainAudio.clip = AudioController.Instance.mainAudioClip;
                if(!mainAudio.isPlaying) 
                    mainAudio.Play();
                break;

            case 1: // Crepuscule
                RenderSettings.skybox = duskMat;
                light.intensity = 1f;
                mainAudio.clip = AudioController.Instance.duskAudioClip;
                if(!mainAudio.isPlaying)
                     mainAudio.Play();
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
