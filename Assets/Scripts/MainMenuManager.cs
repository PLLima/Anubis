using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Handles the Title Screen / Main Menu functionality.
/// </summary>
public class MainMenuManager : MonoBehaviour
{
    [Header("Buttons")]
    public Button playButton;
    public Button settingsButton;
    public Button quitButton;

    private void Start()
    {
        if (playButton != null)
        {
            playButton.onClick.AddListener(OnPlayClicked);
        }

        if (settingsButton != null)
        {
            // Settings button is disabled for now as requested
            settingsButton.interactable = false;
        }

        if (quitButton != null)
        {
            quitButton.onClick.AddListener(OnQuitClicked);
        }
    }

    private void OnDestroy()
    {
        if (playButton != null)
        {
            playButton.onClick.RemoveListener(OnPlayClicked);
        }

        if (quitButton != null)
        {
            quitButton.onClick.RemoveListener(OnQuitClicked);
        }
    }

    private void OnPlayClicked()
    {
        // Change state to AnubisIntro to start the game
        if (GameManager.Instance != null)
        {
            GameManager.Instance.ChangeState(GameState.AnubisIntro);
        }
        else
        {
            Debug.LogError("GameManager instance not found!");
        }
    }

    private void OnQuitClicked()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.QuitGame();
        }
        else
        {
            Debug.LogError("GameManager instance not found!");
        }
    }
}
