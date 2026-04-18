[← Index](INDEX.md)

# Prototype Rules

The prototype rules system lets each encounter run with a different rule set without touching `GameManager` core logic. Rules are swapped by selecting a prototype mode before loading `GameScene`.

## Overview

```
MasterGameManager.PrototypeMode (enum)
    │  set by PrototypeButton before scene load
    ▼
PrototypeRules.CreateForMode(mode) → concrete subclass
    │  called in GameManager.Start()
    ▼
GameManager.activeRules  ← all hooks flow through here
```

## PrototypeRules (abstract base)

**File:** [Assets/PrototypeRules.cs](../Assets/PrototypeRules.cs)  
**Type:** Plain C# abstract class (not a MonoBehaviour)

All methods have default no-op / pass-through implementations so subclasses only override what they need.

| Method | Signature | Default | Purpose |
|---|---|---|---|
| `OnCardPlayed` | `(Card, ZoneSide, GameManager)` | no-op | Called by `GameManager.OnCardPlayed()` after each card action executes |
| `ShouldInterruptPlayerTurn` | `(GameManager) → bool` | `false` | If true, `DropZone` triggers `EnemyTurn` instead of drawing the next card |
| `ModifyEnemyDamage` | `(int baseDamage, GameManager) → int` | pass-through | Adjusts `currentRound * 10` base damage before it's applied to the player |
| `OnEnemyTurnEnd` | `(GameManager)` | no-op | Called at the end of `Enemy.EnemyAction()`; use to reset per-turn counters |
| `GetStatusText` | `(GameManager) → string` | `""` | Returns text for the prototype status label; empty string hides the label |
| `ShowEndTurnButton` | `(GameManager) → bool` | `false` | Controls visibility of the voluntary End Turn button in `GameScene` |
| `CreateForMode` | `static (PrototypeMode) → PrototypeRules` | — | Factory; returns the correct subclass for the given mode |

## BaselineRules

**File:** [Assets/Rules/BaselineRules.cs](../Assets/Rules/BaselineRules.cs)

Inherits `PrototypeRules` with no overrides. Enemy only attacks when the deck empties (original behavior). No status label, no End Turn button.

## RageTimerRules (P5)

**File:** [Assets/Rules/RageTimerRules.cs](../Assets/Rules/RageTimerRules.cs)

Each card played fills a rage meter. When the meter reaches the threshold the enemy attacks immediately (mid-turn interrupt) with bonus damage. The player can voluntarily end their turn early to avoid the bonus.

| Field | Value | Purpose |
|---|---|---|
| `cardsPlayedThisTurn` | `int` (private) | Cards played since the last enemy turn |
| `rageThreshold` | `5` (const) | Cards-per-turn limit before interrupt fires |

| Hook | Behavior |
|---|---|
| `OnCardPlayed` | Increments `cardsPlayedThisTurn` |
| `ShouldInterruptPlayerTurn` | Returns `true` when `cardsPlayedThisTurn >= rageThreshold` |
| `ModifyEnemyDamage` | Adds `(cardsPlayedThisTurn - rageThreshold + 1) * 5` bonus damage when rage was full; otherwise pass-through |
| `OnEnemyTurnEnd` | Resets `cardsPlayedThisTurn` to 0 |
| `GetStatusText` | Returns `"Rage: {cardsPlayedThisTurn} / {rageThreshold}"` |
| `ShowEndTurnButton` | `true` — player can yield before rage fills |

## Adding a New Prototype

1. Create `Assets/Rules/YourRules.cs` inheriting `PrototypeRules`; override only the hooks you need.
2. Add a value to `MasterGameManager.PrototypeMode` enum.
3. Add a case to `PrototypeRules.CreateForMode()`.
4. Add a `PrototypeButton` in `PrototypeSelectScene` and set `prototypeMode` in the Inspector.

No changes to `GameManager`, `DropZone`, or `Enemy` are needed.

## GameScene Inspector Wiring

These fields on `GameManager` must be assigned in the GameScene Inspector for prototype features to work:

| Field | What to wire |
|---|---|
| `prototypeStatusText` | A `TextMeshProUGUI` label for rage/chain/etc. display; hidden automatically when `GetStatusText` returns empty |
| `endTurnButton` | A `GameObject` with a Button whose `onClick` calls `GameManager.EndPlayerTurn()`; hidden when `ShowEndTurnButton` returns false or it is not the player's turn |
| `templateWinPanel` | A panel with `RetryButton.RetryPrototype()` and `RetryButton.BackToMenu()` buttons; shown on win when no `MasterGameManager` instance is present |
| `templateLosePanel` | Same as above, shown on lose in template mode |
