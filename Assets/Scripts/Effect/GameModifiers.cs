/// <summary>
/// Container for all modifications from relics and bosses.
/// Extend this struct as needed when adding new effect types.
/// </summary>
[System.Serializable]
public struct GameModifiers
{
    // Rate modifiers (additive)
    public float mightRateBonus;
    public float blessingRateBonus;
    public float shardRateBonus;
    public float furyRateBonus;
    public float mirrorRateBonus;
    public float totemRateBonus;
    public float blightRateReduction;
    
    // Stat bonuses (additive)
    public float bonusMight;
    public float bonusBlessing;
    
    // Amount multipliers (multiplicative)
    public float mightAmountMult;
    public float blessingAmountMult;
    public float shardAmountMult;
    
    // Fury/Mirror boost (additive)
    public float furyMultBonus;
    public float mirrorMultBonus;
    
    // Totem passive (additive)
    public float totemMightBonus;
    public float totemBlessingBonus;
    
    // Resource bonuses (additive)
    public int flowBonus;
    public int whirlBonus;
    public int startingShards;
    
    // Cascade/Combo bonuses (additive)
    public float comboMultBonus;
    public float cascadeMightBonus;
    public float cascadeBlessingBonus;
    
    // Match length bonus (additive)
    public float matchLengthBonus;
    
    // Action blockers
    public bool blockWhirl;
    
    // Initialize with default values
    public static GameModifiers Default()
    {
        return new GameModifiers
        {
            // Multipliers start at 1 (no change)
            mightAmountMult = 1f,
            blessingAmountMult = 1f,
            shardAmountMult = 1f
        };
    }
}

