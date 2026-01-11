using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SellConfirmationPopup : MonoBehaviour
{
    [SerializeField] private GameObject popupPanel;
    [SerializeField] private Image relicIcon;
    [SerializeField] private TextMeshProUGUI relicNameText;
    [SerializeField] private TextMeshProUGUI sellPriceText;
    [SerializeField] private Button confirmButton;
    [SerializeField] private Button cancelButton;
    [SerializeField] private Shop shop;

    private RelicData currentRelic;

    private void Awake()
    {
        confirmButton.onClick.AddListener(OnConfirm);
        cancelButton.onClick.AddListener(OnCancel);
        popupPanel.SetActive(false);
    }

    public void ShowPopup(RelicData relic)
    {
        if (!shop.IsShopOpen) return;
        
        currentRelic = relic;
        relicIcon.sprite = relic.icon;
        relicNameText.text = relic.relicName;
        sellPriceText.text = $"Sell for {relic.price / 2} Shards?";
        popupPanel.SetActive(true);
    }

    private void OnConfirm()
    {
        if (currentRelic != null)
        {
            shop.SellRelic(currentRelic);
            currentRelic = null;
        }
        popupPanel.SetActive(false);
    }

    private void OnCancel()
    {
        currentRelic = null;
        popupPanel.SetActive(false);
    }
}
