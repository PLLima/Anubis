using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Handles the Title Screen / Main Menu functionality.
/// </summary>
public class MainMenuManager : MonoBehaviour
{
    [Header("UI Containers")]
    public GameObject buttonsContainer;
    public GameObject creditsContainer;

    [Header("Buttons")]
    public Button playButton;
    public Button creditsButton;
    public Button quitButton;
    public Button closeCreditsButton; // Invisible fullscreen button to close credits

    private void Start()
    {
        if (playButton != null) playButton.onClick.AddListener(OnPlayClicked);
        if (creditsButton != null)
        {
            creditsButton.interactable = true;
            creditsButton.onClick.AddListener(OnCreditsClicked);
        }
        if (quitButton != null) quitButton.onClick.AddListener(OnQuitClicked);
        if (closeCreditsButton != null) closeCreditsButton.onClick.AddListener(CloseCredits);

        // Ensure we start on the main buttons
        if (buttonsContainer != null) buttonsContainer.SetActive(true);
        if (creditsContainer != null) creditsContainer.SetActive(false);
    }

    private void OnDestroy()
    {
        if (playButton != null) playButton.onClick.RemoveListener(OnPlayClicked);
        if (creditsButton != null) creditsButton.onClick.RemoveListener(OnCreditsClicked);
        if (quitButton != null) quitButton.onClick.RemoveListener(OnQuitClicked);
        if (closeCreditsButton != null) closeCreditsButton.onClick.RemoveListener(CloseCredits);
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

    private void OnCreditsClicked()
    {
        if (buttonsContainer != null) buttonsContainer.SetActive(false);
        if (creditsContainer != null) creditsContainer.SetActive(true);
    }

    private void CloseCredits()
    {
        if (creditsContainer != null) creditsContainer.SetActive(false);
        if (buttonsContainer != null) buttonsContainer.SetActive(true);
    }
}
