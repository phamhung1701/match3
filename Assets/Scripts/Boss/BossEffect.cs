[System.Serializable]
public enum BossEffectType
{
    BlightRateBonus,      // +X% blight spawn
    MightRateReduction,   // -X% might spawn  
    BlessingRateReduction,// -X% blessing spawn
    DisableWhirl,         // Can't shuffle
    ReduceFlow,           // -X flow at start
    ShardPenalty,         // Shards give X% less
    InvertFury,           // Fury hurts instead of helps
    ReduceMirror,         // Mirror gives X% less bonus
}

[System.Serializable]
public class BossEffect
{
    public BossEffectType type;
    public float value;
}
