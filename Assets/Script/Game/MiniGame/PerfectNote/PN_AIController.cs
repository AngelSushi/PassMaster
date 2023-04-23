using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using Melanchall.DryWetMidi.Interaction;
using UnityEngine;
using Random = UnityEngine.Random;

public class PN_AIController : MonoBehaviour {
    
    // IA EXPLICATION : 
    
    // Facile = 1/3 
    // Moyen = 1/4
    // Difficile = 1/5
    
    // Si la prochaine note est sur la mm ligne == plus de chance de la taper 
    // Si l'écart entre deux même notes et faible alors plus forte chance de rater
    // Pour les notes longues, le pourcentage d'enlever est plus haut au fil du temps 
    // Faire un systeme de mémoire de rythme = s'il y a plusieurs fois le même enchainement de note ca rajoute de la facilité - Extra (voir Config/Perfectote/Pattern)

    [HideInInspector] public List<NoteController> aliveNotes = new List<NoteController>();
    [HideInInspector] public List<Note> allNotes = new List<Note>();
    public Player player;
    
    private List<NoteController> _checkedNotes = new List<NoteController>();

    private float _succeedPercentage;
    
    [Tooltip("The amount of percentage which is removed when two notes follow each other")] [Range(0,1)] [SerializeField] private float followNote;
    [Tooltip("The amount of percentage which is added when the next note and the current note are in the same lane")] [Range(0,1)] [SerializeField] private float sameLane;
    [Tooltip("The percentage to succeed the note per difficulty")] [SerializeField] private float[] percentagesDifficulty;

    private MusicController _controller;

    [HideInInspector] public int lastSucceedNoteIndex = -1;
    
    private void Start() {
        _controller = (MusicController)MusicController.instance;
        
        _succeedPercentage = 1 - (1 - percentagesDifficulty[(int)GameController.Instance.difficulty]);
        
        player = _controller.players.Where(player => player.gameObject == transform.gameObject).ToList()[0];
    }

    public void RandomSucceed(NoteController noteController) {
        Note currentNote = allNotes[_checkedNotes.Count];

        if (aliveNotes.Count == 0)
            return;
        
        float randomSucceed = Random.Range(0f, 1f);

        if (_checkedNotes.Count + 1 < allNotes.Count) {
            Note nextNote = allNotes[_checkedNotes.Count + 1];

            NoteLane nextLane = _controller.lanes.Where(lane => lane.noteRestriction == nextNote.NoteName).ToList()[0];
            NoteLane currentLane = _controller.lanes.Where(lane => lane.noteRestriction == currentNote.NoteName).ToList()[0];

            if (nextLane == currentLane) // les deux notes qui se suivent sont sur la même ligne
                randomSucceed -= sameLane;
            if (currentNote.EndTime - nextNote.EndTime < 0.2f)  // Les deux notes qui se suivent ont un délai très court ==> On enleve de la chance de réussite 
                randomSucceed += followNote;
                    
        }
        
        _checkedNotes.Add(noteController);
        randomSucceed = Mathf.Clamp(randomSucceed, 0, 1);

        if (randomSucceed <= _succeedPercentage) {
            int index = allNotes.IndexOf(currentNote);
            _controller.AddPointToPlayer(player, randomSucceed <= _succeedPercentage / 2 ? _controller.goodReward : _controller.perfectReward,index);
        }
    }
}
