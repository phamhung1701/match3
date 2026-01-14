using UnityEngine;

/// <summary>
/// Abstract base for all relics.
/// Every relic must inherit and implement ApplyEffect.
/// </summary>
public abstract class RelicData : ScriptableObject
{
    [Header("Basic Info")]
    public string relicName;
    [TextArea(2, 4)]
    public string description;
    public Sprite icon;
    public int price;
    
    /// <summary>
    /// Apply this relic's effects to the modifiers.
    /// Must be implemented by each relic subclass.
    /// </summary>
    public abstract void ApplyEffect(GameContext context, ref GameModifiers mods);
}
