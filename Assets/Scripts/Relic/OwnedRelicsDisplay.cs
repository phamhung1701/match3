using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class OwnedRelicsDisplay : MonoBehaviour
{
    [SerializeField] private GameObject display;
    [SerializeField] private Button[] relicSlotButtons;   // Button on each relic image
    [SerializeField] private Image[] relicsIconSlots;     // The Image components
    [SerializeField] private Button[] sellButtons;        // Sell button for each slot
    [SerializeField] private TextMeshProUGUI[] sellPriceTexts; // Price text for each sell button
    [SerializeField] private Shop shop;

    private void Awake()
    {
        // Set up click handlers for each relic slot
        for (int i = 0; i < relicSlotButtons.Length; i++)
        {
            int index = i;
            relicSlotButtons[i].onClick.AddListener(() => OnRelicClicked(index));
        }

        // Set up click handlers for each sell button
        for (int i = 0; i < sellButtons.Length; i++)
        {
            int index = i;
            sellButtons[i].onClick.AddListener(() => OnSellClicked(index));
            sellButtons[i].gameObject.SetActive(false);
        }
    }

    private void OnEnable()
    {
        if (RelicManager.Instance != null)
        {
            RelicManager.Instance.OnRelicsChanged += RefreshDisplay;
        }
        RefreshDisplay();
    }

    public void RefreshDisplay()
    {
        if (RelicManager.Instance == null) return;
        
        // Get minimum length of all arrays
        int slotCount = Mathf.Min(relicsIconSlots.Length, relicSlotButtons.Length, sellButtons.Length);
        var ownedRelics = RelicManager.Instance.OwnedRelics;

        // Hide all slots and sell buttons first
        for (int i = 0; i < slotCount; i++)
        {
            relicsIconSlots[i].gameObject.SetActive(false);
            relicSlotButtons[i].interactable = false;
            sellButtons[i].gameObject.SetActive(false);
        }

        // Show icons for owned relics
        for (int i = 0; i < ownedRelics.Count && i < slotCount; i++)
        {
            var instance = ownedRelics[i];
            if (instance != null && instance.Data != null && instance.Data.icon != null)
            {
                relicsIconSlots[i].sprite = instance.Data.icon;
                relicsIconSlots[i].gameObject.SetActive(true);
                relicSlotButtons[i].interactable = true;
            }
        }
    }

    private void OnRelicClicked(int index)
    {
        if (RelicManager.Instance == null) return;
        
        var ownedRelics = RelicManager.Instance.OwnedRelics;
        if (index >= ownedRelics.Count || ownedRelics[index] == null)
            return;
        if (index >= sellButtons.Length || index >= sellPriceTexts.Length)
            return;

        // Toggle sell button visibility
        bool isVisible = sellButtons[index].gameObject.activeSelf;
        
        // Hide all sell buttons first
        for (int i = 0; i < sellButtons.Length; i++)
        {
            sellButtons[i].gameObject.SetActive(false);
        }

        // Show this one if it wasn't already visible
        if (!isVisible)
        {
            var instance = ownedRelics[index];
            sellPriceTexts[index].text = $"{instance.Data.price / 2}";
            sellButtons[index].gameObject.SetActive(true);
        }
    }

    private void OnSellClicked(int index)
    {
        shop.SellRelic(index);
    }

    private void OnDisable()
    {
        if (RelicManager.Instance != null)
        {
            RelicManager.Instance.OnRelicsChanged -= RefreshDisplay;
        }
    }

    public void SetActiveDisplay(bool active)
    {
        display.SetActive(active);
        if (!active)
        {
            for (int i = 0; i < sellButtons.Length; i++)
            {
                sellButtons[i].gameObject.SetActive(false);
            }
        }
    }
}
