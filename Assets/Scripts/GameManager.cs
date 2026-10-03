using UnityEngine;
using System;
using System.Collections.Generic;

public enum GameState {
    AnubisIntro,       
    CandidateEnter,    
    Interview,         
    CandidateExit,     
    Deliberation,      
    Judgment,          
    ScaleEvaluation,   
    EndGame            
}

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public static event Action<GameState> OnStateChanged;

    public static event Action<string> OnClueAdded;
    public static event Action<DialogueBubble> OnDialogueChanged;
    public static event Action<NPCData> OnNPCChanged;

    public LevelData currentLevel;
    private int currentNPCIndex = 0;
    private int currentDialogueIndex = 0;

    public GameState CurrentState { get; private set; }

    public List<string> currentNPCClues = new List<string>();

    private void Awake() {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start() {
        ChangeState(GameState.AnubisIntro);
    }

    public void ChangeState(GameState newState) {
        CurrentState = newState;
        Debug.Log($"Game State Changed To: {newState}");
        OnStateChanged?.Invoke(newState);
    }

    public void StartGameLoop() {
        currentNPCIndex = 0;
        ChangeState(GameState.CandidateEnter);
    }

    public void AdvanceDialogue() {
        if (currentLevel == null || currentLevel.npcsInLevel == null || currentLevel.npcsInLevel.Length == 0) return;

        if (CurrentState == GameState.AnubisIntro) {
            StartGameLoop();
            return;
        }

        if (CurrentState == GameState.Interview) {
            NPCData currentNPC = currentLevel.npcsInLevel[currentNPCIndex];
            currentDialogueIndex++;
            
            if (currentDialogueIndex < currentNPC.dialogueBubbles.Length) {
                OnDialogueChanged?.Invoke(currentNPC.dialogueBubbles[currentDialogueIndex]);
            } else {
                ChangeState(GameState.CandidateExit);
            }
        }
    }

    public void OnCandidateEntered() {
        if (CurrentState == GameState.CandidateEnter) {
            if (currentLevel == null || currentLevel.npcsInLevel == null || currentLevel.npcsInLevel.Length == 0) {
                Debug.LogError("Current Level is missing or empty! Please assign Level1 to GameManager in the Inspector.");
                return;
            }

            NPCData currentNPC = currentLevel.npcsInLevel[currentNPCIndex];
            
            if (currentNPC == null) {
                Debug.LogError($"NPC at index {currentNPCIndex} in Level1 is empty! Please drag an NPC asset into this slot in the Inspector.");
                return;
            }

            OnNPCChanged?.Invoke(currentNPC);
            
            currentDialogueIndex = 0;
            ChangeState(GameState.Interview);
            OnDialogueChanged?.Invoke(currentNPC.dialogueBubbles[currentDialogueIndex]);
        }
    }

    public void OnCandidateExited() {
        if (CurrentState == GameState.CandidateExit) {
            if (currentLevel == null || currentLevel.npcsInLevel == null) return;

            currentNPCIndex++;
            if (currentNPCIndex < currentLevel.npcsInLevel.Length) {
                ChangeState(GameState.CandidateEnter);
            } else {
                ChangeState(GameState.Deliberation);
            }
        }
    }
    
    public void TrySaveClue(string clueText) {
        if (currentNPCClues.Count >= 4) {
            Debug.Log("Papyrus is full! Max 4 clues allowed.");
            return; 
        }

        if (currentNPCClues.Contains(clueText)) {
            Debug.Log("Clue already saved!");
            return;
        }

        currentNPCClues.Add(clueText);
        Debug.Log($"Clue Saved: {clueText}");
        
        OnClueAdded?.Invoke(clueText);
    }
}
