[← Index](INDEX.md)

# Conventions

## The `Initalize()` Typo

Every game entity uses `Initalize()` (one 'i') instead of `Initialize()`. This is intentional and consistent across the codebase. **Preserve the typo** when adding any new entity that follows this pattern. Fixing it would break the convention and create inconsistency.

Scripts that use it: [Action](../Assets/Action.cs), [Card](../Assets/Card.cs), [Player](../Assets/Player.cs), [Enemy](../Assets/Enemy.cs), [Encounter](../Assets/Encounter.cs).

---

## File Layout

- All C# scripts live directly in `Assets/` — no subdirectories, no subfolders
- No namespaces are used anywhere in the project
- New scripts go in `Assets/` at the root level

---

## Data Models vs. MonoBehaviours

Not everything in this codebase is a MonoBehaviour. Know the difference:

| Class | Type | Why |
|---|---|---|
| [Player](../Assets/Player.cs) | Plain C# class | Pure data; no scene attachment needed |
| [Enemy](../Assets/Enemy.cs) | Plain C# class | Pure data; no scene attachment needed |
| [Action](../Assets/Action.cs) | Plain C# class | Pure data; no scene attachment needed |
| [Encounter](../Assets/Encounter.cs) | Plain C# class | Pure data; no scene attachment needed |
| [Card](../Assets/Card.cs) | MonoBehaviour | Needs to be a GameObject (has a Sprite, exists in scene hierarchy, is dragged) |
| [GameManager](../Assets/GameManager.cs) | MonoBehaviour | Scene controller; needs Unity lifecycle events |
| [MasterGameManager](../Assets/MasterGameManager.cs) | MonoBehaviour | Needs `DontDestroyOnLoad`, which requires MonoBehaviour |
| All UI scripts | MonoBehaviour | Need Unity lifecycle events or Inspector references |

When adding a new entity, default to a plain C# class unless you specifically need scene attachment, `DontDestroyOnLoad`, or Unity lifecycle events.

---

## UI Text

Always use **TextMeshPro** — never Unity's legacy `Text` component.

| Situation | Type to use |
|---|---|
| World space text on a card | `TextMeshPro` |
| UI canvas text (HUD, panels) | `TextMeshProUGUI` |

Import: `using TMPro;`

---

## Card & Encounter Creation

Cards and encounters are created **procedurally in C#** — not via ScriptableObjects, JSON files, or Unity assets. 

- New cards: add to `GameManager.CreateStarterDeck()` and/or `MasterGameManager.CreateStarterDeck()`
- New encounters: add to `MasterGameManager.CreateEncounterPath()`
- New reward cards: add to the `awardCards` list when constructing an `Encounter`

---

## Tag Usage

`DropZone.Start()` finds the GameManager using `GameObject.FindWithTag("GameManager")`. The GameManager GameObject in GameScene must have the tag `"GameManager"` set in the Unity Inspector.

If you add other scenes that use DropZone, ensure the scene's GameManager has this tag.
