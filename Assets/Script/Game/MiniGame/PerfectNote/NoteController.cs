using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoteController : MonoBehaviour {

    public double timeInstantiated;
    public float assignedTime;

    private MusicController controller;

    private void Start() {
        timeInstantiated = MusicController.GetAudioSourceTime();

        controller = (MusicController)MusicController.Instance;
    }

    private void Update() {
        double timeSinceInstantiated = MusicController.GetAudioSourceTime() - timeInstantiated;
        float t = (float)(timeSinceInstantiated / controller.noteTime / 2);

        if (t > 1) {
            Destroy(gameObject);
        }
        else {
            transform.localPosition = Vector3.Lerp(Vector3.left * controller.noteSpawnX, Vector3.left * controller.noteDespawnX, t);
        }
    }
}
