using System;
using System.Collections;
using System.Collections.Generic;
using Melanchall.DryWetMidi.Interaction;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class NoteController : CoroutineSystem {

    public double timeInstantiated;

    public float timeLength;

    [HideInInspector] public NoteLane currentLane;
    
    private MusicController _controller;
    private bool _startPlayNote,_destroy;
    private float _performedTimer;

    private bool _hasHitLine;

    private void Start() {
        timeInstantiated = MusicController.GetAudioSourceTime();

        _controller = (MusicController)MusicController.instance;
    }

    public void ApplyInputs() {
        if(_controller == null)
            _controller = (MusicController)MusicController.instance;

        _controller.inputs.FindAction(currentLane.inputName).started += OnNotePressed;
        _controller.inputs.FindAction(currentLane.inputName).performed += OnNotePressed;
        _controller.inputs.FindAction(currentLane.inputName).canceled += OnNotePressed; 
    }

    public override void Update() {
        double timeSinceInstantiated = MusicController.GetAudioSourceTime() - timeInstantiated;
        float t = (float)(timeSinceInstantiated / (_controller.noteTime * 2));

        if (t > 1 && !_startPlayNote) {
            foreach (PN_AIController aiController in _controller.allAI) {
                if (aiController.aliveNotes.Contains(this))
                    aiController.aliveNotes.Remove(this);
            }

            Destroy(gameObject);
        }
        else
            transform.localPosition = Vector3.Lerp(Vector3.up * _controller.noteSpawnY,
                Vector3.up * _controller.noteDespawnX, t);

        if (_startPlayNote && MusicController.GetAudioSourceTime() <= (timeInstantiated + timeLength)) {
            _performedTimer += Time.deltaTime;

            if (_performedTimer >= 1f) {
                _performedTimer = 0f;
                _controller.AddPointToPlayer(_controller.players[0], 1);
            }
        }

        foreach (RaycastHit2D hit in Physics2D.RaycastAll(transform.position, Vector3.left)) {
            if (hit.collider != null) {
                if (hit.collider.gameObject == _controller.detection && !_hasHitLine) {
                    foreach(PN_AIController aiController in _controller.allAI)
                        aiController.RandomSucceed(this);
                    _hasHitLine = true;
                }
            }

        }
    }



    private void OnNotePressed(InputAction.CallbackContext e) {
        if (e.started) {
            float distance = Vector3.Distance(new Vector3(0, transform.position.y, 0), new Vector3(0, _controller.noteTapY, 0));

            if (distance <= _controller.goodMarginError) {
                _controller.AddPointToPlayer(_controller.players[0], distance <= _controller.perfectMarginError ? _controller.perfectReward : _controller.goodReward); // Always give to the first player to change with local players
                _controller.CreateNoteState(distance <= _controller.perfectMarginError);
                
                _startPlayNote = true;
                _destroy = true;

                transform.GetComponent<SpriteRenderer>().enabled = false;
                transform.GetChild(0).GetComponent<SpriteRenderer>().enabled = false;
            }
        }

        if (e.canceled) {
            _startPlayNote = false;
        }
    }
    
}
