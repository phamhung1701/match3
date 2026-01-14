using UnityEngine;

/// <summary>
/// Abstract base for all bosses.
/// Every boss must inherit and implement ApplyEffect.
/// </summary>
public abstract class BossData : ScriptableObject
{
    [Header("Basic Info")]
    public string bossName;
    [TextArea(2, 4)]
    public string description;
    public Sprite icon;
    
    [Header("Availability")]
    public int minCycle = 1;
    
    [Header("Special")]
    public bool differentModifier;  // 4x score multiplier (like Finality)
    
    /// <summary>
    /// Get the trial display name for this boss.
    /// </summary>
    public string GetTrialName()
    {
        return $"Trial of the {bossName}";
    }
    
    /// <summary>
    /// Apply this boss's effects to the modifiers.
    /// Must be implemented by each boss subclass.
    /// </summary>
    public abstract void ApplyEffect(GameContext context, ref GameModifiers mods);
}
