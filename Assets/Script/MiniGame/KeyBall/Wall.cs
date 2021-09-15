using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wall : MonoBehaviour {
    
    public KBController controller;
    public bool open;
    private float time;

    void Start() {
        transform.gameObject.GetComponent<MeshRenderer>().enabled = open;
    }

    void Update() {

        if(!controller.begin) {
            time += Time.deltaTime;

            if(time >= 3f) {
                open = !open;
                time = 0;
            }

            transform.gameObject.GetComponent<MeshRenderer>().enabled = open;
            transform.gameObject.GetComponent<BoxCollider>().isTrigger = !open;
        }
    }
}
