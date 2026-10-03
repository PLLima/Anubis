using UnityEngine;
using TMPro;

/// <summary>
/// Handles the typing animation and formatting of dialogue within the Speech Bubble.
/// Modularized to sit specifically on the SpeechBubblePanel.
/// </summary>
public class SpeechBubbleUI : MonoBehaviour
{
    [Header("Dialogue Dependencies")]
    public DialogueLinkHandler dialogueLinkHandler; 
    public TextMeshProUGUI dialogueTextComponent;
    public GameObject dialogueArrow;

    [Header("Typing Effect")]
    public float typingSpeed = 40f; // characters per second
    
    private Coroutine typingCoroutine;

    private void OnEnable() 
    {
        GameManager.OnDialogueChanged += ShowDialogue;
    }

    private void OnDisable() 
    {
        GameManager.OnDialogueChanged -= ShowDialogue;
    }

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

        yield return StartCoroutine(RunTypingLoop());
    }

    /// <summary>
    /// Types a standard string without hyperlink formatting (used for Anubis).
    /// </summary>
    public void TypeStandardText(string text, bool showArrowAtEnd = true, System.Action onComplete = null)
    {
        if (dialogueTextComponent == null) return;
        
        if (typingCoroutine != null)
        {
            StopCoroutine(typingCoroutine);
        }
        typingCoroutine = StartCoroutine(TypeStandardCoroutine(text, showArrowAtEnd, onComplete));
    }

    private System.Collections.IEnumerator TypeStandardCoroutine(string text, bool showArrowAtEnd, System.Action onComplete)
    {
        if (dialogueArrow != null) dialogueArrow.SetActive(false);

        dialogueTextComponent.text = text;
        // Unbind any previous bubble links
        if (dialogueLinkHandler != null)
        {
            dialogueLinkHandler.SetCurrentBubble(null);
        }
        dialogueTextComponent.ForceMeshUpdate();

        yield return StartCoroutine(RunTypingLoop(showArrowAtEnd));
        
        onComplete?.Invoke();
    }

    private System.Collections.IEnumerator RunTypingLoop(bool showArrow = true)
    {
        int totalChars = dialogueTextComponent.textInfo.characterCount;
        if (dialogueLinkHandler != null) dialogueLinkHandler.SetVisibleCharacters(0);
        else dialogueTextComponent.maxVisibleCharacters = 0;

        float floatVisible = 0f;
        while (floatVisible < totalChars)
        {
            if (GameManager.Instance != null && GameManager.Instance.WasClickedThisFrame())
            {
                break; // Skip typing on click
            }

            floatVisible += Time.deltaTime * typingSpeed;
            
            if (dialogueLinkHandler != null) dialogueLinkHandler.SetVisibleCharacters(Mathf.FloorToInt(floatVisible));
            else dialogueTextComponent.maxVisibleCharacters = Mathf.FloorToInt(floatVisible);
            
            yield return null;
        }

        if (dialogueLinkHandler != null) dialogueLinkHandler.SetVisibleCharacters(totalChars);
        else dialogueTextComponent.maxVisibleCharacters = totalChars;
        
        if (dialogueArrow != null && showArrow) dialogueArrow.SetActive(true);
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
