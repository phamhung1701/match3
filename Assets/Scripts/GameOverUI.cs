using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class GameOverUI : MonoBehaviour
{
    [SerializeField] GameObject gameOverPanel;
    [SerializeField] TextMeshProUGUI finalScoreText;
    [SerializeField] TextMeshProUGUI progressText;
    [SerializeField] Button restartButton;
    [SerializeField] Match3Skin skin;

    void Awake()
    {
        restartButton.onClick.AddListener(Restart);
        gameOverPanel.SetActive(false);
    }

    public void ShowGameOver(float score, int cycle, int trial)
    {
        skin.GamePanel(false);
        finalScoreText.SetText("Final Score: {0}", score);
        progressText.SetText("Reached Cycle {0}, Trial {1}", cycle, trial);
        gameOverPanel.SetActive(true);
    }

    void Restart()
    {
        // Clear persistent data
        Data.Instance.relics.Clear();
        Data.Instance.Shard = 0;

        gameOverPanel.SetActive(false);
        skin.GamePanel(true);
        skin.RestartGame();
    }
}