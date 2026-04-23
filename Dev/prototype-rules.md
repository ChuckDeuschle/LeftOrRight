[← Index](INDEX.md)

# Prototype Rules

Every encounter runs on the [core loop](#coreloophooks) (intent + countdown) and optionally layers an **archetype** rule set on top. Rules are swapped by selecting a prototype mode before loading `GameScene`.

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

`CoreLoopRules` is the default for every mode except the archetypes. Archetype classes inherit from `CoreLoopRules` so the countdown still drives enemy timing; archetypes add extra mechanics on top.

## PrototypeRules (abstract base)

**File:** [Assets/PrototypeRules.cs](../Assets/PrototypeRules.cs)
**Type:** Plain C# abstract class (not a MonoBehaviour)

All methods have default no-op / pass-through implementations so subclasses only override what they need.

| Method | Signature | Default | Purpose |
|---|---|---|---|
| `Initialize` | `(GameManager)` | no-op | Called once from `GameManager.Start()` after the enemy/player/deck are resolved. Use to flip compound intents on the enemy (e.g. `enemy.addsTrap = true`) |
| `OnCardPlayed` | `(Card, ZoneSide, GameManager)` | no-op | Called by `GameManager.OnCardPlayed()` after each card action executes |
| `ShouldInterruptPlayerTurn` | `(GameManager) → bool` | `false` | If true, `DropZone` triggers `EnemyTurn` instead of drawing the next card |
| `ModifyEnemyDamage` | `(int baseDamage, GameManager) → int` | pass-through | Adjusts the enemy's scheduled `intentValue` before it's applied |
| `OnEnemyTurnEnd` | `(GameManager)` | no-op | Called at the end of `Enemy.EnemyAction()`; use to reset per-turn counters or schedule the next intent |
| `GetStatusText` | `(GameManager) → string` | `""` | Returns text for the prototype status label; empty string hides the label |
| `ShowEndTurnButton` | `(GameManager) → bool` | `false` | Controls visibility of the voluntary End Turn button in `GameScene` |
| `CreateForMode` | `static (PrototypeMode) → PrototypeRules` | — | Factory; returns the correct subclass for the given mode |

## CoreLoopRules (the always-on foundation) <a id="coreloophooks"></a>

**File:** [Assets/Rules/CoreLoopRules.cs](../Assets/Rules/CoreLoopRules.cs)

Implements the core loop: the enemy displays an intent ("Attack 7 in 4 cards"), the countdown decrements as the player swipes cards, and when it hits zero the enemy performs the scheduled action. After the action, a new intent is scheduled with escalation applied.

| Hook | Behavior |
|---|---|
| `OnCardPlayed` | Decrements `currentEnemy.intentCountdown` |
| `ShouldInterruptPlayerTurn` | Returns `true` when `currentEnemy.intentCountdown <= 0` |
| `OnEnemyTurnEnd` | Calls `currentEnemy.ScheduleNextIntent(gm)` — resets countdown, applies escalation |

Intent scheduling, escalation constants, and value live on the `Enemy` data class — see [managers.md](managers.md#enemy).

## EnragedBossRules (Enraged Boss archetype)

**File:** [Assets/Rules/EnragedBossRules.cs](../Assets/Rules/EnragedBossRules.cs)
Inherits from `CoreLoopRules`.

Each card played fills a rage meter on top of the normal countdown. When rage reaches threshold the boss interrupts the scheduled intent and hits harder than countdown alone would have. The player can voluntarily End Turn early to fight the boss at lower rage.

| Field | Value | Purpose |
|---|---|---|
| `cardsPlayedThisTurn` | `int` (private) | Cards played since the last enemy turn |
| `rageThreshold` | `5` (const) | Cards-per-turn limit before the rage interrupt fires |

| Hook | Behavior |
|---|---|
| `OnCardPlayed` | Calls `base` (decrements countdown) then increments `cardsPlayedThisTurn` |
| `ShouldInterruptPlayerTurn` | Returns `base || cardsPlayedThisTurn >= rageThreshold` — either the countdown or the rage meter can trigger the enemy turn |
| `ModifyEnemyDamage` | Adds `(cardsPlayedThisTurn - rageThreshold + 1) * 5` bonus damage when rage was full; otherwise pass-through |
| `OnEnemyTurnEnd` | Calls `base` (schedules next intent) then resets `cardsPlayedThisTurn` to 0 |
| `GetStatusText` | Returns `"Rage: {cardsPlayedThisTurn} / {rageThreshold}"` |
| `ShowEndTurnButton` | `true` — player can yield before rage fills |

## RequeueEnemyRules (Requeue Enemy archetype)

**File:** [Assets/Rules/RequeueEnemyRules.cs](../Assets/Rules/RequeueEnemyRules.cs)
Inherits from `CoreLoopRules`.

When the enemy acts, the discard pile slams back into the deck — but reordered: cards you played to the **left** go to the **top** of the deck (you see them again soon); cards you played to the **right** go to the **bottom** (pushed further away). The enemy disrupts the rhythm the player was building.

Relies on `GameManager.discardDirections`, a parallel list to `discardPile` populated by `DropZone.OnDrop`.

| Hook | Behavior |
|---|---|
| `OnEnemyTurnEnd` | Calls `base` (schedule next intent), then partitions `discardPile` into lefts/rights and rebuilds `deck = lefts + deck + rights`. Clears discard and discardDirections. |
| `GetStatusText` | Returns `"Requeue: left ↑  right ↓"` as a reminder |

## InfiltratorRules (Infiltrator archetype)

**File:** [Assets/Rules/InfiltratorRules.cs](../Assets/Rules/InfiltratorRules.cs)
Inherits from `CoreLoopRules`.

The enemy's intent itself is compound — its displayed action reads "Intends to attack 7 and add trap in 4 cards". When the countdown fires, the enemy both executes its attack **and** shuffles a fresh trap card into the deck. Traps accumulate across intent cycles. When a trap is played (regardless of side) it deals a flat 5 damage to the player, reduced by shield.

Trap-spawning itself lives on `Enemy.SpawnTrap(gm)` (gated by `enemy.addsTrap`) so the compound intent reads naturally to the player. Trap cards are flagged `Card.isTrap = true` and have no-op left/right actions.

| Hook | Behavior |
|---|---|
| `Initialize` | Sets `gm.currentEnemy.addsTrap = true` — flips the compound intent on for the rest of the encounter |
| `OnCardPlayed` | Calls `base` (decrement countdown); if `card.isTrap`, applies `trapDamage` (5) to player minus any shield |
| `GetStatusText` | Returns `"Traps in play: N"` (count of trap cards across deck + discard) |

## MirrorRules (Mirror archetype)

**File:** [Assets/Rules/MirrorRules.cs](../Assets/Rules/MirrorRules.cs)
Inherits from `CoreLoopRules`.

Consecutive same-direction swipes build a chain. Reaching `chainThreshold` (3) grants a one-time bonus (+5 shield). Breaking a chain that had reached threshold empowers the enemy's next intent (`intentValue += 3`). Rewards rhythm; punishes forced variety.

| Field | Value | Purpose |
|---|---|---|
| `chainThreshold` | `3` (const) | Swipes in a row to trigger a bonus or count as "established" before a break penalty |
| `chainBonusShield` | `5` (const) | Shield granted when the chain hits the threshold |
| `breakPenalty` | `3` (const) | Damage added to the enemy's scheduled intent when an established chain breaks |

| Hook | Behavior |
|---|---|
| `OnCardPlayed` | Calls `base`, then updates chain state. Same direction → increment; hit threshold → grant shield bonus. Different direction → if previous chain reached threshold, bump enemy intent; reset chain. |
| `GetStatusText` | `"Chain ← 3"` or `"Chain: —"` before any swipe |

## Adding a New Archetype

1. Create `Assets/Rules/YourArchetypeRules.cs` inheriting `CoreLoopRules` (not `PrototypeRules` directly — you want the countdown).
2. Override only the hooks your archetype needs. Always call `base` for hooks the core loop uses (`OnCardPlayed`, `OnEnemyTurnEnd`).
3. Add a value to `MasterGameManager.PrototypeMode` enum.
4. Add a case to `PrototypeRules.CreateForMode()`.
5. Add a `PrototypeButton` GameObject in `PrototypeSelectScene` and set `prototypeMode` in the Inspector.

No changes to `GameManager`, `DropZone`, or `Enemy` are needed.

## GameScene Inspector Wiring

These fields on `GameManager` are optional — `EnsurePrototypeUI()` creates them at runtime if not wired:

| Field | What to wire |
|---|---|
| `prototypeStatusText` | A `TextMeshProUGUI` label for archetype-specific state (rage, chain); hidden automatically when `GetStatusText` returns empty. The enemy's intent is rendered by `enemyStatusText`, not here. |
| `endTurnButton` | A `GameObject` with a Button whose `onClick` calls `GameManager.EndPlayerTurn()`; hidden when `ShowEndTurnButton` returns false or it is not the player's turn |
| `templateWinPanel` | A panel with `RetryButton.RetryPrototype()` and `RetryButton.BackToMenu()` buttons; shown on win when no `MasterGameManager` instance is present |
| `templateLosePanel` | Same as above, shown on lose in template mode |
