using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class OrderController : MonoBehaviour {
    
    private GameObject[] players;
    public Vector3[] playerPos;
    public Vector3 cameraPos;

    public GameController controller;
    public bool begin;

    public Text[] diceResult;
    public int orderPlayer;

    private Dictionary<int,int> playersResult = new Dictionary<int,int>();
    
    void Start() {
        players = controller.players;

        if(begin)
            BeginOrder();
    }

    public void BeginOrder() {
        begin = true;

        Camera.main.gameObject.transform.position = cameraPos;

        for(int i = 0;i<players.Length;i++) 
            players[i].transform.position = playerPos[i]; 

        Vector3 dicePos = players[orderPlayer].transform.position;
        dicePos.y += 15f;

        controller.dice.transform.position = dicePos;
        controller.dice.GetComponent<DiceController>().lockDice = false;
        
    }

    public void ChangeUser() {
        if(orderPlayer < 3) {  
            diceResult[orderPlayer].text = "" + controller.dice.GetComponent<DiceController>().index;
            playersResult.Add(orderPlayer,controller.dice.GetComponent<DiceController>().index);
            orderPlayer++;
            controller.dice.GetComponent<DiceController>().lockDice = true;
            Vector3 dicePos = players[orderPlayer].transform.position;
            dicePos.y += 15f;

            controller.dice.transform.position = dicePos;
            controller.dice.GetComponent<DiceController>().index = 0;
            controller.dice.GetComponent<DiceController>().lockDice = false;

            if(!players[orderPlayer].GetComponent<PlayerController>().isPlayer)
                players[orderPlayer].GetComponent<PlayerController>().mustJump = true;
            
        }
        else { 
            diceResult[orderPlayer].text = "" + controller.dice.GetComponent<DiceController>().index;
            playersResult.Add(orderPlayer,controller.dice.GetComponent<DiceController>().index);
            orderPlayer = 0;
            controller.dice.SetActive(false);
            controller.dice.GetComponent<DiceController>().index = 0;
            controller.dice.GetComponent<DiceController>().lockDice = true;

            SortPlayers();
        }
    }

    private void SortPlayers() {

        List<int> results = playersResult.Values.ToList();
        List<GameObject> playersOrder = new List<GameObject>();

        results.Sort();
        results.Reverse();

        foreach(int result in results) {
            int player = GetKeyByValue(result);
            playersOrder.Add(players[player]);
            playersResult.Remove(player);
        }

        controller.players = playersOrder.ToArray();

    }

    public int GetKeyByValue(int value) {
        foreach(int key in playersResult.Keys) {
            if(playersResult[key] == value) 
                return key;        
        }

        return -1;
    }
}
