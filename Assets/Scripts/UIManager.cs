using UnityEngine;
using TMPro;

/// <summary>
/// Handles the visibility of UI panels based on game states and populates dialogue data.
/// </summary>
public class UIManager : MonoBehaviour
{
    [Header("UI Panels")]
    public GameObject speechBubblePanel;
    public GameObject papyrusPanel;
    public GameObject selectionPanel;
    public GameObject scalesPanel;
    public GameObject titleScreenPanel;
    [Header("Environment")]
    public GameObject background;
    
    private void OnEnable() 
    {
        GameManager.OnStateChanged += HandleStateChange;
    }

    private void OnDisable() 
    {
        GameManager.OnStateChanged -= HandleStateChange;
    }

    private void HandleStateChange(GameState state) 
    {
        // Toggle visibility based on states
        bool isTitleScreen = state == GameState.TitleScreen;
        bool showSpeechBubble = state == GameState.Interview;
        bool showSelection = state == GameState.Deliberation || state == GameState.Judgment;
        bool showScales = state == GameState.ScaleEvaluation;

        if (speechBubblePanel != null) speechBubblePanel.SetActive(showSpeechBubble);
        if (selectionPanel != null) selectionPanel.SetActive(showSelection);
        if (scalesPanel != null) scalesPanel.SetActive(showScales);
        if (titleScreenPanel != null) titleScreenPanel.SetActive(isTitleScreen);

        // Hide environment during Title Screen
        if (background != null) background.SetActive(!isTitleScreen);
    }

}
