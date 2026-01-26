using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Shop : MonoBehaviour
{
    [SerializeField] int slotCount = 3;
    [SerializeField] RelicDatabase relicDatabase;
    
    // UI References
    [SerializeField] GameObject shopPanel;
    [SerializeField] RelicSlotUI[] relicSlots;  
    [SerializeField] TextMeshProUGUI shardText;
    [SerializeField] Button proceedButton;
    [SerializeField] Button rerollButton;

    [SerializeField] Match3Skin skin;
    [SerializeField] Match3Game game;
    private List<RelicData> currentShop;
    private int rerollPrice;

    private void Awake()
    {
        for (int i = 0; i < slotCount; i++)
        {
            relicSlots[i].Initialize(i, BuyRelic);
        }

        proceedButton.onClick.AddListener(CloseShop);
        rerollButton.onClick.AddListener(RerollShop);
    }

    public void OpenShop()
    {
        rerollPrice = 5;
        GenerateShop();
    }

    private void GenerateShop()
    {
        currentShop = relicDatabase.GetRandomRelics(slotCount);

        shopPanel.SetActive(true);

        // Populate each slot with relic data
        for (int i = 0; i < slotCount; i++)
        {
            relicSlots[i].Display(currentShop[i]);
        }

        shardText.SetText("Shards: {0}", Data.Instance.Shard);
    }

    public void RerollShop()
    {
        if (Data.Instance.Shard >= rerollPrice)
        {
            Data.Instance.Shard -= rerollPrice;
            shardText.SetText("Shards: {0}", Data.Instance.Shard);
            GenerateShop();
        }
    }

    public void BuyRelic(int index)
    {
        if (currentShop[index] == null) return;
        if (RelicManager.Instance == null) return;
        
        bool canAfford = Data.Instance.Shard >= currentShop[index].price;
        bool hasSpace = RelicManager.Instance.HasSpace;
        
        if (canAfford && hasSpace)
        {
            RelicManager.Instance.AddRelic(currentShop[index]);
            Data.Instance.Shard -= currentShop[index].price;
            relicSlots[index].MarkAsSold();
            shardText.SetText("Shards: {0}", Data.Instance.Shard);
            currentShop[index] = null;
        }
    }

    public void CloseShop() 
    {
        //Hide shop panel
        shopPanel.SetActive(false);

        // Save when leaving shop (entering gameplay)
        SaveManager.SaveGame(game, relicDatabase, false);

        // Start next trial
        skin.StartNewGame();
    }

    public void SetActiveShop(bool active)
    {
        shopPanel.SetActive(active);
    }

    public bool IsShopOpen => shopPanel.activeSelf;

    public void SellRelic(int index)
    {
        if (RelicManager.Instance == null) return;
        
        var instance = RelicManager.Instance.GetRelicAt(index);
        if (instance == null) return;
        
        int sellPrice = instance.Data.price / 2;
        Data.Instance.Shard += sellPrice;
        RelicManager.Instance.RemoveRelicAt(index);
        shardText.SetText("Shards: {0}", Data.Instance.Shard);
    }
}
