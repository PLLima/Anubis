using System.Collections;
using UnityEngine;

public class CharacterSliding : MonoBehaviour
{
    public RectTransform activeCharacterRect;
    public float slideDuration = 0.5f;

    private float offScreenLeftX = -1200f; 
    private float targetScreenX = -400f;

    public void SlideCharacterIn() {
        StartCoroutine(SlideRoutine(offScreenLeftX, targetScreenX));
    }

    public void SlideCharacterOut() {
        StartCoroutine(SlideRoutine(targetScreenX, offScreenLeftX));
    }

    private IEnumerator SlideRoutine(float startX, float endX) {
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
    }
}
