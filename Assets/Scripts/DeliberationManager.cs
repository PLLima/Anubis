using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine.UI;

public class DeliberationManager : MonoBehaviour
{
    [Header("UI References")]
    public GameObject activeCharacter;
    public GameObject speechBubbleUI;
    public TMP_Text speechText;
    public Button nextButton;

    [Header("Anubis")]
    public Sprite anubisSprite;

    [Header("Anubis Exit Animation (keep in sync with CharacterMover)")]
    public float offScreenLeftX = -1200f;
    public float targetScreenX = -270f;
    public float waitBeforeDropTime = 0.4f;
    public float dropDuration = 0.2f;
    public float slideDuration = 0.5f;

    [Header("Audio Settings")]
    public AudioSource footstepSource;
    public AudioClip footstepClip;

    [Header("Papyrus Mechanics")]
    public DeliberationPapyrusManager papyrusManager;

    private RectTransform anubisRect;
    private float baseY;

    private int currentPhraseIndex = 0;
    private bool deliberationActive = false;
    private List<string> dialoguePhrases = new List<string>();

    // Verdict (post-judgment) state
    private bool verdictActive = false;   // verdict text is on screen
    private bool verdictReady = false;    // verdict finished typing, arrow click will make Anubis leave
    private bool isLeaving = false;       // Anubis is currently sliding out

    private void Awake()
    {
        if (activeCharacter != null)
        {
            anubisRect = activeCharacter.GetComponent<RectTransform>();
            if (anubisRect != null) baseY = anubisRect.anchoredPosition.y;
        }

        if (footstepSource == null)
        {
            footstepSource = GetComponent<AudioSource>();
        }
    }

    private void OnEnable()
    {
        GameManager.OnStateChanged += HandleStateChanged;
        CharacterMover.OnAnubisEnterFinished += HandleAnubisEnterFinished;

        if (nextButton != null)
        {
            nextButton.onClick.AddListener(AdvanceDialogue);
        }
    }

    private void OnDisable()
    {
        GameManager.OnStateChanged -= HandleStateChanged;
        CharacterMover.OnAnubisEnterFinished -= HandleAnubisEnterFinished;

        if (nextButton != null)
        {
            nextButton.onClick.RemoveListener(AdvanceDialogue);
        }
    }

    private void HandleStateChanged(GameState newState)
    {
        if (newState == GameState.Deliberation)
        {
            StartDeliberationLogic();
        }
        else if (newState == GameState.EndGame)
        {
            StartEndGameLogic();
        }
    }

    private void StartDeliberationLogic()
    {
        if (deliberationActive) return;
        deliberationActive = true;
        verdictActive = false;
        verdictReady = false;
        isLeaving = false;
        currentPhraseIndex = 0;

        // Clear text
        if (speechText != null) speechText.text = string.Empty;

        // Load dialogue from LevelData
        dialoguePhrases.Clear();
        if (GameManager.Instance != null && GameManager.Instance.currentLevel != null && GameManager.Instance.currentLevel.deliberationDialogue != null)
        {
            dialoguePhrases.AddRange(GameManager.Instance.currentLevel.deliberationDialogue);
        }

        // Fallback if level data has no dialogue
        if (dialoguePhrases.Count == 0)
        {
            dialoguePhrases.Add("Yo dude, I'm back! Drag the papyrus of who you think would do the best job to me.");
        }

        // Set Anubis Sprite
        if (activeCharacter != null)
        {
            activeCharacter.SetActive(true);
            Image characterImage = activeCharacter.GetComponent<Image>();
            if (characterImage != null && anubisSprite != null)
            {
                characterImage.sprite = anubisSprite;
            }
        }

        // CharacterMover will automatically slide Anubis in,
        // which triggers HandleAnubisEnterFinished when done.
    }

    private void StartEndGameLogic()
    {
        deliberationActive = false;
        verdictActive = true;
        verdictReady = false;
        isLeaving = false;

        bool won = GameManager.Instance.LastDeliberationResult;
        string text = won
            ? "This person is perfect! Thanks a lot, I'll take some weight off your soul for you!"
            : "This person is completely useless! I'm adding more weight to your soul! Pay more attention next time!";

        if (activeCharacter != null)
        {
            activeCharacter.SetActive(true);
            Image characterImage = activeCharacter.GetComponent<Image>();
            if (characterImage != null && anubisSprite != null)
            {
                characterImage.sprite = anubisSprite;
            }
        }

        if (speechBubbleUI != null)
        {
            speechBubbleUI.SetActive(true);
            var bubbleComponent = speechBubbleUI.GetComponent<SpeechBubbleUI>();
            if (bubbleComponent != null)
            {
                // 'true' keeps the next arrow visible; it becomes the "Anubis leaves" button.
                bubbleComponent.TypeStandardText(text, true, () => verdictReady = true);
            }
            else
            {
                if (speechText != null) speechText.text = text;
                verdictReady = true;
            }
        }
        else
        {
            verdictReady = true;
        }
    }

    private void HandleAnubisEnterFinished()
    {
        // Only trigger dialogue typing if we are actually in the Deliberation state
        if (GameManager.Instance != null && GameManager.Instance.CurrentState == GameState.Deliberation)
        {
            if (speechBubbleUI != null) speechBubbleUI.SetActive(true);
            DisplayCurrentPhrase();
        }
    }

    public void AdvanceDialogue()
    {
        // Verdict screen: the arrow sends Anubis away and starts the next level
        if (verdictActive)
        {
            if (!isLeaving)
            {
                StartCoroutine(AnubisLeaveRoutine());
            }
            return;
        }

        if (!deliberationActive) return;

        if (currentPhraseIndex >= dialoguePhrases.Count - 1)
        {
            // Reached the end of the deliberation dialogue.
            // In the future, this might enable dragging or transition state.
            return;
        }

        currentPhraseIndex++;
        DisplayCurrentPhrase();
    }

    // Same animation CharacterMover uses for exits: revert drop, wait, slide off screen left.
    private IEnumerator AnubisLeaveRoutine()
    {
        isLeaving = true;

        if (speechBubbleUI != null) speechBubbleUI.SetActive(false);

        if (anubisRect != null)
        {
            yield return StartCoroutine(AnimateYOffset(0f)); // Revert drop
            yield return new WaitForSeconds(waitBeforeDropTime);
            yield return StartCoroutine(SlideRoutine(targetScreenX, offScreenLeftX));
        }

        verdictActive = false;
        verdictReady = false;
        isLeaving = false;

        // Starts the next random level, or finishes the run if none are left
        if (GameManager.Instance != null)
        {
            GameManager.Instance.AdvanceToNextLevel();
        }
    }

    private IEnumerator AnimateYOffset(float targetOffset)
    {
        Vector2 startPos = anubisRect.anchoredPosition;
        Vector2 endPos = new Vector2(startPos.x, baseY + targetOffset);

        float timeElapsed = 0;
        while (timeElapsed < dropDuration)
        {
            timeElapsed += Time.deltaTime;
            float t = Mathf.SmoothStep(0, 1, timeElapsed / dropDuration);
            anubisRect.anchoredPosition = Vector2.Lerp(startPos, endPos, t);
            yield return null;
        }
        anubisRect.anchoredPosition = endPos;
    }

    private IEnumerator SlideRoutine(float startX, float endX)
    {
        if (footstepSource != null && footstepClip != null)
        {
            footstepSource.clip = footstepClip;
            footstepSource.loop = true;
            footstepSource.Play();
        }

        float timeElapsed = 0;
        Vector2 pos = anubisRect.anchoredPosition;
        pos.x = startX;
        anubisRect.anchoredPosition = pos;

        while (timeElapsed < slideDuration)
        {
            timeElapsed += Time.deltaTime;
            float t = Mathf.SmoothStep(0, 1, timeElapsed / slideDuration);
            pos.x = Mathf.Lerp(startX, endX, t);
            anubisRect.anchoredPosition = pos;
            yield return null;
        }

        pos.x = endX;
        anubisRect.anchoredPosition = pos;

        if (footstepSource != null)
        {
            footstepSource.Stop();
        }
    }

    private void DisplayCurrentPhrase()
    {
        if (speechText == null) return;

        if (currentPhraseIndex < dialoguePhrases.Count)
        {
            bool isLastPhrase = currentPhraseIndex == (dialoguePhrases.Count - 1);

            if (speechBubbleUI != null)
            {
                var bubbleComponent = speechBubbleUI.GetComponent<SpeechBubbleUI>();
                if (bubbleComponent != null)
                {
                    bubbleComponent.TypeStandardText(dialoguePhrases[currentPhraseIndex], !isLastPhrase, () => {
                        if (isLastPhrase && papyrusManager != null)
                        {
                            papyrusManager.SpawnPapyruses();
                        }
                    });
                    return;
                }
            }

            speechText.text = dialoguePhrases[currentPhraseIndex];

            if (isLastPhrase && papyrusManager != null)
            {
                papyrusManager.SpawnPapyruses();
            }
        }
    }
}