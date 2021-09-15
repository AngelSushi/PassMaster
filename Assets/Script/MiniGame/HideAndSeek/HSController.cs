using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class HSController : CoroutineSystem {
    
    public enum Roles {
        NONE,
        HUNTER,
        SEEKER
    }

    public float gameTime;
    public bool finish;
    public bool begin;
    public bool randomRoles;
    public Text beginText;
    public Text timerText;
    public AudioSource timeSound;
    public AudioSource startSound;
    public AudioSource timerSound;
    public GameObject randomUI;
    public GameObject seekerScrolling;
    public Sprite[] usersSprite;
    public Sprite[] scrollingUsersSprite;
    public GameObject[] players;
    public GameObject interactUI;
    public GameObject hunterWait;
    public Room[] rooms;

    public GameObject prefabDizzy;

    private float beginTime = 4f;
    private float maxRandomTimer;
    private float randomTimer;
    private float randomSpeed = 0.1f;
    private float randomTimerSeconds;
    private string lastTimeText;
    private string lastBeginText;
    private bool hasRandomFirstRoles;
    public int hunter;
    public int seeker01;
    public int seeker02;
    public int seeker03;

    public bool isTraining;

    public bool runSinceMenu;
    public bool test;

    private Dictionary<GameObject,GameObject> seekersFurniture = new Dictionary<GameObject,GameObject>();
    
    public List<GameObject> pots = new List<GameObject>();
    void Start() {
        
    }

    void Update() {
        if(!finish) {
            if(randomRoles) {
                RandomRoles();
            }
            if(begin && !randomRoles) {
                BeginGame();
            }
            if(!randomRoles && !begin) {
                Timer();
                UpdateSeekerScrollingText(players[seeker01]);
                UpdateSeekerScrollingText(players[seeker02]);
                UpdateSeekerScrollingText(players[seeker03]);
                AssignFloor();

                if(test) {
                    AssignRoles();
                    UpdateSeekerScrolling();
                    test = false;
                }

            }
            
        }
        else {
            if(isTraining) {
                SceneManager.LoadScene("MiniGameLabel",LoadSceneMode.Additive);
                SceneManager.UnloadSceneAsync("HideAndSeek");
            }
            else if(runSinceMenu) {
                SceneManager.LoadScene("MainMenu",LoadSceneMode.Single);
                SceneManager.UnloadSceneAsync("HideAndSeek");
            }
            else {
                if(gameTime <= 0) { // Les seekers ont gagné

                }
                else { // Le Hunter a gagné

                }
            }
        }
    }

    private void BeginGame() {
        beginTime -= Time.deltaTime;
        float seconds = Mathf.FloorToInt(beginTime % 60);
            
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

        if(beginTime < 0) {
            beginText.text = "";
            beginText.gameObject.SetActive(false);
            begin = false;
        }

        lastBeginText = beginText.text;
    }

    private void Timer() {
        if(gameTime > 0) {
            gameTime -= Time.deltaTime;

            float minutes = Mathf.FloorToInt(gameTime / 60);
            float seconds = Mathf.FloorToInt(gameTime % 60);

            if(seconds >= 10)
                timerText.text = minutes + ":" + seconds;
            else 
                timerText.text = minutes + ":0" + seconds; 

            if(seconds < 10) {
                timerText.gameObject.GetComponent<Outline>().enabled = true;

                if(timerText.text != lastTimeText)
                    timeSound.Play();

                   lastTimeText = timerText.text;
                }
            }
            else {
                finish = true;
            }
    }

    private void RandomRoles() {


        if(!hasRandomFirstRoles) {
            RandomFirstRoles();
        }
        else {
            if(randomTimer < maxRandomTimer) {
                randomTimer += Time.deltaTime;
                randomTimerSeconds += Time.deltaTime;

                if(randomTimerSeconds >= randomSpeed) {
                    hunter++;
                    if(hunter >= 4)
                        hunter = 0;

                    seeker01++;
                    if(seeker01 >= 4)
                        seeker01 = 0;

                    seeker02++;
                    if(seeker02 >= 4)
                        seeker02 = 0;

                    seeker03++;
                    if(seeker03 >= 4)
                        seeker03 = 0;

                    randomUI.transform.GetChild(2).gameObject.GetComponent<Image>().sprite = usersSprite[hunter];
                    randomUI.transform.GetChild(3).gameObject.GetComponent<Image>().sprite = usersSprite[seeker01];
                    randomUI.transform.GetChild(4).gameObject.GetComponent<Image>().sprite = usersSprite[seeker02];
                    randomUI.transform.GetChild(5).gameObject.GetComponent<Image>().sprite = usersSprite[seeker03];
                    randomTimerSeconds = 0;
                }
            }
            else {
                RunDelayed(0.5f,() => {
                    randomUI.SetActive(false);
                    randomRoles = false;
                    begin = true;
                    
                    AssignRoles();
                    AssignFloor();
                    UpdateSeekerScrolling();
                });
            }

        }
    }

    private void RandomFirstRoles() {
        List<int> randomPos = new List<int>(new int[] {0,1,2,3});
        hunter = Random.Range(0,4);
        randomPos.Remove(hunter);

        seeker01 = Random.Range(0,4);
        if(seeker01 == hunter) {
            seeker01= Random.Range(0,4);
            return;
        }

        randomPos.Remove(seeker01);
        seeker02 = Random.Range(0,4);
        if(seeker02 == hunter || seeker02 == seeker01) {
            seeker02 = Random.Range(0,4);
            return;
        }
            
        randomPos.Remove(seeker02);
        seeker03 = randomPos[0];

        randomUI.transform.GetChild(2).gameObject.GetComponent<Image>().sprite = usersSprite[hunter];
        randomUI.transform.GetChild(3).gameObject.GetComponent<Image>().sprite = usersSprite[seeker01];
        randomUI.transform.GetChild(4).gameObject.GetComponent<Image>().sprite = usersSprite[seeker02];
        randomUI.transform.GetChild(5).gameObject.GetComponent<Image>().sprite = usersSprite[seeker03];
        maxRandomTimer = Random.Range(6f,8f);
        hasRandomFirstRoles = true;
    }

    private void UpdateSeekerScrolling() {
        seekerScrolling.transform.GetChild(0).GetChild(0).gameObject.GetComponent<Image>().sprite = ConvertSprite(seeker01);
        seekerScrolling.transform.GetChild(1).GetChild(0).gameObject.GetComponent<Image>().sprite = ConvertSprite(seeker02);
        seekerScrolling.transform.GetChild(2).GetChild(0).gameObject.GetComponent<Image>().sprite = ConvertSprite(seeker03);
    }

    private void UpdateSeekerScrollingText(GameObject seeker) {
        int index = -1;

        if(seeker == players[seeker01])
            index = 0;
        if(seeker == players[seeker02])
            index = 1;
        if(seeker == players[seeker03])
            index = 2;

        if(seeker.GetComponent<HS_Movement>() != null) {
            if(seeker.GetComponent<HS_Movement>().isDead) {
                seekerScrolling.transform.GetChild(index).GetChild(1).gameObject.GetComponent<Text>().text = "Trouvé";
                seekerScrolling.transform.GetChild(index).GetChild(1).gameObject.GetComponent<Text>().color = new Color(0f,0.51f,1f,1.0f);
            }
            else{
                seekerScrolling.transform.GetChild(index).GetChild(1).gameObject.GetComponent<Text>().text = "Caché";
                seekerScrolling.transform.GetChild(index).GetChild(1).gameObject.GetComponent<Text>().color = new Color(0.349f,0.349f,0.349f,1.0f);
            }
        }
        else if(seeker.GetComponent<HSIA>() != null) {
            if(seeker.GetComponent<HSIA>().isDead) {
                seekerScrolling.transform.GetChild(index).GetChild(1).gameObject.GetComponent<Text>().text = "Trouvé";
                seekerScrolling.transform.GetChild(index).GetChild(1).gameObject.GetComponent<Text>().color = new Color(0f,0.51f,1f,1.0f);
            }
            else {
                seekerScrolling.transform.GetChild(index).GetChild(1).gameObject.GetComponent<Text>().text = "Caché";
                seekerScrolling.transform.GetChild(index).GetChild(1).gameObject.GetComponent<Text>().color = new Color(0.349f,0.349f,0.349f,1.0f);
            }
        }
    }

    private Sprite ConvertSprite(int seeker) {
        return scrollingUsersSprite[seeker];
    }

    private void AssignRoles() {
        if(players[hunter].GetComponent<HS_Movement>() != null) {
            players[hunter].GetComponent<HS_Movement>().role = Roles.HUNTER;
            players[hunter].transform.GetChild(3).gameObject.SetActive(false);
        }
        else if(players[hunter].GetComponent<HSIA>() != null) {
            players[hunter].GetComponent<HSIA>().role = Roles.HUNTER;
            players[hunter].transform.GetChild(2).gameObject.SetActive(false);
        }


            
        if(players[seeker01].GetComponent<HS_Movement>() != null) {
            players[seeker01].GetComponent<HS_Movement>().role = Roles.SEEKER;
            players[seeker01].transform.GetChild(2).gameObject.SetActive(false);
        }
        else if(players[seeker01].GetComponent<HSIA>() != null) {
            players[seeker01].GetComponent<HSIA>().role = Roles.SEEKER;
            players[seeker01].transform.GetChild(1).gameObject.SetActive(false);
        }
            
        if(players[seeker02].GetComponent<HS_Movement>() != null) {
            players[seeker02].GetComponent<HS_Movement>().role = Roles.SEEKER;
            players[seeker02].transform.GetChild(2).gameObject.SetActive(false);
        }
        else if(players[seeker02].GetComponent<HSIA>() != null) {
            players[seeker02].GetComponent<HSIA>().role = Roles.SEEKER;
            players[seeker02].transform.GetChild(1).gameObject.SetActive(false);
        }
        if(players[seeker03].GetComponent<HS_Movement>() != null) {
            players[seeker03].GetComponent<HS_Movement>().role = Roles.SEEKER;
            players[seeker03].transform.GetChild(2).gameObject.SetActive(false);
        }
        else if(players[seeker03].GetComponent<HSIA>() != null) {
            players[seeker03].GetComponent<HSIA>().role = Roles.SEEKER;
            players[seeker03].transform.GetChild(1).gameObject.SetActive(false);
        }
    }

    public GameObject GetHunter() {
        return players[hunter];
    }

    public GameObject[] GetSeekers() {
        GameObject[] seekers = {players[seeker01],players[seeker02],players[seeker03]};
        return seekers;
    }

    public Dictionary<GameObject,GameObject> GetSeekersFurniture() {
        return seekersFurniture;
    }

    public bool IsHunterBot(GameObject hunter) {
        return hunter.GetComponent<HSIA>() != null;
    }

    public bool isSeekerBot(GameObject seeker) {
        return seeker.GetComponent<HSIA>() != null;
    }
    
    public void AssignFloor() {
        foreach(GameObject user in players) {
            if(user.transform.position.y >= 622) {
                if(user.GetComponent<HS_Movement>() != null)
                    user.GetComponent<HS_Movement>().floor = 2;
                else if(user.GetComponent<HSIA>() != null) 
                    user.GetComponent<HSIA>().floor = 2;
            }
            else {
                if(user.GetComponent<HS_Movement>() != null)
                    user.GetComponent<HS_Movement>().floor = 1;
                else if(user.GetComponent<HSIA>() != null) 
                    user.GetComponent<HSIA>().floor = 1;
            }
        }
    }

}
