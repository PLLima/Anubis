using System.Collections;
using UnityEngine;

public class CharacterMover : MonoBehaviour
{
    public RectTransform activeCharacterRect;
    public float slideDuration = 0.5f;

    public UnityEngine.UI.Image activeCharacterImage;

    public float offScreenLeftX = -1200f; 
    public float targetScreenX = -270f;
    
    [Header("Animation Settings")]
    public float waitBeforeDropTime = 0.4f;
    public float dropDuration = 0.2f;
    public float dropYOffset = -30f;

    private float baseY;

    private void Awake() {
        if (activeCharacterRect != null) {
            baseY = activeCharacterRect.anchoredPosition.y;
        }
    }

    private void OnEnable() {
        GameManager.OnStateChanged += HandleStateChange;
        GameManager.OnNPCChanged += HandleNPCChanged;
    }

    private void OnDisable() {
        GameManager.OnStateChanged -= HandleStateChange;
        GameManager.OnNPCChanged -= HandleNPCChanged;
    }

    private void HandleStateChange(GameState state) {
        if (state == GameState.CandidateEnter) {
            SlideCharacterIn();
        } else if (state == GameState.CandidateExit) {
            SlideCharacterOut();
        }
    }

    private void HandleNPCChanged(NPCData npc) {
        if (activeCharacterImage != null && npc != null) {
            activeCharacterImage.sprite = npc.npcSprite;
        }
    }

    public void SlideCharacterIn() {
        StartCoroutine(EnterSequence());
    }

    private IEnumerator EnterSequence() {
        if (activeCharacterRect != null) {
            Vector2 pos = activeCharacterRect.anchoredPosition;
            pos.y = baseY;
            activeCharacterRect.anchoredPosition = pos;
        }

        yield return StartCoroutine(SlideRoutine(offScreenLeftX, targetScreenX));
        
        yield return new WaitForSeconds(waitBeforeDropTime);
        
        if (activeCharacterRect != null) {
            Vector2 startPos = activeCharacterRect.anchoredPosition;
            Vector2 endPos = startPos + new Vector2(0, dropYOffset);
            
            float timeElapsed = 0;
            while (timeElapsed < dropDuration) {
                timeElapsed += Time.deltaTime;
                float t = Mathf.SmoothStep(0, 1, timeElapsed / dropDuration);
                activeCharacterRect.anchoredPosition = Vector2.Lerp(startPos, endPos, t);
                yield return null;
            }
            activeCharacterRect.anchoredPosition = endPos;
        }
        
        GameManager.Instance.OnCandidateEntered();
    }

    public void SlideCharacterOut() {
        StartCoroutine(ExitSequence());
    }

    private IEnumerator ExitSequence() {
        if (activeCharacterRect != null) {
            Vector2 startPos = activeCharacterRect.anchoredPosition;
            Vector2 endPos = new Vector2(startPos.x, baseY);
            
            float timeElapsed = 0;
            while (timeElapsed < dropDuration) {
                timeElapsed += Time.deltaTime;
                float t = Mathf.SmoothStep(0, 1, timeElapsed / dropDuration);
                activeCharacterRect.anchoredPosition = Vector2.Lerp(startPos, endPos, t);
                yield return null;
            }
            activeCharacterRect.anchoredPosition = endPos;
        }

        yield return new WaitForSeconds(waitBeforeDropTime);

        yield return StartCoroutine(SlideRoutine(targetScreenX, offScreenLeftX));
        
        GameManager.Instance.OnCandidateExited();
    }

    private IEnumerator SlideRoutine(float startX, float endX, System.Action onComplete = null) {
        float timeElapsed = 0;
        
        Vector2 pos = activeCharacterRect.anchoredPosition;
        pos.x = startX;
        activeCharacterRect.anchoredPosition = pos;

        while (timeElapsed < slideDuration) {
            timeElapsed += Time.deltaTime;
            
            float t = Mathf.SmoothStep(0, 1, timeElapsed / slideDuration);
            
            pos.x = Mathf.Lerp(startX, endX, t);
            activeCharacterRect.anchoredPosition = pos;
            
            yield return null;
        }

        pos.x = endX;
        activeCharacterRect.anchoredPosition = pos;
        
        onComplete?.Invoke();
    }
}
