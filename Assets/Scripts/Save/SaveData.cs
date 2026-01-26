using System.Collections.Generic;

[System.Serializable]
public class SaveData
{
    public List<string> relicNames = new List<string>();
    public List<int> relicStacks = new List<int>();  // Stacks for each relic
    public int shards;
    public int cycle;
    public int trial;
    public bool isInShop;
}
