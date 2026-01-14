using System;
using UnityEngine;

/// <summary>
/// Manages current boss instance.
/// Handles selection, clearing, and effect application.
/// </summary>
public class BossManager : MonoBehaviour
{
    public static BossManager Instance { get; private set; }
    
    [SerializeField] private BossDatabase bossDatabase;
    
    public BossInstance CurrentBoss { get; private set; }
    public bool HasBoss => CurrentBoss != null;
    
    public event Action<BossInstance> OnBossSelected;
    public event Action OnBossCleared;
    
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
    
    /// <summary>
    /// Select a random boss for the current cycle.
    /// </summary>
    public void SelectBossForCycle(int cycle)
    {
        if (bossDatabase == null)
        {
            Debug.LogWarning("BossDatabase not assigned!");
            return;
        }
        
        BossData data = bossDatabase.GetBossForCycle(cycle);
        if (data != null)
        {
            CurrentBoss = data.CreateInstance();
            OnBossSelected?.Invoke(CurrentBoss);
        }
    }
    
    /// <summary>
    /// Clear current boss.
    /// </summary>
    public void ClearBoss()
    {
        if (CurrentBoss != null)
        {
            CurrentBoss = null;
            OnBossCleared?.Invoke();
        }
    }
    
    /// <summary>
    /// Get the trial display name.
    /// </summary>
    public string GetTrialName(int trial)
    {
        if (trial == 3 && CurrentBoss != null)
        {
            return CurrentBoss.GetTrialName();
        }
        
        return trial switch
        {
            1 => "The Initiate's Trial",
            2 => "The Adept's Trial",
            _ => $"Trial {trial}"
        };
    }
    
    /// <summary>
    /// Get the score multiplier for current boss.
    /// </summary>
    public float GetScoreMultiplier()
    {
        if (CurrentBoss == null || bossDatabase == null) return 1f;
        return bossDatabase.GetScoreMultiplier(CurrentBoss.Data);
    }
    
    /// <summary>
    /// Apply boss effect.
    /// Called by EffectSystem.
    /// </summary>
    public void ApplyEffect(GameContext context, ref GameModifiers mods)
    {
        if (CurrentBoss != null)
        {
            CurrentBoss.ApplyEffects(context, ref mods);
        }
    }
    
    /// <summary>
    /// Called at end of turn.
    /// </summary>
    public void OnTurnEnd()
    {
        CurrentBoss?.OnTurnEnd();
    }
}
