using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Adds premium hover and click animations (scaling, color transition) to UI buttons, removing the need for basic button backgrounds.
/// </summary>
[RequireComponent(typeof(AudioSource))]
public class AnimatedMenuButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler
{
    [Header("Audio Settings")]
    public AudioSource audioSource;
    public AudioClip clickSound;
    public AudioClip hoverSound; // Novo campo para o som de passar o rato

    [Header("Scaling Animation")]
    public float hoverScale = 1.15f;
    public float clickScale = 0.9f;
    public float animationSpeed = 15f;

    [Header("Text Coloring")]
    public Color normalColor = new Color(0.95f, 0.95f, 0.95f);
    public Color hoverColor = new Color(1f, 0.8f, 0.2f); // A golden/yellowish hue to match the Egyptian aesthetic
    public Color clickColor = new Color(0.8f, 0.6f, 0.1f);
    public Color disabledColor = new Color(0.5f, 0.5f, 0.5f, 0.5f);

    private Vector3 targetScale = Vector3.one;
    private TextMeshProUGUI buttonText;
    private Button button;

    private void Awake()
    {
        button = GetComponent<Button>();
        buttonText = GetComponentInChildren<TextMeshProUGUI>();

        // Associa o AudioSource automaticamente
        if (audioSource == null) 
        {
            audioSource = GetComponent<AudioSource>();
        }

        if (buttonText != null)
        {
            buttonText.color = IsInteractable() ? normalColor : disabledColor;
        }

        // Hide the default white background image to make it look like clean floating text
        Image bgImage = GetComponent<Image>();
        if (bgImage != null)
        {
            Color transparent = bgImage.color;
            transparent.a = 0f;
            bgImage.color = transparent;
        }

        // Force the pivot to the center so scaling expands outwards in all directions,
        // while compensating localPosition so the button doesn't visually jump.
        RectTransform rt = GetComponent<RectTransform>();
        if (rt != null && rt.pivot != new Vector2(0.5f, 0.5f))
        {
            Vector2 deltaPivot = new Vector2(0.5f, 0.5f) - rt.pivot;
            Vector3 deltaPosition = new Vector3(deltaPivot.x * rt.rect.width, deltaPivot.y * rt.rect.height, 0f);
            
            rt.pivot = new Vector2(0.5f, 0.5f);
            rt.localPosition += deltaPosition;
        }
    }

    private void Update()
    {
        if (!IsInteractable())
        {
            targetScale = Vector3.one;
            if (buttonText != null) buttonText.color = disabledColor;
        }

        // Smoothly interpolate towards the target scale
        transform.localScale = Vector3.Lerp(transform.localScale, targetScale, Time.deltaTime * animationSpeed);
    }

    private bool IsInteractable()
    {
        return button != null && button.interactable;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!IsInteractable()) return;

        // Toca o som quando o rato passa por cima
        if (audioSource != null && hoverSound != null)
        {
            audioSource.PlayOneShot(hoverSound, 1f);
        }

        targetScale = Vector3.one * hoverScale;
        
        if (buttonText != null) 
        {
            buttonText.color = hoverColor;
        }

        if (GameManager.Instance != null)
        {
            GameManager.Instance.SetCursorHoverState(true);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (!IsInteractable()) return;

        targetScale = Vector3.one;
        
        if (buttonText != null) 
        {
            buttonText.color = normalColor;
        }

        if (GameManager.Instance != null)
        {
            GameManager.Instance.SetCursorHoverState(false);
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (!IsInteractable()) return;

        // Toca o som imediatamente ao clicar
        if (audioSource != null && clickSound != null)
        {
            audioSource.PlayOneShot(clickSound, 1f);
        }

        targetScale = Vector3.one * clickScale;
        
        if (buttonText != null) 
        {
            buttonText.color = clickColor;
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (!IsInteractable()) return;

        targetScale = Vector3.one * hoverScale; // Return to hover state after releasing click
        
        if (buttonText != null) 
        {
            buttonText.color = hoverColor;
        }
    }

    private void OnDisable()
    {
        targetScale = Vector3.one;
        transform.localScale = Vector3.one;
        
        if (buttonText != null)
        {
            buttonText.color = IsInteractable() ? normalColor : disabledColor;
        }

        if (GameManager.Instance != null)
        {
            GameManager.Instance.SetCursorHoverState(false);
        }
    }
}