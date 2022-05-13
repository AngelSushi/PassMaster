using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ArcheryController : MiniGame {

    public Transform[] limitsBottom = new Transform[2];
    public Transform[] limitsTop = new Transform[2];
    public GameObject target;
    public Dictionary<GameObject,int> playersPoint = new Dictionary<GameObject,int>();
    public GameObject[] players;
    public GameObject hudParentScore;
    public GameObject[] splashsPainting;

    public GameObject mainCamera;

    private GameObject bottom;
    private GameObject top;


    void Start() {
        InstantiateTarget();

        foreach(GameObject player in players) {
            if(!playersPoint.ContainsKey(player))
                playersPoint.Add(player,0);
        }

        ManageHudScore();
    }

    void Update() {}

    public override void OnFinish() {             
        GameObject[] objects = SceneManager.GetSceneByName("Archery").GetRootGameObjects();

        foreach(GameObject obj in objects) {
            if(obj.name.Contains("Clone"))
                Destroy(obj);
        }

        List<int> points = new List<int>();

        foreach(int point in playersPoint.Values) 
            points.Add(point);
                    
        points.Sort();
        int winnerPoint = points[points.Count - 1];


        foreach(GameObject player in playersPoint.Keys) {
            if(playersPoint[player] == winnerPoint)
                this.winners.Add(player);
        }

        if(winners[0].name != "User") {
            Vector3 playerPosition = new Vector3(winners[0].transform.position.x,mainCamera.transform.position.y,winners[0].transform.position.z);
            mainCamera.transform.position = Vector3.MoveTowards(mainCamera.transform.position,playerPosition,200 * Time.deltaTime);
        }

        Destroy(top);
        Destroy(bottom);
            
    }

    public void AddPoints(GameObject player,int point) {
        playersPoint[player] += point;
        ManageHudScore();
    }

    private void ManageHudScore() {
        for(int i = 0;i<hudParentScore.transform.childCount;i++) {
            hudParentScore.transform.GetChild(i).GetChild(1).gameObject.GetComponent<Text>().text = "" + playersPoint[players[i]];
        }
    }

    private void InstantiateTarget() {
        bottom = Instantiate(target,new Vector3(0,0,0),Quaternion.Euler(0,0,0));
        bottom.tag = "Target";
        bottom.AddComponent<TargetShifting>();
        bottom.GetComponent<TargetShifting>().speed = 3 + (GameController.difficulty == GameController.Difficulty.EASY ? 0 : GameController.difficulty == GameController.Difficulty.MEDIUM ? 1 : GameController.difficulty == GameController.Difficulty.HARD ? 2 : 0 ) ;
        bottom.GetComponent<TargetShifting>().controller = this;
        bottom.GetComponent<TargetShifting>().left = true;

        top = Instantiate(target,new Vector3(0,0,0),Quaternion.Euler(0,0,0));
        top.tag = "Target";
        top.AddComponent<TargetShifting>();
        bottom.GetComponent<TargetShifting>().speed = 3 + (GameController.difficulty == GameController.Difficulty.EASY ? 0 : GameController.difficulty == GameController.Difficulty.MEDIUM ? 1 : GameController.difficulty == GameController.Difficulty.HARD ? 2 : 0);
        top.GetComponent<TargetShifting>().controller = this;
        top.GetComponent<TargetShifting>().bottom = true;
    }
}
