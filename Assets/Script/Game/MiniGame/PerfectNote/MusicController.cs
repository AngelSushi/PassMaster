using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicController : MonoBehaviour {

    [System.Serializable]
    public class Note {

        public int line;
        public float startTime;
        public float noteTime;
        public Material color;
    }

    public List<Note> notesStartTime;
    public Transform[] noteSpawn;
    public Transform[] noteEnd;
    public GameObject notePrefab;
    public AudioSource song;
    public int index;

    public float bpm = 161;
    private float crotchet; // time duration of a beat
    private float offset; // in MP3 there us teeny gap for metadata
    private float songposition;
    private float dsptimesong;
// 161 bpm

    void Start() {
        SortNote();
    }

    void Update() {

        songposition = (float)(AudioSettings.dspTime - dsptimesong) * song.pitch - offset;


        if(songposition > crotchet * 1.32 && index == 0) {
             Instantiate(notePrefab,noteEnd[0].position,Quaternion.Euler(0,0,0));
             index++;
        }
        /*int i = 0;

        do {
            
        } 
        while (i < 7);

        */
    }

    private void SortNote() {
        List<float> notesStart = new List<float>();
        List<Note> newNotesStartTime = new List<Note>();

        foreach(Note note in notesStartTime) {
            notesStart.Add(note.startTime);
        }

        notesStart.Sort();

        foreach(float start in notesStart) {
            newNotesStartTime.Add(GetNoteByStartTime(start));
        }

        notesStartTime = newNotesStartTime;
    }

    private Note GetNoteByStartTime(float newStartTime) {
        foreach(Note note in notesStartTime) {
            if(note.startTime == newStartTime)
                return note;
        }
        return null;
    }

    private void ApplyVariables(NoteController note,int i,float age,Vector3 pos) {
        note.noteIndex = index;
        note.startPos = pos;
        note.age = age;
        note.gameObject.transform.position = pos;
        note.gameObject.SetActive(true);
        note.gameObject.GetComponent<MeshRenderer>().material = notesStartTime[index + i].color;
    }
}
