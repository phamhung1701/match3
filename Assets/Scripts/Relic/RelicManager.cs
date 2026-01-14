using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Manages all owned relic instances.
/// Handles add, remove, sell, and effect application.
/// </summary>
public class RelicManager : MonoBehaviour
{
    public static RelicManager Instance { get; private set; }
    
    [SerializeField] private int maxSlots = 5;
    
    private List<RelicInstance> ownedRelics = new List<RelicInstance>();
    
    public IReadOnlyList<RelicInstance> OwnedRelics => ownedRelics;
    public int MaxSlots => maxSlots;
    public int CurrentCount => ownedRelics.Count;
    public bool HasSpace => ownedRelics.Count < maxSlots;
    
    public event Action OnRelicsChanged;
    
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    /// <summary>
    /// Add a relic by creating an instance from data.
    /// </summary>
    public RelicInstance AddRelic(RelicData data)
    {
        if (data == null) return null;
        if (!HasSpace)
        {
            Debug.LogWarning("No space for new relic!");
            return null;
        }
        
        var instance = data.CreateInstance();
        ownedRelics.Add(instance);
        OnRelicsChanged?.Invoke();
        return instance;
    }
    
    /// <summary>
    /// Remove a relic instance.
    /// </summary>
    public void RemoveRelic(RelicInstance instance)
    {
        if (ownedRelics.Remove(instance))
        {
            OnRelicsChanged?.Invoke();
        }
    }
    
    /// <summary>
    /// Remove relic at index.
    /// </summary>
    public void RemoveRelicAt(int index)
    {
        if (index >= 0 && index < ownedRelics.Count)
        {
            ownedRelics.RemoveAt(index);
            OnRelicsChanged?.Invoke();
        }
    }
    
    /// <summary>
    /// Get relic instance at index.
    /// </summary>
    public RelicInstance GetRelicAt(int index)
    {
        if (index >= 0 && index < ownedRelics.Count)
        {
            return ownedRelics[index];
        }
        return null;
    }
    
    /// <summary>
    /// Get relic data at index (for UI display).
    /// </summary>
    public RelicData GetRelicDataAt(int index)
    {
        return GetRelicAt(index)?.Data;
    }
    
    /// <summary>
    /// Clear all relics.
    /// </summary>
    public void ClearAllRelics()
    {
        ownedRelics.Clear();
        OnRelicsChanged?.Invoke();
    }
    
    /// <summary>
    /// Expand slot limit.
    /// </summary>
    public void ExpandSlots(int amount)
    {
        maxSlots += amount;
    }
    
    /// <summary>
    /// Apply all relic effects.
    /// Called by EffectSystem.
    /// </summary>
    public void ApplyEffects(GameContext context, ref GameModifiers mods)
    {
        foreach (var instance in ownedRelics)
        {
            instance.ApplyEffects(context, ref mods);
        }
    }
    
    /// <summary>
    /// Called at end of turn for all relics.
    /// </summary>
    public void OnTurnEnd()
    {
        foreach (var instance in ownedRelics)
        {
            instance.OnTurnEnd();
        }
    }
    
    /// <summary>
    /// Called when a match is made.
    /// </summary>
    public void OnMatchMade()
    {
        foreach (var instance in ownedRelics)
        {
            instance.OnMatchMade();
        }
    }
    
    /// <summary>
    /// Reset all relics for new trial.
    /// </summary>
    public void ResetForNewTrial()
    {
        foreach (var instance in ownedRelics)
        {
            instance.ResetForNewTrial();
        }
    }
}
