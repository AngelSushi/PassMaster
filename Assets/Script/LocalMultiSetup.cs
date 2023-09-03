using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class LocalMultiSetup : MonoBehaviour
{

    private static int playerCount;

    public static int PlayerCount
    {
        get => playerCount;
        set => playerCount = value;
    }

    [SerializeField] private GameObject localPrefab;

    public GameObject LocalPrefab
    {
        get => localPrefab;
    }

    private static LocalMultiSetup instance;

    public static LocalMultiSetup Instance
    {
        get => instance;
        private set => instance = value;
    }

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    public void OnPlayerJoin(PlayerInput pi)
    {
        Debug.Log("player join " + pi.playerIndex);
    }
}
