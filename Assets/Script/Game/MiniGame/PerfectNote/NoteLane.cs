using System.Collections;
using System.Collections.Generic;
using Melanchall.DryWetMidi.Interaction;
using Melanchall.DryWetMidi.MusicTheory;
using UnityEngine;
using Note = Melanchall.DryWetMidi.Interaction.Note;

public class NoteLane : MonoBehaviour {

    public NoteName noteRestriction;
    public string inputPath;
    public GameObject notePrefab;

    private List<NoteController> notes = new List<NoteController>();
    private List<double> timeStamps = new List<double>();

    private int spawnIndex = 0;
    private int inputIndex = 0;

    private MusicController _controller;
    void Start() {
        _controller = (MusicController)MusicController.Instance;
    }

    void Update() {
        if (spawnIndex < timeStamps.Count) {
            if (MusicController.GetAudioSourceTime() >= timeStamps[spawnIndex] - _controller.noteTime) {
                GameObject note = Instantiate(notePrefab, transform.position,Quaternion.identity);
                note.transform.parent = transform;
                notes.Add(note.GetComponent<NoteController>());
                note.GetComponent<NoteController>().assignedTime = (float)timeStamps[spawnIndex];
                spawnIndex++;
            }
        }
    }

    public void SetTimeStamps(List<Note> notes) {
        foreach (Note note in notes) {
            if (note.NoteName == noteRestriction) {
                var convert = TimeConverter.ConvertTo<MetricTimeSpan>(note.Time, MusicController.midiFile.GetTempoMap());
                timeStamps.Add((double)convert.Minutes * 60f + convert.Seconds + (double)convert.Milliseconds / 1000f);
            }
        }
    }
}
