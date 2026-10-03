using UnityEngine;
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

    [Header("Dialogue Content")]
    [TextArea(2, 5)]
    public List<string> dialoguePhrases = new List<string>()
    {
        "What's up man! It's me, Anubis, Egyptian God of the dead!",

        "So basically, that means that you died. But you weren't good enough of a person to get to the afterlife!",

        "Good thing is, you came at a good time. I decided that being a God doesn't pay enough, so I want to create a start-up.",

        "And you're going to help me with that!",

        "People who have died are gonna start coming in soon. They'll tell you about their lives and what kind of person they were.",

        "I gave you a pen and some papyrus sheets. Unfortunately, because of budget cuts, you can only record four things about each person!",

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

    private void Start()
    {
        if (nextButton != null)
        {
            nextButton.onClick.AddListener(AdvanceDialogue);
        }

        StartIntroSequence();
    }

    public void StartIntroSequence()
    {
        currentPhraseIndex = 0;
        introActive = true;

        if (activeCharacter != null)
            activeCharacter.SetActive(true);

        if (speechBubbleUI != null)
            speechBubbleUI.SetActive(true);

        if (gameplayCanvas != null)
            gameplayCanvas.SetActive(false);

        DisplayCurrentPhrase();
    }

    private void DisplayCurrentPhrase()
    {
        if (currentPhraseIndex < dialoguePhrases.Count)
        {
            if (speechText != null)
                speechText.text = dialoguePhrases[currentPhraseIndex];
        }
        else
        {
            EndIntroSequence();
        }
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
        introActive = false;

        if (activeCharacter != null)
            activeCharacter.SetActive(false);

        if (speechBubbleUI != null)
            speechBubbleUI.SetActive(false);

        if (gameplayCanvas != null)
            gameplayCanvas.SetActive(true);

        GameManager.Instance.StartGameLoop();
    }

    private void Update()
    {
        if (!introActive)
            return;

        if (Input.GetMouseButtonDown(0))
        {
            AdvanceDialogue();
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