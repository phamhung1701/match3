using UnityEngine;
using UnityEngine.UI;

public class PauseMenuUI : MonoBehaviour
{
    [SerializeField] private GameObject pauseMenu;
    [SerializeField] private Button resumeButton;
    [SerializeField] private Button restartButton;
    [SerializeField] private Button backToMenuButton;
    [SerializeField] private Match3Skin skin;
    [SerializeField] private OwnedRelicsDisplay relicsDisplay;
    [SerializeField] private Shop shop;
    [SerializeField] private StartMenuUI startMenuUI;
    [SerializeField] private InputHandler input;

    private bool isPause;
    
    private void Awake()
    {
        isPause = false;
        resumeButton.onClick.AddListener(OnResumeClicked);
        restartButton.onClick.AddListener(OnRestartClicked);
        backToMenuButton.onClick.AddListener(OnBackToMenuClicked);
    }

    private void Start()
    {
        if (input == null)
        {
            Debug.LogError("PauseMenuUI: InputHandler reference is NULL! Assign it in Inspector.");
            return;
        }
        input.PauseEvent += OnPauseClicked;
        Debug.Log("PauseMenuUI: Successfully subscribed to PauseEvent");
    }

    private void OnDestroy()
    {
        if (input != null)
        {
            input.PauseEvent -= OnPauseClicked;
        }
    }

    private void OnPauseClicked()
    {
        Debug.Log("OnPauseClicked received!");
        isPause = !isPause;
        pauseMenu.SetActive(isPause);

        Time.timeScale = isPause ? 0 : 1;
    }

    private void OnResumeClicked()
    {
        pauseMenu.SetActive(false);
        Time.timeScale = 1;
    }

    private void OnRestartClicked()
    {
        pauseMenu.SetActive(false);
        skin.RestartGame();
        Time.timeScale = 1;
    }

    private void OnBackToMenuClicked()
    {
        isPause = false;
        pauseMenu.SetActive(false);
        Time.timeScale = 1;
        
        // Hide all game UI
        skin.GamePanel(false);
        shop.SetActiveShop(false);
        relicsDisplay.SetActiveDisplay(false);
        
        startMenuUI.SetActiveStartMenu(true);
    }
}
