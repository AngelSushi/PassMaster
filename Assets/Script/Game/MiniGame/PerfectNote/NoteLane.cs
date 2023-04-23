using System.Collections;
using System.Collections.Generic;
using Melanchall.DryWetMidi.Interaction;
using Melanchall.DryWetMidi.MusicTheory;
using UnityEngine;
using UnityEngine.InputSystem;
using Note = Melanchall.DryWetMidi.Interaction.Note;

public class NoteLane : MonoBehaviour {

    public NoteName noteRestriction;
    public string inputName;
    public Color laneColor;
    public Sprite laneKeySprite;

    private List<double> _timeStamps = new List<double>();
    private List<double> _timeLengths = new List<double>();
    private List<Note> _notes = new List<Note>();

    private int _spawnIndex;

    private MusicController _controller;
    
    void Start() {
        _controller = (MusicController)MusicController.instance;
    }

    void Update() {
        if (_spawnIndex < _timeStamps.Count) {
            
            if (MusicController.GetAudioSourceTime() >= _timeStamps[_spawnIndex] - _controller.noteTime) {
                GameObject note = Instantiate(_controller.notePrefab, transform.position + Vector3.up * _controller.noteSpawnY,Quaternion.identity);
              /*  GameObject longNote = Instantiate(_controller.notePrefab, transform.position + new Vector3(0, (float)_timeLengths[_spawnIndex], 0) * _controller.noteSpawnY, Quaternion.identity);
                
                longNote.transform.parent = transform;
                longNote.GetComponent<SpriteRenderer>().color = laneColor;
                longNote.GetComponent<NoteController>().currentLane = this;
                longNote.GetComponent<NoteController>().ApplyInputs();

                longNote.name = "Long Note of " + note.name;
                
                // longNote.GetComponent<NoteController>().time = (float)_timeStamps[_spawnIndex];
                longNote.GetComponent<NoteController>().timeLength = (float)_timeLengths[_spawnIndex];
                longNote.GetComponent<NoteController>().targetNote = _notes[_spawnIndex];
  */
              
                note.transform.parent = transform;
                note.GetComponent<SpriteRenderer>().color = laneColor;
                note.GetComponent<NoteController>().currentLane = this;
                note.GetComponent<NoteController>().ApplyInputs();
                
                note.GetComponent<NoteController>().timeLength = (float)_timeLengths[_spawnIndex];

                note.name = _spawnIndex.ToString();
                
                foreach(PN_AIController aiController in _controller.allAI) 
                    aiController.aliveNotes.Add(note.GetComponent<NoteController>());

                
                _spawnIndex++;
            }
        }
    }

    public void SetTimeStamps(List<Note> notes) {
        foreach (Note note in notes) {
            if (note.NoteName == noteRestriction) {
                var convertTime = TimeConverter.ConvertTo<MetricTimeSpan>(note.Time, MusicController.midiFile.GetTempoMap());
                double time = (double)convertTime.Minutes * 60f + convertTime.Seconds + (double)convertTime.Milliseconds / 1000f;
                _timeStamps.Add(time);
                
                var convertEndTime = TimeConverter.ConvertTo<MetricTimeSpan>(note.EndTime, MusicController.midiFile.GetTempoMap());
                double endTime = (double)convertEndTime.Minutes * 60f + convertEndTime.Seconds + (double)convertEndTime.Milliseconds / 1000f;
                _timeLengths.Add(endTime - time);
                
                _notes.Add(note);
            }
        }
    }
}
