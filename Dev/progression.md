[← Index](INDEX.md)

# Progression

## Encounter Data Model

**File:** [Assets/Encounter.cs](../Assets/Encounter.cs)  
**Type:** Plain C# class (not a MonoBehaviour)

| Field | Type | Purpose |
|---|---|---|
| `encounterName` | `string` | Display name for the encounter |
| `encounterDescription` | `string` | Flavor/detail text shown on the map screen |
| `encounterButton` | `string` | Name of the UI button GameObject this encounter is assigned to |
| `enemy` | `Enemy` | The enemy the player fights |
| `goldAward` | `int` | Gold granted on victory |
| `awardCards` | `List<Card>` | The 3 cards offered as rewards after winning |
| `currentStatus` | `Status` enum | `available`, `pending`, or `complete` (default: `pending`) |

`Initalize()` sets all fields including an explicit initial status.

---

## Encounter Path

Encounters are hardcoded in `MasterGameManager.CreateEncounterPath()` (called from `Awake()`). Currently 2 encounters exist:

| Encounter | Enemy | Starting Status |
|---|---|---|
| Enemy 1 | Enemy 1, 100 HP | `available` |
| Enemy 2 | Enemy 2, 150 HP | `pending` |

**Status lifecycle:**

```
pending ──► available  (unlocked by ContinueButton after the previous encounter is completed)
available ──► complete (set by ContinueButton after the player wins)
```

Only `available` encounters show an interactable button on the map. `pending` buttons are disabled; `complete` encounters are marked done.

To add more encounters: add them in `MasterGameManager.CreateEncounterPath()` with `Status.pending`, and ensure the prior encounter's reward chain points to them.

---

## EncounterSelectionButton

**File:** [Assets/EncounterSelectionButton.cs](../Assets/EncounterSelectionButton.cs)

A single-purpose script attached to each encounter button in EncountersScene.

| Member | Purpose |
|---|---|
| `encounter` | Inspector-assigned `Encounter` reference |
| `UpdateEncounterSelection()` | Sets `MasterGameManager.instance.selectedEncounter = encounter` |

Called via the button's `onClick` event. After this fires, `StartButton.StartEncounter()` loads GameScene.

---

## Rewards

**File:** [Assets/Rewards.cs](../Assets/Rewards.cs)  
**Type:** MonoBehaviour in GameScene's WinPanel

The rewards UI presents 3 card choices after victory. The player clicks one; `ContinueButton` reads the selection and adds it to the deck.

| Member | Purpose |
|---|---|
| `card1Button`, `card2Button`, `card3Button` | The 3 selectable card buttons |
| `gameManager` | Inspector-assigned reference |
| `UpdateRewardsCards(List<Card>)` | Sets each button's sprite image from the `awardCards` list |
| `SelectCard(Button)` | Highlights the chosen button; sets `gameManager.awardCardSelection` to 0, 1, or 2 |

`Start()` wires `onClick` listeners to each button. `OnDestroy()` removes them.

---

## ContinueButton

**File:** [Assets/ContinueButton.cs](../Assets/ContinueButton.cs)  
**Type:** MonoBehaviour; `ContinueGame()` called via button `onClick`

Handles everything that happens after the player clicks Continue on the WinPanel:

1. Marks `MasterGameManager.instance.selectedEncounter.currentStatus = Status.complete`
2. Reads `gameManager.awardCardSelection` to identify which reward card was chosen
3. Adds that card to `MasterGameManager.instance.deck`
4. Awards `selectedEncounter.goldAward` to `MasterGameManager.instance.player.goldValue`
5. Finds the next `pending` encounter in `encounterList` and sets its status to `available`
6. Loads `EncountersScene`

If no pending encounter exists after step 5, the game effectively ends (no further encounters are unlocked).
