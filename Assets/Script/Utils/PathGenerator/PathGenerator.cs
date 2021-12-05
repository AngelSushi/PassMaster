﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
public class PathGenerator : MonoBehaviour {
    public GameObject planePrefab;
    private GameObject target;

    public void GeneratePath() {
        target = transform.gameObject;

        int size = target.transform.childCount;

        for(int i = 0;i<size;i++) { 
            GameObject plane = Instantiate(planePrefab,target.transform.GetChild(i).position,Quaternion.identity,target.transform);
            Vector3 nextPosition = target.transform.GetChild(i + 1).position;
            float stepDistance = Vector3.Distance(plane.transform.position,nextPosition) / 10f; // Un plane est 10x fois plus grand qu'une unité
            plane.transform.localScale = new Vector3(0.31f,1f,stepDistance);
            plane.transform.LookAt(nextPosition);
            plane.transform.eulerAngles = new Vector3(0f,plane.transform.eulerAngles.y - 178f,0f);
        }
    }
}
