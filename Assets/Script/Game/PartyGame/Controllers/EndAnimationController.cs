using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
using System.Linq;
using UnityEngine.SceneManagement;

public class EndAnimationController : CoroutineSystem {

    public Vector3 destination;
    public Vector3 cameraBeginPos;
    public Vector3 cameraBeginRot;
    public Vector3 fireworksPos;
    public bool isInEndAnimation;
    [HideInInspector]
    public bool checkCode;
    public GameObject secretCodeObject;
    public Text victoryText;
    public GameObject fireworks;
    public GameObject leaderboard;
    private bool mooveToDestination;
    private GameController controller;
    private LeaderboardPanel[] panels;

    void Start() {
        controller = GameController.Instance;
        panels = new LeaderboardPanel[4];

        leaderboard.SetActive(true);

        for(int i = 0;i<panels.Length;i++) {
            panels[i] = leaderboard.transform.GetChild(1 + i).gameObject.GetComponent<LeaderboardPanel>();
        }

        leaderboard.SetActive(false);
    }

    void Update() {
        if(mooveToDestination) {
            controller.players[controller.actualPlayer].transform.position = Vector3.MoveTowards(controller.players[controller.actualPlayer].transform.position,destination,controller.players[controller.actualPlayer].GetComponent<NavMeshAgent>().speed * Time.deltaTime);
            controller.players[controller.actualPlayer].GetComponent<NavMeshAgent>().enabled = false;
        }

        if(checkCode) {
            if(mooveToDestination)
                mooveToDestination = false;

            RunDelayed(2f,() => {

                if(!checkCode)
                    return;

                if(controller.players[controller.actualPlayer].GetComponent<UserInventory>().cards == controller.secretCode.Length) {    // VICTORY
                    if(!controller.players[controller.actualPlayer].GetComponent<UserMovement>().constantJump)
                        controller.players[controller.actualPlayer].GetComponent<UserMovement>().Jump();

                    controller.players[controller.actualPlayer].GetComponent<UserMovement>().constantJump = true;
                    
                    if(!secretCodeObject.GetComponent<Animation>().isPlaying) 
                        secretCodeObject.GetComponent<Animation>().Play();

                    checkCode = false;

                    victoryText.text = "Victoire de <color=" + ChooseColorByUserType(controller.players[controller.actualPlayer].GetComponent<UserMovement>().userType) + ">" + controller.players[controller.actualPlayer].name + "</color>";
                    victoryText.gameObject.SetActive(true);

                    RunDelayed(1.5f,() => {
                        controller.blackScreenAnim.Play();
                    });
                }

                else { // Return normal
                    if(controller.players[controller.actualPlayer].GetComponent<UserMovement>().actualStep == null)
                        return;

                    if(!controller.players[controller.actualPlayer].GetComponent<NavMeshAgent>().enabled)
                        controller.players[controller.actualPlayer].GetComponent<NavMeshAgent>().enabled = true;
                }
            });  
        }
    }

    public void BeginEndAnimationEvent() {
        if(!isInEndAnimation)
            return;

        controller.mainCamera.transform.position = cameraBeginPos;
        controller.mainCamera.transform.eulerAngles = cameraBeginRot;
        controller.players[controller.actualPlayer].transform.GetChild(1).gameObject.SetActive(false);

        RunDelayed(1.1f,() => {
            mooveToDestination = true;
        });
    }

    public void DisplayLeaderboardEvent() {
        if(!isInEndAnimation || !victoryText.gameObject.activeSelf)
            return;

        victoryText.gameObject.SetActive(false);
        controller.mainCamera.transform.eulerAngles = new Vector3(335.2164f,0f,0f);
        fireworks.transform.position = fireworksPos;
        fireworks.SetActive(true);

        leaderboard.SetActive(true);
        GameObject winner = controller.players[controller.actualPlayer];

        panels[0].userIcon.sprite = winner.GetComponent<UserUI>().userSprite;
        panels[0].nameText.text = winner.name;
        panels[0].coinsText.text = "" + winner.GetComponent<UserInventory>().coins;
        panels[0].cardsText.text = "" + winner.GetComponent<UserInventory>().cards;

        List<GameObject> players = new List<GameObject>(controller.players);
        players.Remove(controller.players[controller.actualPlayer]);

        players = players.OrderBy(player=>player.GetComponent<UserInventory>().points).ToList(); // Classement
        players.Reverse();

        for(int i = 0;i<players.Count;i++) {
            panels[i + 1].userIcon.sprite = players[i].GetComponent<UserUI>().userSprite;
            panels[i + 1].nameText.text = players[i].name;
            panels[i + 1].coinsText.text = "" + players[i].GetComponent<UserInventory>().coins;
            panels[i + 1].cardsText.text = "" + players[i].GetComponent<UserInventory>().cards;
        }

        RunDelayed(10f,() => {
            controller.blackScreenAnim.Play();

            RunDelayed(1.3f,() => {
                 SceneManager.LoadScene("MainMenu",LoadSceneMode.Single);
            });
        });
    }

    

    private string ChooseColorByUserType(UserType type) {
        switch(type) {
            case UserType.PLAYER:
                return "#FF0000";
            case UserType.BOT_001:
                return "#0390EE";
            case UserType.BOT_002:
                return "#20FA00";
            case UserType.BOT_003:
                return "#FFE700";
            default:
                return "#FF0000";
        }
    }

}
