using UnityEngine;
using TMPro;

public enum VoiceOverlapMode
{
    [Tooltip("Cuts off the previous sound immediately when the next one plays (Undertale / Animal Crossing style). Prevents overlap.")]
    CutOffPrevious,
    [Tooltip("Waits until the current sound finishes playing before triggering another (prevents overlap without cutting off audio).")]
    WaitUntilFinished,
    [Tooltip("Allows sounds to overlap and play on top of each other (PlayOneShot).")]
    Overlap
}

/// <summary>
/// Handles the typing animation and formatting of dialogue within the Speech Bubble.
/// Modularized to sit specifically on the SpeechBubblePanel.
/// </summary>
public class SpeechBubbleUI : MonoBehaviour
{
    [Header("Dialogue Dependencies")]
    public DialogueLinkHandler dialogueLinkHandler; 
    public TextMeshProUGUI dialogueTextComponent;
    public GameObject dialogueArrow;

    [Header("Typing Effect")]
    public float typingSpeed = 40f; // characters per second

    [Header("Audio / Gibberish Settings")]
    public AudioSource voiceAudioSource;

    [Tooltip("Controls how sounds behave so they don't pile up or run over each other.")]
    public VoiceOverlapMode overlapMode = VoiceOverlapMode.CutOffPrevious;

    [Range(0.08f, 0.6f)]
    [Tooltip("Minimum time (in seconds) between voice sounds. Higher value = much slower, calmer speech cadence (e.g. 0.18s to 0.25s).")]
    public float voiceSoundInterval = 0.20f;

    [Range(1, 10)]
    [Tooltip("Plays a voice sound every N characters (e.g. 4 or 5 for spaced-out talking).")]
    public int charactersPerSound = 4;

    [Tooltip("Voice profile used exclusively for Anubis.")]
    public VoiceProfile anubisVoice;
    [Tooltip("Default shared voice profile for NPCs. If an NPC leaves audio clips empty, it will reuse these clips with its own custom pitch!")]
    public VoiceProfile defaultNPCVoice;

    private Coroutine typingCoroutine;
    private NPCData currentSpeakerNPC;
    private VoiceProfile activeVoiceProfile;

    private void Awake()
    {
        if (voiceAudioSource == null)
        {
            voiceAudioSource = GetComponent<AudioSource>();
            if (voiceAudioSource == null)
            {
                voiceAudioSource = gameObject.AddComponent<AudioSource>();
            }
        }
        voiceAudioSource.playOnAwake = false;
    }

    private void OnEnable() 
    {
        GameManager.OnDialogueChanged += ShowDialogue;
        GameManager.OnNPCChanged += HandleNPCChanged;

        if (GameManager.Instance != null && GameManager.Instance.CurrentNPC != null)
        {
            currentSpeakerNPC = GameManager.Instance.CurrentNPC;
        }
    }

    private void OnDisable() 
    {
        GameManager.OnDialogueChanged -= ShowDialogue;
        GameManager.OnNPCChanged -= HandleNPCChanged;
    }

    private void HandleNPCChanged(NPCData npc)
    {
        currentSpeakerNPC = npc;
    }

    private void ShowDialogue(DialogueBubble bubble) 
    {
        if (dialogueTextComponent == null || dialogueLinkHandler == null || bubble == null) 
            return;

        // Ensure we always have the current candidate even if OnNPCChanged fired while this panel was inactive
        if (currentSpeakerNPC == null && GameManager.Instance != null)
        {
            currentSpeakerNPC = GameManager.Instance.CurrentNPC;
        }

        // When a candidate speaks, use their voice profile or fallback
        if (currentSpeakerNPC != null && currentSpeakerNPC.voiceProfile != null)
        {
            activeVoiceProfile = currentSpeakerNPC.voiceProfile;
        }
        else
        {
            activeVoiceProfile = defaultNPCVoice;
        }

        Debug.Log($"[SpeechBubbleUI] Candidato falando: {(currentSpeakerNPC != null ? currentSpeakerNPC.npcName : "Desconhecido")} | Base Pitch: {(activeVoiceProfile != null ? activeVoiceProfile.basePitch : 1f)}");

        if (typingCoroutine != null)
        {
            StopCoroutine(typingCoroutine);
        }
        typingCoroutine = StartCoroutine(TypeDialogue(bubble));
    }

    private System.Collections.IEnumerator TypeDialogue(DialogueBubble bubble)
    {
        if (dialogueArrow != null) dialogueArrow.SetActive(false);

        dialogueTextComponent.text = FormatDialogueForClicking(bubble);
        dialogueLinkHandler.SetCurrentBubble(bubble);
        dialogueTextComponent.ForceMeshUpdate();

        yield return StartCoroutine(RunTypingLoop());
    }

    /// <summary>
    /// Types a standard string without hyperlink formatting (used for Anubis).
    /// </summary>
    public void TypeStandardText(string text, bool showArrowAtEnd = true, System.Action onComplete = null)
    {
        if (dialogueTextComponent == null) return;
        
        // When Anubis speaks via standard text, use Anubis voice profile
        activeVoiceProfile = anubisVoice;

        if (typingCoroutine != null)
        {
            StopCoroutine(typingCoroutine);
        }
        typingCoroutine = StartCoroutine(TypeStandardCoroutine(text, showArrowAtEnd, onComplete));
    }

    private System.Collections.IEnumerator TypeStandardCoroutine(string text, bool showArrowAtEnd, System.Action onComplete)
    {
        if (dialogueArrow != null) dialogueArrow.SetActive(false);

        dialogueTextComponent.text = text;
        // Unbind any previous bubble links
        if (dialogueLinkHandler != null)
        {
            dialogueLinkHandler.SetCurrentBubble(null);
        }
        dialogueTextComponent.ForceMeshUpdate();

        yield return StartCoroutine(RunTypingLoop(showArrowAtEnd));
        
        onComplete?.Invoke();
    }

    private System.Collections.IEnumerator RunTypingLoop(bool showArrow = true)
    {
        int totalChars = dialogueTextComponent.textInfo.characterCount;
        if (dialogueLinkHandler != null) dialogueLinkHandler.SetVisibleCharacters(0);
        else dialogueTextComponent.maxVisibleCharacters = 0;

        float floatVisible = 0f;
        int lastVisibleChars = 0;
        int charCounter = 0;
        float lastSoundTime = -999f;

        while (floatVisible < totalChars)
        {
            if (GameManager.Instance != null && GameManager.Instance.WasClickedThisFrame())
            {
                if (voiceAudioSource != null && overlapMode == VoiceOverlapMode.CutOffPrevious)
                {
                    voiceAudioSource.Stop();
                }
                break; // Skip typing on click
            }

            floatVisible += Time.deltaTime * typingSpeed;
            int currentVisible = Mathf.FloorToInt(floatVisible);

            // Play voice blip as new visible characters appear
            if (currentVisible > lastVisibleChars)
            {
                int end = Mathf.Min(currentVisible, totalChars);
                for (int i = lastVisibleChars; i < end && i < dialogueTextComponent.textInfo.characterInfo.Length; i++)
                {
                    char c = dialogueTextComponent.textInfo.characterInfo[i].character;

                    // Skip whitespace/newlines
                    if (!char.IsWhiteSpace(c))
                    {
                        charCounter++;
                        int frequency = (activeVoiceProfile != null && activeVoiceProfile.charFrequency > 0)
                            ? activeVoiceProfile.charFrequency
                            : charactersPerSound;

                        float interval = (activeVoiceProfile != null && activeVoiceProfile.customInterval > 0f)
                            ? activeVoiceProfile.customInterval
                            : voiceSoundInterval;

                        if (charCounter >= frequency && (Time.time - lastSoundTime >= interval))
                        {
                            PlayGibberishSound(activeVoiceProfile);
                            charCounter = 0;
                            lastSoundTime = Time.time;
                        }
                    }
                }
                lastVisibleChars = currentVisible;
            }
            
            if (dialogueLinkHandler != null) dialogueLinkHandler.SetVisibleCharacters(currentVisible);
            else dialogueTextComponent.maxVisibleCharacters = currentVisible;
            
            yield return null;
        }

        if (dialogueLinkHandler != null) dialogueLinkHandler.SetVisibleCharacters(totalChars);
        else dialogueTextComponent.maxVisibleCharacters = totalChars;
        
        if (dialogueArrow != null && showArrow) dialogueArrow.SetActive(true);
    }

    private void PlayGibberishSound(VoiceProfile profile)
    {
        if (voiceAudioSource == null) return;

        AudioClip clip = profile != null ? profile.GetRandomClip() : null;

        // If this character doesn't have custom audio clips assigned, fallback to shared default NPC clips
        if (clip == null && defaultNPCVoice != null)
        {
            clip = defaultNPCVoice.GetRandomClip();
        }

        if (clip == null) return;

        float basePitch = profile != null ? profile.basePitch : 1.0f;
        if (basePitch <= 0.05f) basePitch = 1.0f;
        float variation = profile != null ? profile.pitchVariation : 0.05f;

        float pitchOffset = Random.Range(-variation, variation);
        voiceAudioSource.pitch = Mathf.Clamp(basePitch + pitchOffset, 0.2f, 3.0f);

        switch (overlapMode)
        {
            case VoiceOverlapMode.CutOffPrevious:
                voiceAudioSource.Stop();
                voiceAudioSource.clip = clip;
                voiceAudioSource.Play();
                break;

            case VoiceOverlapMode.WaitUntilFinished:
                if (!voiceAudioSource.isPlaying)
                {
                    voiceAudioSource.clip = clip;
                    voiceAudioSource.Play();
                }
                break;

            case VoiceOverlapMode.Overlap:
                voiceAudioSource.PlayOneShot(clip);
                break;
        }
    }

    private string FormatDialogueForClicking(DialogueBubble bubble) 
    {
        System.Text.StringBuilder combinedText = new System.Text.StringBuilder();
        for (int i = 0; i < bubble.snippets.Length; i++) 
        {
            combinedText.Append($"<link=\"{i}\"> {bubble.snippets[i]}</link> ");
        }
        return combinedText.ToString().TrimEnd();
    }
}
