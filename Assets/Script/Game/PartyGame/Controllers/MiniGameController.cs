using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class MiniGameController : CoroutineSystem {

    public List<MiniGameData> minigames = new List<MiniGameData>();
    public GameObject render;
    public float maxTimer;
    public float timer;
    public int index;
    public float step;
    private float speed = 0.08f;
    public MiniGameData actualMiniGame;
    private GameController gameController;
    private bool hasTurnChange;
    private bool hasLoadScene;
    private bool hasClassementShow;
    private bool displayReward;
    private bool moove;
    private Dictionary<GameObject,GameObject> playerMove = new Dictionary<GameObject,GameObject>();
    private Dictionary<GameObject,Vector2> playerPositionToGo = new Dictionary<GameObject,Vector2>();
    
    private List<GameObject> classedPlayer;

    void Start() {
        gameController = GameController.Instance;
    }

    public IEnumerator RandomMiniGame() {
        if(maxTimer == 0) {
            for(int i = 0;i<render.transform.parent.childCount;i++) 
                render.transform.parent.GetChild(i).gameObject.SetActive(true);

            maxTimer = Random.Range(4.0f,6.0f);
        }

        while(timer <= maxTimer) {
            yield return null;
            
            timer += Time.deltaTime;
            step += Time.deltaTime;
            if(step >= speed) {
                step = 0;

                render.GetComponent<Image>().sprite = minigames[index].minigameSprite;
                render.transform.parent.GetChild(2).GetChild(0).gameObject.GetComponent<Text>().text = minigames[index].minigameName;
                render.transform.parent.GetChild(2).GetChild(1).gameObject.GetComponent<Text>().text = minigames[index].minigameDesc;
                index++;

                if(maxTimer - timer <= 2) 
                    speed += 0.1f;      

                if(index >= 3)
                    index = 0;
            }

        }
        
        actualMiniGame = minigames.Where(minigame => minigame.minigameName == render.transform.parent.GetChild(2).GetChild(0).gameObject.GetComponent<Text>().text).First();
        RunDelayed(1.0f,() => {
            
            if(!hasLoadScene) {
                render.transform.parent.gameObject.SetActive(false);
                gameController.part = GameController.GamePart.MINIGAME;

                hasTurnChange = false;
               // gameController.dayController.mainAudio.Stop();
                gameController.players[gameController.actualPlayer].GetComponent<UserUI>().showTurnInfo = false;
                gameController.loadingScene.loadScene = true;
                hasLoadScene = true;
                speed = 0.08f;
                timer = 0;
                maxTimer = 0;
                index = 0;
                step = 0;
                gameController.hasChangeState = false;
            }

        });
    }
    
    public void EndMiniGame(GameObject[] classementPanels,List<GameObject> winners,GameObject endText) {
        hasLoadScene = false;

        RunDelayed(3f,() => {
            if(!hasClassementShow) {
                    
                endText.gameObject.SetActive(false);
                gameController.ActualizePlayerClassement();
                classementPanels[0].transform.parent.gameObject.SetActive(true);

                for(int i = 0;i<classementPanels.Length;i++)  {

                    classementPanels[i].SetActive(true);
                    classedPlayer = gameController.classedPlayers.Keys.ToList();
                    UserInventory inv = classedPlayer[i].GetComponent<UserInventory>();

                    classementPanels[i].transform.GetChild(1).gameObject.GetComponent<Text>().text = "" +  (gameController.FindPlayerClassement(gameController.GetKeyByValue(i,gameController.classedPlayers)) + 1);
                    classementPanels[i].transform.GetChild(1).gameObject.GetComponent<Text>().color = gameController.classedColors[gameController.FindPlayerClassement(gameController.GetKeyByValue(i,gameController.classedPlayers))];
                    classementPanels[i].transform.GetChild(0).gameObject.GetComponent<Image>().sprite = GetPlayerSprite(classedPlayer[i]);
                        
                    string name = classedPlayer[i].name;
                    if(classedPlayer[i].name.Contains("00")) 
                        name = classedPlayer[i].name.Replace("0","");

                    classementPanels[i].transform.GetChild(2).gameObject.GetComponent<Text>().text = name;
                    classementPanels[i].transform.GetChild(3).GetChild(1).gameObject.GetComponent<Text>().text = "" + inv.coins;
                    classementPanels[i].transform.GetChild(4).GetChild(1).gameObject.GetComponent<Text>().text =  "" + inv.cards;
                }

                hasClassementShow = true;
            }

            if(winners != null && winners.Count > 0) { // Un joueur a gagné 
                RunDelayed(1.5f,() => { 
                    if(!displayReward) {
                        foreach(GameObject winner in winners) {

                            ConvertPlayer(winner).transform.parent.gameObject.SetActive(true);
                            ConvertPlayer(winner).transform.parent.parent.gameObject.SetActive(true);
                            ConvertPlayer(winner).SetActive(true);
                            ConvertPlayer(winner).GetComponent<UserAudio>().CoinsGain();
                            classementPanels[ConvertPlayerIndex(winner)].transform.parent.GetChild(5).gameObject.SetActive(true);
                            ConvertPlayer(winner).GetComponent<UserInventory>().coins += 15;
                            classementPanels[ConvertPlayerIndex(winner)].transform.parent.GetChild(5).GetChild(0).gameObject.GetComponent<Text>().text = "+" + 15;

                            int index = -1;
                            foreach(GameObject obj in gameController.classedPlayers.Keys) {
                                index++;
                                if(obj.name == winner.name) {
                                    switch(index) {
                                        case 0:
                                            classementPanels[ConvertPlayerIndex(winner)].transform.parent.GetChild(5).gameObject.GetComponent<CoinsReward>().beginY = 204;
                                            break;

                                        case 1:
                                            classementPanels[ConvertPlayerIndex(winner)].transform.parent.GetChild(5).gameObject.GetComponent<CoinsReward>().beginY = 46;
                                            break;

                                        case 2:
                                            classementPanels[ConvertPlayerIndex(winner)].transform.parent.GetChild(5).gameObject.GetComponent<CoinsReward>().beginY = -112;
                                            break;

                                        case 3:
                                            classementPanels[ConvertPlayerIndex(winner)].transform.parent.GetChild(5).gameObject.GetComponent<CoinsReward>().beginY = -271;
                                            break;
                                    }
                                }
                            }
                                
                            classementPanels[ConvertPlayerIndex(winner)].transform.parent.GetChild(5).gameObject.GetComponent<CoinsReward>().changePos = true;
                            classementPanels[ConvertPlayerIndex(winner)].transform.parent.GetChild(5).gameObject.GetComponent<CoinsReward>().hasFinishAnimation = false;
                            classementPanels[ConvertPlayerIndex(winner)].transform.parent.GetChild(5).gameObject.GetComponent<CoinsReward>().minigame = true;
                            classementPanels[ConvertPlayerIndex(winner)].transform.parent.GetChild(5).gameObject.GetComponent<CoinsReward>().RunCoroutine();

                        }
                        displayReward = true;
                    }

                    
                    if(classementPanels != null && classementPanels[ConvertPlayerIndex(winners[0])].transform.parent.GetChild(5).gameObject.GetComponent<CoinsReward>().hasFinishAnimation) {
                        RunDelayed(1f,() => {
                            
                            gameController.ActualizePlayerClassement();

                            List<GameObject> newClassedPlayers = gameController.classedPlayers.Keys.ToList();

                            for(int i = 0;i<newClassedPlayers.Count;i++) {
                                if(newClassedPlayers[i] != classedPlayer[i]) {
                                    for(int j = 0;j<classedPlayer.Count;j++) {
                                        if(classedPlayer[j] == newClassedPlayers[i]) {
                                            if(!playerMove.Keys.Contains(newClassedPlayers[j]))
                                                playerMove.Add(newClassedPlayers[j],classedPlayer[j]);
                                        }
                                    }
                                }
                            }                               

                            if(!moove) {

                                if(playerMove.Keys.Count == 0) {
                                    for(int i = 0;i<newClassedPlayers.Count;i++) {
                                        UserInventory inv = ConvertPlayer(newClassedPlayers[i]).GetComponent<UserInventory>();
                                        classementPanels[i].transform.GetChild(3).GetChild(1).gameObject.GetComponent<Text>().text = "" + inv.coins;
                                        classementPanels[i].transform.GetChild(1).gameObject.GetComponent<Text>().text = ""  + (gameController.FindPlayerClassement(newClassedPlayers[i]) + 1);     
                                        classementPanels[i].transform.GetChild(1).gameObject.GetComponent<Text>().color = gameController.classedColors[gameController.FindPlayerClassement(newClassedPlayers[i])];
                                    }
                                }

                                foreach(GameObject pMove in playerMove.Keys) {

                                    UserInventory inv = pMove.GetComponent<UserInventory>();
                                    int index = -1;
                                    int indexMove = -1;
                                    GameObject playerInfo = null;
                                    GameObject playerMoveInfo = null;

                                    for(int i = 0;i<classedPlayer.Count;i++) {
                                        if(pMove == classedPlayer[i])
                                            index = i;
                                        if(classedPlayer[i] == playerMove[pMove]) 
                                            indexMove = i;
                                    }

                                    if(index == 0)
                                        playerInfo = classementPanels[0];
                                    else if(index == 1)
                                        playerInfo = classementPanels[1];
                                    else if(index == 2)
                                        playerInfo = classementPanels[2];
                                    else if(index == 3)
                                        playerInfo = classementPanels[3];

                                    if(indexMove == 0) 
                                        playerMoveInfo = classementPanels[0];
                                    else if(indexMove == 1) 
                                        playerMoveInfo = classementPanels[1];                           
                                    else if(indexMove == 2)
                                        playerMoveInfo = classementPanels[2];
                                    else if(indexMove == 3)
                                        playerMoveInfo = classementPanels[3];
                                            
                                    if(!playerPositionToGo.Keys.Contains(playerInfo)) 
                                        playerPositionToGo.Add(playerInfo,playerMoveInfo.transform.position);

                                    playerInfo.transform.GetChild(3).GetChild(1).gameObject.GetComponent<Text>().text = "" + inv.coins;
                                    playerInfo.transform.GetChild(1).gameObject.GetComponent<Text>().text = ""  + (gameController.FindPlayerClassement(pMove) + 1);     
                                    playerInfo.transform.GetChild(1).gameObject.GetComponent<Text>().color = gameController.classedColors[gameController.FindPlayerClassement(pMove)];
                                                                                    
                                }

                                moove = true;
                            }

                            foreach(GameObject pInfo in playerPositionToGo.Keys) {
                                pInfo.transform.position = Vector2.MoveTowards(pInfo.transform.position,playerPositionToGo[pInfo],100 * Time.deltaTime);
                            }

                            float wait;

                            if(playerPositionToGo.Keys.Count == 0) 
                                wait = 3;
                            else 
                                wait = 14;
                            
                            RunDelayed(wait,() => {
                                if(!hasTurnChange) {
                                    gameController.turn++;
                                    gameController.nightIndex--;
                                    if(gameController.nightIndex < 0)
                                        gameController.nightIndex =
                                            gameController.difficulty == GameController.Difficulty.EASY ? 4 :
                                            gameController.difficulty == GameController.Difficulty.MEDIUM ? 3 :
                                            gameController.difficulty == GameController.Difficulty.HARD ? 2 : 4;
                                    
                                    gameController.ManageTurn();

                                    hasTurnChange = true;
                                }

                                displayReward = false;
                                hasClassementShow = false;
                                moove = false;
                                playerMove.Clear();
                                playerPositionToGo.Clear();
                                classedPlayer.Clear();
                                gameController.part = GameController.GamePart.PARTYGAME;
                                gameController.BeginTurn(false);
                            });
                        });
                    }
                    else {
                        RunDelayed(3f,() => {
                            if(!hasTurnChange) {
                                gameController.turn++;
                                gameController.nightIndex--;
                                if(gameController.nightIndex < 0) 
                                    gameController.nightIndex =
                                        gameController.difficulty == GameController.Difficulty.EASY ? 4 :
                                        gameController.difficulty == GameController.Difficulty.MEDIUM ? 3 :
                                        gameController.difficulty == GameController.Difficulty.HARD ? 2 : 4;

                                gameController.ManageTurn();
                                hasTurnChange = true;
                            }

                            displayReward = false;
                            hasClassementShow = false;
                            moove = false;
                            playerMove.Clear();
                            playerPositionToGo.Clear();
                            classedPlayer.Clear();
                            gameController.part = GameController.GamePart.PARTYGAME;
                            gameController.BeginTurn(false);

                        });
                    }
                });
            }
            else {
                RunDelayed(3f,() => {
                    if(!hasTurnChange) {
                        gameController.turn++;
                        gameController.nightIndex--;
                        if(gameController.nightIndex < 0) {
                            gameController.nightIndex =
                                gameController.difficulty == GameController.Difficulty.EASY ? 4 :
                                gameController.difficulty == GameController.Difficulty.MEDIUM ? 3 :
                                gameController.difficulty == GameController.Difficulty.HARD ? 2 : 4;
                        }
                                    
                        gameController.ManageTurn();
                        hasTurnChange = true;
                    }

                    displayReward = false;
                    hasClassementShow = false;
                    moove = false;
                    playerMove.Clear();
                    playerPositionToGo.Clear();
                    classedPlayer.Clear();
                    gameController.part = GameController.GamePart.PARTYGAME;
                    gameController.BeginTurn(false);

                });
            }
        });    
    }

    private bool ContainsTag(string tag,List<GameObject> winners) {
        foreach(GameObject winner in winners) {
            if(winner.tag == tag)
                return true;
        }

        return false;
    }
    private GameObject ConvertPlayer(GameObject player) {
        switch(player.GetComponent<UserMovement>().userType) {
            case UserType.PLAYER:
                return gameController.players[0];
                break;

            case UserType.BOT_001:
                return gameController.players[1];
                break;

            case UserType.BOT_002:
                return gameController.players[2];
                break;

            case UserType.BOT_003:
                return gameController.players[3];
                break;            
        }

        return null;
    }

    private int ConvertPlayerIndex(GameObject player) {
        switch(player.GetComponent<UserMovement>().userType) {
            case UserType.PLAYER:
                return 0;
                break;

            case UserType.BOT_001:
                return 1;
                break;

            case UserType.BOT_002:
                return 2;
                break;

            case UserType.BOT_003:
                return 3;
                break;            
        }

        return -1;
    }
    
    private Sprite GetPlayerSprite(GameObject player) {
        switch(player.GetComponent<UserMovement>().userType) {
            case UserType.PLAYER:
                return gameController.smallSprites[0];
                break;

            case UserType.BOT_001:
                return gameController.smallSprites[1];
                break;

            case UserType.BOT_002:
                return gameController.smallSprites[2];
                break;

            case UserType.BOT_003:
                return gameController.smallSprites[3];
                break;            
        }

        return null;

    }
}
