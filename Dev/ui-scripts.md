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

## TutorialPanel

**File:** [Assets/TutorialPanel.cs](../Assets/TutorialPanel.cs)
**Scene:** GameScene (built programmatically; no scene wiring)

Overlay that auto-opens on GameScene load and teaches the player what they're looking at. Built entirely in code from [`GameManager.EnsureTutorialPanel()`](../Assets/GameManager.cs), mirroring the [`EnsurePrototypeUI`](../Assets/GameManager.cs) pattern — no prefab or Inspector wiring required.

### Two launch modes

| Mode | Trigger | Close behavior |
|---|---|---|
| `Splash` | [`TutorialButton`](../Assets/TutorialButton.cs) on SplashScreen sets `MasterGameManager.isTutorialLaunch = true` before loading GameScene | `SceneManager.LoadScene("SplashScreen")` |
| `Encounter` | Normal GameScene load from an encounter | `overlayRoot.SetActive(false)` — battle state is preserved (no scene reload) |

`GameManager.Start()` consumes `isTutorialLaunch` immediately after creating the panel so a Retry or reload doesn't re-enter splash-mode.

### Two views max

Only two pages are ever shown — never a global archetype picker:

- **Core** — arrows/labels pointing at Player Health, Enemy Health, Enemy Intent, Current Card, the Left/Right swipe zones, and the View Deck button.
- **Archetype** — mechanics-specific explanation for the current `MasterGameManager.PrototypeMode` (Enraged Boss, Requeue, Infiltrator, Mirror). For `CoreLoop` encounters there is no separate archetype page and the toggle is hidden.

In Encounter mode the overlay opens on the Archetype page with a **"View Core Tutorial"** toggle link at the bottom that swaps to Core and back. In Splash mode only the Core page is shown (toggle hidden).

### Overlay stacking

The overlay uses its own child `Canvas` with `overrideSorting = true` and `sortingOrder = 100`, so it renders above the scene's WinPanel / GameOverPanel / DeckView. A semi-transparent `Image` backdrop blocks raycasts so swipes don't fall through to cards underneath.

Three buttons live on their own `Canvas` siblings at `sortingOrder = 101` so they stay clickable even while the backdrop is showing:

| Button | Position | Behavior |
|---|---|---|
| Tutorial / Close toggle | Bottom-right | Opens the overlay when hidden ("Tutorial"), closes it when visible ("Close"). Same button, label flips. |
| View Core / View Encounter toggle | Bottom-right (left of the HUD toggle) | Swaps the active page between Core and Archetype. Hidden in Splash mode and in Core Loop encounters. |
| Exit | Bottom-left | Destroys `MasterGameManager` (if present), clears `isTutorialLaunch`, and loads `PrototypeSelectScene` — same semantics as `RetryButton.BackToMenu`. |

### Callout positioning

Arrows and labels use the lightweight [`TutorialCallout`](#tutorialcallout) component so they anchor to live HUD elements and follow the Canvas Scaler. Each callout has either a `uiTarget` (a `RectTransform` on the Canvas — player health, enemy status, drop zones in the HUD, etc.) or a `worldTarget` (a world-space `Transform` projected through `Camera.main` — the active card, which `GameManager.DrawCard` parks at world `(0, 5, -2.1)`). Offsets are applied in screen pixels.

### Public methods

| Method | Purpose |
|---|---|
| `static Create(Canvas, GameManager, PrototypeMode, PanelMode)` | Factory. Called once from `GameManager.EnsureTutorialPanel()`. |
| `AutoOpen()` | Called by `Create`; picks the starting view based on mode + archetype. |
| `OpenHud()` | HUD button when the overlay is hidden — reopens on the archetype page. |
| `Close()` | HUD button when the overlay is visible. Splash mode returns to `SplashScreen`; Encounter mode just hides the overlay. |
| `ToggleView()` | View Core / View Encounter button — swaps Core ↔ Archetype (no-op in Splash mode or CoreLoop encounter). |

### Adding a new archetype

`BuildArchetypePage()` is a switch on `PrototypeMode`. To add a new archetype: add a case with its arrows/labels. No other changes needed — the rest of the panel (open/close/toggle/HUD/Exit) is archetype-agnostic.

---

## TutorialCallout

**File:** [Assets/TutorialCallout.cs](../Assets/TutorialCallout.cs)

Tiny helper component attached to every tutorial arrow/label. Each `LateUpdate` it reads its target's live position and sets `transform.position = target + pixelOffset`.

| Field | Purpose |
|---|---|
| `uiTarget` | `RectTransform` on the scene Canvas. Position is read directly — works for Screen Space Overlay. |
| `worldTarget` | 3D `Transform`. Projected via `worldCamera.WorldToScreenPoint` (camera falls back to `Camera.main`). |
| `pixelOffset` | Screen-pixel offset from the target. Positive Y is up. |

Exactly one of `uiTarget` / `worldTarget` should be set per callout. `TutorialPanel.AddCallout` and `TutorialPanel.AddWorldCallout` wire the component up.

**Why this pattern:** the callouts stay aligned regardless of what the Canvas Scaler is doing at different screen sizes, because they follow the target's post-scaling screen position every frame.

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
**Scene:** SplashScreen

| Method | What it does |
|---|---|
| `LoadTutorial()` | Sets `MasterGameManager.isTutorialLaunch = true`, then `LoadScene("GameScene")`. GameScene reads the flag in `Start()` and opens [`TutorialPanel`](#tutorialpanel) in splash-mode — Close returns to SplashScreen. |

### PrototypeSelectButton
**File:** [Assets/PrototypeSelectButton.cs](../Assets/PrototypeSelectButton.cs)

| Method | Loads |
|---|---|
| `SelectPrototype0()` | `EncountersScene` |

This is the point where `MasterGameManager` first comes into existence — it is created when EncountersScene loads for the first time.

### PrototypeButton
**File:** [Assets/PrototypeButton.cs](../Assets/PrototypeButton.cs)  
**Scene:** PrototypeSelectScene

Attach to each per-prototype button. Set `prototypeMode` in the Inspector to the desired `MasterGameManager.PrototypeMode` value.

| Inspector Field | Purpose |
|---|---|
| `prototypeMode` | Which prototype rule set this button launches |

| Method | What it does |
|---|---|
| `SelectPrototype()` | Sets `MasterGameManager.pendingPrototype` (and `selectedPrototype` + `SetupTemplateEncounter()` if an MGM instance exists), then loads `GameScene` directly — no EncountersScene map |

### RetryButton
**File:** [Assets/RetryButton.cs](../Assets/RetryButton.cs)  
**Scene:** GameScene (on templateWinPanel / templateLosePanel)

| Method | What it does |
|---|---|
| `RetryPrototype()` | Calls `SetupTemplateEncounter()` on MGM (if present) and reloads `GameScene`; `pendingPrototype` static is still set so rules are preserved |
| `BackToMenu()` | Destroys `MasterGameManager` if present and loads `PrototypeSelectScene` |

### Restart
**File:** [Assets/Restart.cs](../Assets/Restart.cs)

| Method | What it does |
|---|---|
| `RestartGame()` | Calls `Destroy(MasterGameManager.instance.gameObject)` to wipe all persistent state, then loads `PrototypeSelectScene` |

This is a full hard reset. The next `PrototypeSelectButton` click will create a fresh `MasterGameManager`.
