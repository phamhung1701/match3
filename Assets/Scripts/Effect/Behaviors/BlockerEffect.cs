using UnityEngine;

/// <summary>
/// Blocker type for blocking effects.
/// </summary>
public enum BlockerType
{
    BlockWhirl,
    // Add more as needed
}

/// <summary>
/// Effect that blocks certain abilities.
/// </summary>
[CreateAssetMenu(menuName = "Braed/Effects/Blocker")]
public class BlockerEffect : EffectBehavior
{
    [Header("Block Settings")]
    public BlockerType blockerType;
    
    public override void Apply(IEffectSource instance, GameContext context, ref GameModifiers mods)
    {
        switch (blockerType)
        {
            case BlockerType.BlockWhirl:
                mods.blockWhirl = true;
                break;
        }
    }
}
