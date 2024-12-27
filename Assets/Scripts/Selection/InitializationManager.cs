using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InitializationManager : MonoBehaviour
{
    public static InitializationManager Instance { get; private set; }

    public static event Action OnInitializationComplete;

    public Transform PlayerTransform { get; private set; }

    private bool isInitialized = false;

    private void Awake()
    {
        // If there is an instance, and it's not me, delete myself.
        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(Instance);
        }
    }
    public void InitializeGame(Transform playerTransform)
    {
        if (isInitialized) return;

        PlayerTransform = playerTransform;
        isInitialized = true;

        //inform all listeners
        OnInitializationComplete?.Invoke();
    }
}
