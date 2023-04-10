using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;

[System.Serializable]
public class NoteExcelClass {
    public float Column1, Column3, Column4, Column5, Column6, Column7, Column8, Column9, Column10, Column11, Column12, Column13, Column14, Column15, Column16, Column17, Column18, Column19, Column20, Column21, Column22;
}

[System.Serializable]
public class NoteExcelArray {
    public NoteExcelClass[] excelClass;
}
public class NoteCreator : MonoBehaviour {

    [System.Serializable]
    public class Note {
        public float time;
        public float duration;
        public int noteIndex;

        private float _timeBeat;
        public float timeBeat; /* {
            get {
                return _timeBeat - ((MusicController)MusicController.Instance).firstBeatOffset / ((MusicController)MusicController.Instance).secPerBeat; // Cause the first note spawned on beat number 1 and not beat number 0
            }

            set
            {
                _timeBeat = value;
            }

        }*/
    
        public float durationBeat;

        public Note(float time,float duration,int noteIndex,float timeBeat,float durationBeat) {
            this.time = time;
            this.duration = duration;
            this.noteIndex = noteIndex;
            this.timeBeat = timeBeat;
            this.durationBeat = durationBeat;
        }
    }
    
    public TextAsset notesAsset;

    [HideInInspector] public NoteExcelArray notesArray;
    public MusicController controller;


    public List<Note> pendingNotes = new List<Note>();
    public Queue<Note> notes = new Queue<Note>();


    private void Start() {
       // GenerateAllNotes();

      /* foreach (Note note in pendingNotes) {
         GenerateNote(note.time,note.duration,note.noteIndex);  
       }
       */
       
       //notes = new Queue<Note>(pendingNotes);
    }

 /*   public void GenerateNote(float time, float duration,int noteIndex) {
       if (time == 0 || duration == 0 || noteIndex == 0) 
            return;

        time -= ((MusicController)MusicController.Instance).firstBeatOffset; // A tester
        
        float timeBeat = time / controller.secPerBeat;
        float durationBeat = duration / controller.secPerBeat;
        
        Note note = new Note(time,duration,noteIndex,timeBeat,durationBeat);
        notes.Enqueue(note);
    }

    public void GenerateAllNotes() {
        notes.Clear();
        notesArray = JsonUtility.FromJson<NoteExcelArray>(notesAsset.text);

        foreach (NoteExcelClass noteClass in notesArray.excelClass) {
            GenerateNote(noteClass.Column1,noteClass.Column3,(int)noteClass.Column4);
            GenerateNote(noteClass.Column5,noteClass.Column6,(int)noteClass.Column7);
            GenerateNote(noteClass.Column8,noteClass.Column9,(int)noteClass.Column10);
            GenerateNote(noteClass.Column11,noteClass.Column12,(int)noteClass.Column13);
            GenerateNote(noteClass.Column14,noteClass.Column15,(int)noteClass.Column16);
            GenerateNote(noteClass.Column17,noteClass.Column18,(int)noteClass.Column19);
            GenerateNote(noteClass.Column20,noteClass.Column21,(int)noteClass.Column22);
        }

        
        notes = new Queue<Note>(notes.OrderBy(note => note.time).ToList());
    }
    */
}
