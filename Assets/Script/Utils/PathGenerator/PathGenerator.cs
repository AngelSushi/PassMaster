using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathGenerator : MonoBehaviour {
    public GameObject pathParent;
    public GameObject planePrefab;
    private GameObject target;

    public void GeneratePath() {
        target = pathParent == null ? transform.gameObject : pathParent;
        bool change = true;

        int nextIndex = 0;

        for(int i = 0;i</*target.transform.childCount*/ 2;i++) { 
            if(i > 0) {
                if(change) 
                    i--; 
            }

            if(change) {
                Debug.Log(i + " " + (i+1));
                GameObject plane = Instantiate(planePrefab,target.transform.GetChild(i).position,Quaternion.identity);
                plane.transform.parent = target.transform;

                Vector3 nextPosition = target.transform.GetChild(i + 1).position;
                float stepDistance = Vector3.Distance(plane.transform.position,nextPosition) / 10f;
                plane.transform.localScale = new Vector3(0.31f,1f,stepDistance);
               // COSINUS = adjacent / hypothénus
                // adjacent = stepDistance

                // Droite perpendiculaire = mx + h

                Vector3 cPoint  = nextPosition + nextPosition.normalized * 15;
                float cOriginalPosDistance = Vector3.Distance(plane.transform.position,cPoint);

                float angle = Mathf.Cos(stepDistance / cOriginalPosDistance) * Mathf.Rad2Deg;

                Debug.Log("angle: " + angle);

             //   plane.transform.eulerAngles = new Vector3(0f,angle * 100,0f);
            }

            change = !change;
        }


    }
}
