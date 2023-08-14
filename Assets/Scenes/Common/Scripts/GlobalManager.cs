using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

/// <summary>
/// The GlobalManager class is responsible for managing global game state and configuration.
/// </summary>
public class GlobalManager : MonoBehaviour
{
    public static GlobalManager Instance { get; private set; }
    public string configFile;

    /// <summary>
    /// Called when the script instance is being loaded.
    /// </summary>
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Debug.Log("Warning: Repeated Global Manager initialization");
            Destroy(this);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }
}
