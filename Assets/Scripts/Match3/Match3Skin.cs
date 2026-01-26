using UnityEngine;
using Unity.Mathematics;
using System.Collections;

using static Unity.Mathematics.math;
using TMPro;
using UnityEngine.UI;

public class Match3Skin : MonoBehaviour
{
    [SerializeField] 
    Transform tileContainer;  
    [SerializeField]
    private GameObject gamePanel;
    [SerializeField]
    private Shop shop;
    public Button proceedButton;

    [SerializeField, Range(0.1f, 20f)]
    float dropSpeed = 8f;

    [SerializeField, Range(0f, 10f)]
    float newDropOffset = 2f;

    [SerializeField]
    FloatingScore floatingScorePrefab;

    [SerializeField]
    Tile[] tilePrefabs;

    [SerializeField]
    Match3Game game;

    [SerializeField]
    RelicDatabase relicDatabase;

    [SerializeField, Range(0.1f, 1f)]
    float dragThreshold = 0.5f;

    Grid2D<Tile> tiles;
    float2 tileOffset;

    [SerializeField]
    TileSwapper tileSwapper;

    float busyDuration;

    float floatingScoreZ;

    // Score animation
    private float displayedScore = 0f;
    private float displayedMight = 0f;
    private float displayedBlessing = 0f;
    private Coroutine scoreAnimCoroutine;
    private Coroutine mightAnimCoroutine;
    private Coroutine blessingAnimCoroutine;

    [SerializeField]
    TextMeshPro totalScoreText;

    [SerializeField]
    TextMeshPro mightText;

    [SerializeField]
    TextMeshPro blessingText;

    [SerializeField]
    TextMeshPro shardText;

    [SerializeField]
    TextMeshPro flowText;

    [SerializeField]
    TextMeshPro whirlText;

    [SerializeField]
    TextMeshPro requiredText;

    [SerializeField]
    TextMeshPro trialNameText;

    [SerializeField]
    TextMeshPro bossDescriptionText;

    private void Awake()
    {
        gamePanel.SetActive(false);
        mightText.color = new Color(0.5f, 0f, 0f);            // dark red-ish
        blessingText.color = new Color(0f, 0.8f, 0.6f);       // aquamarine-ish
        shardText.color = Color.yellow;                       // built-in
        flowText.color = new Color(0.941f, 0.973f, 1f);       // aliceblue-ish (#F0F8FF)
        whirlText.color = new Color(1f, 0.647f, 0f);          // orange-ish (#FFA500)
    }

    private bool _isPlaying = false;
    public bool isPlaying => _isPlaying;
    public bool isBusy => false;
    public bool IsBusy => busyDuration > 0f;
    public void DoWork() {
        if (busyDuration > 0f)
        {
            tileSwapper.Update();
            busyDuration -= Time.deltaTime;
            if (busyDuration > 0f)
            {
                return;
            }
        }

        if (game.HasMatches)
        {
            ProcessMatches();
        }

        else if (game.NeedsFilling)
        {
            DropTiles();
        }
    }

    string FormatSmart(float value)
    {
        // Integer when whole, 1 decimal when fractional
        return Mathf.Approximately(value % 1f, 0f)
            ? ((int)value).ToString()
            : value.ToString("0.0");
    }

    void ProcessMatches()
    {
        game.ProcessMatches();

        for (int i = 0; i < game.ClearedTileCoordinates.Count; i++)
        {
            int2 c = game.ClearedTileCoordinates[i];
            busyDuration = Mathf.Max(tiles[c].Disappear(), busyDuration);
            tiles[c] = null;
        }

        ResetUI();

        for (int i = 0; i < game.Scores.Count; i++)
        {
            SingleScore score = game.Scores[i];
            floatingScorePrefab.Show(
                new Vector3(
                    score.position.x + tileOffset.x,
                    score.position.y + tileOffset.y,
                    floatingScoreZ
                ),
                score.value,
                score.color
            );
            floatingScoreZ = floatingScoreZ <= -0.02f ? 0f : floatingScoreZ - 0.001f;
        }
    }
    public bool EvaluateDrag(Vector3 start, Vector3 end)
    {
        float2 a = ScreenToTileSpace(start), b = ScreenToTileSpace(end);
        //Debug.Log($"Start: {a}, End: {b}");

        var move = new Move(
            (int2)floor(a), (b - a) switch
            {
                var d when d.x > dragThreshold => MoveDirection.Right,
                var d when d.x < -dragThreshold => MoveDirection.Left,
                var d when d.y > dragThreshold => MoveDirection.Up,
                var d when d.y < -dragThreshold => MoveDirection.Down,
                _ => MoveDirection.None
            }
        );
        if (
            move.IsValid() && tiles.AreValidCoordinates(move.From) && tiles.AreValidCoordinates(move.To)
            )
        {
            DoMove(move);
            return false;
        }
        return true;
    }

    public void StartNewGame()
    {
        _isPlaying = true;
        gamePanel.SetActive(true);
        busyDuration = 0f;
        displayedScore = 0f;  // Reset for new game
        game.StartNewGame();
        requiredText.SetText("Required: "+ game.RequireScore);
        
        // Display trial name
        if (trialNameText != null)
        {
            trialNameText.SetText(game.GetTrialName());
        }
        
        // Display boss description (Trial 3 only)
        if (bossDescriptionText != null)
        {
            if (game.currentBoss != null)
            {
                bossDescriptionText.gameObject.SetActive(true);
                bossDescriptionText.SetText(game.currentBoss.description);
            }
            else
            {
                bossDescriptionText.gameObject.SetActive(false);
            }
        }
        
        ResetUI();
        tileOffset = -0.5f * (float2)(game.Size - 1);
        if (tiles.IsUndefined)
        {
            tiles = new Grid2D<Tile>(game.Size);
        }
        else
        {
            for (int y = 0; y < tiles.SizeY; y++)
            {
                for (int x = 0; x < tiles.SizeX; x++)
                {
                    tiles[x, y].Despawn();
                    tiles[x, y] = null;
                }
            }
        }
        for (int y = 0; y < tiles.SizeY; y++)
        {
            for (int x = 0; x < tiles.SizeX; x++)
            {
                tiles[x, y] = SpawnTile(game[x, y], x, y);
            }
        }
    }
    Tile SpawnTile(TileState t, float x, float y) =>
    tilePrefabs[(int)t - 1].Spawn(
        new Vector3(x + tileOffset.x, y + tileOffset.y),
        tileContainer
    );

    public void ResetGrid()
    {
        for (int y = 0; y < tiles.SizeY; y++)
        {
            for (int x = 0; x < tiles.SizeX; x++)
            {
                tiles[x, y].Despawn();
                tiles[x, y] = null;
            }
        }

        game.ResetGrid();
        whirlText.SetText("Whirl: {0}", game.whirl);

        for (int y = 0; y < tiles.SizeY; y++)
        {
            for (int x = 0; x < tiles.SizeX; x++)
            {
                tiles[x, y] = SpawnTile(game[x, y], x, y);
            }
        }
    }

    void DoMove(Move move)
    {
        if (game.flow != 0)
        {
            bool success = game.TryMove(move);
            Tile a = tiles[move.From], b = tiles[move.To];
            busyDuration = tileSwapper.Swap(a, b, !success);
            if (success)
            {
                tiles[move.From] = b;
                tiles[move.To] = a;
                game.flow--;
                flowText.SetText("Flow: {0}", game.flow);
            }
        }
    }

    float2 ScreenToTileSpace(Vector3 screenPosition)
    {
        Ray ray = Camera.main.ScreenPointToRay(screenPosition);
        Vector3 p = ray.origin - ray.direction * (ray.origin.z / ray.direction.z);
        return float2(p.x - tileOffset.x + 0.5f, p.y - tileOffset.y + 0.5f);
    }

    void DropTiles()
    {
        game.DropTiles();

        for (int i = 0; i < game.DroppedTiles.Count; i++)
        {
            TileDrop drop = game.DroppedTiles[i];
            Tile tile;
            if (drop.fromY < tiles.SizeY)
            {
                tile = tiles[drop.coordinates.x, drop.fromY];
            }
            else
            {
                tile = SpawnTile(game[drop.coordinates], drop.coordinates.x, drop.fromY + newDropOffset);
            }
            tiles[drop.coordinates] = tile;
            busyDuration = Mathf.Max(
                tile.Fall(drop.coordinates.y + tileOffset.y, dropSpeed), busyDuration
            );
        }
    }

    [SerializeField] private GameObject gameOver;
    [SerializeField] private GameOverUI gameOverUI;

    public void Cast()
    {
        int result = game.Cast();
        
        // Animate score up, might and blessing down to 0
        AnimateScoreTo(game.TotalScore);
        AnimateMightTo(0);
        AnimateBlessingTo(0);
        
        // Reset actual game values after cast
        game.ResetMightBlessing();
        
        if (result == 1) // Passed
        {
            // Wait for animation before opening shop
            StartCoroutine(OpenShopAfterDelay());
        }
        else if (result == -1) // Game Over
        {
            // Wait for animation before showing game over
            StartCoroutine(ShowGameOverAfterDelay());
        }
    }

    private IEnumerator ShowGameOverAfterDelay()
    {
        // Wait for score animation (1s) + extra viewing time (1.5s)
        yield return new WaitForSeconds(2.5f);
        
        gameOver.SetActive(true);
        gameOverUI.ShowGameOver(game.TotalScore, game.cycle, game.trial);
    }

    private IEnumerator OpenShopAfterDelay()
    {
        // Wait for score animation (1s) + extra viewing time (1.5s)
        yield return new WaitForSeconds(2.5f);
        
        ResetUI();
        gamePanel.SetActive(false);
        
        // Auto-save after completing trial (player entering shop)
        SaveManager.SaveGame(game, relicDatabase, true);
        
        shop.OpenShop();
    }

    public void Proceed()
    {
        shop.CloseShop();
    }

    public void RestartGame()
    {
        gamePanel.SetActive(true);
        if (RelicManager.Instance != null) RelicManager.Instance.ClearAllRelics();
        Data.Instance.Shard = 0;
        game.trial = 1;
        game.cycle = 1;
        StartNewGame();
        ResetUI();
    }

    private void ResetUI()
    {
        // Animate score count-up
        AnimateScoreTo(game.TotalScore);
        
        // Sync displayed values with actual game values
        displayedMight = game.Might;
        displayedBlessing = game.Blessing;
        
        mightText.SetText("Might: " + FormatSmart(game.Might));
        blessingText.SetText("Blessing: " + FormatSmart(game.Blessing));
        shardText.SetText("Shard: {0}", Data.Instance.Shard);
        flowText.SetText("Flow: {0}", game.flow);
        whirlText.SetText("Whirl: {0}", game.whirl);
    }

    private void AnimateScoreTo(float targetScore)
    {
        if (scoreAnimCoroutine != null)
        {
            StopCoroutine(scoreAnimCoroutine);
        }
        scoreAnimCoroutine = StartCoroutine(AnimateScoreCoroutine(targetScore));
    }

    private IEnumerator AnimateScoreCoroutine(float targetScore)
    {
        float startScore = displayedScore;
        float duration = 1f;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);
            // Ease out for smooth deceleration
            t = 1f - (1f - t) * (1f - t);
            displayedScore = Mathf.Lerp(startScore, targetScore, t);
            totalScoreText.SetText("Strike: {0}", Mathf.RoundToInt(displayedScore));
            yield return null;
        }

        displayedScore = targetScore;
        totalScoreText.SetText("Strike: {0}", Mathf.RoundToInt(displayedScore));
    }

    private void AnimateMightTo(float targetMight)
    {
        if (mightAnimCoroutine != null)
        {
            StopCoroutine(mightAnimCoroutine);
        }
        mightAnimCoroutine = StartCoroutine(AnimateMightCoroutine(targetMight));
    }

    private IEnumerator AnimateMightCoroutine(float targetMight)
    {
        float startMight = displayedMight;
        float duration = 1f;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);
            t = 1f - (1f - t) * (1f - t);
            displayedMight = Mathf.Lerp(startMight, targetMight, t);
            mightText.SetText("Might: " + FormatSmart(displayedMight));
            yield return null;
        }

        displayedMight = targetMight;
        mightText.SetText("Might: " + FormatSmart(displayedMight));
    }

    private void AnimateBlessingTo(float targetBlessing)
    {
        if (blessingAnimCoroutine != null)
        {
            StopCoroutine(blessingAnimCoroutine);
        }
        blessingAnimCoroutine = StartCoroutine(AnimateBlessingCoroutine(targetBlessing));
    }

    private IEnumerator AnimateBlessingCoroutine(float targetBlessing)
    {
        float startBlessing = displayedBlessing;
        float duration = 1f;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);
            t = 1f - (1f - t) * (1f - t);
            displayedBlessing = Mathf.Lerp(startBlessing, targetBlessing, t);
            blessingText.SetText("Blessing: " + FormatSmart(displayedBlessing));
            yield return null;
        }

        displayedBlessing = targetBlessing;
        blessingText.SetText("Blessing: " + FormatSmart(displayedBlessing));
    }

    public void GamePanel(bool status)
    {
        gamePanel.SetActive(status);
    }
}
