using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class VolumeControl : MonoBehaviour {
    
    public string volumeParameterName = "MasterVolume";
    public AudioMixer mixer;
    public Slider slider;
    public float multiplier;

    void Awake() {
        slider.onValueChanged.AddListener(SliderValueChanged);
    }

    void Start() {
        slider.value = PlayerPrefs.GetFloat(volumeParameterName);
    }

    private void SliderValueChanged(float value) {
        mixer.SetFloat(volumeParameterName,Mathf.Log10(value) * multiplier);
    }

    private void OnDisable() {
        PlayerPrefs.SetFloat(volumeParameterName,slider.value);
    }
}
