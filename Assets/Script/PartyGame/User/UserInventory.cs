using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PathCreation;
using PathCreation.Examples;

public class UserInventory : MonoBehaviour {
    public GameController gameController;
    public bool hasDoubleDice; // Dé double
    public bool hasReverseDice; // Dé Inverse
    public bool hasHourglass; // Sablier
    public bool hasStar; // Etoile
    public bool hasLightning; // Eclair
    public bool hasParachute;
    public bool hasBomb;

    public int coins;
    public int cards;
    public int[] secretCode = new int[6];

    
    private List<Vector3> drop = new List<Vector3>();
    private List<string> objectSpawn = new List<string>();
    private List<int> objList = new List<int>();

    public void CoinGain(int coinsGain) {
        coins += coinsGain;

    }

    public void CoinLoose(int coinsLoose) {
        if(coins - coinsLoose >= 0) coins -= coinsLoose; 
        
        else coins = 0;
        
    }

    public void AddCards(int cardsGain) {
        cards += cardsGain;
    }


    public void DropItem(GameObject player) {
        // Perd tt ses coins si etoile --> la moitié si éclair
        UserInventory inventory = player.GetComponent<UserInventory>();

        bool[] objects = {inventory.hasDoubleDice,inventory.hasReverseDice,inventory.hasHourglass,inventory.hasStar,inventory.hasLightning,inventory.hasParachute,inventory.hasBomb};

        for(int i = 0;i<objects.Length;i++) {
            Debug.Log("i: " + i);
            if(objects[i] && !objList.Contains(i)) {
                objList.Add(i);
            }
         }

         Debug.Log("size: " + objList.Count);

        for(int i = 0;i<objList.Count;i++) {
          //  Debug.Log("index: " + i);
            Transform random = player.transform.GetChild(4).GetChild(Random.Range(0,player.transform.GetChild(4).childCount - 1));
            Debug.Log("vector: " + random.position);
            Debug.Log("contains?: " + drop.Contains(random.position));
            if(!drop.Contains(random.position) && !objectSpawn.Contains(gameController.GetPrefabObjects()[i].name)) {

                GameObject obj = Instantiate(gameController.GetPrefabObjects()[i],player.transform.position,gameController.GetPrefabObjects()[i].transform.rotation);
                PathFollower follower = obj.AddComponent<PathFollower>() as PathFollower;

                follower.pathCreator =  random.GetChild(0).GetComponent<PathCreator>();
                follower.endOfPathInstruction = EndOfPathInstruction.Stop;
                follower.speed = 30;
                string name = obj.name.Replace("(Clone)","");
                
                objectSpawn.Add(name);
                drop.Add(random.position);
            }
        }
    }

}
