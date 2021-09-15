using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "MiniGame", menuName = "PassMaster/MiniGame", order = 1)]
public class MiniGame : ScriptableObject {

    public string minigameName;
    public string minigameDesc;
    public Sprite minigameSprite;
    public string controls;
}
