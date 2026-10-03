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

* **GameManager (`GameManager.cs`)**: Central state machine controlling the game flow via the `GameState` enum (`AnubisIntro`, `CandidateEnter`, `Interview`, `CandidateExit`, `Deliberation`, `Judgment`, `ScaleEvaluation`, `EndGame`). It tracks collected clues and broadcasts state changes.
* **UIManager (`UIManager.cs`)**: Subscribes to `GameManager` events to manage UI panels (speech bubbles, selections, scales, and the papyrus menu) and formats dialogue snippets.
* **DialogueLinkHandler (`DialogueLinkHandler.cs`)**: Implements `IPointerClickHandler` and utilizes `TMP_TextUtilities.FindIntersectingLink` to detect user clicks on specific words within TextMeshPro elements, dispatching the data to the `GameManager`.
* **CharacterMover (`CharacterMover.cs`)**: Handles 2D sprite sliding animations and character transitions via coroutines, reacting to `OnStateChanged` and `OnNPCChanged` events.
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
* **Pending**: Implementation of the interaction logic during the `Deliberation` state to allow the player to select the correct soul based on clues, and finalizing the `Judgment` and `ScaleEvaluation` visual sequences.
