using UnityEngine;
using UnityEngine.UI;

public class GameOverManager : MonoBehaviour
{
    [Header("Buttons")]
    public Button restartButton;
    public Button quitButton;

    private void Start()
    {
        if (restartButton != null)
        {
            restartButton.onClick.AddListener(OnRestartClicked);
        }

        if (quitButton != null)
        {
            quitButton.onClick.AddListener(OnQuitClicked);
        }
    }

    private void OnDestroy()
    {
        if (restartButton != null)
        {
            restartButton.onClick.RemoveListener(OnRestartClicked);
        }

        if (quitButton != null)
        {
            quitButton.onClick.RemoveListener(OnQuitClicked);
        }
    }

    private void OnRestartClicked()
    {
        if (GameManager.Instance != null)
        {
            // Restart the game completely by skipping the title screen
            GameManager.Instance.StartGameLoop();
        }
    }

    private void OnQuitClicked()
    {
        if (GameManager.Instance != null)
        {
            // Go back to the title screen instead of quitting the application
            GameManager.Instance.ChangeState(GameState.TitleScreen);
        }
    }
}
