using System;
using UnityEngine;
using TMPro;

public class UIManager : MonoBehaviour
{
    [Header("UI Panels")]
    public GameObject speechBubblePanel;
    public GameObject papyrusPanel;
    public GameObject selectionPanel;
    public GameObject scalesPanel;
    public GameObject foregroundWallImage; 

    [Header("Dialogue Dependencies")]
    public DialogueLinkHandler dialogueLinkHandler; 
    public TextMeshProUGUI dialogueTextComponent;   

    private void OnEnable() {
        GameManager.OnStateChanged += HandleStateChange;
    }

    private void OnDisable() {
        GameManager.OnStateChanged -= HandleStateChange;
    }

    private void HandleStateChange(GameState state) {
        speechBubblePanel.SetActive(false);
        selectionPanel.SetActive(false);
        scalesPanel.SetActive(false);
        
        switch (state) {
            case GameState.AnubisIntro:
            case GameState.Interview:
                speechBubblePanel.SetActive(true);
                break;
            case GameState.Deliberation:
            case GameState.Judgment:
                selectionPanel.SetActive(true);
                break;
            case GameState.ScaleEvaluation:
                scalesPanel.SetActive(true);
                break;
        }
    }

    public void ShowDialogue(DialogueBubble bubble) {
        dialogueTextComponent.text = FormatDialogueForClicking(bubble);
        dialogueLinkHandler.SetCurrentBubble(bubble);
    }

    public string FormatDialogueForClicking(DialogueBubble bubble) {
        string combinedText = "";
        for (int i = 0; i < bubble.snippets.Length; i++) {
            combinedText += $"<link=\"{i}\"> {bubble.snippets[i]}</link> ";
        }
        return combinedText.TrimEnd();
    }
}
