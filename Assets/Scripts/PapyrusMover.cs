using System.Collections;
using UnityEngine;
using TMPro;

/// <summary>
/// Handles the sliding animation of the Papyrus UI and typing the candidate's name.
/// </summary>
public class PapyrusMover : MonoBehaviour
{
    [Header("References")]
    public TextMeshProUGUI npcNameText;

    [Header("Papyrus Animation")]
    public float papyrusSlideDuration = 0.5f;
    public float typingSpeed = 40f; // characters per second
    public float offScreenRightX = 1500f;
    public float offScreenBottomY = -1500f;
    
    private RectTransform papyrusRect;
    private Vector2 papyrusBasePos;
    private Coroutine activeCoroutine;

    private void Awake()
    {
        papyrusRect = GetComponent<RectTransform>();
        if (papyrusRect != null) 
        {
            papyrusBasePos = papyrusRect.anchoredPosition;
            // Force the papyrus outside the window immediately when the game starts
            Vector2 startPos = new Vector2(papyrusBasePos.x + offScreenRightX, papyrusBasePos.y);
            papyrusRect.anchoredPosition = startPos;
        }
    }

    private void OnEnable()
    {
        CharacterMover.OnCharacterEnterFinished += HandleCharacterEnterFinished;
        CharacterMover.OnCharacterExitFinished += HandleCharacterExitFinished;
        GameManager.OnNPCChanged += HideNameText;
    }

    private void OnDisable()
    {
        CharacterMover.OnCharacterEnterFinished -= HandleCharacterEnterFinished;
        CharacterMover.OnCharacterExitFinished -= HandleCharacterExitFinished;
        GameManager.OnNPCChanged -= HideNameText;
    }

    private void HideNameText(NPCData npc)
    {
        if (npcNameText != null && npc != null)
        {
            // Do not hide the text if we are already in the interview phase!
            if (GameManager.Instance != null && GameManager.Instance.CurrentState == GameState.Interview)
                return;

            npcNameText.text = npc.npcName;
            npcNameText.maxVisibleCharacters = 0;
        }
    }

    private void HandleCharacterEnterFinished()
    {
        if (activeCoroutine != null) StopCoroutine(activeCoroutine);
        activeCoroutine = StartCoroutine(EnterSequence());
    }

    private void HandleCharacterExitFinished()
    {
        if (activeCoroutine != null) StopCoroutine(activeCoroutine);
        activeCoroutine = StartCoroutine(ExitSequence());
    }

    private IEnumerator EnterSequence()
    {
        if (papyrusRect != null)
        {
            Vector2 startPos = new Vector2(papyrusBasePos.x + offScreenRightX, papyrusBasePos.y);
            papyrusRect.anchoredPosition = startPos;
            
            float elapsed = 0;
            while (elapsed < papyrusSlideDuration)
            {
                elapsed += Time.deltaTime;
                float t = Mathf.SmoothStep(0, 1, elapsed / papyrusSlideDuration);
                papyrusRect.anchoredPosition = Vector2.Lerp(startPos, papyrusBasePos, t);
                yield return null;
            }
            papyrusRect.anchoredPosition = papyrusBasePos;
        }

        // Type the NPC name
        if (npcNameText != null)
        {
            npcNameText.ForceMeshUpdate();
            int totalChars = npcNameText.textInfo.characterCount;
            npcNameText.maxVisibleCharacters = 0;
            
            float floatVisible = 0f;
            while (floatVisible < totalChars)
            {
                if (GameManager.Instance != null && GameManager.Instance.WasClickedThisFrame())
                {
                    break;
                }

                floatVisible += Time.deltaTime * typingSpeed;
                npcNameText.maxVisibleCharacters = Mathf.FloorToInt(floatVisible);
                yield return null;
            }
            
            npcNameText.maxVisibleCharacters = totalChars;
            npcNameText.ForceMeshUpdate();
        }

        // Force this panel to render on top of the speech bubble
        if (papyrusRect != null) 
        {
            papyrusRect.SetAsLastSibling();
        }

        // Proceed to Interview
        GameManager.Instance.OnCandidateEntered();
    }

    private IEnumerator ExitSequence()
    {
        if (papyrusRect != null)
        {
            // Move down to become hidden
            Vector2 targetPos = new Vector2(papyrusBasePos.x, papyrusBasePos.y + offScreenBottomY);
            Vector2 startPos = papyrusRect.anchoredPosition;
            
            float elapsed = 0;
            while (elapsed < papyrusSlideDuration)
            {
                elapsed += Time.deltaTime;
                float t = Mathf.SmoothStep(0, 1, elapsed / papyrusSlideDuration);
                papyrusRect.anchoredPosition = Vector2.Lerp(startPos, targetPos, t);
                yield return null;
            }
            papyrusRect.anchoredPosition = targetPos;
        }
        
        GameManager.Instance.OnCandidateExited();
    }
}
