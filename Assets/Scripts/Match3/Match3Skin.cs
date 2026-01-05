using UnityEngine;
using Unity.Mathematics;

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

    [SerializeField, Range(0.1f, 1f)]
    float dragThreshold = 0.5f;

    Grid2D<Tile> tiles;
    float2 tileOffset;

    [SerializeField]
    TileSwapper tileSwapper;

    float busyDuration;

    float floatingScoreZ;

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

    private void Awake()
    {
        mightText.color = new Color(0.5f, 0f, 0f);            // dark red-ish
        blessingText.color = new Color(0f, 0.8f, 0.6f);       // aquamarine-ish
        shardText.color = Color.yellow;                       // built-in
        flowText.color = new Color(0.941f, 0.973f, 1f);       // aliceblue-ish (#F0F8FF)
        whirlText.color = new Color(1f, 0.647f, 0f);          // orange-ish (#FFA500)
    }

    public bool isPlaying => true;
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
        return Mathf.Approximately(value % 1f, 0f)
            ? ((int)value).ToString()
            : value.ToString("0.##");
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

        totalScoreText.SetText("Strike: {0}", game.TotalScore);
        mightText.SetText("Might: " + FormatSmart(game.Might));
        blessingText.SetText("Blessing: " + FormatSmart(game.Blessing));
        shardText.SetText("Shard: {0}", game.Shard);

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
        gamePanel.SetActive(true);
        busyDuration = 0f;
        totalScoreText.SetText("Total: 0");
        game.StartNewGame();
        requiredText.SetText("Required: "+ game.RequireScore);
        shardText.SetText("Shard: {0}", game.Shard);
        flowText.SetText("Flow: {0}", game.flow);
        whirlText.SetText("Whirl: {0}", game.whirl);
        mightText.SetText("Might: {0}", game.Might);
        blessingText.SetText("Blessing {0}", game.Blessing);
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

    public void Cast()
    {
        if(game.Cast())
        {
            totalScoreText.SetText("Strike: {0}", game.TotalScore);
            mightText.SetText("Might: " + FormatSmart(game.Might));
            blessingText.SetText("Blessing: " + FormatSmart(game.Blessing));
            shardText.SetText("Shard: {0}", game.Shard);
            gamePanel.SetActive(false);
            shop.OpenShop();
        }
        totalScoreText.SetText("Strike: {0}", game.TotalScore);
    }

    public void Proceed()
    {
        shop.CloseShop();
    }
}
