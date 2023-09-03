using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LocalInstantiate : MonoBehaviour
{

    [SerializeField] private GameObject localPrefab;

    private GameObject _refLocal;
    
    private void Awake()
    {
        GameObject parent = GameObject.Find("Players");

        if (parent == null)
        {
            return;
        }

        _refLocal = Instantiate(localPrefab,localPrefab.transform.position, Quaternion.identity,parent.transform);

        _refLocal.GetComponent<RectTransform>().localPosition = Vector3.zero;
        _refLocal.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
        _refLocal.GetComponent<RectTransform>().rotation = Quaternion.Euler(0, 0, 0);
       
    }

    private void Update()
    {
        _refLocal.GetComponent<RectTransform>().rotation = Quaternion.Euler(0, 0, 0);
    }
}
