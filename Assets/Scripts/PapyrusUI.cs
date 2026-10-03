using System.Collections;
using TMPro;
using UnityEngine;

/// <summary>
/// Handles appending text clues to the Papyrus UI and clearing it when necessary.
/// </summary>
[RequireComponent(typeof(TextMeshProUGUI))]
public class PapyrusUI : MonoBehaviour
{
    private TextMeshProUGUI papyrusText;
    private Coroutine typingCoroutine;

    [Header("Typing Effect")]
    public float clueTypingDuration = 0.8f;

    private void Awake()
    {
        papyrusText = GetComponent<TextMeshProUGUI>();
        
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

        float elapsed = 0f;

        while (elapsed < clueTypingDuration)
        {
            elapsed += Time.deltaTime;

            float percent = Mathf.Clamp01(elapsed / clueTypingDuration);

            int visibleCharacters = Mathf.RoundToInt(
                Mathf.Lerp(
                    oldCharacterCount,
                    totalCharacterCount,
                    percent
                )
            );

            papyrusText.maxVisibleCharacters = visibleCharacters;

            yield return null;
        }

        papyrusText.maxVisibleCharacters = totalCharacterCount;
    }

    public void ClearPapyrus()
    {
        if (typingCoroutine != null)
        {
            StopCoroutine(typingCoroutine);
            typingCoroutine = null;
        }

        if (papyrusText != null)
        {
            papyrusText.text = string.Empty;
            papyrusText.maxVisibleCharacters = int.MaxValue;
        }
    }
}