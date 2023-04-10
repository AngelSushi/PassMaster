using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;


using Melanchall.DryWetMidi.Core;
using Melanchall.DryWetMidi.Interaction;
using System.IO;
using System.Linq;
using UnityEngine.Networking;

public class MusicController : MiniGame {
    
   /* [SerializeField] private float songBPM; // How many beats there are in 1 minutes ; 161
    [HideInInspector] public float secPerBeat; // How many seconds there are in 1 beat
    private float _songPosition; // The current position of the song in seconds
    public float _beatSongPosition; // The current position of the song in beat;
    private float _dspSongTime; // How many songs have passed since the beginning of the song
    public float firstBeatOffset; // The offset to the first beat of the song in seconds ; 8.13

    private bool isPlayed;
    [SerializeField] private AudioSource music;

    [SerializeField] private GameObject notePrefab;
    [SerializeField] private Transform noteStart;
    [SerializeField] private Transform noteEnd;
    [SerializeField] private float startOffset;

    private float _distance;
    public float noteSpeed;
    private float _time;

    [SerializeField] private NoteCreator creator;


    private int counter;
    
    */
   [SerializeField] [Tooltip("The song delay in seconds")] public float songDelay;
   [SerializeField] [Tooltip("The input delay in milliseconds")] public int inputDelay;

   [SerializeField] private string fileLocation;
   public float noteTime;
   public float noteSpawnX;
   [SerializeField] private float noteTapX;
   [SerializeField] private double marginError;

   public float noteDespawnX {
       get {
           return noteTapX - (noteSpawnX - noteTapX);
       }
   }

   public NoteLane[] lanes;

   public static MidiFile midiFile;

   public override void Awake() {
       base.Awake();
   }

   void Start() {
       ReadMidiFile();
   }

   private void ReadMidiFile() {
       midiFile = MidiFile.Read(Application.streamingAssetsPath + "/" + fileLocation);
       GetDataFromMidi();
   }

   private void GetDataFromMidi() {
       List<Note> notes = midiFile.GetNotes().ToList();

       foreach (NoteLane lane in lanes)
       {
           lane.SetTimeStamps(notes);
       } 
       Invoke(nameof(StartSong),songDelay);
   }

   private void StartSong() {
      mainAudio.Play(); 
      Debug.Log("start main audio");
   }

   public static double GetAudioSourceTime() {
       
       
       return (double)Instance.mainAudio.timeSamples / Instance.mainAudio.clip.frequency;
   }
   
    /*public override void Awake() {
        base.Awake();
        secPerBeat = 60f / songBPM; 
        _dspSongTime = (float)AudioSettings.dspTime;

        _distance = Vector2.Distance(noteStart.position, noteEnd.position);
        
        _time = _distance / noteSpeed;
        music.Play();
        
        Debug.Log("play mode");
        inputs.FindAction("Player/Jump").started += OnSpaceBar;
    }

    private void OnSpaceBar(InputAction.CallbackContext e) {
        Debug.Log("position " + _songPosition + " beatSongPosition " + _beatSongPosition);
    }

    private int index = 0;
    private bool active = false;

    private void Update() { 
        _songPosition = (float)(AudioSettings.dspTime - _dspSongTime - firstBeatOffset);
        _beatSongPosition = _songPosition / secPerBeat;

        if (creator.notes.Count == 0)
            return;

        NoteCreator.Note nextNote = creator.notes.Peek();
        
        //Debug.Log("index " + nextNote.noteIndex);
        //Debug.Log("nextNote " + nextNote.time + " beats " + nextNote.timeBeat);
            
        float pathDurationBeat = _time / secPerBeat;
        float spawnBeat = (nextNote.timeBeat - pathDurationBeat); // Caused the first note is on the beat number 1 and not number 0
      
        
        if(index > 0)
            return;

        
        if (_beatSongPosition >= spawnBeat && !active) { // Called exactly when the first note is spawning

            Vector3 position = noteStart.position;
            position.z -= startOffset * (nextNote.noteIndex - 1);
            Debug.Log("spawn " + noteEnd.transform.position);

            position.y += 0.03f;
            GameObject note = Instantiate(notePrefab, position,notePrefab.transform.rotation);
            note.GetComponent<NoteController>().startPos = position;
            note.GetComponent<NoteController>().note = nextNote;

            active = true;
            //creator.notes.Dequeue();
        }

        if (_beatSongPosition > +nextNote.timeBeat) {
            
            Vector3 position = noteEnd.position;
            position.z -= startOffset * (nextNote.noteIndex - 1);
            
            Debug.Log("spawn on perfect time");
            
            GameObject note = Instantiate(notePrefab, position,notePrefab.transform.rotation);
            note.GetComponent<NoteController>().enabled = false;
            index++;
        }
        
    }
*/
    
    
    public override void OnFinish() {
        
    }
}
