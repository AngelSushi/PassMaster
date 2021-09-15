using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dizzy : MonoBehaviour {
    public bool end;
    private int timer;
    void Update() {
        if(transform.gameObject.activeSelf) {
            timer++;

            if(timer >= 1.5f) {
                timer = 0;
                transform.Rotate(0,5,0);
            }

            if(end)
                Destroy(transform.gameObject);
        }
    }
}
