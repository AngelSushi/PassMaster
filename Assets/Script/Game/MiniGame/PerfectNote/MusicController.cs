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
using UnityStandardAssets.Effects;

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

   public Transform canvas;
   public GameObject noteStatePrefab;
   public Transform uiKeyParent;
   public Transform classementParent;

   public List<PN_AIController> allAI;

   public List<Note> allNotes;

   public List<Texture2D> patternsRef;
   public List<int[]> patterns = new List<int[]>();


   public override void Start() { 
       base.Start();
        
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

       foreach (PN_AIController aiController in allAI) 
           aiController.allNotes = allNotes;
       
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
       
       if (!begin && !mainAudio.isPlaying ) 
           OnFinish();
       
   }

   public void AddPointToPlayer(Player player,int point,int noteIndex = -1) {
       if (noteIndex != -1) {
           PN_AIController aiController = allAI.Where(aiController => aiController.player == player).ToList()[0];
           if (aiController.lastSucceedNoteIndex == noteIndex)
               return;

           aiController.lastSucceedNoteIndex = noteIndex;
       }
       
       playersPoint[player] += point;

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
       int currentMaxPoint = 0;

       foreach (int point in playersPoint.Values) {
           if (point >= currentMaxPoint) {
               currentMaxPoint = point;
           }
       }

       foreach (Player player in playersPoint.Keys) {
           if (playersPoint[player] == currentMaxPoint) {
               winners.Add(player.gameObject);
           }
       }


       foreach (Player player in players) {
           for (int i = 0; i < player.gameObject.transform.childCount; i++) {
               player.gameObject.transform.GetChild(i).gameObject.SetActive(winners.Contains(player.gameObject));
               player.gameObject.SetActive(winners.Contains(player.gameObject));
           }
       }
       
       canvas.GetChild(0).gameObject.SetActive(false);
       canvas.GetChild(2).gameObject.SetActive(false);

       if (winners.Count > 1) {
           Vector3 startPosition = winners[0].transform.position;

           bool isPair = winners.Count % 2 == 0;

           for(int i = 0;i < winners.Count;i++) {
               GameObject  player = winners[i];
               for (int j = 0; j < player.gameObject.transform.childCount; j++) {
                   player.gameObject.transform.GetChild(j).gameObject.SetActive(true);
               }
           }

           int offset = isPair ? winners.Count / 2 : winners.Count % 2;

           int winnerIndex = -1;
           for (int i =-offset; i < offset + 1; i++) {

               if (isPair && i == 0)
                   continue; 
               
               winnerIndex++;

               Vector3 newPosition = startPosition + Vector3.left   * i;
               Debug.Log("newPosition " + newPosition + " isPair " + isPair);
               winners[winnerIndex].transform.position = newPosition;
               
           }
       }

       circleTransition.Play();
   }

   public override void OnTransitionEnd() {
       if (actualState == GameState.END) {
           if (!_hasPlayedSfx) {
               win.Play();
               _hasPlayedSfx = true;

               if (endText != null)
                   endText.gameObject.SetActive(true);

               if (confetti != null) {
                   confetti.SetActive(true);
                   confetti.transform.position = winners[0].transform.position;
                   confetti.GetComponent<ParticleSystem>().enableEmission = true;
                   confetti.GetComponent<ParticleSystem>().Play();
               }
           }

           foreach (GameObject winner in winners) 
               winner.GetComponent<Animator>().SetBool("Victory", true);
           
           uiKeyParent.gameObject.SetActive(false);
           FinishMiniGame();
       }


   }
   
   public override void OnSwitchCamera() { 
       if(actualState == GameState.END) 
           endCinematicCameras[0].gameObject.SetActive(true);
   }

   public override void OnStartCinematicEnd() {
       uiKeyParent.gameObject.SetActive(true);
       classementParent.gameObject.SetActive(true);
   }
 }
