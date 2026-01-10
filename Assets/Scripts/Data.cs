using System;
using System.Collections.Generic;
using UnityEngine;

public class Data : MonoBehaviour
{
    public static Data Instance { get; private set; }

    public List<RelicData> relics = new List<RelicData>();
    public int Shard = 0;
    public static Action OnRelicsChanged;
    
    // Ensure this runs before other scripts
    void OnEnable()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }
    
    public void addRelic(RelicData data)
    {
        relics.Add(data);
        OnRelicsChanged?.Invoke();
    }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }
    }

    #region Stat Bonuses
    public float GetTotalBonusMight()
    {
        float total = 0;
        foreach (RelicData r in relics) total += r.bonusMight;
        return total;
    }

    public float GetTotalBonusBlessing()
    {
        float total = 0;
        foreach (RelicData r in relics) total += r.bonusBlessing;
        return total;
    }
    #endregion

    #region Rate Modifiers
    public float GetMightRateBonus()
    {
        float total = 0;
        foreach (RelicData r in relics) total += r.mightRateBonus;
        return total;
    }

    public float GetBlessingRateBonus()
    {
        float total = 0;
        foreach (RelicData r in relics) total += r.blessingRateBonus;
        return total;
    }

    public float GetShardRateBonus()
    {
        float total = 0;
        foreach (RelicData r in relics) total += r.shardRateBonus;
        return total;
    }

    public float GetFuryRateBonus()
    {
        float total = 0;
        foreach (RelicData r in relics) total += r.furyRateBonus;
        return total;
    }

    public float GetMirrorRateBonus()
    {
        float total = 0;
        foreach (RelicData r in relics) total += r.mirrorRateBonus;
        return total;
    }

    public float GetTotemRateBonus()
    {
        float total = 0;
        foreach (RelicData r in relics) total += r.totemRateBonus;
        return total;
    }

    public float GetBlightRateReduction()
    {
        float total = 0;
        foreach (RelicData r in relics) total += r.blightRateReduction;
        return total;
    }
    #endregion

    #region Amount Multipliers
    public float GetMightAmountMult()
    {
        float mult = 1f;
        foreach (RelicData r in relics) mult *= r.mightAmountMult;
        return mult;
    }

    public float GetBlessingAmountMult()
    {
        float mult = 1f;
        foreach (RelicData r in relics) mult *= r.blessingAmountMult;
        return mult;
    }

    public float GetShardAmountMult()
    {
        float mult = 1f;
        foreach (RelicData r in relics) mult *= r.shardAmountMult;
        return mult;
    }
    #endregion

    #region Fury/Mirror Boost
    public float GetFuryMultBonus()
    {
        float total = 0;
        foreach (RelicData r in relics) total += r.furyMultBonus;
        return total;
    }

    public float GetMirrorMultBonus()
    {
        float total = 0;
        foreach (RelicData r in relics) total += r.mirrorMultBonus;
        return total;
    }
    #endregion

    #region Totem Passive
    public float GetTotemMightBonus()
    {
        float total = 0;
        foreach (RelicData r in relics) total += r.totemMightBonus;
        return total;
    }

    public float GetTotemBlessingBonus()
    {
        float total = 0;
        foreach (RelicData r in relics) total += r.totemBlessingBonus;
        return total;
    }
    #endregion

    #region Resource Bonuses
    public int GetBonusFlow()
    {
        int total = 0;
        foreach (RelicData r in relics) total += r.bonusFlow;
        return total;
    }

    public int GetBonusWhirl()
    {
        int total = 0;
        foreach (RelicData r in relics) total += r.bonusWhirl;
        return total;
    }

    public int GetStartingShards()
    {
        int total = 0;
        foreach (RelicData r in relics) total += r.startingShards;
        return total;
    }
    #endregion

    #region Cascade/Combo Bonuses
    public float GetComboMultBonus()
    {
        float total = 0;
        foreach (RelicData r in relics) total += r.comboMultBonus;
        return total;
    }

    public float GetCascadeMightBonus()
    {
        float total = 0;
        foreach (RelicData r in relics) total += r.cascadeMightBonus;
        return total;
    }

    public float GetCascadeBlessingBonus()
    {
        float total = 0;
        foreach (RelicData r in relics) total += r.cascadeBlessingBonus;
        return total;
    }
    #endregion

    #region Match Length Bonus
    public float GetMatchLengthBonus()
    {
        float total = 0;
        foreach (RelicData r in relics) total += r.matchLengthBonus;
        return total;
    }
    #endregion
}
