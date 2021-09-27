using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class OrderController : CoroutineSystem {
    
    private GameObject[] players;
    public Vector3[] playerPos;
    public Vector3 cameraPos;

    public GameController controller;
    public bool begin;

    public Text[] diceResult;
    public int orderPlayer;

    private Dictionary<int,int> playersResult = new Dictionary<int,int>();

    public GameObject[] orderPanels;
    
    void Start() {
        players = controller.players;

      /*  if(begin)
            BeginOrder();
    */
    }

    public void BeginOrder() {
        begin = true;

        Camera.main.gameObject.transform.position = cameraPos;
        Camera.main.gameObject.transform.rotation = Quaternion.Euler(0,0,0);

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

        controller.SortUserSprite();

        RunAnim();

    }

    private void RunAnim() {
        RunDelayed(0.2f,() => {
            for(int i = 0;i<orderPanels.Length;i++) {
                Image icon = orderPanels[i].transform.GetChild(0).gameObject.GetComponent<Image>();
                Text text = orderPanels[i].transform.GetChild(1).gameObject.GetComponent<Text>();
                
                icon.sprite = controller.smallSprites[i];
                text.text = controller.players[i].name;         
            }

            orderPanels[0].transform.parent.gameObject.SetActive(true);
            
            RunDelayed(0.75f,() => {
                orderPanels[0].SetActive(true);
                RunDelayed(1.2f,() => {
                    orderPanels[1].SetActive(true);
                    RunDelayed(1.2f,() => {
                        orderPanels[2].SetActive(true);
                        RunDelayed(1.2f,() =>  {
                            orderPanels[3].SetActive(true);
                            RunDelayed(1.5f,() => {
                                FinishAnim();
                                begin = false;
                            });
                        });
                    });
                });
            });
        });
    }

    private void FinishAnim() {
        controller.part = GameController.GamePart.PARTYGAME;

        foreach(GameObject panel in orderPanels) 
            panel.SetActive(false);

        orderPanels[0].transform.parent.gameObject.SetActive(false);

        foreach(Text text  in diceResult) 
            text.text = "";
    }

    public int GetKeyByValue(int value) {
        foreach(int key in playersResult.Keys) {
            if(playersResult[key] == value) 
                return key;        
        }

        return -1;
    }
}
