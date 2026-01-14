using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Manages all owned relics. Handles add, remove, sell, and effect application.
/// </summary>
public class RelicManager : MonoBehaviour
{
    public static RelicManager Instance { get; private set; }
    
    [SerializeField] private int maxSlots = 5;
    
    private List<RelicData> ownedRelics = new List<RelicData>();
    
    public IReadOnlyList<RelicData> OwnedRelics => ownedRelics;
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
    
    public void AddRelic(RelicData relic)
    {
        if (relic == null) return;
        if (!HasSpace)
        {
            Debug.LogWarning("No space for new relic!");
            return;
        }
        
        ownedRelics.Add(relic);
        OnRelicsChanged?.Invoke();
    }
    
    public void RemoveRelic(RelicData relic)
    {
        if (ownedRelics.Remove(relic))
        {
            OnRelicsChanged?.Invoke();
        }
    }
    
    public void RemoveRelicAt(int index)
    {
        if (index >= 0 && index < ownedRelics.Count)
        {
            ownedRelics.RemoveAt(index);
            OnRelicsChanged?.Invoke();
        }
    }
    
    public RelicData GetRelicAt(int index)
    {
        if (index >= 0 && index < ownedRelics.Count)
        {
            return ownedRelics[index];
        }
        return null;
    }
    
    public void ClearAllRelics()
    {
        ownedRelics.Clear();
        OnRelicsChanged?.Invoke();
    }
    
    public void ExpandSlots(int amount)
    {
        maxSlots += amount;
    }
    
    /// <summary>
    /// Apply all relic effects to the modifiers.
    /// Called by EffectSystem.
    /// </summary>
    public void ApplyEffects(GameContext context, ref GameModifiers mods)
    {
        foreach (var relic in ownedRelics)
        {
            if (relic != null)
            {
                relic.ApplyEffect(context, ref mods);
            }
        }
    }
    
    /// <summary>
    /// Load relics from save data.
    /// </summary>
    public void LoadRelics(List<RelicData> relics)
    {
        ownedRelics.Clear();
        if (relics != null)
        {
            ownedRelics.AddRange(relics);
        }
        OnRelicsChanged?.Invoke();
    }
}
