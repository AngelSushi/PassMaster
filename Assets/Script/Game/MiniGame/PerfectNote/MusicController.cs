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
using UnityEngine.PlayerLoop;
using UnityEngine.UI;

public class MusicController : MiniGame {
    
   [SerializeField] [Tooltip("The song delay in seconds")] public float songDelay;

   [SerializeField] private string fileLocation;
   public float noteTime;
   public float noteSpawnY;
   public float noteTapY;
   
   
   public float goodMarginError;
   public float perfectMarginError;

   [Tooltip("The reward when you do a good note in point")] public int goodReward;
   [Tooltip("The reward when you do a perfect note in point")] public int perfectReward;

   public Dictionary<Player, int> playersPoint = new Dictionary<Player, int>();
   public Transform[] playersUI;

   public float noteDespawnX {
       get {
           return noteTapY - (noteSpawnY - noteTapY);
       }
   }

   public NoteLane[] lanes;
   

   public static MidiFile midiFile;

   public GameObject notePrefab;
   public GameObject longNotePrefab;
   public GameObject detection;
   public GameObject start;

   public Transform canvas;
   public GameObject noteStatePrefab;
   public Transform uiKeyParent;

   public List<PN_AIController> allAI;

   public List<Note> allNotes;

   public List<Texture2D> patternsRef;
   public List<int[]> patterns = new List<int[]>();


   void Start() {

       foreach (Player player in players) 
           playersPoint.Add(player,0);
       
       foreach(PN_AIController aiController in FindObjectsOfType<PN_AIController>())
           allAI.Add(aiController);
       
       ReadMidiFile();
       ManageUIKey();
      // GenerateAllPatterns();
       
       Debug.Log("difficulty " + GameController.Instance.difficulty);
   }

   private void ReadMidiFile() {
       midiFile = MidiFile.Read(Application.streamingAssetsPath + "/" + fileLocation);
       GetDataFromMidi();
   }

   private void GetDataFromMidi() {
       allNotes = midiFile.GetNotes().ToList();

       foreach (NoteLane lane in lanes)
           lane.SetTimeStamps(allNotes);

       foreach (PN_AIController aiController in allAI) {
           aiController.allNotes = allNotes;
       }
       
       Invoke(nameof(StartSong),songDelay);
   }

   private void StartSong() {
      mainAudio.Play(); 
   }

   public static double GetAudioSourceTime() {
       return (double)instance.mainAudio.timeSamples / instance.mainAudio.clip.frequency;
   }

   private void ManageUIKey() {
       if (uiKeyParent.transform.childCount != lanes.Length) {
           Debug.LogError("Errror when generation ui keys");
       }

       for (int i = 0; i < lanes.Length; i++) {
           uiKeyParent.GetChild(i).GetChild(0).gameObject.SetActive(true);
           uiKeyParent.GetChild(i).GetChild(1).gameObject.SetActive(false);
           uiKeyParent.GetChild(i).GetChild(0).GetComponent<Image>().sprite = lanes[i].laneKeySprite;
       }
       
   }

   public override void Update() {
       base.Update();
       
       start.SetActive(begin);
       
       
       Debug.DrawLine(detection.transform.position,detection.transform.position + Vector3.up * goodMarginError,Color.yellow);
       Debug.DrawLine(detection.transform.position,detection.transform.position - Vector3.up * goodMarginError,Color.yellow);
       
       
       Debug.DrawLine((detection.transform.position + Vector3.left ),(detection.transform.position + Vector3.left )+ Vector3.up * perfectMarginError ,Color.red);
       Debug.DrawLine((detection.transform.position + Vector3.left ),(detection.transform.position + Vector3.left ) - Vector3.up * perfectMarginError,Color.red);

       if (!begin && !mainAudio.isPlaying ) {
           Debug.Log("finish");
           OnFinish();
       }
   }

   public void AddPointToPlayer(Player player,int point) {
       playersPoint[player] += point;
       
       Debug.Log("player " + player);

       List<int> allPoints = playersPoint.Values.ToList();
       allPoints.Sort();
       allPoints.Reverse();

       List<Player> classedPlayers = new List<Player>();

       foreach(int actualPoint in allPoints) {
           foreach (Player targetPlayer in playersPoint.Keys) {
               if (playersPoint[targetPlayer] == actualPoint && !classedPlayers.Contains(targetPlayer)) {
                   classedPlayers.Add(targetPlayer);
                   break;
               }
           }
       }

       for (int i = 0; i < classedPlayers.Count; i++) {
           playersUI[i].GetChild(2).gameObject.GetComponent<Text>().text = allPoints[i].ToString("D3");
           playersUI[i].GetChild(1).gameObject.GetComponent<Image>().sprite = classedPlayers[i].uiIcon;
       }


   }
   
   public void CreateNoteState(bool isPerfect) {
       GameObject noteState = Instantiate(noteStatePrefab,canvas.transform);
       noteState.transform.position = Camera.main.WorldToScreenPoint(new Vector3(transform.position.x, -3, 0));
       noteState.GetComponent<Text>().text = isPerfect ? "Perfect" : "Good";
       noteState.GetComponent<Text>().color = isPerfect ? Color.yellow : Color.gray;
   }

   private void GenerateAllPatterns() {
       foreach (Texture2D reference in patternsRef)
           GeneratePattern(reference);
       
       for (int i = 0; i < patterns.Count; i++) {
           int[] currentPattern = patterns[i];

           foreach(int note in currentPattern){
               Debug.Log("note " + note);
           }

           Debug.Log("----------------");
       }
   }

   private void GeneratePattern(Texture2D reference) {
       int[] pattern = new int[reference.width];
       
       int arrayIndex = 0;
       
       for (int i = 0; i < reference.width; i++) {
           for (int j = 0; j < reference.height; j++) {
               Color pixelColor = reference.GetPixel(i, j);
               
               if (pixelColor != Color.white) {
                  if (arrayIndex < pattern.Length) {
                      pattern[arrayIndex] = j;
                      arrayIndex++;
                  }
               }
           }
       }

       patterns.Add(pattern);
   }
   
   public override void OnFinish() {
       finish = true;

   }
}
