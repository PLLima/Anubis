using UnityEngine;

/// <summary>
/// Defines voice audio clips and pitch settings for character gibberish speech.
/// </summary>
[System.Serializable]
public class VoiceProfile
{
    [Tooltip("One or more short audio clips (blips). If multiple, one will be chosen randomly per sound trigger.")]
    public AudioClip[] audioClips;

    [Range(0.5f, 2.0f)]
    [Tooltip("Base pitch of the voice. Values < 1.0 are deeper/lower, > 1.0 are higher-pitched.")]
    public float basePitch = 1.0f;

    [Range(0f, 0.2f)]
    [Tooltip("Subtle random pitch variation applied to each blip to make the voice feel organic.")]
    public float pitchVariation = 0.05f;

    [Range(0, 10)]
    [Tooltip("Triggers a voice sound every N non-whitespace characters. Set to 0 to use the global SpeechBubbleUI setting.")]
    public int charFrequency = 0;

    [Range(0f, 0.5f)]
    [Tooltip("Optional custom time interval (in seconds) between sounds for this specific character. Set to 0 to use the global SpeechBubbleUI interval.")]
    public float customInterval = 0f;

    public AudioClip GetRandomClip()
    {
        if (audioClips == null || audioClips.Length == 0) return null;
        return audioClips[Random.Range(0, audioClips.Length)];
    }
}
