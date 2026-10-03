using UnityEngine;
using System;
using System.Collections.Generic;

public enum GameState
{
    AnubisIntro,
    CandidateEnter,
    Interview,
    CandidateExit,
    Deliberation,
    Judgment,
    ScaleEvaluation,
    EndGame
}

/// <summary>
/// Manages the core game loop, states, and global data for the Anubis interview process.
/// </summary>
public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    // Events
    public static event Action<GameState> OnStateChanged;
    public static event Action<string> OnClueAdded;
    public static event Action OnPapyrusCleared;
    public static event Action<DialogueBubble> OnDialogueChanged;
    public static event Action<NPCData> OnNPCChanged;

    [Header("Level Configuration")]
    public LevelData currentLevel;

    // State
    public GameState CurrentState { get; private set; }

    // Data tracking
    private int currentNPCIndex = 0;
    private int currentDialogueIndex = 0;

    public List<string> CurrentNPCClues { get; private set; } = new List<string>();

    public Dictionary<string, List<string>> SavedCluesByNPC { get; private set; }
        = new Dictionary<string, List<string>>();

    private const int MAX_CLUES = 4;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        ChangeState(GameState.CandidateEnter);
    }

    public void ChangeState(GameState newState)
    {
        CurrentState = newState;

        Debug.Log($"Game State Changed To: {newState}");

        if (newState == GameState.CandidateEnter)
        {
            InitializeCandidate();
        }

        OnStateChanged?.Invoke(newState);
    }

    private void InitializeCandidate()
    {
        if (
            currentLevel != null &&
            currentLevel.npcsInLevel != null &&
            currentNPCIndex < currentLevel.npcsInLevel.Length
        )
        {
            NPCData npc = currentLevel.npcsInLevel[currentNPCIndex];

            if (npc != null)
            {
                OnNPCChanged?.Invoke(npc);
            }
        }
    }

    public void StartGameLoop()
    {
        currentNPCIndex = 0;
        ChangeState(GameState.CandidateEnter);
    }

    public void AdvanceDialogue()
    {
        if (
            currentLevel == null ||
            currentLevel.npcsInLevel == null ||
            currentLevel.npcsInLevel.Length == 0
        )
        {
            return;
        }

        if (CurrentState == GameState.AnubisIntro)
        {
            StartGameLoop();
            return;
        }

        if (CurrentState == GameState.Interview)
        {
            NPCData currentNPC =
                currentLevel.npcsInLevel[currentNPCIndex];

            currentDialogueIndex++;

            if (currentDialogueIndex < currentNPC.dialogueBubbles.Length)
            {
                OnDialogueChanged?.Invoke(
                    currentNPC.dialogueBubbles[currentDialogueIndex]
                );
            }
            else
            {
                ChangeState(GameState.CandidateExit);
            }
        }
    }

    public void OnCandidateEntered()
    {
        if (CurrentState != GameState.CandidateEnter)
            return;

        if (
            currentLevel == null ||
            currentLevel.npcsInLevel == null ||
            currentLevel.npcsInLevel.Length == 0
        )
        {
            Debug.LogError(
                "Current Level is missing or empty! Please assign Level1 to GameManager in the Inspector."
            );

            return;
        }

        NPCData currentNPC =
            currentLevel.npcsInLevel[currentNPCIndex];

        if (currentNPC == null)
        {
            Debug.LogError(
                $"NPC at index {currentNPCIndex} is empty!"
            );

            return;
        }

        currentDialogueIndex = 0;

        ChangeState(GameState.Interview);

        OnDialogueChanged?.Invoke(
            currentNPC.dialogueBubbles[currentDialogueIndex]
        );
    }

    public void OnCandidateExited()
    {
        if (CurrentState != GameState.CandidateExit)
            return;

        if (
            currentLevel == null ||
            currentLevel.npcsInLevel == null
        )
        {
            return;
        }

        NPCData currentNPC =
            currentLevel.npcsInLevel[currentNPCIndex];

        // Save the clues permanently for this NPC
        SavedCluesByNPC[currentNPC.npcName] =
            new List<string>(CurrentNPCClues);

        // Reset the 4-clue limit for the next NPC
        CurrentNPCClues.Clear();

        // Clear only the visual text on the papyrus
        OnPapyrusCleared?.Invoke();

        currentNPCIndex++;

        if (currentNPCIndex < currentLevel.npcsInLevel.Length)
        {
            ChangeState(GameState.CandidateEnter);
        }
        else
        {
            ChangeState(GameState.Deliberation);
        }
    }

    public void TrySaveClue(string clueText)
    {
        if (CurrentNPCClues.Count >= MAX_CLUES)
        {
            Debug.Log("Papyrus is full! Max 4 clues allowed.");
            return;
        }

        if (CurrentNPCClues.Contains(clueText))
        {
            Debug.Log("Clue already saved!");
            return;
        }

        CurrentNPCClues.Add(clueText);

        Debug.Log($"Clue Saved: {clueText}");

        OnClueAdded?.Invoke(clueText);
    }
}