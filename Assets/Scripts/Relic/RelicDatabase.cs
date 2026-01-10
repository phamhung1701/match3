using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Braed/RelicDatabase")]
public class RelicDatabase : ScriptableObject
{
    public List<RelicData> allRelics;

    public List<RelicData> GetRandomRelics(int count)
    {
        // TODO: Create a shuffled copy, return first 'count' items
        // Skeleton for you to implement:
        
        List<RelicData> shuffled = new List<RelicData>(allRelics);
        
        // Fisher-Yates shuffle
        for (int i = shuffled.Count - 1; i > 0; i--)
        {
            int randomIndex = Random.Range(0, i + 1);
            // TODO: Swap shuffled[i] and shuffled[randomIndex]
            RelicData temp = shuffled[i];    
            shuffled[i] = shuffled[randomIndex];
            shuffled[randomIndex] = temp;

        }

        // TODO: Return first 'count' items using GetRange
        return shuffled.GetRange(0, count);
    }

    public RelicData GetRelicByName(string name)
    {
        foreach (RelicData relic in allRelics)
        {
            if (relic.relicName == name)
            {
                return relic;
            }
        }
        return null;
    }
}
