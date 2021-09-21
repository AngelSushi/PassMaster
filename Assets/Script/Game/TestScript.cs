using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestScript : MonoBehaviour {

    public GameObject[] list;

    private GameObject f;
    void Update() {

        Debug.Log(transform.forward);


        for(int i = 0;i<100;i++) {
           Vector3 newVec = transform.position + transform.forward * i;

            f = IsObj((int)newVec.x,(int)newVec.z);
            if(f != null) {
                break;
            } 
        }

        Debug.Log("forwardStep: " + f);
    }

    private GameObject IsObj(int x,int z) {
        foreach(GameObject obj in list) {
            if(x >= obj.transform.position.x - 10 && x <= obj.transform.position.x + 10 && z >= obj.transform.position.z - 10 && z <= obj.transform.position.z + 10) 
                    return obj;
   
        }

        return null;
    }
}
