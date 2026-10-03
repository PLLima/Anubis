using UnityEngine;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using Unity.Tutorials.Editor;
using UnityEngine.UI;

public class AnubisTutorialManager : MonoBehaviour
{

    [Header("UI References")]
    public GameObject activeCharacter;
    public GameObject speechBubbleUI;
    public TMP_Text speechText;
    public Button nextButton;
    public GameObject gameplayCanvas;

    [Header("Scripts to Enable")]
    ClickeableText dialogueTextScript;

    [Header("Dialogue Content")]
    [LocalizableTextArea(2, 5)]
    public List<string> dialoguePhrases = new List<string>()
    {
        "What's up man! It's me, Anubis, Egyptian God of the dead!",
        "So basically, that means that you died. But you weren't good enough of a person to get to the afterlife!",
        "Good thing is, you came at a good time. I decided that being a God doesn't pay enough, so I want to create a start-up.",
        "And you're going to help me with that!",
        "People who have died are gonna start coming in soon. They'll tell you about their lives and what kind of person they are.",
        "I gave you a pen and papyrus sheets. Unfortunately, because of budget cuts, we only have enough ink and papyrus so that you can record four words per person!",
        "When people are talking, just click on some of the words they're saying to write them down.",
        "Be careful! You can't ask them to repeat themselves, so think hard about what to write down!",
        "I'm a busy guy, since I'm a God and stuff, so I have to go now to think of my start-up idea.",
        "I'll come back later to tell you what kind of guy I need.",
        "If you can give me someone who fits the position, I'll think about letting you into the afterlife.",
        "But, if keep sending me people who are useless, I'll have to feed your heart to Ammit!",
        "Alright, I have to go do my God duties, I'll see you later!"
    };

    private int currentPhraseIndex = 0;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
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
        activeCharacter.SetActive(true);
        speechBubbleUI.SetActive(true);

        if (gameplayCanvas != null)
            gameplayCanvas.SetActive(false);

        DisplayCurrentPhrase();
    }

    void DisplayCurrentPhrase()
    {
        if (currentPhraseIndex < dialoguePhrases.Count)
        {
            speechText.text = dialoguePhrases[currentPhraseIndex];
        }
        else
        {
            EndIntroSequence();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (speechBubbleUI.activeSelf && Input.GetMouseButtonDown(0))
        {
            AdvanceDialogue();
        }
    }
}
