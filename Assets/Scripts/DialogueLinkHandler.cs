using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;

[RequireComponent(typeof(TextMeshProUGUI))]
public class DialogueLinkHandler : MonoBehaviour, IPointerClickHandler
{
    private TextMeshProUGUI textMeshPro;
    
    private DialogueBubble currentBubble; 

    private void Awake() {
        textMeshPro = GetComponent<TextMeshProUGUI>();
    }

    public void SetCurrentBubble(DialogueBubble bubble) {
        currentBubble = bubble;
    }

    public void OnPointerClick(PointerEventData eventData) {
        int linkIndex = TMP_TextUtilities.FindIntersectingLink(textMeshPro, eventData.position, eventData.pressEventCamera);

        if (linkIndex != -1) {
            TMP_LinkInfo linkInfo = textMeshPro.textInfo.linkInfo[linkIndex];
            string linkID = linkInfo.GetLinkID();
            
            if (int.TryParse(linkID, out int snippetIndex)) {
                string clickedText = currentBubble.snippets[snippetIndex];
                
                GameManager.Instance.TrySaveClue(clickedText);
            }
        } else {
            // They clicked the speech bubble, but not a clue. 
            // This could be your trigger to advance to the next dialogue piece!
            Debug.Log("Clicked background to continue dialogue.");
            GameManager.Instance.AdvanceDialogue();
        }
    }
}
