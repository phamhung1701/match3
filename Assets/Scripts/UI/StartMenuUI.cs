using UnityEngine;
using UnityEngine.UI;

public class StartMenuUI : MonoBehaviour
{
    [SerializeField] private Match3Skin skin;
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

    private void OnStartClicked()
    {
        startGameObject.SetActive(false);
        skin.GamePanel(true);
        relicsDisplay.SetActiveDisplay(true);
    }

    private void OnContinueClicked()
    {
        OnStartClicked();

        //TODO: Load file save
    }

    private void OnQuitClicked()
    {
        Application.Quit();
    }

    public void SetActiveStartMenu(bool active)
    {
        startGameObject.SetActive(active);
    }
}
