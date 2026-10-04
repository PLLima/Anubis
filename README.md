<div align="center">
  <h1>⚖️ Anubis</h1>
  <p><i>The choice only comes after.</i></p>
  <p>
    <b>A 2D UI-based narrative puzzle game developed during the CStudio Game Jam 2026 at CentraleSupélec.</b>
  </p>
</div>

---

## 📖 Overview

**Anubis** is a narrative puzzle game where you play as an employee at an afterlife startup founded by the Egyptian god Anubis. Your task is to interview deceased souls, gather clues from their stories, and make the ultimate judgment on the Scales of Justice. 

> **Will you ascend to heaven, or will your heart be devoured by Ammit?**

---

## 🎮 Core Mechanics

| Mechanic | Description |
| :--- | :--- |
| **🗣️ Dialogue & Investigation** | Interview candidates and click specific words to gather actionable evidence. |
| **📜 Papyrus System** | Strategically store up to 4 vital clues per session in your dedicated UI panel. |
| **⚖️ Judgment Phase** | Weigh the gathered evidence to choose the correct soul for the final judgment. If successful, continue your divine job infinitely. Make a mistake, and face an instant Game Over. |

---

## 🏗️ Technical Architecture

Built with a single-scene, robust UI-driven architecture in Unity, prioritizing modularity and scalability.

### Core Systems
* **`GameManager.cs`**: The central state machine driving the game flow (from `TitleScreen` to `GameOver` / endless loop). It broadcasts state changes and tracks player progression.
* **`UIManager.cs`**: Subscribes to core events to manage global panel visibility seamlessly.
* **`SpeechBubbleUI.cs` & `DialogueLinkHandler.cs`**: Modular components handling dynamic text typing and interactive text parsing via `TMP_TextUtilities.FindIntersectingLink`.
* **`CharacterMover.cs` & `PapyrusMover.cs`**: Singleton-based handling of smooth 2D sprite sliding, UI drop-ins, and audio synchronization via coroutines.
* **`DeliberationManager.cs` & `ScalesManager.cs`**: Manages the final judgment phase, including draggable clues, dynamic Anubis dialogue, and the dramatic Scale Evaluation visual sequence.

### Data Management
Uses Unity **ScriptableObjects** (`DialogueBubble`, `NPCData`, `LevelData`) to define narrative branching and randomized win conditions, keeping logic strictly separated from content.

---

## 👥 Interview Subjects

1. 🏺 **Khepri (Merchant)**: Focused on persuasion, salesmanship, and human behavior.
2. 🧱 **Nefru (Builder)**: Focused on manual labor, temple construction, and practical problem-solving.
3. ✍️ **Hori (Scribe)**: Focused on observation, record-keeping, calculation, and distrust of memory.

---

## 🚀 Installation & Execution

1. **Clone** the repository.
2. **Open** the project using a compatible Unity Editor version.
3. **Load** the primary main scene.
4. **Press Play** in the Unity Editor to initiate the game loop.

---

## 🗺️ Roadmap & Status

- [x] **Foundation**: Base UI setup, interactive text parsing, and character transitions.
- [x] **Core Loop**: Fully implemented state machine, dynamic dialogue, and clue saving.
- [x] **Judgment Phase**: Draggable UI clues, Scale Evaluation visual sequence, and endless loop / Game Over mechanics.
- [ ] **Polish**: Expand level data, add comprehensive sound effects and background music, and refine micro-animations.

---

## 🌟 Credits

| Role | Team Members |
| :--- | :--- |
| **Head Programmer** | Pedro Lubaszewski Lima |
| **Assistant Programmers** | Artur Bandeira Chan Jorge, Sabina Wang |
| **Art Designer** | Sabina Wang |
| **Sound Designer** | Gabriel Flores Coelho |
| **Game Designers** | Pedro Lubaszewski Lima, Artur Bandeira Chan Jorge, Sabina Wang, Gabriel Flores Coelho |
| **Writers** | Sabina Wang, Gabriel Flores Coelho |

**✨ Special Mentions**:
* **Henrique**: Game Tester
* **Artur**: Voice Actor & Aura Manager

---

## 📄 License

This project operates under a split license model:
* **Code**: MIT License ([LICENSE](LICENSE))
* **Art & Audio**: Creative Commons Attribution-NonCommercial 4.0 International ([CC BY-NC 4.0](https://creativecommons.org/licenses/by-nc/4.0/))
