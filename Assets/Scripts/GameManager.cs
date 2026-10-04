using UnityEngine;
using System;
using System.Collections.Generic;

public enum GameState
{
    TitleScreen,
    AnubisIntro,
    CandidateEnter,
    Interview,
    CandidateExit,
    Deliberation,
    Judgment,
    ScaleEvaluation,
    EndGame,        // Anubis delivers his verdict for the current level
    RunComplete     // All levels finished
}

/// <summary>
/// Manages the core game loop, states, and global data for the Anubis interview process.
/// Supports multiple levels played once each in a random order per run.
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
    public static event Action<bool> OnDeliberationSubmitted;
    public static event Action<LevelData> OnLevelStarted;   // NEW

    [Header("Level Configuration")]
    [Tooltip("Pool of levels. They are shuffled at the start of every run.")]
    public LevelData[] allLevels;                            // NEW

    [Tooltip("Set at runtime. If allLevels is empty, this is used as a single-level fallback.")]
    public LevelData currentLevel;

    [Header("Audio Settings")]
    public AudioSource audioSource;
    public AudioClip advanceDialogueSound;

    [Header("Cursor Settings")]
    public Texture2D cursorNormal;
    public Texture2D cursorClicked;
    public Texture2D cursorHover;
    public Vector2 cursorSize = new Vector2(32, 32);
    public Vector2 cursorHotSpot = Vector2.zero;
    public CursorMode cursorMode = CursorMode.Auto;

    private bool isHoveringInteractable = false;

    // State
    public GameState CurrentState { get; private set; }
    public bool LastDeliberationResult { get; private set; }

    // Run progress (NEW)
    private readonly List<LevelData> levelOrder = new List<LevelData>();
    private int levelOrderIndex = -1;
    public int LevelsCompleted { get; private set; }
    public int CorrectJudgments { get; private set; }
    public int TotalLevelsInRun => levelOrder.Count;
    public bool HasMoreLevels => levelOrderIndex + 1 < levelOrder.Count;

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

            if (audioSource == null)
            {
                audioSource = GetComponent<AudioSource>();
            }
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        // Resize cursors so they aren't massive, which offsets the hotspot and breaks raycasts
        cursorNormal = ResizeCursorTexture(cursorNormal, (int)cursorSize.x, (int)cursorSize.y);
        cursorClicked = ResizeCursorTexture(cursorClicked, (int)cursorSize.x, (int)cursorSize.y);
        cursorHover = ResizeCursorTexture(cursorHover, (int)cursorSize.x, (int)cursorSize.y);

        SetCursor(cursorNormal);
        ChangeState(GameState.TitleScreen);
    }

    private Texture2D ResizeCursorTexture(Texture2D source, int width, int height)
    {
        if (source == null) return null;

        RenderTexture rt = RenderTexture.GetTemporary(width, height);
        RenderTexture.active = rt;

        // Copy the texture using the GPU
        Graphics.Blit(source, rt);

        Texture2D result = new Texture2D(width, height, TextureFormat.RGBA32, false);
        result.ReadPixels(new Rect(0, 0, width, height), 0, 0);
        result.Apply();

        RenderTexture.active = null;
        RenderTexture.ReleaseTemporary(rt);

        return result;
    }

    private void Update()
    {
        bool isDown = false;
        bool isUp = false;

#if ENABLE_INPUT_SYSTEM
        if (UnityEngine.InputSystem.Mouse.current != null)
        {
            isDown = UnityEngine.InputSystem.Mouse.current.leftButton.wasPressedThisFrame;
            isUp = UnityEngine.InputSystem.Mouse.current.leftButton.wasReleasedThisFrame;
        }
#else
        isDown = Input.GetMouseButtonDown(0);
        isUp = Input.GetMouseButtonUp(0);
#endif

        if (isDown)
        {
            if (isHoveringInteractable)
            {
                SetCursor(cursorClicked);
            }
        }
        else if (isUp)
        {
            UpdateCursorToCurrentState();
        }
    }

    /// <summary>
    /// Call this method from UI elements (e.g. OnPointerEnter/Exit) to change the cursor state.
    /// </summary>
    public void SetCursorHoverState(bool isHovering)
    {
        isHoveringInteractable = isHovering;

        bool isPressed = false;
#if ENABLE_INPUT_SYSTEM
        if (UnityEngine.InputSystem.Mouse.current != null)
        {
            isPressed = UnityEngine.InputSystem.Mouse.current.leftButton.isPressed;
        }
#else
        isPressed = Input.GetMouseButton(0);
#endif

        // Don't override the clicked texture if the user is currently holding the mouse button down
        if (!isPressed)
        {
            UpdateCursorToCurrentState();
        }
    }

    /// <summary>
    /// Safely checks if the left mouse button was clicked this frame, supporting both Input Systems.
    /// </summary>
    public bool WasClickedThisFrame()
    {
#if ENABLE_INPUT_SYSTEM
        if (UnityEngine.InputSystem.Mouse.current != null)
        {
            return UnityEngine.InputSystem.Mouse.current.leftButton.wasPressedThisFrame;
        }
        return false;
#else
        return Input.GetMouseButtonDown(0);
#endif
    }

    private void UpdateCursorToCurrentState()
    {
        if (isHoveringInteractable && cursorHover != null)
        {
            SetCursor(cursorHover);
        }
        else
        {
            SetCursor(cursorNormal);
        }
    }

    private void SetCursor(Texture2D cursorTexture)
    {
        if (cursorTexture != null)
        {
            Cursor.SetCursor(cursorTexture, cursorHotSpot, cursorMode);
        }
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

    // ------------------------------------------------------------------
    // Run / level flow (NEW)
    // ------------------------------------------------------------------

    /// <summary>
    /// Starts a fresh run: shuffles all levels and begins the first one.
    /// </summary>
    public void StartGameLoop()
    {
        LevelsCompleted = 0;
        CorrectJudgments = 0;

        BuildShuffledLevelOrder();

        if (levelOrder.Count == 0)
        {
            Debug.LogError("No levels available! Assign levels to 'allLevels' (or 'currentLevel') in the Inspector.");
            return;
        }

        levelOrderIndex = -1;
        BeginNextLevel();
    }

    /// <summary>
    /// Call this from your ScaleEvaluation UI (e.g. a "Continue" button / after the scale animation).
    /// Moves to the next random level, or ends the game if all levels are done.
    /// </summary>
    public void AdvanceToNextLevel()
    {
        // The verdict is shown during EndGame; Anubis leaving triggers this call.
        if (CurrentState != GameState.EndGame && CurrentState != GameState.ScaleEvaluation) return;

        LevelsCompleted++;

        if (HasMoreLevels)
        {
            BeginNextLevel();
        }
        else
        {
            ChangeState(GameState.RunComplete);
        }
    }

    private void BuildShuffledLevelOrder()
    {
        levelOrder.Clear();

        if (allLevels != null)
        {
            foreach (LevelData level in allLevels)
            {
                if (level != null) levelOrder.Add(level);
            }
        }
    }

    private void BeginNextLevel()
    {
        levelOrderIndex++;
        currentLevel = levelOrder[levelOrderIndex];

        currentNPCIndex = 0;
        currentDialogueIndex = 0;
        CurrentNPCClues.Clear();
        SavedCluesByNPC.Clear();
        OnPapyrusCleared?.Invoke();

        Debug.Log($"Starting level {levelOrderIndex + 1}/{levelOrder.Count}: {currentLevel.name}");
        OnLevelStarted?.Invoke(currentLevel);

        ChangeState(GameState.CandidateEnter);
    }

    // ------------------------------------------------------------------

    public void AdvanceDialogue()
    {
        if (audioSource != null && advanceDialogueSound != null)
        {
            audioSource.PlayOneShot(advanceDialogueSound);
        }

        if (
            currentLevel == null ||
            currentLevel.npcsInLevel == null ||
            currentLevel.npcsInLevel.Length == 0
        )
        {
            return;
        }

        if (CurrentState == GameState.CandidateEnter)
        {
            OnCandidateEntered();
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
                "Current Level is missing or empty! Please assign levels to GameManager in the Inspector."
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

    public void SubmitDeliberationChoice(NPCData chosenNPC)
    {
        if (CurrentState != GameState.Deliberation && CurrentState != GameState.Judgment) return;

        ChangeState(GameState.ScaleEvaluation);

        bool isCorrect = (currentLevel != null && currentLevel.correctNPC == chosenNPC);
        LastDeliberationResult = isCorrect;
        if (isCorrect) CorrectJudgments++;
        OnDeliberationSubmitted?.Invoke(isCorrect);
    }

    public void QuitGame()
    {
        Debug.Log("Quitting Game...");
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}