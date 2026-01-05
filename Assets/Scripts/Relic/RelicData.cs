using UnityEngine;

[CreateAssetMenu(menuName = "Braed/Relic")]
public class RelicData : ScriptableObject
{
    public string relicName;
    public string description;
    public Sprite icon;
    public int price;

    // Passive bonuses
    public float bonusBlessing;
    public float bonusMight;
}
