using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Shop : MonoBehaviour
{
    public int RelicNumber = 0;
    private int MaxRelicCount = 4;

    [SerializeField] int slotCount = 3;
    [SerializeField] RelicDatabase relicDatabase;
    
    // UI References
    [SerializeField] GameObject shopPanel;
    [SerializeField] RelicSlotUI[] relicSlots;  
    [SerializeField] TextMeshProUGUI shardText;
    [SerializeField] Button proceedButton;
    [SerializeField] Button rerollButton;

    [SerializeField] Match3Skin skin;
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
        if (currentShop[index] == null)
        {
            return;
        }
        if (Data.Instance.Shard >= currentShop[index].price && RelicNumber < MaxRelicCount)
        {
            Data.Instance.addRelic(currentShop[index]);
            Data.Instance.Shard -= currentShop[index].price;
            relicSlots[index].MarkAsSold();
            shardText.SetText("Shards: {0}", Data.Instance.Shard);
            currentShop[index] = null;
            RelicNumber++;
        }
    }

    public void CloseShop() 
    {
        //Hide shop panel
        shopPanel.SetActive(false);

        // TODO: Trigger next trial - call
        skin.StartNewGame();
    }

    public void SetActiveShop(bool active)
    {
        shopPanel.SetActive(active);
    }
}
