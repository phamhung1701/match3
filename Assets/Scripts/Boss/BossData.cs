using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Static configuration for a boss.
/// Contains effect behaviors that are applied via BossInstance.
/// </summary>
[CreateAssetMenu(menuName = "Braed/Boss")]
public class BossData : ScriptableObject
{
    [Header("Basic Info")]
    public string bossName;
    [TextArea(2, 4)]
    public string description;
    public Sprite icon;
    
    [Header("Availability")]
    public int minCycle = 1;
    
    [Header("Special")]
    public bool differentModifier;  // 4x score multiplier (Finality boss)
    
    [Header("Effects")]
    [Tooltip("List of effect behaviors this boss applies")]
    public List<EffectBehavior> effects = new List<EffectBehavior>();
    
    /// <summary>
    /// Get the trial display name for this boss.
    /// </summary>
    public string GetTrialName()
    {
        return $"Trial of the {bossName}";
    }
    
    /// <summary>
    /// Create a runtime instance of this boss.
    /// </summary>
    public BossInstance CreateInstance()
    {
        return new BossInstance(this);
    }
}
