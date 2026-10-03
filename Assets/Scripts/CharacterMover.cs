using System.Collections;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Handles the visual sliding and dropping animations for the NPC character UI elements.
/// </summary>
public class CharacterMover : MonoBehaviour
{
    public static event System.Action OnCharacterEnterFinished;
    public static event System.Action OnCharacterExitFinished;
    public static event System.Action OnAnubisEnterFinished;

    [Header("Core References")]
    public RectTransform activeCharacterRect;
    public Image activeCharacterImage;

    [Header("Position Boundaries")]
    public float offScreenLeftX = -1200f; 
    public float targetScreenX = -270f;
    
    [Header("Animation Settings")]
    public float waitBeforeDropTime = 0.4f;
    public float dropDuration = 0.2f;
    public float dropYOffset = -30f;
    public float slideDuration = 0.5f;

    [Header("Audio Settings")]
    public AudioSource footstepSource;
    public AudioClip footstepClip;

    private float baseY;
    private Coroutine activeCoroutine;

    private void Awake() 
    {
        if (activeCharacterRect != null) 
        {
            baseY = activeCharacterRect.anchoredPosition.y;
        }

        if (footstepSource == null)
        {
            footstepSource = GetComponent<AudioSource>();
        }
    }

    private void OnEnable() 
    {
        GameManager.OnStateChanged += HandleStateChange;
        GameManager.OnNPCChanged += HandleNPCChanged;
    }

    private void OnDisable() 
    {
        GameManager.OnStateChanged -= HandleStateChange;
        GameManager.OnNPCChanged -= HandleNPCChanged;
    }

    private void HandleStateChange(GameState state) 
    {
        if (state == GameState.TitleScreen)
        {
            SetCharacterYPosition(baseY);
            if (activeCharacterRect != null)
            {
                Vector2 pos = activeCharacterRect.anchoredPosition;
                pos.x = offScreenLeftX;
                activeCharacterRect.anchoredPosition = pos;
            }
        }
        else if (state == GameState.CandidateEnter) 
        {
            StartAnimation(EnterSequence(false));
        } 
        else if (state == GameState.CandidateExit) 
        {
            StartAnimation(ExitSequence());
        }
        else if (state == GameState.AnubisIntro || state == GameState.Deliberation)
        {
            StartAnimation(EnterSequence(true));
        }
    }

    private void HandleNPCChanged(NPCData npc) 
    {
        if (activeCharacterImage != null && npc != null) 
        {
            activeCharacterImage.sprite = npc.npcSprite;
        }
    }

    private void StartAnimation(IEnumerator sequence)
    {
        if (activeCoroutine != null)
        {
            StopCoroutine(activeCoroutine);
        }
        activeCoroutine = StartCoroutine(sequence);
    }

    private IEnumerator EnterSequence(bool isAnubis) 
    {
        SetCharacterYPosition(baseY);
        
        yield return StartCoroutine(SlideRoutine(offScreenLeftX, targetScreenX));
        
        yield return new WaitForSeconds(waitBeforeDropTime);
        
        yield return StartCoroutine(AnimateYOffset(dropYOffset));
        
        if (isAnubis)
        {
            OnAnubisEnterFinished?.Invoke();
        }
        else
        {
            OnCharacterEnterFinished?.Invoke();
        }
    }

    private IEnumerator ExitSequence() 
    {
        yield return StartCoroutine(AnimateYOffset(0f)); // Revert drop
        
        yield return new WaitForSeconds(waitBeforeDropTime);

        yield return StartCoroutine(SlideRoutine(targetScreenX, offScreenLeftX));
        
        OnCharacterExitFinished?.Invoke();
    }

    private void SetCharacterYPosition(float yPos)
    {
        if (activeCharacterRect == null) return;
        Vector2 pos = activeCharacterRect.anchoredPosition;
        pos.y = yPos;
        activeCharacterRect.anchoredPosition = pos;
    }

    private IEnumerator AnimateYOffset(float targetOffset)
    {
        if (activeCharacterRect == null) yield break;

        Vector2 startPos = activeCharacterRect.anchoredPosition;
        Vector2 endPos = new Vector2(startPos.x, baseY + targetOffset);
        
        float timeElapsed = 0;
        while (timeElapsed < dropDuration) 
        {
            timeElapsed += Time.deltaTime;
            float t = Mathf.SmoothStep(0, 1, timeElapsed / dropDuration);
            activeCharacterRect.anchoredPosition = Vector2.Lerp(startPos, endPos, t);
            yield return null;
        }
        activeCharacterRect.anchoredPosition = endPos;
    }

    private IEnumerator SlideRoutine(float startX, float endX) 
    {
        if (activeCharacterRect == null) yield break;

        if (footstepSource != null && footstepClip != null)
        {
            footstepSource.clip = footstepClip;
            footstepSource.loop = true; // Força o som a repetir
            footstepSource.Play();
        }

        float timeElapsed = 0;
        Vector2 pos = activeCharacterRect.anchoredPosition;
        pos.x = startX;
        activeCharacterRect.anchoredPosition = pos;

        while (timeElapsed < slideDuration) 
        {
            timeElapsed += Time.deltaTime;
            float t = Mathf.SmoothStep(0, 1, timeElapsed / slideDuration);
            pos.x = Mathf.Lerp(startX, endX, t);
            activeCharacterRect.anchoredPosition = pos;
            yield return null;
        }

        pos.x = endX;
        activeCharacterRect.anchoredPosition = pos;

        if (footstepSource != null)
        {
            footstepSource.Stop();
        }
    }
}
