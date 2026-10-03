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
    public TextMeshProUGUI npcNameText;

    private void OnEnable() {
        GameManager.OnStateChanged += HandleStateChange;
        GameManager.OnDialogueChanged += ShowDialogue;
        GameManager.OnNPCChanged += UpdateNPCName;
    }

    private void OnDisable() {
        GameManager.OnStateChanged -= HandleStateChange;
        GameManager.OnDialogueChanged -= ShowDialogue;
        GameManager.OnNPCChanged -= UpdateNPCName;
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
        if (dialogueTextComponent == null) {
            Debug.LogError("DialogueTextComponent is not assigned in the UIManager Inspector!");
            return;
        }
        if (dialogueLinkHandler == null) {
            Debug.LogError("DialogueLinkHandler is not assigned in the UIManager Inspector!");
            return;
        }
        if (bubble == null) return;

        dialogueTextComponent.text = FormatDialogueForClicking(bubble);
        dialogueLinkHandler.SetCurrentBubble(bubble);
    }

    private void UpdateNPCName(NPCData npc) {
        if (npcNameText != null && npc != null) {
            npcNameText.text = npc.npcName;
        }
    }

    public string FormatDialogueForClicking(DialogueBubble bubble) {
        string combinedText = "";
        for (int i = 0; i < bubble.snippets.Length; i++) {
            combinedText += $"<link=\"{i}\"> {bubble.snippets[i]}</link> ";
        }
        return combinedText.TrimEnd();
    }
}
