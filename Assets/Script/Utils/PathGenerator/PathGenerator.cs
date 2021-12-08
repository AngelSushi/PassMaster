using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
public class PathGenerator : MonoBehaviour {
    public GameObject prefab;
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
        }
    }

    public void AffectParameters() {
        game.excelArray = ExcelReader.LoadJsonExcelClass(game.stepFile.text);
        ExcelReader.AffectParameters(game.excelArray);
    }

    public void GenerateChest() {
        int size = transform.childCount;

        for(int i = 4;i<5;i++) {
            Vector3 position = transform.GetChild(i).position;
            Step step = transform.GetChild(i).gameObject.GetComponent<Step>();
            position.y += 3;
            GameObject chest = Instantiate(prefab,position,Quaternion.identity,target.transform); 

            int amplifierDirection = step.positive == true ? 1 : -1;
            chest.transform.position += GetChestDirection(chest,step) * amplifierDirection;
            chest.transform.localScale = new Vector3(0.66f,0.66f,0.66f);
            chest.transform.LookAt(transform.GetChild(i).position);
            chest.transform.eulerAngles = new Vector3(0f,chest.transform.eulerAngles.y - 90.0f,0f);
        }
    }

    private Vector3 GetChestDirection(GameObject obj,Step step) {
        bool forward = step.useVectors[0];
        bool back = step.useVectors[1];
        bool right = step.useVectors[2];
        bool left = step.useVectors[3];

        if(forward) {
            if(right && !left) 
                return obj.transform.forward * 6.5f + obj.transform.right * 6.5f;
            else if(!right && left) 
                return obj.transform.forward * 6.5f + obj.transform.right * -1 * 6.5f;
            else 
                return obj.transform.forward * 13;
        }
        else if(back) {
            if(right && !left) 
                return obj.transform.forward * -1 * 6.5f + obj.transform.right * 6.5f;
            else if(!right && left) 
                return obj.transform.forward * -1 *  6.5f + obj.transform.right * -1 * 6.5f;
            else 
                return obj.transform.forward * -1 * 13;
        }
        else if(right) 
            return obj.transform.right * 13;
        else if(left)
            return obj.transform.right * -1 * 13;

        return Vector3.zero;
    }
}
