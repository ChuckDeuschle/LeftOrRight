[← Index](INDEX.md)

# Combat

## Overview

The card-battle loop from player input to next state:

```
Player grabs card (DraggableCard.OnBeginDrag)
    │
    ▼
Player drags card to a drop zone (DraggableCard.OnDrag — tracks mouse)
    │
    ▼
Player releases over a DropZone (DropZone.OnDrop)
    ├── reads DropZone.zoneSide (Left or Right)
    ├── calls card.leftAction.Play(gameManager)  OR  card.rightAction.Play(gameManager)
    │       └── Action.Play() mutates enemy.currentHealth  OR  player.shield
    ├── moves card GameObject to discardPile
    └── if deck is not empty → GameManager.DrawCard()
        else if deck empty after reshuffle still no card → transitions to EnemyTurn

Enemy turn (GameManager.Update detects EnemyTurn state)
    └── Enemy.EnemyAction(gameManager) → damages player health reduced by shield
            └── gameState returns to PlayerTurn
```

If card is released outside a drop zone, `DraggableCard.OnEndDrag` snaps it back to its original position.

---

## Card

**File:** [Assets/Card.cs](../Assets/Card.cs)  
**Type:** MonoBehaviour (attached to card prefab GameObjects)

| Member | Type | Purpose |
|---|---|---|
| `cardName` | `string` | Display name shown on the card |
| `leftAction` | `Action` | The action triggered when dragged left |
| `rightAction` | `Action` | The action triggered when dragged right |
| `sprite` | `Sprite` | Card artwork |
| `Initalize(name, leftAction, rightAction)` | method | Sets all fields and calls `SetText()` |
| `SetText()` | method | Updates the attached TMPro component with name + action labels |

Card GameObjects are instantiated from `cardPrefab` and marked `DontDestroyOnLoad` when added to the player's deck via MasterGameManager.

---

## Action

**File:** [Assets/Action.cs](../Assets/Action.cs)  
**Type:** Plain C# class (not a MonoBehaviour)

| Member | Type | Purpose |
|---|---|---|
| `ActionType` | `Actions` enum | `Attack` or `Shield` |
| `Label` | `string` | Short display text (e.g. "Attack 10") |
| `Magnitude` | `int` | Numeric value of the effect |
| `Initalize(label, action, magnitude)` | method | Sets all fields |
| `Play(gameManager)` | method | If `Attack`: reduces `enemy.currentHealth` by `Magnitude`. If `Shield`: adds `Magnitude` to `player.shield` |

Shield is consumed (reset to 0) at the start of each enemy turn inside `Enemy.EnemyAction()`.

---

## DraggableCard

**File:** [Assets/DraggableCard.cs](../Assets/DraggableCard.cs)  
**Type:** MonoBehaviour, implements `IBeginDragHandler`, `IDragHandler`, `IEndDragHandler`

| Event | What happens |
|---|---|
| `Awake()` | Caches the `Image` component |
| `OnBeginDrag` | Saves `originalPosition`; sets `image.raycastTarget = false` so the card doesn't block drop zone detection |
| `OnDrag` | Converts `eventData.position` (screen space) to world space at `z = 7.7f` and moves the card there |
| `OnEndDrag` | Snaps card back to `originalPosition`; re-enables `raycastTarget` |

`OnEndDrag` always snaps back — the actual "consume card" logic happens in `DropZone.OnDrop`, which fires before `OnEndDrag` when a valid drop occurs.

---

## DropZone

**File:** [Assets/DropZone.cs](../Assets/DropZone.cs)  
**Type:** MonoBehaviour, implements `IDropHandler`

| Member | Type | Purpose |
|---|---|---|
| `zoneSide` | `ZoneSide` enum | `Left` or `Right` — set in the Inspector for each drop zone GameObject |
| `Start()` | event | Finds and caches `GameManager` using the tag `"GameManager"` |
| `OnDrop(eventData)` | method | Core drop handler (see below) |

**OnDrop logic:**
1. Gets the `DraggableCard` component from the dropped object
2. Gets the `Card` component from the same object
3. Executes `card.leftAction.Play(gameManager)` or `card.rightAction.Play(gameManager)` based on `zoneSide`
4. Adds the card GameObject to `gameManager.discardPile`
5. Sets `gameManager.currentCard = null`
6. If `gameManager.deck.Count > 0`: calls `gameManager.DrawCard()`
7. Otherwise: sets `gameManager.gameState = GameState.EnemyTurn`
