using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZoneSwapper : MonoBehaviour {

    public GameObject[] rightSwappers;
    public GameObject[] leftSwappers;
    public int areaIndex;
    private float startY;
    private CookController _cookController;

    private void Start() {
        startY = transform.position.y;
        _cookController = (CookController)CookController.instance;

        foreach (CookController.Instance instance in _cookController.instances) {
            instance.instanceCamera.gameObject.SetActive(false);
            for(int i = 0;i < instance.allCanvas.Count;i++)
                instance.allCanvas[i].gameObject.SetActive(false);
        }
        
       
        _cookController.instances[areaIndex].instanceCamera.gameObject.SetActive(true);
        for(int i = 0;i <  _cookController.instances[areaIndex].allCanvas.Count;i++)
            _cookController.instances[areaIndex].allCanvas[i].gameObject.SetActive(true);
    }


    private void Update() {
        float sideRight = SideSwapValue(rightSwappers[areaIndex].transform.position);
        float sideLeft = SideSwapValue(leftSwappers[areaIndex].transform.position);

        if (sideRight < 0) {
            if (areaIndex < rightSwappers.Length) {
                _cookController.instances[areaIndex].instanceCamera.gameObject.SetActive(false);
                for(int i = 0;i <  _cookController.instances[areaIndex].allCanvas.Count;i++)
                    _cookController.instances[areaIndex].allCanvas[i].gameObject.SetActive(false);
                
                areaIndex++;
                transform.position = leftSwappers[areaIndex].transform.position;
                
                _cookController.instances[areaIndex].instanceCamera.gameObject.SetActive(true);
                for(int i = 0;i <  _cookController.instances[areaIndex].allCanvas.Count;i++)
                    _cookController.instances[areaIndex].allCanvas[i].gameObject.SetActive(true);
                
            }
        }

        if (sideLeft > 0) {
            if (areaIndex > 0) {
                _cookController.instances[areaIndex].instanceCamera.gameObject.SetActive(false);
                for(int i = 0;i <  _cookController.instances[areaIndex].allCanvas.Count;i++)
                    _cookController.instances[areaIndex].allCanvas[i].gameObject.SetActive(false);
                
                areaIndex--;
                transform.position = rightSwappers[areaIndex].transform.position;
                
                _cookController.instances[areaIndex].instanceCamera.gameObject.SetActive(true);
                for(int i = 0;i <  _cookController.instances[areaIndex].allCanvas.Count;i++)
                    _cookController.instances[areaIndex].allCanvas[i].gameObject.SetActive(true);
            }
        }
    }

    private float SideSwapValue(Vector3 swapperPositiion) {
        Vector3 playerPosition = new Vector3(transform.position.x, startY, transform.position.z);

        Vector3 playerToSwapper = swapperPositiion - playerPosition;
        Vector3 normalVector = Vector2.Perpendicular(playerPosition);

        return Vector3.Dot(playerToSwapper, normalVector);
    }
}
