using UnityEngine;

public class UIManager : MonoBehaviour
{
[Header("UI Panels")]
    public GameObject speechBubblePanel;
    public GameObject papyrusPanel;
    public GameObject selectionPanel;
    public GameObject scalesPanel;
    public GameObject foregroundWallImage; // For the layering trick

    private void OnEnable() {
        GameManager.OnStateChanged += HandleStateChange;
    }

    private void OnDisable() {
        GameManager.OnStateChanged -= HandleStateChange;
    }

    private void HandleStateChange(GameState state) {
        // 1. Reset: Turn off situation-specific panels by default
        speechBubblePanel.SetActive(false);
        selectionPanel.SetActive(false);
        scalesPanel.SetActive(false);
        
        // Papyrus stays open most of the game, so we leave it alone here unless we want to hide it at the very end.

        // 2. Activate specific UI based on state
        switch (state) {
            case GameState.AnubisIntro:
            case GameState.Interview:
                speechBubblePanel.SetActive(true);
                break;
            
            case GameState.CandidateEnter:
            case GameState.CandidateExit:
                // Trigger your ActiveCharacter sliding coroutines here
                break;

            case GameState.Deliberation:
            case GameState.Judgment:
                selectionPanel.SetActive(true);
                break;

            case GameState.ScaleEvaluation:
                scalesPanel.SetActive(true);
                break;
        }
    }
}
