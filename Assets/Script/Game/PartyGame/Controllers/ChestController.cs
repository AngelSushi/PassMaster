using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChestController : MonoBehaviour {
    
    // Lister tt les enfants de stepList pour récupérer tt les transform de chaque step 

    private List<GameObject> chestObj = new List<GameObject>();
    private GameObject actualChestObj;

    void Start() {
        
        int length = transform.childCount;

        for(int i = 0;i<length;i++) {
            chestObj.Add(transform.GetChild(i).gameObject);
        }

//        actualChestObj = chestObj[Random.Range(0,length - 1)];
//
//       actualChestObj.SetActive(true);


    }

    void Update() {
        
    }

    IEnumerator AppearChest() {

        for(int i = 0;i<200;i++) {
            if(transform.position.y > 4984) {

                yield return new WaitForSeconds(0.1f);
                transform.localPosition = new Vector3(transform.position.x,transform.position.y - 0.2f,transform.position.z);

            }
            else {
                break;
            }
        }

    }
}
