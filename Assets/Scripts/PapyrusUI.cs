using System.Collections;
using TMPro;
using UnityEngine;

/// <summary>
/// Handles appending text clues to the Papyrus UI and clearing it when necessary.
/// </summary>
[RequireComponent(typeof(TextMeshProUGUI))]
[RequireComponent(typeof(AudioSource))]
public class PapyrusUI : MonoBehaviour
{
    private TextMeshProUGUI papyrusText;
    private Coroutine typingCoroutine;

    [Header("Audio Settings")]
    public AudioSource audioSource;
    public AudioClip clickSound;

    [Header("Typing Effect")]
    public float typingSpeed = 40f; // characters per second

    private void Awake()
    {
        papyrusText = GetComponent<TextMeshProUGUI>();
        
        if (audioSource == null) 
        {
            audioSource = GetComponent<AudioSource>();
        }

        if (papyrusText != null)
        {
            papyrusText.color = Color.black;
        }

        if (papyrusText == null)
        {
            papyrusText = GetComponentInChildren<TextMeshProUGUI>(true);

            if (papyrusText == null)
            {
                Debug.LogError("PapyrusUI: No TextMeshProUGUI found.");
            }
        }
    }

    private void OnEnable()
    {
        GameManager.OnClueAdded += AddClue;
        GameManager.OnPapyrusCleared += ClearPapyrus;
    }

    private void OnDisable()
    {
        GameManager.OnClueAdded -= AddClue;
        GameManager.OnPapyrusCleared -= ClearPapyrus;
    }

    private void Start()
    {
        ClearPapyrus();
    }

    private void AddClue(string clue)
    {
        if (papyrusText == null)
            return;

        string oldText = papyrusText.text;

        string newText;

        if (string.IsNullOrEmpty(oldText))
        {
            newText = $"- {clue}";
        }
        else
        {
            newText = $"{oldText}\n\n- {clue}";
        }

        if (typingCoroutine != null)
        {
            StopCoroutine(typingCoroutine);
        }

        typingCoroutine = StartCoroutine(TypeNewClue(oldText, newText));
    }

    private IEnumerator TypeNewClue(string oldText, string newText)
    {
        papyrusText.text = newText;
        papyrusText.ForceMeshUpdate();

        int oldCharacterCount = oldText.Length;
        int totalCharacterCount = papyrusText.textInfo.characterCount;

        papyrusText.maxVisibleCharacters = oldCharacterCount;

        float floatVisible = oldCharacterCount;
        
        if (audioSource != null && clickSound != null)
        {
            audioSource.clip = clickSound;
            audioSource.loop = true; // Faz o som repetir continuamente
            audioSource.volume = 1f;
            audioSource.Play();
        }

        while (floatVisible < totalCharacterCount)
        {
            if (GameManager.Instance != null && GameManager.Instance.WasClickedThisFrame())
            {
                break;
            }

            floatVisible += Time.deltaTime * typingSpeed;
            papyrusText.maxVisibleCharacters = Mathf.FloorToInt(floatVisible);
            
            yield return null;
        }

        papyrusText.maxVisibleCharacters = totalCharacterCount;
        
        if (audioSource != null)
        {
            audioSource.Stop();
        }
    }

    public void ClearPapyrus()
    {
        if (typingCoroutine != null)
        {
            StopCoroutine(typingCoroutine);
            typingCoroutine = null;
        }

        if (audioSource != null)
        {
            audioSource.Stop();
        }

        if (papyrusText != null)
        {
            papyrusText.text = string.Empty;
            papyrusText.maxVisibleCharacters = int.MaxValue;
        }
        
    }
}