using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BreakObject : MonoBehaviour {
    private float timer;

    private void OnCollisionStay(Collision hit) {
        if(hit.gameObject.tag == "Player" || hit.gameObject.tag == "Bot") {
            timer += Time.deltaTime;

            if(timer >= 3.0f) {
                timer = 0;
                Destroy(transform.gameObject);
            }
        }
    }

    private void OnCollisionExit(Collision hit) {
        if(hit.gameObject.tag == "Player" || hit.gameObject.tag == "Bot") {
            timer = 0;
        }
    }
}
