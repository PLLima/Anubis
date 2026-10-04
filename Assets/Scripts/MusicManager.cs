using System.Collections;
using UnityEngine;

/// <summary>
/// Manages the 3 theme musics (Title Screen, Gameplay, Game Over)
/// and handles smooth audio transitions (crossfade) between game states.
/// </summary>
public class MusicManager : MonoBehaviour
{
    public static MusicManager Instance { get; private set; }

    [Header("Audio Source")]
    [Tooltip("The AudioSource used to play the music. If left empty, one will be fetched or added automatically.")]
    public AudioSource audioSource;

    [Header("Music Themes")]
    [Tooltip("Music played on the Title Screen / Main Menu.")]
    public AudioClip titleMusic;

    [Tooltip("Music played during gameplay (Anubis intro, candidate interviews, deliberation, etc.).")]
    public AudioClip gameplayMusic;

    [Tooltip("Music played on the Game Over screen.")]
    public AudioClip gameOverMusic;

    [Header("Settings")]
    [Range(0f, 1f)]
    [Tooltip("Volume of the background music.")]
    public float musicVolume = 0.5f;

    [Range(0f, 3f)]
    [Tooltip("Duration of the crossfade transition in seconds. Set to 0 for instant cut.")]
    public float crossfadeDuration = 0.8f;

    [Tooltip("Whether the Game Over music should loop or play only once.")]
    public bool loopGameOverMusic = true;

    private Coroutine transitionCoroutine;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        if (audioSource == null)
        {
            audioSource = GetComponent<AudioSource>();
            if (audioSource == null)
            {
                audioSource = gameObject.AddComponent<AudioSource>();
            }
        }

        // Configure music AudioSource defaults
        audioSource.loop = true;
        audioSource.playOnAwake = false;
        audioSource.pitch = 1.0f;
        audioSource.volume = musicVolume;
    }

    private void OnEnable()
    {
        GameManager.OnStateChanged += HandleStateChange;
    }

    private void OnDisable()
    {
        GameManager.OnStateChanged -= HandleStateChange;
    }

    private void Start()
    {
        // On game start, play music according to current state (usually TitleScreen)
        if (GameManager.Instance != null)
        {
            HandleStateChange(GameManager.Instance.CurrentState);
        }
        else if (titleMusic != null)
        {
            PlayTrack(titleMusic, true);
        }
    }

    private void HandleStateChange(GameState state)
    {
        switch (state)
        {
            case GameState.TitleScreen:
                PlayTrack(titleMusic, true);
                break;

            case GameState.GameOver:
                PlayTrack(gameOverMusic, loopGameOverMusic);
                break;

            default:
                // All other states belong to gameplay
                PlayTrack(gameplayMusic, true);
                break;
        }
    }

    /// <summary>
    /// Switches to the specified music track with a smooth crossfade.
    /// If the track is already playing, it will keep playing uninterrupted.
    /// </summary>
    public void PlayTrack(AudioClip targetClip, bool loop = true)
    {
        if (targetClip == null) return;

        // If this track is already playing, don't restart it
        if (audioSource.clip == targetClip && audioSource.isPlaying)
        {
            audioSource.loop = loop;
            return;
        }

        if (transitionCoroutine != null)
        {
            StopCoroutine(transitionCoroutine);
        }

        if (crossfadeDuration > 0f && audioSource.isPlaying)
        {
            transitionCoroutine = StartCoroutine(CrossfadeRoutine(targetClip, loop));
        }
        else
        {
            audioSource.clip = targetClip;
            audioSource.loop = loop;
            audioSource.volume = musicVolume;
            audioSource.Play();
        }
    }

    private IEnumerator CrossfadeRoutine(AudioClip newClip, bool loop)
    {
        float halfDuration = crossfadeDuration * 0.5f;
        float startVolume = audioSource.volume;

        // Fade Out
        float elapsed = 0f;
        while (elapsed < halfDuration)
        {
            elapsed += Time.unscaledDeltaTime;
            audioSource.volume = Mathf.Lerp(startVolume, 0f, elapsed / halfDuration);
            yield return null;
        }

        audioSource.volume = 0f;
        audioSource.clip = newClip;
        audioSource.loop = loop;
        audioSource.Play();

        // Fade In
        elapsed = 0f;
        while (elapsed < halfDuration)
        {
            elapsed += Time.unscaledDeltaTime;
            audioSource.volume = Mathf.Lerp(0f, musicVolume, elapsed / halfDuration);
            yield return null;
        }

        audioSource.volume = musicVolume;
        transitionCoroutine = null;
    }

    /// <summary>
    /// Changes the global music volume smoothly.
    /// </summary>
    public void SetVolume(float volume)
    {
        musicVolume = Mathf.Clamp01(volume);
        if (transitionCoroutine == null && audioSource != null)
        {
            audioSource.volume = musicVolume;
        }
    }
}
