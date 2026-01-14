using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Boss", menuName = "Braed/Boss Data")]
public class BossData : ScriptableObject
{
    [Header("Basic Info")]
    public string bossName;
    [TextArea(2, 4)]
    public string description;  // e.g., "Blight tiles spawn 15% more often"
    public Sprite icon;
    
    [Header("Availability")]
    public int minCycle = 1;  // Earliest cycle this boss can appear

    [Header("Special")]
    public bool differentModifier;
    
    [Header("Effects")]
    public List<BossEffect> effects = new List<BossEffect>();
    
    public string GetTrialName()
    {
        return $"Trial of the {bossName}";
    }
}
