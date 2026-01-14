using UnityEngine;

/// <summary>
/// Condition types for conditional effects.
/// </summary>
public enum ConditionType
{
    // Resource conditions
    NoWhirlLeft,
    NoFlowLeft,
    LowFlow,           // Flow <= threshold
    HighFlow,          // Flow >= threshold
    
    // Stack conditions
    HasStacks,         // Stacks > 0
    MaxStacks,         // Stacks >= threshold
    
    // Match conditions
    MatchLength5Plus,
    MatchLength4Plus,
    
    // Cooldown conditions
    NotOnCooldown,
    
    // Trial conditions
    IsTrial3,
    IsCycle2Plus
}

/// <summary>
/// Conditional effect that only applies if a condition is met.
/// Wraps another effect behavior.
/// </summary>
[CreateAssetMenu(menuName = "Braed/Effects/Conditional")]
public class ConditionalEffect : EffectBehavior
{
    [Header("Condition")]
    public ConditionType condition;
    public int threshold;  // For conditions that need a value
    
    [Header("Effect to Apply")]
    public EffectBehavior wrappedEffect;
    
    public override void Apply(IEffectSource instance, GameContext context, ref GameModifiers mods)
    {
        // Check condition
        if (!CheckCondition(instance, context)) return;
        
        // Apply wrapped effect
        if (wrappedEffect != null)
        {
            wrappedEffect.Apply(instance, context, ref mods);
        }
    }
    
    private bool CheckCondition(IEffectSource instance, GameContext context)
    {
        return condition switch
        {
            ConditionType.NoWhirlLeft => context.currentWhirl <= 0,
            ConditionType.NoFlowLeft => context.currentFlow <= 0,
            ConditionType.LowFlow => context.currentFlow <= threshold,
            ConditionType.HighFlow => context.currentFlow >= threshold,
            ConditionType.HasStacks => instance.Stacks > 0,
            ConditionType.MaxStacks => instance.Stacks >= threshold,
            ConditionType.MatchLength5Plus => context.matchLength >= 5,
            ConditionType.MatchLength4Plus => context.matchLength >= 4,
            ConditionType.NotOnCooldown => !instance.IsOnCooldown,
            ConditionType.IsTrial3 => context.trial == 3,
            ConditionType.IsCycle2Plus => context.cycle >= 2,
            _ => true
        };
    }
}
