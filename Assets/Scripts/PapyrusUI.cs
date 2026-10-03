// writes the clicked text os the pipyrus. U just have to put this script in the papyrus object and it will do everything

using TMPro;
using UnityEngine;

public class PapyrusUI : MonoBehaviour
{
    private TextMeshProUGUI papyrusText;

    private void Awake()
    {
        papyrusText = GetComponentInChildren<TextMeshProUGUI>(true);

        if (papyrusText == null)
        {
            Debug.LogError("PapyrusUI: No TextMeshProUGUI found inside Papyrus.");
        }
    }

    private void OnEnable()
    {
        GameManager.OnClueAdded += AddClue;
        GameManager.DeleteTextPapyrus += ClearPapyrus;
    }

    private void OnDisable()
    {
        GameManager.OnClueAdded -= AddClue;
        GameManager.DeleteTextPapyrus -= ClearPapyrus;
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
            papyrusText.text = "- " + clue;
        }
        else
        {
            papyrusText.text += "\n- " + clue;
        }
    }

    public void ClearPapyrus()
    {
        if (papyrusText != null)
        {
            papyrusText.text = "";
        }
    }
}