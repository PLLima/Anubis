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
