using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LeaderboardPanel : MonoBehaviour {
    
    public Image userIcon;
    public Text rankText;
    public Text nameText;
    public Text coinsText;
    public Text cardsText;

    void Awake() {
        userIcon = transform.GetChild(0).gameObject.GetComponent<Image>();
        rankText = transform.GetChild(1).gameObject.GetComponent<Text>();
        nameText = transform.GetChild(2).gameObject.GetComponent<Text>();
        coinsText = transform.GetChild(3).GetChild(1).gameObject.GetComponent<Text>();
        cardsText = transform.GetChild(4).GetChild(1).gameObject.GetComponent<Text>();
    }
}
