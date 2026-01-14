using UnityEngine;

/// <summary>
/// Central orchestrator for all effects.
/// Combines RelicManager and BossManager results.
/// </summary>
public class EffectSystem : MonoBehaviour
{
    public static EffectSystem Instance { get; private set; }
    
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    /// <summary>
    /// Calculate all modifiers from relics and boss.
    /// </summary>
    public GameModifiers CalculateModifiers(GameContext context)
    {
        GameModifiers mods = GameModifiers.Default();
        
        // Apply relic effects
        if (RelicManager.Instance != null)
        {
            RelicManager.Instance.ApplyEffects(context, ref mods);
        }
        
        // Apply boss effect
        if (BossManager.Instance != null)
        {
            BossManager.Instance.ApplyEffect(context, ref mods);
        }
        
        return mods;
    }
    
    /// <summary>
    /// Shorthand for rate calculation.
    /// </summary>
    public GameModifiers GetRateModifiers(Match3Game game)
    {
        return CalculateModifiers(GameContext.ForRateCalculation(game));
    }
    
    /// <summary>
    /// Shorthand for score calculation.
    /// </summary>
    public GameModifiers GetScoreModifiers(Match3Game game)
    {
        return CalculateModifiers(GameContext.ForScoreCalculation(game));
    }
}
</Parameter>
<parameter name="Complexity">4
