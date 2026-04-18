# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview

**LeftOrRight** is a deck-building roguelike card game built in Unity 6000.1.1f1 (company: MagicRedBeard). Players battle enemies by dragging cards left or right to activate different actions (Attack/Shield). Combat is turn-based with a roguelike encounter progression.

## Build & Run

This is a standard Unity project — open the project folder in Unity Hub or Unity Editor (6000.1.1f1). No build scripts or CI exist; use the Unity Editor Play button for testing. All testing is manual in-editor.

The scene play order is defined in File > Build Settings. Run `SplashScreen` to start from the beginning, or open any scene directly for isolated testing.

## Architecture

### Scene Flow
```
SplashScreen → EncountersScene → GameScene → EncountersScene (repeats)
```
- `StartButton.StartGame()` loads EncountersScene
- `EncounterSelectionButton` sets the selected encounter on MasterGameManager, then `StartButton.StartEncounter()` loads GameScene
- `ContinueButton.ContinueGame()` marks the encounter complete, unlocks the next, grants rewards, and returns to EncountersScene

### Cross-Scene State: MasterGameManager
`MasterGameManager.cs` is a singleton with `DontDestroyOnLoad`. It owns all persistent state:
- Player data and the deck (as a list of Card GameObjects, also marked DontDestroyOnLoad)
- The encounter list and the currently selected encounter

GameManager (battle scene) reads from MasterGameManager at scene load; if MasterGameManager doesn't exist it creates local defaults so the GameScene can be run standalone.

### Battle State Machine: GameManager
`GameManager.cs` runs the per-battle loop via `Update()` with an enum state:
1. **PlayerTurn** — waits for a card to be dragged to a DropZone; draws cards as needed
2. **EnemyTurn** — enemy attacks (`currentRound * 10` damage), then returns to PlayerTurn
3. **Win** — shows reward panel (card selection via `Rewards.cs`)
4. **Lose** — shows game over panel

### Card & Combat System
- `Card.cs` — data model with `cardName`, `leftAction`, `rightAction`, sprite, display text
- `Action.cs` — enum (Attack/Shield) with `Label`, `Magnitude`, and `Play(gameManager)` that directly mutates health/shield
- `DraggableCard.cs` — implements Unity drag interfaces; moves card to mouse, snaps back on release
- `DropZone.cs` — detects dropped card, calls the left or right action, moves card to discard pile, draws next card or triggers EnemyTurn

Deck management lives in GameManager: separate `deck` and `discardPile` lists; discard is reshuffled back into deck when deck empties.

### Encounter System
- `Encounter.cs` — data object: name, description, enemy, goldAward, awardCards, status (available/pending/complete)
- Encounters are hard-coded in `MasterGameManager.Awake()` — not data-driven
- `EncounterPathManager.cs` — drives the EncountersScene UI (player health, deck list, encounter details)

## Development Environment

**Primary IDE:** VS Code 1.113+ with the following extensions (all already installed):
- `visualstudiotoolsforunity.vstuc` — Unity debugger and project sync
- `ms-dotnettools.csdevkit` + `ms-dotnettools.csharp` — C# IntelliSense and build
- `anthropic.claude-code` — Claude Code sidebar (side-loaded by the CLI)

The `.vscode/` folder is committed to the repo and pre-configures file exclusions, YAML associations for Unity assets, and `dotnet.defaultSolution = LeftOrRight.sln`.

## Git & GitHub

- Remote: `https://github.com/ChuckDeuschle/LeftOrRight.git` (owner: ChuckDeuschle)
- Main branch: `master`
- Do **not** add `Co-Authored-By:` lines to commit messages.

## Key Conventions

- **`Initalize()` method** (note the typo) — every game entity uses this name consistently; preserve the typo when adding new entities
- All C# scripts are in `Assets/` root — no subdirectories, no namespaces
- Cards and encounters are created procedurally in code, not via ScriptableObjects or JSON
- TextMeshPro (`TMPro`) is used for all UI text
