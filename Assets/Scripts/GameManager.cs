using UnityEngine;
using System;

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

    public GameState CurrentState { get; private set; }

    private void Awake() {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start() {
        // Start the game loop
        ChangeState(GameState.AnubisIntro);
    }

    public void ChangeState(GameState newState) {
        CurrentState = newState;
        Debug.Log($"Game State Changed To: {newState}");
        OnStateChanged?.Invoke(newState);
    }
}
