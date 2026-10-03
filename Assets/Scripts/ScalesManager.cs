using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ScaleManager : MonoBehaviour
{
    [Header("Scale Image")]
    public Image scaleImage;

    [Header("Scale Sprites")]
    public Sprite neutralSprite;
    public Sprite winSprite;
    public Sprite lossSprite;

    [Header("Black Overlay")]
    public Image blackOverlay;

    [Header("Animation Settings")]
    public float blackFadeDuration = 1f;
    public float scaleFallDuration = 1f;
    public float resultFadeDuration = 1f;
    public float finalFadeDuration = 1f;

    public float pauseAfterNeutral = 1f;
    public float pauseAfterResult = 2.0f;

    private RectTransform scaleRect;
    private Vector2 scaleStartPosition;
    private Vector2 scaleFinalPosition;

    private Coroutine animationCoroutine;

    private void Awake()
    {
        // Force Canvas to finalize layout positions before reading anchoredPosition
        Canvas.ForceUpdateCanvases();

        if (scaleImage != null)
        {
            scaleRect = scaleImage.GetComponent<RectTransform>();

            scaleFinalPosition = scaleRect.anchoredPosition;

            // Start the scale above the screen.
            scaleStartPosition = scaleFinalPosition;
            scaleStartPosition.y += 800f;
        }

        // Start with everything invisible.
        if (blackOverlay != null)
        {
            Color color = blackOverlay.color;
            color.a = 0f;
            blackOverlay.color = color;
        }

        if (scaleImage != null)
        {
            Color color = scaleImage.color;
            color.a = 0f;
            scaleImage.color = color;
        }
    }

    private void OnEnable()
    {
        GameManager.OnDeliberationSubmitted += PlayScaleEvaluation;
    }

    private void OnDisable()
    {
        GameManager.OnDeliberationSubmitted -= PlayScaleEvaluation;
    }

    public void PlayScaleEvaluation(bool playerWasCorrect)
    {
        if (animationCoroutine != null)
        {
            StopCoroutine(animationCoroutine);
        }

        animationCoroutine =
            StartCoroutine(
                ScaleEvaluationSequence(playerWasCorrect)
            );
    }

    private IEnumerator ScaleEvaluationSequence(bool playerWasCorrect)
    {
        // ----------------------------------------
        // 1. BLACK OVERLAY FADES IN TO 50%
        // ----------------------------------------

        yield return StartCoroutine(
            FadeOverlay(0f, 0.5f, blackFadeDuration)
        );


        // ----------------------------------------
        // 2. SET SCALE TO NEUTRAL
        // ----------------------------------------

        if (scaleImage != null)
        {
            scaleImage.sprite = neutralSprite;

            scaleRect.anchoredPosition =
                scaleStartPosition;

            Color color = scaleImage.color;
            color.a = 1f;
            scaleImage.color = color;
        }


        // ----------------------------------------
        // 3. SCALE FALLS INTO PLACE
        // ----------------------------------------

        yield return StartCoroutine(
            FallScale()
        );


        // Small pause while neutral scale is visible.
        yield return new WaitForSeconds(
            pauseAfterNeutral
        );


        // ----------------------------------------
        // 4. FADE NEUTRAL → RESULT
        // ----------------------------------------

        Sprite resultSprite =
            playerWasCorrect
                ? winSprite
                : lossSprite;

        yield return StartCoroutine(
            FadeToResult(resultSprite)
        );


        // Let player see result.
        yield return new WaitForSeconds(
            pauseAfterResult
        );


        // ----------------------------------------
        // 5. OVERLAY FADES OUT
        // ----------------------------------------

        yield return StartCoroutine(
            FadeOverlayOut()
        );

        if (GameManager.Instance != null)
        {
            GameManager.Instance.ChangeState(GameState.EndGame);
        }
    }

    private IEnumerator FadeOverlay(
        float startAlpha,
        float targetAlpha,
        float duration
    )
    {
        if (blackOverlay == null)
            yield break;

        Color color = blackOverlay.color;

        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;

            float t =
                Mathf.SmoothStep(
                    0f,
                    1f,
                    elapsed / duration
                );

            color.a =
                Mathf.Lerp(
                    startAlpha,
                    targetAlpha,
                    t
                );

            blackOverlay.color = color;

            yield return null;
        }

        color.a = targetAlpha;
        blackOverlay.color = color;
    }

    private IEnumerator FallScale()
    {
        if (scaleRect == null)
            yield break;

        float elapsed = 0f;

        while (elapsed < scaleFallDuration)
        {
            elapsed += Time.deltaTime;

            float t =
                Mathf.SmoothStep(
                    0f,
                    1f,
                    elapsed / scaleFallDuration
                );

            scaleRect.anchoredPosition =
                Vector2.Lerp(
                    scaleStartPosition,
                    scaleFinalPosition,
                    t
                );

            yield return null;
        }

        scaleRect.anchoredPosition =
            scaleFinalPosition;
    }

    private IEnumerator FadeToResult(Sprite resultSprite)
    {
        if (scaleImage == null)
            yield break;

        // Create a temporary result Image
        // on top of the neutral scale.
        GameObject resultObject =
            new GameObject("ResultScale");

        resultObject.transform.SetParent(
            scaleImage.transform.parent,
            false
        );

        Image resultImage =
            resultObject.AddComponent<Image>();

        resultImage.sprite = resultSprite;

        RectTransform resultRect =
            resultImage.rectTransform;

        resultRect.anchorMin =
            scaleImage.rectTransform.anchorMin;

        resultRect.anchorMax =
            scaleImage.rectTransform.anchorMax;

        resultRect.pivot =
            scaleImage.rectTransform.pivot;

        resultRect.anchoredPosition =
            scaleImage.rectTransform.anchoredPosition;

        resultRect.sizeDelta =
            scaleImage.rectTransform.sizeDelta;

        // Result starts invisible.
        Color resultColor =
            resultImage.color;

        resultColor.a = 0f;

        resultImage.color = resultColor;


        float elapsed = 0f;

        while (elapsed < resultFadeDuration)
        {
            elapsed += Time.deltaTime;

            float t =
                Mathf.SmoothStep(
                    0f,
                    1f,
                    elapsed / resultFadeDuration
                );

            // Neutral fades out.
            Color neutralColor =
                scaleImage.color;

            neutralColor.a =
                Mathf.Lerp(
                    1f,
                    0f,
                    t
                );

            scaleImage.color =
                neutralColor;


            // Result fades in.
            resultColor.a =
                Mathf.Lerp(
                    0f,
                    1f,
                    t
                );

            resultImage.color =
                resultColor;

            yield return null;
        }


        // Make result fully visible.
        resultColor.a = 1f;
        resultImage.color = resultColor;

        // Remove neutral scale.
        scaleImage.color =
            new Color(
                1f,
                1f,
                1f,
                0f
            );


        // Replace the main scale sprite.
        scaleImage.sprite =
            resultSprite;

        Color finalColor =
            scaleImage.color;

        finalColor.a = 1f;

        scaleImage.color =
            finalColor;


        Destroy(resultObject);
    }

    private IEnumerator FadeOverlayOut()
    {
        float elapsed = 0f;

        Color overlayColor =
            blackOverlay != null
                ? blackOverlay.color
                : Color.clear;

        while (elapsed < finalFadeDuration)
        {
            elapsed += Time.deltaTime;

            float t =
                Mathf.SmoothStep(
                    0f,
                    1f,
                    elapsed / finalFadeDuration
                );

            if (blackOverlay != null)
            {
                Color color = overlayColor;
                color.a =
                    Mathf.Lerp(
                        0.5f,
                        0f,
                        t
                    );

                blackOverlay.color = color;
            }

            yield return null;
        }

        if (blackOverlay != null)
        {
            Color color = blackOverlay.color;
            color.a = 0f;
            blackOverlay.color = color;
        }
    }

    public void SetNeutral()
    {
        if (scaleImage != null && neutralSprite != null)
        {
            scaleImage.sprite = neutralSprite;
        }
    }

    public void SetWin()
    {
        if (scaleImage != null && winSprite != null)
        {
            scaleImage.sprite = winSprite;
        }
    }

    public void SetLoss()
    {
        if (scaleImage != null && lossSprite != null)
        {
            scaleImage.sprite = lossSprite;
        }
    }
}