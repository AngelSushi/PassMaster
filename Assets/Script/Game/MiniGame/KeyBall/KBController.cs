using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class KBController : MiniGame {

    public Dictionary<GameObject,int> playersPoint = new Dictionary<GameObject,int>();
    public GameObject[] stopwatch = new GameObject[4];
    public Sprite[] destroySprite = new Sprite[48];

    public Dictionary<Player,int> playersPoints = new Dictionary<Player,int>();

    public Transform areaDeath;

    //public GameObject confetti;
    public GameObject mainCamera;

    public GameObject[] pointsText = new GameObject[4];
    public GameObject[] walls = new GameObject[4];
    public Vector3[] destroyPoints = new Vector3[3];
    public Vector3[] portalPoints = new Vector3[4];

    public GameObject portal;

    public Material skybox;

    [HideInInspector]
    public Vector3 tpPos;
    

    public override void Start() {
        base.Start();
        
  /*      playersPoint.Add(players[0],0);
        playersPoint.Add(players[1],0);
        playersPoint.Add(players[2],0);
        playersPoint.Add(players[3],0);
    */    
        Debug.Log("start");

        RenderSettings.skybox = skybox;

//        tpPos = portal.transform.position;
    }

    public override void OnFinish() {
        List<int> points = new List<int>();

        foreach(int point in playersPoint.Values) 
            points.Add(point);
                
        points.Sort();
        int winnerPoint = points[points.Count - 1];        

        foreach(GameObject player in playersPoint.Keys) {
            if(playersPoint[player] == winnerPoint)
                this.winners.Add(player);
        }

        if(winners.Count == 1 && winners[0].name != "User") {
            Vector3 playerPosition = new Vector3(winners[0].transform.position.x - 15,mainCamera.transform.position.y,winners[0].transform.position.z);
            mainCamera.transform.position = Vector3.MoveTowards(mainCamera.transform.position,playerPosition,200 * Time.deltaTime);
        }       
    }



    public int ConvertPlayerInt(GameObject player) {
        switch(player.name) {
            case "User":
                return 0;
                break;

            case "Bot_001":
                return 1;
                break;

            case "Bot_002":
                return 2;
                break;

            case "Bot_003":
                return 3;
                break;
        }

        return -1;
    }
}
