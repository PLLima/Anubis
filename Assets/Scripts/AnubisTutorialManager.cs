
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine.UI;

public class AnubisTutorialManager : MonoBehaviour
{
    [Header("UI References")]
    public GameObject activeCharacter;
    public GameObject speechBubbleUI;
    public TMP_Text speechText;
    public Button nextButton;
    public GameObject gameplayCanvas;

    [Header("Tutorial Character")]
    public Sprite anubisSprite;

    [Header("Character Movement")]
    public RectTransform activeCharacterRect;
    public float slideDuration = 0.5f;
    public float offScreenLeftX = -1200f;
    public float targetScreenX = -400f;

    [Header("Dialogue Content")]
    [TextArea(2, 5)]
    public List<string> dialoguePhrases = new List<string>()
    {
        "What's up man! It's me, Anubis, Egyptian God of the dead!",

        "So basically, that means that you died. But you weren't good enough of a person to get to the afterlife!",

        "Good thing is, you came at a good time. I decided that being a God doesn't pay enough, so I want to create a start-up.",

        "And you're going to help me with that!",

        "People who have died are gonna start coming in soon. They'll tell you about their lives and what kind of person they were.",

        "I gave you a pen and some papyrus sheets.",

        "Unfortunately, because of budget cuts, you can only record four things about each person!",

        "When people are talking, click on the things you think are important to write them down.",

        "Be careful! You can't ask them to repeat themselves, so think hard about what you write down!",

        "I'm a busy guy, since I'm a God and stuff, so I have to go think about my start-up idea.",

        "I'll come back later to tell you what kind of person I need.",

        "If you can give me someone who fits the position, I'll think about letting you into the afterlife.",

        "But if you keep sending me people who are useless, I'll have to feed your heart to Ammit!",

        "Alright, I have to go do my God duties. I'll see you later!"
    };

    private int currentPhraseIndex = 0;
    private bool introActive = false;
    private Coroutine typingCoroutine;

    // Typing speed is now controlled by the SpeechBubbleUI script to respect modularity
    private void Start()
    {
        if (nextButton != null)
        {
            nextButton.onClick.AddListener(AdvanceDialogue);
        }

        CharacterMover.OnAnubisEnterFinished += HandleAnubisEnterFinished;
        GameManager.OnStateChanged += HandleStateChange;
    }

    private void OnDestroy()
    {
        if (nextButton != null)
        {
            nextButton.onClick.RemoveListener(AdvanceDialogue);
        }
        CharacterMover.OnAnubisEnterFinished -= HandleAnubisEnterFinished;
        GameManager.OnStateChanged -= HandleStateChange;
    }

    private void HandleStateChange(GameState state)
    {
        if (state == GameState.AnubisIntro)
        {
            StartIntroSequence();
        }
    }

    public void StartIntroSequence()
    {
        currentPhraseIndex = 0;
        introActive = true;

        // Make sure Active Character exists.
        if (activeCharacter == null)
        {
            Debug.LogError("AnubisTutorialManager: Active Character is not assigned!");
            return;
        }

        // Make sure RectTransform exists.
        if (activeCharacterRect == null)
        {
            activeCharacterRect = activeCharacter.GetComponent<RectTransform>();
        }

        if (activeCharacterRect == null)
        {
            Debug.LogError("AnubisTutorialManager: Active Character needs a RectTransform!");
            return;
        }

        // Show Active Character.
        activeCharacter.SetActive(true);

        // Change sprite to Anubis.
        Image characterImage = activeCharacter.GetComponent<Image>();

        if (characterImage != null && anubisSprite != null)
        {
            characterImage.sprite = anubisSprite;
        }
        else if (characterImage == null)
        {
            Debug.LogError("AnubisTutorialManager: Active Character does not have an Image component!");
        }
        else if (anubisSprite == null)
        {
            Debug.LogError("AnubisTutorialManager: Anubis Sprite is not assigned!");
        }

        // We DO NOT start the dialogue or slide here!
        // CharacterMover will automatically detect the GameState.AnubisIntro and start the animation!
    }

    private void HandleAnubisEnterFinished()
    {
        if (speechBubbleUI != null)
        {
            speechBubbleUI.SetActive(true);
        }
        DisplayCurrentPhrase();
    }

    private void DisplayCurrentPhrase()
    {
        if (currentPhraseIndex < dialoguePhrases.Count)
        {
            if (speechText != null)
            {
                if (typingCoroutine != null)
                {
                    StopCoroutine(typingCoroutine);
                }
                typingCoroutine = StartCoroutine(TypeDialogue(dialoguePhrases[currentPhraseIndex]));
            }
        }
        else
        {
            EndIntroSequence();
        }
    }

    private System.Collections.IEnumerator TypeDialogue(string textToType)
    {
        if (nextButton != null) nextButton.gameObject.SetActive(false);

        speechText.text = textToType;
        speechText.ForceMeshUpdate();

        int totalChars = speechText.textInfo.characterCount;
        speechText.maxVisibleCharacters = 0;

        float floatVisible = 0f;
        
        // Fetch the global typing speed from the SpeechBubbleUI script
        float currentTypingSpeed = 40f;
        if (speechBubbleUI != null)
        {
            var uiComponent = speechBubbleUI.GetComponent<SpeechBubbleUI>();
            if (uiComponent != null)
            {
                currentTypingSpeed = uiComponent.typingSpeed;
            }
        }

        while (floatVisible < totalChars)
        {
            if (GameManager.Instance != null && GameManager.Instance.WasClickedThisFrame())
            {
                break; // skip typing
            }
            floatVisible += Time.deltaTime * currentTypingSpeed;
            speechText.maxVisibleCharacters = Mathf.FloorToInt(floatVisible);
            yield return null;
        }

        speechText.maxVisibleCharacters = totalChars;
        if (nextButton != null) nextButton.gameObject.SetActive(true);
    }

    public void AdvanceDialogue()
    {
        if (!introActive)
            return;

        currentPhraseIndex++;
        DisplayCurrentPhrase();
    }

    private void EndIntroSequence()
    {
        if (!introActive)
            return;

        introActive = false;

        //if (activeCharacter != null)
        //{
        //    activeCharacter.SetActive(false);
        //}

        if (speechBubbleUI != null)
        {
            speechBubbleUI.SetActive(false);
        }

        if (gameplayCanvas != null)
        {
            gameplayCanvas.SetActive(true);
        }

        if (GameManager.Instance != null)
        {
            GameManager.Instance.StartGameLoop();
        }
    }

    // Update method removed to prevent clicking background to advance

}

