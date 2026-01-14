using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Static configuration for a relic.
/// Contains effect behaviors that are applied via RelicInstance.
/// </summary>
[CreateAssetMenu(menuName = "Braed/Relic")]
public class RelicData : ScriptableObject
{
    [Header("Basic Info")]
    public string relicName;
    [TextArea(2, 4)]
    public string description;
    public Sprite icon;
    public int price;
    
    [Header("Effects")]
    [Tooltip("List of effect behaviors this relic applies")]
    public List<EffectBehavior> effects = new List<EffectBehavior>();
    
    /// <summary>
    /// Create a runtime instance of this relic.
    /// </summary>
    public RelicInstance CreateInstance()
    {
        return new RelicInstance(this);
    }
}
