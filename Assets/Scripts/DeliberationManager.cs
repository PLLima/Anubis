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

    [Header("Character Movement")]
    public RectTransform activeCharacterRect;
    public float slideDuration = 0.5f;
    public float offScreenLeftX = -1200f;
    public float targetScreenX = -400f;

    [Header("Dialogue")]
    [TextArea(2, 5)]
    public List<string> dialoguePhrases = new List<string>()
    {
        "Yo dude, I'm back! Did you interview everyone?",
        "I've got a great idea for a start-up.",
        "I'm going to start a pyramid construction company! Drag the papyrus of who you think would do the best job to me."
    };

    private int currentPhraseIndex = 0;
    private bool deliberationActive = false;
    private Coroutine slideCoroutine;

    private void Start()
    {
        // Clear any dialogue left over from the previous NPC.
        if (speechText != null)
        {
            speechText.text = "";
        }

        // Connect the Next button.
        if (nextButton != null)
        {
            nextButton.onClick.AddListener(AdvanceDialogue);
        }
    }

    private void OnEnable()
    {
        GameManager.OnStateChanged += HandleStateChanged;
    }

    private void OnDisable()
    {
        GameManager.OnStateChanged -= HandleStateChanged;
    }

    private void HandleStateChanged(GameState newState)
    {
        if (newState == GameState.Deliberation)
        {
            StartDeliberation();
        }
    }


    private void StartDeliberation()
    {
        if (deliberationActive)
            return;

        deliberationActive = true;
        currentPhraseIndex = 0;

        // 1. Force clear text first
        if (speechText != null)
        {
            speechText.text = string.Empty;
        }

        // 2. Setup Anubis sprite
        if (activeCharacter != null)
        {
            activeCharacter.SetActive(true);
        }

        if (activeCharacterRect == null && activeCharacter != null)
        {
            activeCharacterRect = activeCharacter.GetComponent<RectTransform>();
        }

        if (activeCharacterRect == null)
        {
            Debug.LogError("DeliberationManager: Active Character needs a RectTransform.");
            return;
        }

        Image characterImage = activeCharacter.GetComponent<Image>();
        if (characterImage != null && anubisSprite != null)
        {
            characterImage.sprite = anubisSprite;
        }

        // 3. Set off-screen starting position
        Vector2 startPosition = activeCharacterRect.anchoredPosition;
        startPosition.x = offScreenLeftX;
        activeCharacterRect.anchoredPosition = startPosition;

        // 4. Show speech bubble & set Anubis's first phrase
        if (speechBubbleUI != null)
        {
            speechBubbleUI.SetActive(true);
        }

        DisplayCurrentPhrase(); // Display "Yo dude, I'm back!..."

        // 5. Start sliding animation
        if (slideCoroutine != null)
        {
            StopCoroutine(slideCoroutine);
        }

        slideCoroutine = StartCoroutine(SlideAnubisIn());
    }

    private IEnumerator SlideAnubisIn()
    {

        float timeElapsed = 0f;

        Vector2 position =
            activeCharacterRect.anchoredPosition;

        position.x = offScreenLeftX;

        activeCharacterRect.anchoredPosition = position;

        while (timeElapsed < slideDuration)
        {
            timeElapsed += Time.deltaTime;

            float t =
                Mathf.SmoothStep(
                    0f,
                    1f,
                    timeElapsed / slideDuration
                );

            position.x =
                Mathf.Lerp(
                    offScreenLeftX,
                    targetScreenX,
                    t
                );

            activeCharacterRect.anchoredPosition = position;

            yield return null;
        }

        position.x = targetScreenX;
        activeCharacterRect.anchoredPosition = position;
    }

    public void AdvanceDialogue()
    {
        if (!deliberationActive)
            return;

        // If we're already on the final sentence,
        // don't advance anywhere yet.
        if (currentPhraseIndex >= dialoguePhrases.Count - 1)
        {
            return;
        }

        currentPhraseIndex++;

        DisplayCurrentPhrase();
    }

    private void DisplayCurrentPhrase()
    {
        if (speechText == null)
            return;

        if (currentPhraseIndex < dialoguePhrases.Count)
        {
            speechText.text =
                dialoguePhrases[currentPhraseIndex];
        }
    }

    private void OnDestroy()
    {
        if (nextButton != null)
        {
            nextButton.onClick.RemoveListener(AdvanceDialogue);
        }
    }


}