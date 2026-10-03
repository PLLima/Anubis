# Anubis

Game developed during the CStudio Game Jam 2026 at CentraleSupélec using Unity.

## Game Concept

* **Theme**: The choice only comes after.
* **Genre**: 2D UI-based narrative/puzzle.
* **Core Mechanic**: The player works for Anubis, the founder and boss of an afterlife startup. The player's task is to interview souls, read their dialogue, and interact with specific words to save them as clues in the 'Papyrus' menu (maximum of 4 clues). These clues are evaluated during the deliberation phase to choose the correct soul for the final judgment on the Scales of Justice. Ultimately, Anubis delivers the final verdict on the player's performance: if the selected soul's heart is heavier than the feather (the scale falls), the player loses and has their own heart eaten by Ammit. If the player makes the right choice, they ascend to heaven and win the game.

## Core Mechanics

* **Dialogue & Investigation**: The game loop revolves around interviewing candidates. Clickable words within the dialogue text are processed and stored as actionable clues.
* **Papyrus System**: A dedicated UI panel stores up to 4 gathered clues per session.
* **Judgment Phase**: Using collected evidence, the player makes the final choice of which soul to weigh on the scales. This aligns with the game jam's theme, where choices follow the initial data gathering phase.

## Technical Architecture

The project utilizes a single-scene, UI-driven architecture in Unity:

* **GameManager (`GameManager.cs`)**: Central state machine controlling the game flow via the `GameState` enum (`AnubisIntro`, `CandidateEnter`, `Interview`, `CandidateExit`, `Deliberation`, `Judgment`, `ScaleEvaluation`, `EndGame`). It tracks collected clues, manages `LastDeliberationResult`, and broadcasts state changes.
* **UIManager (`UIManager.cs`)**: Subscribes to `GameManager` events to manage overall visibility of UI panels.
* **SpeechBubbleUI (`SpeechBubbleUI.cs`)**: A modular component attached to the speech bubble panel that listens to dialogue changes, manages dialogue layout, and controls dynamic text typing animations.
* **DialogueLinkHandler (`DialogueLinkHandler.cs`)**: Implements `IPointerClickHandler` and utilizes `TMP_TextUtilities.FindIntersectingLink` to detect user clicks on specific words within TextMeshPro elements, dispatching the data to the `GameManager`. Includes visual hover feedback.
* **CharacterMover (`CharacterMover.cs`)**: Handles 2D sprite sliding animations, drop-in effects, and footstep audio via coroutines.
* **PapyrusMover (`PapyrusMover.cs`)**: Manages the papyrus scroll UI panel animation, hiding it offscreen and animating it into place alongside typing out the character's header name.
* **AnubisTutorialManager (`AnubisTutorialManager.cs`)**: Controls the opening tutorial dialog.
* **Deliberation Phase**: Managed by `DeliberationManager.cs`, `DeliberationPapyrusManager.cs`, and `DeliberationPapyrusDrag.cs`. Handles spawning draggable clues, dialogue logic for Anubis during deliberation, and Anubis's final win/loss verdict.
* **ScalesManager (`ScalesManager.cs`)**: Handles the Scale Evaluation sequence. Drops the scales, swaps the result sprites (Heart vs Feather), and controls the fading animations to highlight the outcome.
* **Data Management (`GameData.cs`)**: Uses Unity ScriptableObjects (`DialogueBubble`, `NPCData`, `LevelData`) to define and serialize narrative and character data.

## NPCs

The logic loop includes three primary interview subjects:
1. **Khepri (Merchant)**: Focused on persuasion, salesmanship, and human behavior.
2. **Nefru (Builder)**: Focused on manual labor, temple construction, and practical problem-solving.
3. **Hori (Scribe)**: Focused on observation, record-keeping, calculation, and distrust of memory.

## Installation and Execution

1. Clone the repository.
2. Open the project using a compatible Unity Editor version.
3. Open the primary main scene.
4. Press Play in the Unity Editor to initiate the game loop.

## Current Status and Roadmap

* **Completed**: Base UI setup, automated ScriptableObject generation (`NPCDataGenerator.cs`), game loop logic (state machine), character transitions, and interactive text parsing.
* **Completed (Core Loop)**: The entire core gameplay loop is now fully implemented! This includes the `Deliberation` state where the player drags their choice to Anubis, the `ScaleEvaluation` visual sequence, and the final win/loss dialogue in the `EndGame` state.
* **Pending**: Adding sound effects/background music, expanding level data, and polishing visual micro-animations.
