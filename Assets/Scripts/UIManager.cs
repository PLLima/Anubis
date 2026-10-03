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
    public TextMeshProUGUI npcNameText;

    private void OnEnable() 
    {
        GameManager.OnStateChanged += HandleStateChange;
        GameManager.OnDialogueChanged += ShowDialogue;
        GameManager.OnNPCChanged += UpdateNPCName;
    }

    private void OnDisable() 
    {
        GameManager.OnStateChanged -= HandleStateChange;
        GameManager.OnDialogueChanged -= ShowDialogue;
        GameManager.OnNPCChanged -= UpdateNPCName;
    }

    private void HandleStateChange(GameState state) 
    {
        // Toggle visibility based on states
        bool showSpeechBubble = state == GameState.AnubisIntro || state == GameState.Interview;
        bool showSelection = state == GameState.Deliberation || state == GameState.Judgment;
        bool showScales = state == GameState.ScaleEvaluation;

        if (speechBubblePanel != null) speechBubblePanel.SetActive(showSpeechBubble);
        if (selectionPanel != null) selectionPanel.SetActive(showSelection);
        if (scalesPanel != null) scalesPanel.SetActive(showScales);
    }

    [Header("Typing Effect")]
    public float textDisplayDuration = 2.0f;
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

        float timeElapsed = 0f;
        while (timeElapsed < textDisplayDuration)
        {
            timeElapsed += Time.deltaTime;
            float percent = Mathf.Clamp01(timeElapsed / textDisplayDuration);
            dialogueLinkHandler.SetVisibleCharacters(Mathf.RoundToInt(percent * totalChars));
            yield return null;
        }

        dialogueLinkHandler.SetVisibleCharacters(totalChars);
        
        if (dialogueArrow != null) dialogueArrow.SetActive(true);
    }

    private void UpdateNPCName(NPCData npc) 
    {
        if (npcNameText != null && npc != null) 
        {
            npcNameText.text = npc.npcName;
        }
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
