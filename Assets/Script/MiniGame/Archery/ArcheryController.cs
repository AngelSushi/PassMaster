using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ArcheryController : CoroutineSystem {

    public Transform[] limitsBottom = new Transform[2];
    public Transform[] limitsTop = new Transform[2];
    public GameObject target;
    public Dictionary<GameObject,int> playersPoint = new Dictionary<GameObject,int>();
    public GameObject[] players;
    public GameObject hudParentScore;
    public GameObject[] splashsPainting;
    public Text timer;
    public bool isTraining;
    public bool begin;
    public Text beginText;
    public bool finish;
    
    public GameObject[] classementPanels;
    public Text endText;

    public GameObject confetti;
    public AudioSource win;
    private bool hasPlayedSFX;

    private float beginTimer = 4f;
    private float gameTime = 45f;
    public GameObject mainCamera;

    private string lastBeginText;
    private string lastTimeText;
    public AudioSource timerSound;
    public AudioSource startSound;
    public AudioSource timeSound;

    private GameObject bottom;
    private GameObject top;
    public bool runSinceMenu;


    void Start() {
        //GameController.difficulty = 2;

        InstantiateTarget();

        foreach(GameObject player in players) {
            if(!playersPoint.ContainsKey(player))
                playersPoint.Add(player,0);
        }

        ManageHudScore();
    }

    void Update() {
        if(begin) {
            beginTimer -= Time.deltaTime;
            float seconds = Mathf.FloorToInt(beginTimer % 60);
            
            if(seconds > 0)
                beginText.text = "" + seconds;
            else
                beginText.text = "GO";

            if(lastBeginText == null || beginText.text != lastBeginText) {
                if(beginText.text == "GO")
                    startSound.Play();
                else 
                    timerSound.Play();
            }

            if(beginTimer < 0) {
                beginText.text = "";
                begin = false;
            }

            lastBeginText = beginText.text;
        }
        else {
            if(!finish) {
                gameTime -= Time.deltaTime;

                float minutes = Mathf.FloorToInt(gameTime / 60);
                float seconds = Mathf.FloorToInt(gameTime % 60);

                if(gameTime > 0) {
                    if(seconds >= 10)
                        timer.text = minutes + ":" + seconds;
                    else 
                        timer.text = minutes + ":0" + seconds;

                    if(gameTime <= 10) {
                        timer.gameObject.GetComponent<Outline>().enabled = true;
                        if(timer.text != lastTimeText) 
                            timeSound.Play();
                    }

                    lastTimeText = timer.text;  
                    
                }
                else
                    finish = true;
            }
            else {
                if(isTraining) {
                    SceneManager.LoadScene("MiniGameLabel",LoadSceneMode.Additive);
                    SceneManager.UnloadSceneAsync("Archery");
                }
                else if(runSinceMenu) {
                    SceneManager.LoadScene("MainMenu",LoadSceneMode.Single);
                    SceneManager.UnloadSceneAsync("Archery");
                }
                else {
                    // Récupérer celui qui a le plus de points 
                    // Si il y en a plusieurs --> plusieurs récompenses 
                    
                    GameObject[] objects = SceneManager.GetSceneByName("Archery").GetRootGameObjects();

                    foreach(GameObject obj in objects) {
                        if(obj.name.Contains("Clone"))
                            Destroy(obj);
                    }

                    //GameObject player = players[0];

                    List<int> points = new List<int>();

                    foreach(int point in playersPoint.Values) {
                        points.Add(point);
                    }

                    points.Sort();
                    int winnerPoint = points[points.Count - 1];

                    List<GameObject> winners = new List<GameObject>();

                    foreach(GameObject player in playersPoint.Keys) {
                        if(playersPoint[player] == winnerPoint)
                            winners.Add(player);
                    }

                    if(!hasPlayedSFX) {
                        win.Play();
                        hasPlayedSFX = true;
                        endText.gameObject.SetActive(true);
                        confetti.SetActive(true);
                        confetti.transform.position = winners[0].transform.position;
                        confetti.GetComponent<ParticleSystem>().enableEmission = true;
                        confetti.GetComponent<ParticleSystem>().Play();

                    }


                    if(winners[0].name != "User") {
                        Vector3 playerPosition = new Vector3(winners[0].transform.position.x,mainCamera.transform.position.y,winners[0].transform.position.z);
                        mainCamera.transform.position = Vector3.MoveTowards(mainCamera.transform.position,playerPosition,200 * Time.deltaTime);
                    }

                    Destroy(top);
                    Destroy(bottom);

                    GameObject.FindGameObjectsWithTag("Game")[0].GetComponent<GameController>().EndMiniGame(classementPanels,winners,endText.gameObject);

                }
            }
        }
 
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
        bottom.GetComponent<TargetShifting>().speed = 3 + GameController.difficulty;
        bottom.GetComponent<TargetShifting>().controller = this;
        bottom.GetComponent<TargetShifting>().left = true;

        top = Instantiate(target,new Vector3(0,0,0),Quaternion.Euler(0,0,0));
        top.tag = "Target";
        top.AddComponent<TargetShifting>();
        top.GetComponent<TargetShifting>().speed = 3 +  GameController.difficulty;
        top.GetComponent<TargetShifting>().controller = this;
        top.GetComponent<TargetShifting>().bottom = true;
    }
}
