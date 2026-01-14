using System;
using UnityEngine;

/// <summary>
/// Global game data singleton. Holds currency only.
/// Relics are now managed by RelicManager.
/// </summary>
public class Data : MonoBehaviour
{
    public static Data Instance { get; private set; }

    public int Shard = 0;
    
    void OnEnable()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }
    }
    
    public void ResetForNewRun()
    {
        Shard = 0;
    }
}
