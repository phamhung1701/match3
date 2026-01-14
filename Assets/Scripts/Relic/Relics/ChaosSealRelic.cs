using UnityEngine;

/// <summary>
/// Example relic: Blocks the ability to use Whirl.
/// Demonstrates how to create a custom relic with unique behavior.
/// </summary>
[CreateAssetMenu(menuName = "Braed/Relics/Chaos Seal")]
public class ChaosSealRelic : RelicData
{
    public override void ApplyEffect(GameContext context, ref GameModifiers mods)
    {
        // Block whirl ability
        mods.blockWhirl = true;
    }
}

