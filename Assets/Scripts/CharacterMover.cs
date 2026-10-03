using System.Collections;
using UnityEngine;

public class CharacterMover : MonoBehaviour
{
    public RectTransform activeCharacterRect;
    public float slideDuration = 0.5f;

    public UnityEngine.UI.Image activeCharacterImage;

    public float offScreenLeftX = -1200f; 
    public float targetScreenX = -270f;

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
        StartCoroutine(SlideRoutine(offScreenLeftX, targetScreenX, () => GameManager.Instance.OnCandidateEntered()));
    }

    public void SlideCharacterOut() {
        StartCoroutine(SlideRoutine(targetScreenX, offScreenLeftX, () => GameManager.Instance.OnCandidateExited()));
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
