using System.Collections.Generic;
using UnityEngine;

public class Data : MonoBehaviour
{
    public static Data Instance { get; private set; }

    public List<RelicData> relics = new List<RelicData>();
    public int Shard = 0;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public float GetTotalBonusBlessing()
    {
        float totalBonus = 0;
        foreach (RelicData relic in relics)
        {
            totalBonus += relic.bonusBlessing;
        }

        return totalBonus;
    }

    public float GetTotalBonusMight()
    {
        float totalBonus = 0;
        foreach (RelicData relic in relics)
        {
            totalBonus += relic.bonusMight;
        }

        return totalBonus;
    }
}
