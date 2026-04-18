[← Index](INDEX.md)

# UI Scripts

## EncounterPathManager

**File:** [Assets/EncounterPathManager.cs](../Assets/EncounterPathManager.cs)  
**Scene:** EncountersScene

Populates the encounter map screen on load. Has no public methods — all work happens in `Start()`.

| Inspector Field | Purpose |
|---|---|
| `playerHealthText` | Shows current player health |
| `deckListText` | Lists the player's current deck (sorted, with action labels) |
| `encounterDetailText` | Shows name and description of the first available encounter |

`Start()` sequence:
1. Calls `MasterGameManager.instance.AssignEncountersToButtons()` to wire encounters to buttons
2. Writes player health to `playerHealthText`
3. Writes encounter details to `encounterDetailText`
4. Builds a sorted deck list (by card name) and writes it to `deckListText`, including each card's left/right action labels and magnitudes

---

## DeckView

**File:** [Assets/DeckView.cs](../Assets/DeckView.cs)  
**Scene:** GameScene

An in-battle overlay that lets the player inspect their full deck and discard pile. Opened/closed via UI buttons wired to the public methods.

| Member | Purpose |
|---|---|
| `contentPanel` | Parent Transform for the text display |
| `gameManager` | Inspector-assigned reference |
| `deckViewPanel` | The panel GameObject toggled visible/hidden |
| `closeDeckViewButton` | The close button shown while panel is open |
| `scrollViewText` | TMPro text component that shows the deck list |
| `PopulateDeckAndDiscard()` | Calls both populate methods in sequence; called when opening the panel |
| `PopulateDeckView(List<Card>)` | Writes a sorted deck list to `scrollViewText` |
| `PopulateDeckViewDiscard(List<Card>)` | Appends the discard pile to `scrollViewText` with a separator |
| `CloseDeckView()` | Hides `deckViewPanel` and `closeDeckViewButton` |

---

## Navigation Scripts

Small single-purpose MonoBehaviours wired to button `onClick` events. None have `Start()` or `Update()`.

### StartButton
**File:** [Assets/StartButton.cs](../Assets/StartButton.cs)

| Method | Loads |
|---|---|
| `StartGame()` | `PrototypeSelectScene` |
| `StartEncounter()` | `GameScene` |

### TutorialButton
**File:** [Assets/TutorialButton.cs](../Assets/TutorialButton.cs)

| Method | Loads |
|---|---|
| `LoadTutorial()` | `TutorialScene` |

### PrototypeSelectButton
**File:** [Assets/PrototypeSelectButton.cs](../Assets/PrototypeSelectButton.cs)

| Method | Loads |
|---|---|
| `SelectPrototype0()` | `EncountersScene` |

This is the point where `MasterGameManager` first comes into existence — it is created when EncountersScene loads for the first time.

### Restart
**File:** [Assets/Restart.cs](../Assets/Restart.cs)

| Method | What it does |
|---|---|
| `RestartGame()` | Calls `Destroy(MasterGameManager.instance.gameObject)` to wipe all persistent state, then loads `PrototypeSelectScene` |

This is a full hard reset. The next `PrototypeSelectButton` click will create a fresh `MasterGameManager`.
