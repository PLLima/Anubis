using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;

/// <summary>
/// Detects clicks on TMP hyperlinks inside the dialogue text and saves them as clues.
/// </summary>
[RequireComponent(typeof(TextMeshProUGUI))]
public class DialogueLinkHandler : MonoBehaviour, IPointerClickHandler
{
    private TextMeshProUGUI textMeshPro;
    private DialogueBubble currentBubble; 

    private void Awake() 
    {
        textMeshPro = GetComponent<TextMeshProUGUI>();
    }

    public void SetCurrentBubble(DialogueBubble bubble) 
    {
        currentBubble = bubble;
    }

    public void OnPointerClick(PointerEventData eventData) 
    {
        if (textMeshPro == null || currentBubble == null) return;

        int linkIndex = TMP_TextUtilities.FindIntersectingLink(textMeshPro, eventData.position, eventData.pressEventCamera);

        if (linkIndex != -1) 
        {
            TMP_LinkInfo linkInfo = textMeshPro.textInfo.linkInfo[linkIndex];
            string linkID = linkInfo.GetLinkID();
            
            if (int.TryParse(linkID, out int snippetIndex) && snippetIndex < currentBubble.snippets.Length) 
            {
                string clickedText = currentBubble.snippets[snippetIndex];
                GameManager.Instance.TrySaveClue(clickedText);
            }
        }
    }
}
