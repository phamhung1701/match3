using UnityEngine;

/// <summary>
/// Abstract base for all effect behaviors.
/// These are reusable building blocks that can be composed to create relics/bosses.
/// </summary>
public abstract class EffectBehavior : ScriptableObject
{
    [Header("Behavior Info")]
    [Tooltip("Description of what this effect does")]
    public string description;
    
    /// <summary>
    /// Apply this effect's modifications.
    /// </summary>
    /// <param name="instance">The relic/boss instance (for accessing state)</param>
    /// <param name="context">Current game state snapshot</param>
    /// <param name="mods">Modifiers to apply changes to</param>
    public abstract void Apply(
        IEffectSource instance,
        GameContext context,
        ref GameModifiers mods
    );
}

/// <summary>
/// Interface for anything that can be the source of effects (RelicInstance, BossInstance).
/// </summary>
public interface IEffectSource
{
    string Name { get; }
    int Stacks { get; set; }
    int Cooldown { get; set; }
    bool IsOnCooldown { get; }
}
