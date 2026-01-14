using System.Collections.Generic;

/// <summary>
/// Runtime wrapper for a boss.
/// Holds state like current phase, HP, and references the static BossData.
/// </summary>
public class BossInstance : IEffectSource
{
    public BossData Data { get; private set; }
    
    // Runtime state
    public int Stacks { get; set; }
    public int Cooldown { get; set; }
    public int CurrentPhase { get; set; }
    public float CurrentHP { get; set; }
    public float MaxHP { get; set; }
    public int TurnsActive { get; set; }
    
    // IEffectSource implementation
    public string Name => Data.bossName;
    public bool IsOnCooldown => Cooldown > 0;
    
    public BossInstance(BossData data)
    {
        Data = data;
        Stacks = 0;
        Cooldown = 0;
        CurrentPhase = 0;
        MaxHP = 100f;  // Default, can be set by boss data
        CurrentHP = MaxHP;
        TurnsActive = 0;
    }
    
    /// <summary>
    /// Apply all effects from this boss.
    /// </summary>
    public void ApplyEffects(GameContext context, ref GameModifiers mods)
    {
        if (Data == null || Data.effects == null) return;
        
        foreach (var effect in Data.effects)
        {
            if (effect != null)
            {
                effect.Apply(this, context, ref mods);
            }
        }
    }
    
    /// <summary>
    /// Called at the end of each turn.
    /// </summary>
    public void OnTurnEnd()
    {
        if (Cooldown > 0) Cooldown--;
        TurnsActive++;
    }
    
    /// <summary>
    /// Get the trial name for this boss.
    /// </summary>
    public string GetTrialName()
    {
        return Data.GetTrialName();
    }
}
