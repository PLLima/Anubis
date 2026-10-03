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
    public GameObject foregroundWallImage; 

    [Header("Dialogue Dependencies")]
    public DialogueLinkHandler dialogueLinkHandler; 
    public TextMeshProUGUI dialogueTextComponent;

    private void OnEnable() 
    {
        GameManager.OnStateChanged += HandleStateChange;
        GameManager.OnDialogueChanged += ShowDialogue;
    }

    private void OnDisable() 
    {
        GameManager.OnStateChanged -= HandleStateChange;
        GameManager.OnDialogueChanged -= ShowDialogue;
    }

    private void HandleStateChange(GameState state) 
    {
        // Toggle visibility based on states
        bool showSpeechBubble = state == GameState.Interview;
        bool showSelection = state == GameState.Deliberation || state == GameState.Judgment;
        bool showScales = state == GameState.ScaleEvaluation;

        if (speechBubblePanel != null) speechBubblePanel.SetActive(showSpeechBubble);
        if (selectionPanel != null) selectionPanel.SetActive(showSelection);
        if (scalesPanel != null) scalesPanel.SetActive(showScales);
    }

    [Header("Typing Effect")]
    public float typingSpeed = 40f; // characters per second
    public GameObject dialogueArrow;
    
    private Coroutine typingCoroutine;

    private void ShowDialogue(DialogueBubble bubble) 
    {
        if (dialogueTextComponent == null || dialogueLinkHandler == null || bubble == null) 
            return;

        if (typingCoroutine != null)
        {
            StopCoroutine(typingCoroutine);
        }
        typingCoroutine = StartCoroutine(TypeDialogue(bubble));
    }

    private System.Collections.IEnumerator TypeDialogue(DialogueBubble bubble)
    {
        if (dialogueArrow != null) dialogueArrow.SetActive(false);

        dialogueTextComponent.text = FormatDialogueForClicking(bubble);
        dialogueLinkHandler.SetCurrentBubble(bubble);
        dialogueTextComponent.ForceMeshUpdate();

        int totalChars = dialogueTextComponent.textInfo.characterCount;
        dialogueLinkHandler.SetVisibleCharacters(0);

        float floatVisible = 0f;
        while (floatVisible < totalChars)
        {
            if (GameManager.Instance != null && GameManager.Instance.WasClickedThisFrame())
            {
                break; // Skip typing on click
            }

            floatVisible += Time.deltaTime * typingSpeed;
            dialogueLinkHandler.SetVisibleCharacters(Mathf.FloorToInt(floatVisible));
            yield return null;
        }

        dialogueLinkHandler.SetVisibleCharacters(totalChars);
        
        if (dialogueArrow != null) dialogueArrow.SetActive(true);
    }

    private string FormatDialogueForClicking(DialogueBubble bubble) 
    {
        System.Text.StringBuilder combinedText = new System.Text.StringBuilder();
        for (int i = 0; i < bubble.snippets.Length; i++) 
        {
            combinedText.Append($"<link=\"{i}\"> {bubble.snippets[i]}</link> ");
        }
        return combinedText.ToString().TrimEnd();
    }
}
