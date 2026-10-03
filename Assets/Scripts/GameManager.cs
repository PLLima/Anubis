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
    
    public void TrySaveClue(string clueText) {
        if (currentNPCClues.Count >= 4) {
            Debug.Log("Papyrus is full! Max 4 clues allowed.");
            // Optional: Trigger a "fail" sound or shake the UI
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
