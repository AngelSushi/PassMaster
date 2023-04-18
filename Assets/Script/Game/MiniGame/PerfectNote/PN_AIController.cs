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
    
    
    
    
    /**
     *
     *
     *
     * Détecter si la note suivante faite partie d'un pattern => si oui continuait le pattern sinon le détruire 
     *
     *
     * 
     */
    
    [HideInInspector] public List<NoteController> aliveNotes = new List<NoteController>();
    [HideInInspector] public List<Note> allNotes = new List<Note>();
    [SerializeField] private Player player;
    
    private List<Note> _checkedNotes = new List<Note>();

    private float _succeedPercentage;
    [Tooltip("The amount of percentage which is removed when two notes follow each other")] [Range(0,1)] [SerializeField] private float followNote;
    [Tooltip("The amount of percentage which is added when the next note and the current note are in the same lane")] [Range(0,1)] [SerializeField] private float sameLane;
    [Tooltip("The percentage to succeed the note per difficulty")] [SerializeField] private float[] percentagesDifficulty;
    [Tooltip("The max percentage to succeed the note per difficulty")] [SerializeField] private float[] maxPercentageDifficulty;

    private float _currentMaxPercentageDifficulty;
    private MusicController _controller;
    

    private void Start() {
        _controller = (MusicController)MusicController.instance;
        
        _succeedPercentage = 1 - (1 - percentagesDifficulty[(int)GameController.Instance.difficulty]);
        _currentMaxPercentageDifficulty = maxPercentageDifficulty[(int)GameController.Instance.difficulty];
        
        
    }

    private void Update() {
        if (allNotes.Count > 0) {
            Note currentNote = allNotes[_checkedNotes.Count];

            if (aliveNotes.Count == 0)
                return;
            
            IEnumerable<NoteController> nc = aliveNotes.Where(noteController => noteController.targetNote == currentNote);
            
            if (currentNote == null || nc.ToList().Count == 0)
                return;

            NoteController noteController = nc.ToList()[0];
        
            float distance = Vector3.Distance(new Vector3(0, noteController.transform.position.y, 0), new Vector3(0, _controller.noteTapY, 0));

            if (distance <= _controller.goodMarginError && !_checkedNotes.Contains(currentNote)) {
                float randomSucceed = Random.Range(0f, 1f);

                if (_checkedNotes.Count + 1 < allNotes.Count) {
                    Note nextNote = allNotes[_checkedNotes.Count + 1];

                    NoteLane nextLane = _controller.lanes.Where(lane => lane.noteRestriction == nextNote.NoteName).ToList()[0];
                    NoteLane currentLane = _controller.lanes.Where(lane => lane.noteRestriction == currentNote.NoteName).ToList()[0];

                    if (nextLane == currentLane) // les deux notes qui se suivent sont sur la même ligne
                        randomSucceed -= sameLane;
                    if (currentNote.EndTime - nextNote.EndTime < 0.2f)  // Les deux notes qui se suivent ont un délai très court ==> On enleve de la chance de réussite 
                        randomSucceed -= followNote;
                    
                }

                
                randomSucceed = Mathf.Clamp(randomSucceed, 0, _currentMaxPercentageDifficulty);
              //  Debug.Log("random " + randomSucceed);


                if (randomSucceed <= _succeedPercentage) {
                    _controller.AddPointToPlayer(player,randomSucceed <= _succeedPercentage / 2 ? _controller.goodReward : _controller.perfectReward);
                  //  _currentPattern.Add(noteController.noteIndex);
                }
                
                _checkedNotes.Add(currentNote);
            }
            
        }
        
        
        
        
        
    }

}
