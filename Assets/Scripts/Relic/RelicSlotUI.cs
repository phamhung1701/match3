using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class RelicSlotUI : MonoBehaviour
{
    [SerializeField] Image iconImage;
    [SerializeField] TextMeshProUGUI nameText;
    [SerializeField] TextMeshProUGUI priceText;
    [SerializeField] TextMeshProUGUI descriptionText;
    [SerializeField] Button buyButton;
    [SerializeField] GameObject soldOverlay;

    private RelicData currentRelic;
    private int slotIndex;

    public void Initialize(int index, System.Action<int> onBuyClicked)
    {
        slotIndex = index;
        buyButton.onClick.AddListener(() => onBuyClicked(slotIndex));
    }

    public void Display(RelicData relic)
    {
        currentRelic = relic;
        
        if (relic == null)
        {
            // TODO: Show empty state
            // gameObject.SetActive(false);
            return;
        }

        // TODO: Populate UI elements
        iconImage.sprite = relic.icon;
        nameText.SetText(relic.relicName);
        priceText.SetText("{0}", relic.price);
        descriptionText.SetText(relic.description);
        soldOverlay.SetActive(false);
        buyButton.interactable = true;
    }

    public void MarkAsSold()
    {
        // TODO: Show sold state
        soldOverlay.SetActive(true);
        buyButton.interactable = false;
    }

    public void UpdateAffordability(int playerShards)
    {
        // TODO: Gray out if player can't afford
        // bool canAfford = currentRelic != null && playerShards >= currentRelic.price;
        // buyButton.interactable = canAfford;
    }
}
