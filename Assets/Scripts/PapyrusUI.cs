using TMPro;
using UnityEngine;

/// <summary>
/// Handles appending text clues to the Papyrus UI and clearing it when necessary.
/// </summary>
[RequireComponent(typeof(TextMeshProUGUI))]
public class PapyrusUI : MonoBehaviour
{
    private TextMeshProUGUI papyrusText;

    private void Awake()
    {
        papyrusText = GetComponent<TextMeshProUGUI>();

        if (papyrusText == null)
        {
            // Fallback for previous setup
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

        if (string.IsNullOrEmpty(papyrusText.text))
        {
            papyrusText.text = $"- {clue}";
        }
        else
        {
            papyrusText.text += $"\n- {clue}";
        }
    }

    public void ClearPapyrus()
    {
        if (papyrusText != null)
        {
            papyrusText.text = string.Empty;
        }
    }
}