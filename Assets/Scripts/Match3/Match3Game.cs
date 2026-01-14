using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using static Unity.Mathematics.math;
using Random = UnityEngine.Random;

public class Match3Game : MonoBehaviour
{
    [SerializeField] private Shop shop;

    //Score and Currency
    public float Might
    { get; private set; }
    public float Blessing
    { get; private set; }
    public int Shard
    { get; private set; }

    public float might_rate;
    public float blessing_rate;
    public float shard_rate;
    public float fury_rate;
    public float mirror_rate;
    public float totem_rate;
    public float blight_rate;

    //Turn and Cycle
    public int flow;
    public int whirl;

    //Cycle and Trial
    public int cycle = 1;
    public int trial = 1;

    // Boss System
    [SerializeField] private BossDatabase bossDatabase;
    public BossData currentBoss { get; private set; }

    private void SetRate()
    {
        // Base rates
        might_rate = 0.18f;
        blessing_rate = 0.18f;
        shard_rate = 0.18f;
        fury_rate = 0.14f;
        mirror_rate = 0.10f;
        totem_rate = 0.12f;
        blight_rate = 0.10f;

        // Apply relic rate modifiers
        might_rate += Data.Instance.GetMightRateBonus();
        blessing_rate += Data.Instance.GetBlessingRateBonus();
        shard_rate += Data.Instance.GetShardRateBonus();
        fury_rate += Data.Instance.GetFuryRateBonus();
        mirror_rate += Data.Instance.GetMirrorRateBonus();
        totem_rate += Data.Instance.GetTotemRateBonus();
        blight_rate -= Data.Instance.GetBlightRateReduction();

        // Apply boss effects (Trial 3 only)
        if (trial == 3 && currentBoss != null)
        {
            ApplyBossEffectsToRates();
        }

        // Clamp rates to valid range
        blight_rate = Mathf.Max(0f, blight_rate);
    }

    private void ApplyBossEffectsToRates()
    {
        foreach (BossEffect effect in currentBoss.effects)
        {
            switch (effect.type)
            {
                case BossEffectType.BlightRateBonus:
                    blight_rate += effect.value;
                    break;
                case BossEffectType.MightRateReduction:
                    might_rate -= effect.value;
                    break;
                case BossEffectType.BlessingRateReduction:
                    blessing_rate -= effect.value;
                    break;
            }
        }
    }

    private int GetBossEffectValue(BossEffectType type)
    {
        if (currentBoss == null) return 0;
        foreach (BossEffect effect in currentBoss.effects)
        {
            if (effect.type == type)
                return (int)effect.value;
        }
        return 0;
    }

    public string GetTrialName()
    {
        if (trial == 3 && currentBoss != null)
        {
            return currentBoss.GetTrialName();
        }
        
        return trial switch
        {
            1 => "The Initiate's Trial",
            2 => "The Adept's Trial",
            _ => $"Trial {trial}"
        };
    }

    public bool HasBossEffect(BossEffectType type)
    {
        if (currentBoss == null) return false;
        foreach (BossEffect effect in currentBoss.effects)
        {
            if (effect.type == type) return true;
        }
        return false;
    }


    [SerializeField]
    int2 size = 8;

    Grid2D<TileState> grid;

    List<Match> matches;

    int scoreMultiplier;

    public List<SingleScore> Scores
    { get; private set; }

    public List<int2> ClearedTileCoordinates
    { get; private set; }

    public bool NeedsFilling
    { get; private set; }

    public List<TileDrop> DroppedTiles
    { get; private set; }

    public float TotalScore
    { get; private set; }
    public float RequireScore;

    private void ScaleRequiredScore(int cycle, int trial)
    {
        Debug.Log(cycle + " " + trial);
        // Rebalanced: Cycle 1 starts at 200, grows more gradually
        float baseScore = Mathf.Round(200 * Mathf.Pow(1.75f, cycle - 1) / 50) * 50;
        RequireScore = baseScore * (0.5f * trial + 0.5f);
        
        // Apply boss score multiplier for Trial 3
        if (trial == 3 && currentBoss != null && bossDatabase != null)
        {
            RequireScore *= bossDatabase.GetScoreMultiplier(currentBoss);
        }
    }

    public bool HasMatches => matches.Count > 0;

    public TileState this[int x, int y] => grid[x, y];

    public TileState this[int2 c] => grid[c];

    public int2 Size => size;

    private void Awake()
    {
        cycle = 1;
        trial = 1;
    }

    public void StartNewGame()
    {
        // Select boss for Trial 3
        if (trial == 3 && bossDatabase != null)
        {
            currentBoss = bossDatabase.GetBossForCycle(cycle);
        }
        else
        {
            currentBoss = null;
        }
        
        // Base resources + relic bonuses
        flow = 4 + Data.Instance.GetBonusFlow();
        whirl = 3 + Data.Instance.GetBonusWhirl();
        
        // Apply boss flow reduction
        if (currentBoss != null)
        {
            flow -= GetBossEffectValue(BossEffectType.ReduceFlow);
            flow = Mathf.Max(1, flow); // Minimum 1 flow
        }
        
        TotalScore = 0;
        ScaleRequiredScore(cycle, trial);
        Might = 0;
        Blessing = 0;

        // Apply relic stat bonuses
        Might += Data.Instance.GetTotalBonusMight();
        Blessing += Data.Instance.GetTotalBonusBlessing();

        // Starting shards bonus
        Data.Instance.Shard += Data.Instance.GetStartingShards();

        if (grid.IsUndefined)
        {
            grid = new Grid2D<TileState>(size);
            matches = new List<Match>();
            ClearedTileCoordinates = new();
            DroppedTiles = new();
            Scores = new();
        }
        FillGrid();
    }

    // Method to generate a tile based on appearance rates
    private TileState GenerateTileByRate()
    {
        float randomValue = Random.value;

        // Create cumulative rates for proper threshold checking
        float cumulativeMight = might_rate;
        float cumulativeBlessing = cumulativeMight + blessing_rate;
        float cumulativeShard = cumulativeBlessing + shard_rate;
        float cumulativeFury = cumulativeShard + fury_rate;
        float cumulativeMirror = cumulativeFury + mirror_rate;
        float cumulativeTotem = cumulativeMirror + totem_rate;
        float cumulativeBlight = cumulativeTotem + blight_rate;

        // Normalize the random value to the total rate range
        float totalRate = cumulativeBlight;
        randomValue *= totalRate;

        // Check each tile type against cumulative rates
        if (randomValue < cumulativeMight)
            return TileState.Might;
        else if (randomValue < cumulativeBlessing)
            return TileState.Blessing;
        else if (randomValue < cumulativeShard)
            return TileState.Shard;
        else if (randomValue < cumulativeFury)
            return TileState.Fury;
        else if (randomValue < cumulativeMirror)
            return TileState.Mirror;
        else if (randomValue < cumulativeTotem)
            return TileState.Totem;
        else if (randomValue < cumulativeBlight)
            return TileState.Blight;

        // Default fallback to Might if no rate matches
        return TileState.Might;
    }

    // Alternative method using weighted selection (FIXED)
    private TileState GenerateTileByWeight()
    {
        SetRate();
        // Create weights array (higher values = more likely to appear)
        float[] weights = {
            might_rate,
            blessing_rate,
            shard_rate,
            fury_rate,
            mirror_rate,
            totem_rate,
            blight_rate
        };

        // Calculate total weight
        float totalWeight = 0f;
        for (int i = 0; i < weights.Length; i++)
        {
            totalWeight += weights[i];
        }

        // Prevent division by zero
        if (totalWeight <= 0f)
        {
            return (TileState)Random.Range(1, 8); // Fallback to uniform distribution
        }

        // Generate random value within total weight
        float randomValue = Random.value * totalWeight;

        // Find which tile type this random value corresponds to
        float currentWeight = 0f;
        for (int i = 0; i < weights.Length; i++)
        {
            currentWeight += weights[i];
            if (randomValue <= currentWeight)
            {
                return (TileState)(i + 1); // +1 because TileState.None is 0
            }
        }

        return TileState.Might; // Fallback
    }

    bool FindMatches()
    {
        for (int y = 0; y < size.y; y++)
        {
            TileState start = grid[0, y];
            int length = 1;
            for (int x = 1; x < size.x; x++)
            {
                TileState t = grid[x, y];
                if (t == start)
                {
                    length += 1;
                }
                else
                {
                    if (length >= 3)
                    {
                        matches.Add(new Match(x - length, y, length, true));
                    }
                    start = t;
                    length = 1;
                }
            }
            if (length >= 3)
            {
                matches.Add(new Match(size.x - length, y, length, true));
            }
        }

        for (int x = 0; x < size.x; x++)
        {
            TileState start = grid[x, 0];
            int length = 1;
            for (int y = 1; y < size.y; y++)
            {
                TileState t = grid[x, y];
                if (t == start)
                {
                    length += 1;
                }
                else
                {
                    if (length >= 3)
                    {
                        matches.Add(new Match(x, y - length, length, false));
                    }
                    start = t;
                    length = 1;
                }
            }
            if (length >= 3)
            {
                matches.Add(new Match(x, size.y - length, length, false));
            }
        }

        return HasMatches;
    }

    public void ResetGrid()
    {
        if (whirl > 0)
        {
            whirl--;
            FillGrid();
        }
    }

    private void FillGrid()
    {
        for (int y = 0; y < size.y; y++)
        {
            for (int x = 0; x < size.x; x++)
            {
                TileState a = TileState.None, b = TileState.None;
                HashSet<TileState> forbiddenTiles = new HashSet<TileState>();

                // Check for potential horizontal matches
                if (x > 1)
                {
                    a = grid[x - 1, y];
                    if (a == grid[x - 2, y] && a != TileState.None)
                    {
                        forbiddenTiles.Add(a);
                    }
                }

                // Check for potential vertical matches
                if (y > 1)
                {
                    b = grid[x, y - 1];
                    if (b == grid[x, y - 2] && b != TileState.None)
                    {
                        forbiddenTiles.Add(b);
                    }
                }

                // Generate tile using rate system while avoiding matches
                TileState newTile = TileState.None;
                int attempts = 0;

                // Try to generate a tile that doesn't create immediate matches
                while (attempts < 20)
                {
                    newTile = GenerateTileByWeight();

                    if (!forbiddenTiles.Contains(newTile))
                    {
                        break; // Found a valid tile
                    }
                    attempts++;
                }

                // If we still couldn't find a valid tile, manually pick one
                if (attempts >= 20 || forbiddenTiles.Contains(newTile))
                {
                    // Create a list of all possible tiles and remove forbidden ones
                    List<TileState> availableTiles = new List<TileState>();
                    for (int tileType = 1; tileType <= 7; tileType++)
                    {
                        TileState candidateTile = (TileState)tileType;
                        if (!forbiddenTiles.Contains(candidateTile))
                        {
                            availableTiles.Add(candidateTile);
                        }
                    }

                    if (availableTiles.Count > 0)
                    {
                        newTile = availableTiles[Random.Range(0, availableTiles.Count)];
                    }
                    else
                    {
                        // Shouldn't happen, but safety fallback
                        newTile = TileState.Might;
                    }
                }

                grid[x, y] = newTile;
            }
        }

        // Debug: Count tile distribution
        int[] tileCounts = new int[8];
        for (int y = 0; y < size.y; y++)
        {
            for (int x = 0; x < size.x; x++)
            {
                tileCounts[(int)grid[x, y]]++;
            }
        }
    }

    public bool TryMove(Move move)
    {
        scoreMultiplier = 1;
        grid.Swap(move.From, move.To);
        if (FindMatches())
        {
            return true;
        }
        grid.Swap(move.From, move.To);
        return false;
    }

    public void ProcessMatches()
    {
        ClearedTileCoordinates.Clear();
        Scores.Clear();

        // Get relic bonuses once
        float mightMult = Data.Instance.GetMightAmountMult();
        float blessingMult = Data.Instance.GetBlessingAmountMult();
        float shardMult = Data.Instance.GetShardAmountMult();
        float furyBoost = Data.Instance.GetFuryMultBonus();
        float mirrorBoost = Data.Instance.GetMirrorMultBonus();
        float totemMight = Data.Instance.GetTotemMightBonus();
        float totemBlessing = Data.Instance.GetTotemBlessingBonus();
        float lengthBonus = Data.Instance.GetMatchLengthBonus();
        float cascadeMight = Data.Instance.GetCascadeMightBonus();
        float cascadeBlessing = Data.Instance.GetCascadeBlessingBonus();
        float comboBonus = Data.Instance.GetComboMultBonus();

        // Apply cascade bonuses (for matches beyond first)
        if (scoreMultiplier > 1)
        {
            Might += cascadeMight * (scoreMultiplier - 1);
            Blessing += cascadeBlessing * (scoreMultiplier - 1);
        }

        for (int m = 0; m < matches.Count; m++)
        {
            Match match = matches[m];
            int2 step = match.isHorizontal ? int2(1, 0) : int2(0, 1);
            int2 c = match.coordinates;

            Color color = Color.white;
            TileState tile = grid[c];
            float floating;
            int shardCount;

            // Match length bonus (tiles over 3)
            float lengthExtra = (match.length > 3) ? lengthBonus * (match.length - 3) : 0;

            // Combo multiplier bonus
            float comboMult = 1f + comboBonus * (scoreMultiplier - 1);

            switch (tile)
            {
                case TileState.Might:
                    color = Color.red;
                    // Rebalanced: 5 base (was 10)
                    floating = (5 * match.length + lengthExtra) * scoreMultiplier * mightMult * comboMult;
                    scoreMultiplier++;
                    Might += floating;
                    floatScore(match, step, color, floating);
                    break;
                case TileState.Blessing:
                    color = Color.cyan;
                    // Rebalanced: 2 base (was 1)
                    floating = (2 * match.length + lengthExtra) * scoreMultiplier * blessingMult * comboMult;
                    scoreMultiplier++;
                    Blessing += floating;
                    floatScore(match, step, color, floating);
                    break;
                case TileState.Shard:
                    color = Color.yellow;
                    shardCount = Mathf.RoundToInt((match.length + lengthExtra) * scoreMultiplier * shardMult * comboMult);
                    scoreMultiplier++;
                    Data.Instance.Shard += shardCount;
                    floatScore(match, step, color, shardCount);
                    break;
                case TileState.Fury:
                    color = Color.green;
                    // Rebalanced: 0.05 base mult (was 0.1)
                    floating = 1 + (0.05f + furyBoost) * match.length * scoreMultiplier;
                    scoreMultiplier++;
                    Might *= floating;
                    floatScore(match, step, color, floating);
                    break;
                case TileState.Mirror:
                    color = Color.magenta;
                    // Rebalanced: Same formula as Fury (was length * mult)
                    floating = 1 + (0.1f + mirrorBoost) * match.length * scoreMultiplier;
                    scoreMultiplier++;
                    Blessing *= floating;
                    floatScore(match, step, color, floating);
                    break;
                case TileState.Totem:
                    color = Color.white;
                    // Totem passive from relics
                    if (totemMight > 0 || totemBlessing > 0)
                    {
                        Might += totemMight * match.length;
                        Blessing += totemBlessing * match.length;
                        floatScore(match, step, color, totemMight * match.length);
                    }
                    break;
                case TileState.Blight:
                    color = Color.black;
                    // Rebalanced: Less punishing
                    floating = match.length * scoreMultiplier;
                    scoreMultiplier++;
                    Might = Mathf.Max(Might - floating * 2, 0);
                    Blessing = Mathf.Max(Blessing - floating, 0);
                    floatScore(match, step, color, -floating);
                    break;
                default:
                    Debug.Log("Unknown tile type");
                    break;
            }

            for (int i = 0; i < match.length; c += step, i++)
            {
                if (grid[c] != TileState.None)
                {
                    grid[c] = TileState.None;
                    ClearedTileCoordinates.Add(c);
                }
            }
        }

        matches.Clear();
        NeedsFilling = true;
    }

    private void floatScore(Match match, int2 step, Color color, float floatingScore)
    {
        var score = new SingleScore
        {
            position = match.coordinates + (float2)step * (match.length - 1) * 0.5f,
            value = floatingScore,
            color = color
        };
        Scores.Add(score);
    }

    [SerializeField] private GameObject gameOver;
    [SerializeField] private GameOverUI gameOverUI;

    // Returns: 1 = passed, -1 = game over, 0 = continue
    public int Cast()
    {
        TotalScore += Might * Blessing;  // Accumulate score
        if (TotalScore > RequireScore)
        {
            if (trial == 3)
            {
                trial = 1;
                cycle++;
            }
            else trial++;
            Data.Instance.Shard += flow * 2 + whirl;
            ScaleRequiredScore(cycle, trial);
            return 1; // Passed
        }
        if (flow == 0 && TotalScore < RequireScore)
        {
            return -1; // Game Over
        }
        return 0; // Continue playing
    }

    public void ResetMightBlessing()
    {
        Might = 0;
        Blessing = 0;
    }

    public void DropTiles()
    {
        DroppedTiles.Clear();

        for (int x = 0; x < size.x; x++)
        {
            int holeCount = 0;
            for (int y = 0; y < size.y; y++)
            {
                if (grid[x, y] == TileState.None)
                {
                    holeCount += 1;
                }
                else if (holeCount > 0)
                {
                    grid[x, y - holeCount] = grid[x, y];
                    DroppedTiles.Add(new TileDrop(x, y - holeCount, holeCount));
                }
            }

            for (int h = 1; h <= holeCount; h++)
            {
                // Use rate-based generation for new tiles
                grid[x, size.y - h] = GenerateTileByWeight();
                DroppedTiles.Add(new TileDrop(x, size.y - h, holeCount));
            }
        }

        NeedsFilling = false;
        FindMatches();
    }

    // Method to adjust rates during gameplay (for dynamic difficulty)
    public void AdjustRates(float mightMod, float blessingMod, float shardMod, float furyMod, float mirrorMod, float totemMod, float blightMod)
    {
        might_rate = Mathf.Clamp01(might_rate + mightMod);
        blessing_rate = Mathf.Clamp01(blessing_rate + blessingMod);
        shard_rate = Mathf.Clamp01(shard_rate + shardMod);
        fury_rate = Mathf.Clamp01(fury_rate + furyMod);
        mirror_rate = Mathf.Clamp01(mirror_rate + mirrorMod);
        totem_rate = Mathf.Clamp01(totem_rate + totemMod);
        blight_rate = Mathf.Clamp01(blight_rate + blightMod);
    }
}