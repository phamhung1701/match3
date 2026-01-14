using UnityEngine;

/// <summary>
/// Snapshot of current game state passed to all effect calculations.
/// Extend this class as needed when adding new effect types.
/// </summary>
public class GameContext
{
    // Current phase flags
    public bool isRateCalculation;      // Calculating tile spawn rates
    public bool isScoreCalculation;     // Calculating score on Cast
    public bool isMatchProcessing;      // Processing a match
    public bool isShardGain;            // Gaining shards
    public bool isFlowConsume;          // Consuming flow
    public bool isWhirlAttempt;         // Attempting to whirl
    
    // Match info (when isMatchProcessing)
    public TileState matchedTile;
    public int matchLength;
    
    // Current stats (for reference)
    public float currentMight;
    public float currentBlessing;
    public int currentFlow;
    public int currentWhirl;
    
    // Trial info
    public int cycle;
    public int trial;
    
    // Reference to game (for advanced effects)
    public Match3Game game;
    
    public static GameContext ForRateCalculation(Match3Game game)
    {
        return new GameContext
        {
            isRateCalculation = true,
            game = game,
            cycle = game.cycle,
            trial = game.trial
        };
    }
    
    public static GameContext ForScoreCalculation(Match3Game game)
    {
        return new GameContext
        {
            isScoreCalculation = true,
            game = game,
            currentMight = game.Might,
            currentBlessing = game.Blessing,
            cycle = game.cycle,
            trial = game.trial
        };
    }
    
    public static GameContext ForMatch(Match3Game game, TileState tile, int length)
    {
        return new GameContext
        {
            isMatchProcessing = true,
            matchedTile = tile,
            matchLength = length,
            game = game
        };
    }
}
