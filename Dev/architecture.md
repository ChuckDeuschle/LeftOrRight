[← Index](INDEX.md)

# Architecture

## Scene Flow

```
SplashScreen
    └── StartButton.StartGame()
            └── PrototypeSelectScene
                    └── PrototypeSelectButton.SelectPrototype0()
                            └── EncountersScene ◄──────────────────────┐
                                    │  EncounterSelectionButton sets     │
                                    │  MasterGameManager.selectedEncounter│
                                    │  StartButton.StartEncounter()      │
                                    ▼                                    │
                                GameScene                                │
                                    └── ContinueButton.ContinueGame() ──┘
                            
                            TutorialScene (reachable from EncountersScene via TutorialButton)
```

Restart destroys MasterGameManager and returns to PrototypeSelectScene, fully resetting state.

## GameManager State Machine

`GameManager.Update()` polls an enum `GameState` each frame:

```
PlayerTurn ──► (card dropped on DropZone) ──► EnemyTurn
    ▲                                              │
    └──────────────────────────────────────────────┘
         (enemy attack resolves, player still alive)

PlayerTurn or EnemyTurn ──► Win   (enemy health ≤ 0)
PlayerTurn or EnemyTurn ──► Lose  (player health ≤ 0)
```

- **PlayerTurn**: waits for `DropZone.OnDrop`; draws a card if none is active
- **EnemyTurn**: calls `Enemy.EnemyAction()` then immediately returns to PlayerTurn
- **Win**: shows WinPanel, triggers `Rewards.UpdateRewardsCards()`
- **Lose**: shows GameOverPanel

## Cross-Scene State

| Owner | Data | Lifetime |
|---|---|---|
| [`MasterGameManager`](../Assets/MasterGameManager.cs) | `player`, `deck` (Card GameObjects), `encounterList`, `selectedEncounter` | Entire run (DontDestroyOnLoad) |
| [`GameManager`](../Assets/GameManager.cs) | `currentCard`, `discardPile`, `currentRound`, UI text refs, `gameState` | Single battle scene |

**Standalone GameScene**: if `MasterGameManager.instance` is null when GameScene loads, `GameManager.Start()` creates a local default player, enemy, and starter deck so the scene can be tested in isolation without running the full flow.

## Scenes

| Scene | Role |
|---|---|
| `SplashScreen.unity` | Entry point; shows logo and Start/Tutorial buttons |
| `PrototypeSelectScene.unity` | Prototype hub; routes to EncountersScene (Prototype 0) |
| `EncountersScene.unity` | Encounter map; player picks the next battle |
| `GameScene.unity` | Card battle; the main gameplay loop |
| `TutorialScene.unity` | How-to-play screen |
