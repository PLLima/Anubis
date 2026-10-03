using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// Attach this script to any UI element (like a Button) to trigger the custom hover cursor when the mouse is over it.
/// </summary>
public class UICursorHover : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.SetCursorHoverState(true);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.SetCursorHoverState(false);
        }
    }

    private void OnDisable()
    {
        // Ensure we don't get stuck in a hover state if the UI element is disabled while hovering
        if (GameManager.Instance != null)
        {
            GameManager.Instance.SetCursorHoverState(false);
        }
    }
}
