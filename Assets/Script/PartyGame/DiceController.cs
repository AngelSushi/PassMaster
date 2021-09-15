using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DiceController : MonoBehaviour {

    public float speed;

    private int[] rotateX = {0,0,90,0,0,0};
    private int[] rotateY = {-12,167,-23,-12,-102,-280};
    private int[] rotateZ = {-85,175,80,-180,-180,-180};

    // 1 0 ; -12.334 ; -85
    // 2 0 ; 167.483 ; 175.084
    // 3 90 ; -23 ; 80
    // 4 0 ; -12.334 ; -180
    // 5 0 ; -101.74 ; -180
    // 6 0 ; -280 ; -180

    public int index;
    private int seconds = 0;
    private int wait = 25;

    public bool lockDice;

    void Start() {
        
        if(!lockDice) {
            StartCoroutine(RotateDice());
        }
          
    }

    public IEnumerator RotateDice() {
       while(!lockDice) {
            transform.rotation = Quaternion.Euler(0, 0, 0); // RESET ROTATION
            transform.Rotate(rotateX[index],rotateY[index],rotateZ[index]);

            index++;

            if(index == 6) index = 0;

            yield return new WaitForSeconds(speed);
       }

    }
}
