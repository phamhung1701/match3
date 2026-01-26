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

        // Save relic names and stacks from RelicManager
        if (RelicManager.Instance != null)
        {
            foreach (var instance in RelicManager.Instance.OwnedRelics)
            {
                data.relicNames.Add(instance.Data.relicName);
                data.relicStacks.Add(instance.Stacks);
            }
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

    /// <summary>
    /// Restore relics from save data.
    /// Call this after loading save data.
    /// </summary>
    public static void RestoreRelics(SaveData data, RelicDatabase relicDatabase)
    {
        if (data == null || RelicManager.Instance == null) return;
        
        RelicManager.Instance.ClearAllRelics();
        
        for (int i = 0; i < data.relicNames.Count; i++)
        {
            RelicData relicData = relicDatabase.GetRelicByName(data.relicNames[i]);
            if (relicData != null)
            {
                var instance = RelicManager.Instance.AddRelic(relicData);
                
                // Restore stacks if available
                if (instance != null && i < data.relicStacks.Count)
                {
                    instance.Stacks = data.relicStacks[i];
                }
            }
        }
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
