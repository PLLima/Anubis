using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections.Generic;

public class DeliberationPapyrusDrag : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    private DeliberationPapyrusManager manager;
    private NPCData npcData;
    private RectTransform rectTransform;
    private CanvasGroup canvasGroup;
    
    private Vector2 startPosition;
    private Vector2 pointerOffset;

    public void Initialize(DeliberationPapyrusManager manager, NPCData npcData)
    {
        this.manager = manager;
        this.npcData = npcData;
        rectTransform = GetComponent<RectTransform>();
        
        canvasGroup = GetComponent<CanvasGroup>();
        if (canvasGroup == null) canvasGroup = gameObject.AddComponent<CanvasGroup>();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (manager == null || !manager.IsFrontPapyrus(gameObject) || manager.IsAnimating)
        {
            eventData.pointerDrag = null; // Cancel drag
            return;
        }

        // TOCA O SOM AQUI (Aproveitando as variáveis públicas já configuradas no Manager)
        if (manager.audioSource != null && manager.slideSound != null)
        {
            manager.audioSource.clip = manager.slideSound;
            manager.audioSource.loop = true;
            manager.audioSource.Play();
        }

        startPosition = rectTransform.anchoredPosition;
        canvasGroup.blocksRaycasts = false;
        transform.SetAsLastSibling(); // Ensure it renders on top while dragging
        
        // Scale down while dragging
        rectTransform.localScale = Vector3.one * manager.GetDragScale();

        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            (RectTransform)rectTransform.parent, 
            eventData.position, 
            eventData.pressEventCamera, 
            out pointerOffset);
            
        pointerOffset = (Vector2)rectTransform.localPosition - pointerOffset;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
            (RectTransform)rectTransform.parent, 
            eventData.position, 
            eventData.pressEventCamera, 
            out Vector2 localPointerPosition))
        {
            rectTransform.localPosition = localPointerPosition + pointerOffset + manager.GetDragOffset();
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        // PARA O SOM AQUI (Quando o jogador solta o clique do rato)
        if (manager != null && manager.audioSource != null)
        {
            manager.audioSource.Stop();
        }

        canvasGroup.blocksRaycasts = true;
        rectTransform.localScale = Vector3.one; // Reset scale

        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventData, results);

        bool droppedOnAnubis = false;
        foreach (var result in results)
        {
            // ActiveCharacter is the name of the GameObject holding Anubis's Image
            if (result.gameObject.name == "ActiveCharacter")
            {
                droppedOnAnubis = true;
                break;
            }
        }

        if (droppedOnAnubis && GameManager.Instance != null)
        {
            GameManager.Instance.SubmitDeliberationChoice(npcData);
        }
        else
        {
            rectTransform.anchoredPosition = startPosition;
            manager.ForceUpdatePositions(); // Resets back to correct position visually
        }
    }
}