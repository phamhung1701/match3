using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Boss Database", menuName = "Braed/Boss Database")]
public class BossDatabase : ScriptableObject
{
    [SerializeField] private List<BossData> allBosses = new List<BossData>();
    
    // Base score multiplier for all bosses
    public float baseScoreMultiplier = 1.5f;
    
    // Finality boss multiplier (4x the base)
    public float finalityMultiplier = 4f;
    
    /// <summary>
    /// Get a random boss valid for the current cycle
    /// </summary>
    public BossData GetBossForCycle(int cycle)
    {
        List<BossData> validBosses = new List<BossData>();
        
        foreach (BossData boss in allBosses)
        {
            if (boss.minCycle <= cycle)
            {
                validBosses.Add(boss);
            }
        }
        
        if (validBosses.Count == 0)
        {
            return null;
        }
        
        return validBosses[Random.Range(0, validBosses.Count)];
    }
    
    /// <summary>
    /// Get the score multiplier for a boss
    /// </summary>
    public float GetScoreMultiplier(BossData boss)
    {
        if (boss == null) return 1f;
        return boss.differentModifier ? baseScoreMultiplier * finalityMultiplier : baseScoreMultiplier;
    }
}
