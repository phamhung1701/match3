using UnityEngine;
using UnityEngine.UI;

public class OwnedRelicsDisplay : MonoBehaviour
{
    [SerializeField] private GameObject display;
    [SerializeField] private Image[] relicsIconSlots;  // Pre-placed Image slots in UI

    private void OnEnable()
    {
        Data.OnRelicsChanged += RefreshDisplay;
    }

    public void RefreshDisplay()
    {
        // Hide all slots first
        for (int i = 0; i < relicsIconSlots.Length; i++)
        {
            relicsIconSlots[i].gameObject.SetActive(false);
        }

        // Show icons for owned relics
        for (int i = 0; i < Data.Instance.relics.Count && i < relicsIconSlots.Length; i++)
        {
            if (Data.Instance.relics[i] != null && Data.Instance.relics[i].icon != null)
            {
                relicsIconSlots[i].sprite = Data.Instance.relics[i].icon;
                relicsIconSlots[i].gameObject.SetActive(true);
            }
        }
    }

    private void OnDisable()
    {
        Data.OnRelicsChanged -= RefreshDisplay;
    }

    public void SetActiveDisplay(bool active)
    {
        display.SetActive(active);
    }
}