using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;

/// <summary>
/// Detects clicks on TMP hyperlinks inside the dialogue text and saves them as clues.
/// Handles hover effects highlighting the text snippets with Ancient Egyptian styling.
/// </summary>
[RequireComponent(typeof(TextMeshProUGUI))]
public class DialogueLinkHandler : MonoBehaviour, IPointerClickHandler, IPointerMoveHandler, IPointerExitHandler, IPointerEnterHandler
{
    private TextMeshProUGUI textMeshPro;
    private TextMeshProUGUI shadowText;
    private DialogueBubble currentBubble; 
    private int currentLink = -1;

    private void Awake() {
        textMeshPro = GetComponent<TextMeshProUGUI>();
        
        GameObject shadowObj = new GameObject("ShadowText");
        shadowObj.transform.SetParent(textMeshPro.transform, false);
        
        RectTransform rt = shadowObj.AddComponent<RectTransform>();
        rt.anchorMin = Vector2.zero;
        rt.anchorMax = Vector2.one;
        rt.offsetMin = Vector2.zero;
        rt.offsetMax = Vector2.zero;

        shadowText = shadowObj.AddComponent<TextMeshProUGUI>();
        shadowText.font = textMeshPro.font;
        shadowText.fontSize = textMeshPro.fontSize;
        shadowText.alignment = textMeshPro.alignment;
        shadowText.textWrappingMode = textMeshPro.textWrappingMode;
        shadowText.margin = textMeshPro.margin;
        shadowText.color = textMeshPro.color;
        shadowText.raycastTarget = false; 
    }

    public void SetCurrentBubble(DialogueBubble bubble) {
        currentBubble = bubble;
        
        // Ensure the mesh will fully generate before we process vertices in ResetHover
        SetVisibleCharacters(999999);
        
        if (shadowText != null && textMeshPro != null)
        {
            shadowText.text = textMeshPro.text;
        }
        ResetHover();
    }

    public void SetVisibleCharacters(int count)
    {
        if (textMeshPro != null) textMeshPro.maxVisibleCharacters = count;
        if (shadowText != null) shadowText.maxVisibleCharacters = count;
    }

    private void ResetHover()
    {
        if (textMeshPro != null)
        {
            currentLink = -1;
            if (GameManager.Instance != null) GameManager.Instance.SetCursorHoverState(false);
            
            textMeshPro.ForceMeshUpdate();
            if (shadowText != null)
            {
                shadowText.ForceMeshUpdate();
                HideAllShadowVertices();
            }
        }
    }

    private void HideAllShadowVertices()
    {
        if (shadowText == null) return;
        TMP_TextInfo textInfo = shadowText.textInfo;
        for (int i = 0; i < textInfo.characterCount; i++)
        {
            TMP_CharacterInfo charInfo = textInfo.characterInfo[i];
            if (!charInfo.isVisible) continue;

            int matIndex = charInfo.materialReferenceIndex;
            int vertIndex = charInfo.vertexIndex;
            
            if (matIndex >= textInfo.meshInfo.Length) continue;
            Color32[] vertexColors = textInfo.meshInfo[matIndex].colors32;
            
            if (vertexColors == null || vertIndex + 3 >= vertexColors.Length) continue;

            Color32 clear = new Color32(0, 0, 0, 0);
            vertexColors[vertIndex + 0] = clear;
            vertexColors[vertIndex + 1] = clear;
            vertexColors[vertIndex + 2] = clear;
            vertexColors[vertIndex + 3] = clear;
        }
        shadowText.UpdateVertexData(TMP_VertexDataUpdateFlags.Colors32);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        CheckHover(eventData);
    }

    public void OnPointerMove(PointerEventData eventData)
    {
        CheckHover(eventData);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        ResetHover();
    }

    public void OnPointerClick(PointerEventData eventData) 
    {
        if (textMeshPro == null || currentBubble == null) return;
        
        int linkIndex = TMP_TextUtilities.FindIntersectingLink(textMeshPro, eventData.position, eventData.pressEventCamera);

        if (linkIndex != -1) {
            TMP_LinkInfo linkInfo = textMeshPro.textInfo.linkInfo[linkIndex];
            string linkID = linkInfo.GetLinkID();
            
            if (int.TryParse(linkID, out int snippetIndex)) {
                string clickedText = currentBubble.snippets[snippetIndex];
                
                GameManager.Instance.TrySaveClue(clickedText);
            }
        }
    }

    private void CheckHover(PointerEventData eventData)
    {
        if (textMeshPro == null) return;

        // enterEventCamera is used for hover raycasting
        Camera targetCamera = eventData.enterEventCamera;
        int linkIndex = TMP_TextUtilities.FindIntersectingLink(textMeshPro, eventData.position, targetCamera);

        if (linkIndex != currentLink)
        {
            currentLink = linkIndex;
            
            if (GameManager.Instance != null) 
            {
                GameManager.Instance.SetCursorHoverState(currentLink != -1);
            }

            textMeshPro.ForceMeshUpdate(); 
            if (shadowText != null)
            {
                shadowText.ForceMeshUpdate();
                HideAllShadowVertices();
            }
            
            if (currentLink != -1)
            {
                ApplyHoverEffect(currentLink);
            }
        }
    }

    private void ApplyHoverEffect(int linkIndex)
    {
        TMP_TextInfo textInfo = textMeshPro.textInfo;
        TMP_LinkInfo linkInfo = textInfo.linkInfo[linkIndex];

        TMP_TextInfo popTextInfo = shadowText != null ? shadowText.textInfo : null;

        // Updated visual identity colors
        Color32 hoverColor = new Color32(222, 134, 1, 255); // Primary Color
        Color32 shadowColor = new Color32(87, 59, 18, 255); // Accent Color (Dark brown shadow)

        for (int i = 0; i < linkInfo.linkTextLength; i++)
        {
            int charIndex = linkInfo.linkTextfirstCharacterIndex + i;
            TMP_CharacterInfo charInfo = textInfo.characterInfo[charIndex];

            if (!charInfo.isVisible) continue;

            int materialIndex = charInfo.materialReferenceIndex;
            int vertexIndex = charInfo.vertexIndex;

            if (materialIndex >= textInfo.meshInfo.Length) continue;
            Color32[] vertexColors = textInfo.meshInfo[materialIndex].colors32;
            Vector3[] vertices = textInfo.meshInfo[materialIndex].vertices;
            
            if (vertexColors == null || vertexIndex + 3 >= vertexColors.Length) continue;

            // PARENT TEXT BECOMES THE SHADOW (Renders Underneath)
            vertexColors[vertexIndex + 0] = shadowColor;
            vertexColors[vertexIndex + 1] = shadowColor;
            vertexColors[vertexIndex + 2] = shadowColor;
            vertexColors[vertexIndex + 3] = shadowColor;
            // No shift or scale applied to the parent text so it stays in its original spot as an anchor

            // CHILD TEXT BECOMES THE POPPED TEXT (Renders On Top)
            if (popTextInfo != null && charIndex < popTextInfo.characterInfo.Length)
            {
                TMP_CharacterInfo popCharInfo = popTextInfo.characterInfo[charIndex];
                if (popCharInfo.isVisible)
                {
                    int popMatIndex = popCharInfo.materialReferenceIndex;
                    int popVertIndex = popCharInfo.vertexIndex;

                    if (popMatIndex >= popTextInfo.meshInfo.Length) continue;

                    Color32[] popVertexColors = popTextInfo.meshInfo[popMatIndex].colors32;
                    Vector3[] popVertices = popTextInfo.meshInfo[popMatIndex].vertices;
                    
                    if (popVertexColors == null || popVertIndex + 3 >= popVertexColors.Length) continue;

                    popVertexColors[popVertIndex + 0] = hoverColor;
                    popVertexColors[popVertIndex + 1] = hoverColor;
                    popVertexColors[popVertIndex + 2] = hoverColor;
                    popVertexColors[popVertIndex + 3] = hoverColor;

                    // Pop effect scaling - softened to 1.08f
                    Vector3 offset = (popVertices[popVertIndex + 0] + popVertices[popVertIndex + 2]) / 2;
                    float scale = 1.08f;
                    
                    popVertices[popVertIndex + 0] = offset + (popVertices[popVertIndex + 0] - offset) * scale;
                    popVertices[popVertIndex + 1] = offset + (popVertices[popVertIndex + 1] - offset) * scale;
                    popVertices[popVertIndex + 2] = offset + (popVertices[popVertIndex + 2] - offset) * scale;
                    popVertices[popVertIndex + 3] = offset + (popVertices[popVertIndex + 3] - offset) * scale;

                    // Shift popped text very slightly left and up
                    Vector3 popShift = new Vector3(-1.5f, 0.5f, 0);
                    popVertices[popVertIndex + 0] += popShift;
                    popVertices[popVertIndex + 1] += popShift;
                    popVertices[popVertIndex + 2] += popShift;
                    popVertices[popVertIndex + 3] += popShift;
                }
            }
        }

        // Push the changes
        textMeshPro.UpdateVertexData(TMP_VertexDataUpdateFlags.Colors32 | TMP_VertexDataUpdateFlags.Vertices);
        if (shadowText != null)
        {
            shadowText.UpdateVertexData(TMP_VertexDataUpdateFlags.Colors32 | TMP_VertexDataUpdateFlags.Vertices);
        }
    }
}
