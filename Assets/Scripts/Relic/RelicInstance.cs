using System.Collections.Generic;

/// <summary>
/// Runtime wrapper for a relic.
/// Holds state like stacks, cooldowns, and references the static RelicData.
/// </summary>
public class RelicInstance : IEffectSource
{
    public RelicData Data { get; private set; }
    
    // Runtime state
    public int Stacks { get; set; }
    public int Cooldown { get; set; }
    public int TurnsActive { get; set; }
    public int MatchesMade { get; set; }
    
    // IEffectSource implementation
    public string Name => Data.relicName;
    public bool IsOnCooldown => Cooldown > 0;
    
    public RelicInstance(RelicData data)
    {
        Data = data;
        Stacks = 0;
        Cooldown = 0;
        TurnsActive = 0;
        MatchesMade = 0;
    }
    
    /// <summary>
    /// Apply all effects from this relic.
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
    /// Called at the end of each turn to update cooldowns.
    /// </summary>
    public void OnTurnEnd()
    {
        if (Cooldown > 0) Cooldown--;
        TurnsActive++;
    }
    
    /// <summary>
    /// Called when a match is made.
    /// </summary>
    public void OnMatchMade()
    {
        MatchesMade++;
    }
    
    /// <summary>
    /// Reset state for new trial.
    /// </summary>
    public void ResetForNewTrial()
    {
        Cooldown = 0;
        TurnsActive = 0;
        MatchesMade = 0;
        // Stacks persist across trials
    }
}
