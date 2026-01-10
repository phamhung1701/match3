using System.IO;
using System.Collections.Generic;
using UnityEngine;

public static class SaveManager
{
    private static string SavePath => Path.Combine(Application.persistentDataPath, "save.json");

    public static void SaveGame(Match3Game game, RelicDatabase relicDatabase, bool isInShop)
    {
        SaveData data = new SaveData
        {
            shards = Data.Instance.Shard,
            cycle = game.cycle,
            trial = game.trial,
            isInShop = isInShop
        };

        // Save relic names for lookup on load
        foreach (RelicData relic in Data.Instance.relics)
        {
            data.relicNames.Add(relic.relicName);
        }

        string json = JsonUtility.ToJson(data, true);
        File.WriteAllText(SavePath, json);
        Debug.Log($"Game saved to {SavePath} (isInShop: {isInShop})");
    }

    public static SaveData LoadGame()
    {
        if (!HasSaveFile())
        {
            Debug.LogWarning("No save file found");
            return null;
        }

        string json = File.ReadAllText(SavePath);
        SaveData data = JsonUtility.FromJson<SaveData>(json);
        Debug.Log($"Game loaded from {SavePath}");
        return data;
    }

    public static bool HasSaveFile()
    {
        return File.Exists(SavePath);
    }

    public static void DeleteSave()
    {
        if (HasSaveFile())
        {
            File.Delete(SavePath);
            Debug.Log("Save file deleted");
        }
    }
}
