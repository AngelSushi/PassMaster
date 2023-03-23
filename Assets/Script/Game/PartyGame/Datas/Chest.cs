using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[SelectionBase]
public class Chest : MonoBehaviour {

    public void EndChestAnimation() => GameController.Instance.chestController.EndChestAnim();
    
    
}
