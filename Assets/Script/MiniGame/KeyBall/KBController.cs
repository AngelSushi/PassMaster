using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class KBController : MonoBehaviour {

    public Text timer;
    public Dictionary<GameObject,int> playersPoint = new Dictionary<GameObject,int>();
    private float gameTime = 60;
    public bool isTraining;
    public GameObject[] stopwatch = new GameObject[4];
    public Sprite[] destroySprite = new Sprite[48];

    public Dictionary<GameObject,int> playersPoints = new Dictionary<GameObject,int>();

    public GameObject[] players;
    public bool begin;
    public Text beginText;
    private float beginTimer = 4f;
    public Transform areaDeath;
    public bool finish;

    private bool hasPlayedSFX;
    public AudioSource win;
    public GameObject endText;
    public GameObject confetti;
    public GameObject mainCamera;
    public GameObject[] classementPanels;

    private string lastBeginText;
    private string lastTimeText;

    public AudioSource mainAudio;
    public AudioSource timerSound;
    public AudioSource startSound;
    public AudioSource timeSound;

    public GameObject[] pointsText = new GameObject[4];
    public GameObject[] walls = new GameObject[4];
    public Vector3[] destroyPoints = new Vector3[3];
    public Vector3[] portalPoints = new Vector3[4];

    public GameObject portal;

    public bool runSinceMenu;

    public Material skybox;
    void Start() {
        playersPoint.Add(players[0],0);
        playersPoint.Add(players[1],0);
        playersPoint.Add(players[2],0);
        playersPoint.Add(players[3],0);

        ActualizePlayerPoint(players[0]);
        ActualizePlayerPoint(players[1]);
        ActualizePlayerPoint(players[2]);
        ActualizePlayerPoint(players[3]);

    }

    void Update() {
        RenderSettings.skybox = skybox;
        if(!finish) {
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
        }
        else {
            if(isTraining) {
                SceneManager.LoadScene("MiniGameLabel",LoadSceneMode.Additive);
                SceneManager.UnloadSceneAsync("KeyBall");
            }
            else if(runSinceMenu) {
                SceneManager.LoadScene("MainMenu",LoadSceneMode.Single);
                SceneManager.UnloadSceneAsync("KeyBall");
            }
            else {

                List<int> points = new List<int>();

                foreach(int point in playersPoint.Values) {
                    points.Add(point);
                }

                points.Sort();
                int winnerPoint = points[points.Count - 1];

                mainAudio.Stop();

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

            //    winners[0].GetComponent<KB_PlayerMovement>().isJumping = true;  

                if(winners.Count == 1 && winners[0].name != "User") {
                    Vector3 playerPosition = new Vector3(winners[0].transform.position.x - 15,mainCamera.transform.position.y,winners[0].transform.position.z);
                    mainCamera.transform.position = Vector3.MoveTowards(mainCamera.transform.position,playerPosition,200 * Time.deltaTime);
                }

                GameObject.FindGameObjectsWithTag("Game")[0].GetComponent<GameController>().EndMiniGame(classementPanels,winners,endText.gameObject);
            }
        }
    }

    public void AddPoint(GameObject player,int point) {
        playersPoint[player] = playersPoint[player] + 1;
        ActualizePlayerPoint(player);
    }

    private void ActualizePlayerPoint(GameObject player) {
        pointsText[ConvertPlayerInt(player)].GetComponent<Text>().text = playersPoint[player].ToString();
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
