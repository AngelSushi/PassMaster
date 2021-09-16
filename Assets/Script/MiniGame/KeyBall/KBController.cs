using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class KBController : MiniGame {

    public Dictionary<GameObject,int> playersPoint = new Dictionary<GameObject,int>();
    public bool isTraining;
    public GameObject[] stopwatch = new GameObject[4];
    public Sprite[] destroySprite = new Sprite[48];

    public Dictionary<GameObject,int> playersPoints = new Dictionary<GameObject,int>();

    public GameObject[] players;
    public Transform areaDeath;

    private bool hasPlayedSFX;
    public AudioSource win;
    public GameObject endText;
    public GameObject confetti;
    public GameObject mainCamera;
    public GameObject[] classementPanels;


    public AudioSource mainAudio;

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
    }

    public override void OnFinish() {
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
