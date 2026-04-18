[← Index](INDEX.md)

# Managers

## MasterGameManager

**File:** [Assets/MasterGameManager.cs](../Assets/MasterGameManager.cs)  
**Role:** Singleton that persists across all scenes via `DontDestroyOnLoad`. It is the single source of truth for everything that must survive a scene load: the player, the deck, and the encounter list.

### Lifecycle

| Event | What happens |
|---|---|
| `Awake()` | Singleton guard (destroys duplicate if one already exists); initializes `player`; calls `CreateStarterDeck()` to populate `deck`; calls `CreateEncounterPath()` to populate `encounterList` |
| `Start()` | Calls `AssignEncountersToButtons()` to wire encounter data to UI buttons in EncountersScene |
| `OnDestroy()` | Clears the static `instance` reference |

### Public Surface

| Member | Type | Purpose |
|---|---|---|
| `instance` | `static MasterGameManager` | Singleton access point |
| `player` | `Player` | Persistent player stats (health, gold, shield) |
| `deck` | `List<Card>` | The player's current deck (Card GameObjects, also DontDestroyOnLoad) |
| `encounterList` | `List<Encounter>` | All encounters and their statuses |
| `selectedEncounter` | `Encounter` | The encounter chosen on the map screen |
| `cardPrefab` | `GameObject` | Inspector-assigned prefab used to instantiate new cards |
| `CreateStarterDeck()` | method | Returns a fresh `List<Card>` of 5 starter cards |
| `AssignEncountersToButtons()` | method | Links each encounter to its UI button; sets button interactivity based on encounter status |

### Encounter Path

Two encounters are hardcoded in the private `CreateEncounterPath()` method called from `Awake()`. To add more encounters, add them there. See [progression.md](progression.md) for the data model.

---

## GameManager

**File:** [Assets/GameManager.cs](../Assets/GameManager.cs)  
**Role:** Per-battle controller. Owns the active combat state for one GameScene session. Reads from MasterGameManager at startup; writes back via ContinueButton when the battle ends.

### Lifecycle

| Event | What happens |
|---|---|
| `Start()` | Checks for `MasterGameManager.instance`; if present, copies player/deck/enemy from it; if absent, creates local defaults (enables standalone testing). Shuffles deck, sets `gameState = PlayerTurn`, draws first card. |
| `Update()` | Runs the state machine every frame: checks win/lose conditions, handles EnemyTurn logic, updates UI. |

### Public Surface

| Member | Type | Purpose |
|---|---|---|
| `player` | `Player` | Active player for this battle |
| `currentEnemy` | `Enemy` | The enemy being fought |
| `deck` / `discardPile` | `List<Card>` | Active deck management |
| `currentCard` | `GameObject` | The card currently in play (being dragged) |
| `currentRound` | `int` | Increments each time the deck is reshuffled; drives enemy damage scaling |
| `gameState` | `GameState` enum | Current state: `PlayerTurn`, `EnemyTurn`, `Win`, `Lose` |
| `awardCardSelection` | `int` | Index (0/1/2) of the reward card the player selected; -1 if none yet |
| `DrawCard()` | method | Moves top card from deck to `currentCard`; reshuffles discard into deck if deck is empty |
| `UpdateHealthDisplay()` | method | Refreshes health text UI for player and enemy |
| `UpdateStatusDisplays()` | method | Calls `UpdatePlayerStatus` and `UpdateEnemyStatus` on the data models |
| `UpdateWinDisplay()` | method | Updates victory text with gold reward amount |
| `CreateStarterDeck()` | method | Fallback starter deck used when no MasterGameManager exists |

### Deck Management

- `Shuffle(List<Card>)` uses Fisher-Yates; it also increments `currentRound`
- Enemy damage per turn = `currentRound * 10` (scales with each reshuffle)
- `DrawCard()` automatically reshuffles the discard pile if the deck is empty before drawing

---

## Player

**File:** [Assets/Player.cs](../Assets/Player.cs)  
**Type:** Plain C# class (not a MonoBehaviour)

Holds persistent player stats for the entire run. Owned by `MasterGameManager`; copied into `GameManager` at battle start.

| Member | Type | Purpose |
|---|---|---|
| `currentHealth` | `int` | Current player HP |
| `shield` | `int` | Shield absorbed before health is reduced; reset to 0 each enemy turn |
| `goldValue` | `int` | Accumulated gold across encounters |
| `Initalize(startingHealth, goldValue)` | method | Sets `currentHealth` and `goldValue`; shield defaults to 0 |
| `UpdatePlayerStatus(gameManager)` | method | Writes current health and shield values to the GameManager's status text UI |

---

## Enemy

**File:** [Assets/Enemy.cs](../Assets/Enemy.cs)  
**Type:** Plain C# class (not a MonoBehaviour)

Holds per-encounter enemy stats. Created inside each `Encounter` object in `MasterGameManager.CreateEncounterPath()`.

| Member | Type | Purpose |
|---|---|---|
| `name` | `string` | Display name |
| `currentHealth` | `int` | Current enemy HP; when this reaches 0 GameManager transitions to Win |
| `Initalize(name, startingHealth)` | method | Sets both fields |
| `UpdateEnemyStatus(gameManager)` | method | Writes current enemy health to the GameManager's status text UI |
| `EnemyAction(gameManager)` | method | Deals `currentRound * 10` damage to the player, reduced by `player.shield`; resets shield to 0 after applying |
