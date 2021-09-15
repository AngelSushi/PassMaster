using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoteController : MonoBehaviour {

    public Vector3 startPos;
    public int noteIndex;
    public float age;

    void Start() {
          transform.position = startPos;
    }

    void Update() {
        if(startPos != Vector3.zero) {
            age -= Time.deltaTime;
            Debug.Log("enter: " + startPos);
            Vector3 position = startPos - ( Vector3.left * age  );
            transform.position = position;
        }
        else {
            transform.gameObject.SetActive(false);
        }
    }
}
