using UnityEngine;
using UnityEngine.UI;

public class StartMenuUI : MonoBehaviour
{
    [SerializeField] private Match3Skin skin;
    [SerializeField] private Match3Game game;
    [SerializeField] private Shop shop;
    [SerializeField] private RelicDatabase relicDatabase;
    [SerializeField] private OwnedRelicsDisplay relicsDisplay;
    [SerializeField] private Button startButton;
    [SerializeField] private Button quitButton;
    [SerializeField] private Button continueButton;
    [SerializeField] private GameObject startGameObject;

    private void Awake()
    {
        startButton.onClick.AddListener(OnStartClicked);
        quitButton.onClick.AddListener(OnQuitClicked);
        continueButton.onClick.AddListener(OnContinueClicked);
    }

    private void Start()
    {
        // Enable/disable Continue button based on save file existence
        continueButton.interactable = SaveManager.HasSaveFile();
    }

    private void OnStartClicked()
    {
        // Delete old save when starting new game
        SaveManager.DeleteSave();
        
        startGameObject.SetActive(false);
        skin.RestartGame();
        relicsDisplay.SetActiveDisplay(true);
    }

    private void OnContinueClicked()
    {
        SaveData saveData = SaveManager.LoadGame();
        if (saveData == null)
        {
            Debug.LogError("Failed to load save data");
            return;
        }

        // Apply saved data
        Data.Instance.Shard = saveData.shards;
        game.cycle = saveData.cycle;
        game.trial = saveData.trial;
        shop.RelicNumber = saveData.relicNames.Count;

        // Restore relics from names
        Data.Instance.relics.Clear();
        foreach (string relicName in saveData.relicNames)
        {
            RelicData relic = relicDatabase.GetRelicByName(relicName);
            if (relic != null)
            {
                Data.Instance.relics.Add(relic);
            }
        }

        startGameObject.SetActive(false);
        relicsDisplay.SetActiveDisplay(true);
        Data.OnRelicsChanged?.Invoke();

        // Restore to correct state
        if (saveData.isInShop)
        {
            // Return to shop
            skin.GamePanel(false);
            shop.OpenShop();
        }
        else
        {
            // Return to gameplay
            skin.GamePanel(true);
            skin.StartNewGame();
        }
    }

    private void OnQuitClicked()
    {
        Application.Quit();
    }

    public void SetActiveStartMenu(bool active)
    {
        startGameObject.SetActive(active);
        if (active)
        {
            continueButton.interactable = SaveManager.HasSaveFile();
        }
    }
}
