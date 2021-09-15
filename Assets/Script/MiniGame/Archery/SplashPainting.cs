using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SplashPainting : CoroutineSystem {
    
    #region Variables

    private bool disappear;
    private float timer;

    #endregion

    #region Unity's Functions
    
    void Start() {
        RunDelayed(3f,() => {
            disappear = true;
        });
    }

    void Update() {
        if(disappear) {
            timer += Time.deltaTime;

            if(timer >= 0.1f) {
                GetComponent<SpriteRenderer>().color = new Color(1.0f,1.0f,1.0f,GetComponent<SpriteRenderer>().color.a - 0.05f);               
                if(GetComponent<SpriteRenderer>().color.a <= 0) 
                    Destroy(transform.gameObject);
                
                timer = 0;
            }

        }
    }

    #endregion

}
