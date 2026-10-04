<div align="center">
  <img src="Assets/Art/Icon.png" width="150" alt="Anubis Logo">
  <h1>⚖️ Anubis</h1>
  <p><i>The choice only comes after.</i></p>
  <p>
    <b>A 2D UI-based narrative puzzle game developed during the CStudio Game Jam 2026 at CentraleSupélec.</b>
  </p>
  <p>
    <img src="https://img.shields.io/badge/Style-2D-lightgrey?style=flat-square" alt="2d">
    <img src="https://img.shields.io/badge/Monster-Ammit-red?style=flat-square" alt="ammit">
    <img src="https://img.shields.io/badge/God-Anubis-black?style=flat-square" alt="anubis">
    <img src="https://img.shields.io/badge/Language-C%23-239120?style=flat-square&logo=c-sharp&logoColor=white" alt="csharp">
    <img src="https://img.shields.io/badge/Theme-Egyptian%20Mythology-gold?style=flat-square" alt="egyptian-mythology">
    <img src="https://img.shields.io/badge/Event-Game%20Jam-orange?style=flat-square" alt="game-jam">
    <img src="https://img.shields.io/badge/Genre-Narrative%20Game-blue?style=flat-square" alt="narrative-game">
    <img src="https://img.shields.io/badge/Genre-Puzzle%20Game-blue?style=flat-square" alt="puzzle-game">
    <img src="https://img.shields.io/badge/Lore-Startup-success?style=flat-square" alt="startup">
    <img src="https://img.shields.io/badge/Architecture-UI--Driven-blueviolet?style=flat-square" alt="ui-driven">
    <img src="https://img.shields.io/badge/Engine-Unity-black?style=flat-square&logo=unity" alt="unity">
  </p>
</div>

---

## 📖 Overview

**Anubis** is a narrative puzzle game where you play as an employee at an afterlife startup founded by the Egyptian god Anubis. Your task is to interview deceased souls, gather clues from their stories, and make the ultimate judgment on the Scales of Justice. 

> **Will you ascend to heaven, or will your heart be devoured by Ammit?**

<br>
<div align="center">
  <img src="img/TitleScreen.png" width="48%" alt="Title Screen">
  <img src="img/Intro.png" width="48%" alt="Introduction Scene">
</div>

---

## 🎮 Core Mechanics

| Mechanic | Description |
| :--- | :--- |
| **🗣️ Dialogue & Investigation** | Interview candidates and click specific words to gather actionable evidence. |
| **📜 Papyrus System** | Strategically store up to 4 vital clues per session in your dedicated UI panel. |
| **⚖️ Judgment Phase** | Weigh the gathered evidence to choose the correct soul for the final judgment. If successful, continue your divine job infinitely. Make a mistake, and face an instant Game Over. |

<br>
<div align="center">
  <img src="img/Interview.png" width="48%" alt="Interview Phase">
  <img src="img/Deliberation.png" width="48%" alt="Deliberation Phase">
</div>
<br>
<div align="center">
  <img src="img/Judgement.png" width="48%" alt="Judgment Scene">
  <img src="img/Gameover.png" width="48%" alt="Game Over Scene">
</div>

---

## 🏗️ Technical Architecture

Built with a single-scene, robust UI-driven architecture in Unity, prioritizing modularity and scalability.

### Core Systems
* **`GameManager.cs`**: The central state machine driving the game flow (from `TitleScreen` to `GameOver` / endless loop). It broadcasts state changes and tracks player progression.
* **`UIManager.cs`**: Subscribes to core events to manage global panel visibility seamlessly.
* **`SpeechBubbleUI.cs` & `DialogueLinkHandler.cs`**: Modular components handling dynamic text typing and interactive text parsing via `TMP_TextUtilities.FindIntersectingLink`.
* **`AnubisTutorialManager.cs`**: Orchestrates the introductory sequence and includes a skip intro functionality for faster pacing.
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

### 📥 Playing the Release Build
1. Go to the **Releases** tab and download the compressed archive (`.zip`) for your operating system (Windows or Linux).
2. **Extract** the entire archive into an empty folder. *(Do not run the executable directly from inside the `.zip`.)*
3. **Windows**: Run `Anubis.exe`.
4. **Linux**: Ensure the executable has correct execution permissions (e.g., `chmod +x Anubis.x86_64`), then run `./Anubis.x86_64`.

### 🛠️ Developer Setup (Unity)
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

<div align="center">
  <img src="img/Credits.png" width="80%" alt="Credits Screen">
</div>
<br>

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
