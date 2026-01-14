using UnityEngine;

/// <summary>
/// Modifier type for stat effects.
/// </summary>
public enum ModifierStat
{
    // Rate modifiers
    MightRate,
    BlessingRate,
    ShardRate,
    FuryRate,
    MirrorRate,
    TotemRate,
    BlightReduction,
    
    // Stat bonuses
    BonusMight,
    BonusBlessing,
    
    // Amount multipliers
    MightMult,
    BlessingMult,
    ShardMult,
    
    // Resource bonuses
    FlowBonus,
    WhirlBonus,
    StartingShards,
    
    // Special
    FuryMultBonus,
    MirrorMultBonus,
    ComboMultBonus
}

/// <summary>
/// Simple stat modifier effect.
/// Adds or multiplies a stat value.
/// </summary>
[CreateAssetMenu(menuName = "Braed/Effects/Stat Modifier")]
public class StatModifierEffect : EffectBehavior
{
    [Header("Modifier Settings")]
    public ModifierStat stat;
    public float value;
    public bool isPerStack;  // Multiply by instance stacks?
    
    [Header("Phase Filter")]
    public bool onlyDuringRateCalc;
    public bool onlyDuringScoreCalc;
    
    public override void Apply(IEffectSource instance, GameContext context, ref GameModifiers mods)
    {
        // Check phase filters
        if (onlyDuringRateCalc && !context.isRateCalculation) return;
        if (onlyDuringScoreCalc && !context.isScoreCalculation) return;
        
        // Calculate final value
        float finalValue = value;
        if (isPerStack && instance.Stacks > 0)
        {
            finalValue *= instance.Stacks;
        }
        
        // Apply to correct stat
        switch (stat)
        {
            case ModifierStat.MightRate: mods.mightRateBonus += finalValue; break;
            case ModifierStat.BlessingRate: mods.blessingRateBonus += finalValue; break;
            case ModifierStat.ShardRate: mods.shardRateBonus += finalValue; break;
            case ModifierStat.FuryRate: mods.furyRateBonus += finalValue; break;
            case ModifierStat.MirrorRate: mods.mirrorRateBonus += finalValue; break;
            case ModifierStat.TotemRate: mods.totemRateBonus += finalValue; break;
            case ModifierStat.BlightReduction: mods.blightRateReduction += finalValue; break;
            case ModifierStat.BonusMight: mods.bonusMight += finalValue; break;
            case ModifierStat.BonusBlessing: mods.bonusBlessing += finalValue; break;
            case ModifierStat.MightMult: mods.mightAmountMult *= (1 + finalValue); break;
            case ModifierStat.BlessingMult: mods.blessingAmountMult *= (1 + finalValue); break;
            case ModifierStat.ShardMult: mods.shardAmountMult *= (1 + finalValue); break;
            case ModifierStat.FlowBonus: mods.flowBonus += (int)finalValue; break;
            case ModifierStat.WhirlBonus: mods.whirlBonus += (int)finalValue; break;
            case ModifierStat.StartingShards: mods.startingShards += (int)finalValue; break;
            case ModifierStat.FuryMultBonus: mods.furyMultBonus += finalValue; break;
            case ModifierStat.MirrorMultBonus: mods.mirrorMultBonus += finalValue; break;
            case ModifierStat.ComboMultBonus: mods.comboMultBonus += finalValue; break;
        }
    }
}
