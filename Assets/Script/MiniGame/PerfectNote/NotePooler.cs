using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NotePooler : MonoBehaviour {

    [System.Serializable]
    public class NotePool {

        public string tag;
        public NoteController prefab;
        public int size;
    }

    public List<NotePool> notePools;
    public Dictionary<string,Queue<NoteController>> notesDictionnary;

    public static NotePooler Instance;
    void Awake() {
        Instance = this;
    }

    void Start() {
        notesDictionnary = new Dictionary<string, Queue<NoteController>>();

        foreach(NotePool pool in notePools) {
            Queue<NoteController> objectPool = new Queue<NoteController>();

            for(int i = 0;i<pool.size;i++) {
                NoteController obj =  Instantiate(pool.prefab);
                obj.gameObject.SetActive(false);

                objectPool.Enqueue(obj);
            }

            notesDictionnary.Add(pool.tag,objectPool);   
        }
    }

    public NoteController SpawnNote(string tag) {
        if(!notesDictionnary.ContainsKey(tag)) {
            Debug.Log("not a tag");
            return null;
        }

        NoteController nc = notesDictionnary[tag].Dequeue();
        GameObject objToSpawn = notesDictionnary[tag].Dequeue().gameObject;
        objToSpawn.SetActive(true);

        notesDictionnary[tag].Enqueue(nc);

        return nc;

    }
}
