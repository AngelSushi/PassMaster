using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SeekersScrolling : MonoBehaviour
{
    public Vector2 beginPosition;
    public Vector2 endPosition;
    public HSController controller;
    public float speed;

    void Start() {
        transform.position = beginPosition;
        ActiveChild(false);
    }
    
    void Update() {
        if(!controller.begin && !controller.finish && !controller.randomRoles) {
            ActiveChild(true);
            transform.position = Vector2.MoveTowards(transform.position,endPosition,speed * Time.deltaTime);

            if(transform.position.x == 535)
                transform.position = beginPosition;
        }
    }

    private void ActiveChild(bool active) {
        for(int i = 0;i<transform.childCount;i++) {
            transform.GetChild(i).gameObject.SetActive(active);
        }
    }
}
