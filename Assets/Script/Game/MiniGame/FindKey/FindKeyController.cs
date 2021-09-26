using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using UnityEngine.Events;

public class FindKeyController : MonoBehaviour {

    public GameObject keysSpawn;
    public List<GameObject> gameKeys = new List<GameObject>(); // Les clés du tour de la partie
    public GameObject prefabKey;
    public GameObject prefabPlayer;
    public GameObject prefabBot;

    public List<GameObject> playersAlive = new List<GameObject>(); // Les joueurs restants dans la partie
    public List<GameObject> playersQualified = new List<GameObject>(); // Les joueurs qualifiés

    public int playerCount;
    public int botCount;

    public GameObject[] spawnPoints = new GameObject[4];
    public CinemachineFreeLook characterCamera;

    public GameController gameController;

    // Dictionnary<bot,key>
    public Dictionary<GameObject,GameObject> playersKey = new Dictionary<GameObject,GameObject>(); // La clé de chaque bot
    private Color[] colors = new Color[4];

    public bool finish;

    void Start() {
        gameController = GameObject.FindGameObjectsWithTag("Game")[0].GetComponent<GameController>();
        // générer les clés
        colors[0] = Color.yellow;
        colors[1] = Color.red;
        colors[2] = Color.blue;
        colors[3] = Color.magenta;

      /*  for(int i = 0;i<botCount;i++) {
            int keyIndex = Random.Range(0,keysSpawn.transform.childCount - 1);
            GameObject key = keysSpawn.transform.GetChild(keyIndex).gameObject;
// Attention aux spawns de clés au méme endroit
            GameObject keyIns = Instantiate(prefabKey,key.transform.position,Quaternion.Euler(0,0,0));
            keyIns.name = "Key_00" + i;
            gameKeys.Add(keyIns);
        }

        // On génère les bots
        for(int i = 0;i<botCount;i++) {
            GameObject bot = Instantiate(prefabBot,spawnPoints[i].transform.position,Quaternion.Euler(0,0,0));
            bot.SetActive(true);
            bot.name = "bot_00" + i;
            // random la clé

            int random = Random.Range(0,gameKeys.Count);
            GameObject targetKey = gameKeys[random];

            bot.GetComponent<FindKey_IAController>().awardLocation = targetKey.transform.GetChild(0);
            bot.GetComponent<FindKey_IAController>().color = colors[i];

            bot.GetComponent<FindKey_IAController>().canMoove = true;

            // POUR LE TEST
            bot.GetComponent<FindKey_IAController>().choosePath = true;

            playersKey.Add(bot,targetKey);

            Debug.Log("bot: " + bot.name + " key: " + targetKey.name);

        }
        */
    }

    void Update() {

        if(finish) {
            gameController.BeginTurn(false);
            finish = false;
        }
    }

    public void SetFinish(bool finishGame) {
        finish = finishGame;
    }

    public void Qualify(GameObject playerQualify,GameObject key) {

        playersQualified.Add(playerQualify);
        playersAlive.Remove(playerQualify);
        gameKeys.Remove(key);
        playersKey.Remove(playerQualify);




        if(gameKeys.Count == 0 && playersAlive.Count == 1) { // Fin du tour il n'y a pas de clé a aller chercher et il reste un dernier joueur
            GameObject player = playersAlive[0];

            if(player.GetComponent<FindKey_IAController>() == null) { // On regarde si c'est un joueur
                // isDead sur le joueur
            }
            else {
                player.GetComponent<FindKey_IAController>().isDead = true;
            }

            NextTurn();
        }

        else { // La partie n'est pas encore fini il reste des personnes a qualifiés

            // Check si la clé n'était pas aussi celle d'un autre bot
            if(playersKey.ContainsValue(key)) {
                int random = Random.Range(0,gameKeys.Count);
                GameObject newKey = gameKeys[random];

                foreach(GameObject bot in playersKey.Keys) {
                    if(playersKey[bot] == key) bot.GetComponent<FindKey_IAController>().awardLocation = newKey.transform.GetChild(0);  
                }
            }

        }

    }

    public void NextTurn() {

    }

}
