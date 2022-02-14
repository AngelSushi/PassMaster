using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
public class PathGenerator : MonoBehaviour {
    public GameObject prefab,planePrefab;
    public GameObject target;
    public GameController game;

    public void GeneratePath() {
        target = transform.gameObject;

        int size = target.transform.childCount;

        for(int i = 0;i<size;i++) { 
            GameObject plane = Instantiate(prefab,target.transform.GetChild(i).position,Quaternion.identity,target.transform);
            Vector3 nextPosition = target.transform.GetChild(i + 1).position;
            float stepDistance = Vector3.Distance(plane.transform.position,nextPosition) / 10f; // Un plane est 10x fois plus grand qu'une unité
            plane.transform.localScale = new Vector3(0.31f,1f,stepDistance);
            plane.transform.LookAt(nextPosition);
            plane.transform.eulerAngles = new Vector3(0f,plane.transform.eulerAngles.y - 178f,0f);
            plane.layer = 31;

            if(plane.transform.childCount > 0) {
                for(int j = 0;j<plane.transform.childCount;j++) 
                    plane.transform.GetChild(j).gameObject.layer = 31; 
            }
            
        }
    }

    public void AffectParameters() {
        game.excelArray = ExcelReader.LoadJsonExcelClass(game.stepFile.text);
        ExcelReader.AffectParameters(game.excelArray);
    }

    public void GenerateChest() {
        for(int i = 0;i<transform.childCount;i++) {
            Vector3 position = transform.GetChild(i).position;
            Step step = transform.GetChild(i).gameObject.GetComponent<Step>();
            
            if(step == null) {
                Debug.Log("error when getting step component in " + transform.GetChild(i).gameObject);
                continue;
            }

            position.y += 2.5f;
            GameObject chest = Instantiate(prefab,position,Quaternion.identity,target.transform); 
            int amplifierDirection = step.positive == true ? 1 : -1;
            chest.transform.position += GetChestDirection(chest,step) * amplifierDirection;
            chest.transform.localScale = new Vector3(0.5f,0.5f,0.5f);
            chest.transform.LookAt(transform.GetChild(i).position);
            chest.transform.eulerAngles = new Vector3(0f,chest.transform.eulerAngles.y - 90.0f - 180f,0f); // A supprimer pr la deuxieme ile
            chest.AddComponent<BoxCollider>();
            chest.GetComponent<BoxCollider>().center = new Vector3(-0.325f,-0.283f,-0.235f);
            chest.GetComponent<BoxCollider>().size = new Vector3(9.072f,10.917f,14.796f);
            step.chest = chest;
            
            position.y -= 2.5f;
            GameObject plane = Instantiate(planePrefab,position,Quaternion.identity,target.transform);
            Vector3 nextPosition = chest.transform.position;
            float stepDistance = Vector3.Distance(position,nextPosition) / 10f; // Un plane est 10x fois plus grand qu'une unité
            plane.transform.localScale = new Vector3(0.31f,1f,stepDistance);
            plane.transform.LookAt(nextPosition);
            plane.transform.eulerAngles = new Vector3(0f,plane.transform.eulerAngles.y - 178f,0f);
            plane.layer = 31;

            if(plane.transform.childCount > 0) {
                for(int j = 0;j<plane.transform.childCount;j++) 
                    plane.transform.GetChild(j).gameObject.layer = 31;
            }
            
            plane.transform.parent = chest.transform;
        }
    }

    private Vector3 GetChestDirection(GameObject obj,Step step) {
        if(step.useVectors.Length > 0) {
            bool forward = step.useVectors[0];
            bool back = step.useVectors[1];
            bool right = step.useVectors[2];
            bool left = step.useVectors[3];

            if(forward) {
                if(right && !left) 
                    return obj.transform.forward * 9.5f + obj.transform.right * 9.5f;
                else if(!right && left) 
                    return obj.transform.forward * 9.5f + obj.transform.right * -1 * 9.5f;
                else 
                    return obj.transform.forward * 13;
            }
            else if(back) {
                if(right && !left) 
                    return obj.transform.forward * -1 * 9.5f + obj.transform.right * 9.5f;
                else if(!right && left) 
                    return obj.transform.forward * -1 *  9.5f + obj.transform.right * -1 * 9.5f;
                else 
                    return obj.transform.forward * -1 * 13;
            }
            else if(right) 
                return obj.transform.right * 13;
            else if(left)
                return obj.transform.right * -1 * 13;

        }

        Debug.Log("error with " + step + " chest generation");
        return Vector3.zero;
        
    }
}
