using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Step : MonoBehaviour {
    public bool[] useVectors; // size 4 : forward , back , right , left
    public bool positive;
    public StepType type;
    public List<GameObject> playerInStep = new List<GameObject>();
    public GameObject stack;
    public GameObject chest;
    public GameObject shop;
    public bool skipIA;
    public Vector3 avoidPos; // position of players to avoid other player on path

    public Dictionary<ItemType,int> itemsInStep = new Dictionary<ItemType, int>();

}
