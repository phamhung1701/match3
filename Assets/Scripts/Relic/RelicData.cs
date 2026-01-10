using UnityEngine;

[CreateAssetMenu(menuName = "Braed/Relic")]
public class RelicData : ScriptableObject
{
    [Header("Basic Info")]
    public string relicName;
    public string description;
    public Sprite icon;
    public int price;

    [Header("Stat Bonuses")]
    public float bonusMight;
    public float bonusBlessing;

    [Header("Rate Modifiers")]
    public float mightRateBonus;
    public float blessingRateBonus;
    public float shardRateBonus;
    public float furyRateBonus;
    public float mirrorRateBonus;
    public float totemRateBonus;
    public float blightRateReduction;

    [Header("Amount Multipliers")]
    public float mightAmountMult = 1f;
    public float blessingAmountMult = 1f;
    public float shardAmountMult = 1f;

    [Header("Fury/Mirror Boost")]
    public float furyMultBonus;
    public float mirrorMultBonus;

    [Header("Totem Passive")]
    public float totemMightBonus;
    public float totemBlessingBonus;

    [Header("Resource Bonuses")]
    public int bonusFlow;
    public int bonusWhirl;
    public int startingShards;

    [Header("Cascade/Combo Bonuses")]
    public float comboMultBonus;
    public float cascadeMightBonus;
    public float cascadeBlessingBonus;

    [Header("Match Length Bonus")]
    public float matchLengthBonus;
}
