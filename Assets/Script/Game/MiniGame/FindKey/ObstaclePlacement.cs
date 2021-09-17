using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstaclePlacement : MonoBehaviour {
    
    private void OnTriggerEnter(Collider hit) {
        if(hit.gameObject.tag == "Sol") {
         
        //    transform.rotation = Quaternion.Euler(0,hit.gameObject.GetComponent<Obstacle>().rotation,0);
        //    transform.position = new Vector3(hit.gameObject.transform.position.x,transform.position.y,hit.gameObject.transform.position.z);
            
            if(hit.gameObject.GetComponent<Obstacle>().rotation == -90) {
        //        transform.localScale = new Vector3(13.83f,13.83f,54.04f);
            }
            
        }
    }
}
