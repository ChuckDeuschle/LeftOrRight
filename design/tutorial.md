[← Index](INDEX.md)

# Tutorial

The tutorial is an **overlay** that sits on top of the live GameScene UI. Arrows and labels point at the real HUD elements so there is no separate mockup to maintain — if the HUD moves, the tutorial's anchors move with it.

## Entry points

- **Splash screen**: the Tutorial button loads GameScene in splash-mode. The overlay opens on the Core Loop page. Close returns to SplashScreen.
- **Inside any encounter**: the tutorial **auto-opens** when GameScene starts, pre-selected to the archetype page for the current encounter (Enraged Boss, Requeue, Infiltrator, Mirror). Close hides the overlay and returns control to the live battle with no state loss. The HUD Tutorial button reopens it at any time.

## Two views max

The player only ever sees one of two pages at a time — never a browse-every-archetype list:

- **Core** — what every screen is (health, intent, swipe zones, deck button, the card).
- **Archetype** — what *this* encounter adds on top (rage meter, requeue reorder, traps, chain).

In Encounter mode a single toggle link at the bottom swaps between the two. In Splash mode (and Core Loop encounters) the toggle is hidden — there is no archetype layer to switch to.

## Why this shape

- **One-at-a-time** keeps the tutorial focused on what the player is actually doing. A global archetype picker would show arrows pointing at UI that isn't on this screen.
- **Auto-open at encounter start** means testers who don't know a mechanic can't miss it.
- **The HUD button is a toggle** (label flips "Tutorial" ↔ "Close") — one control, no separate back/exit button to explain.

## Always-on HUD buttons

Three buttons live on the HUD at all times (they remain clickable even when the tutorial overlay is open):

- **Tutorial / Close** (bottom-right) — opens/closes the overlay.
- **View Core / View Encounter** (bottom-right, left of Tutorial) — only shown when both pages exist; swaps between them.
- **Exit** (bottom-left) — returns to the prototype select menu and clears run state.

## Authoring new archetypes

Add a switch case in [`TutorialPanel.BuildArchetypePage`](../Assets/TutorialPanel.cs) with the arrows and labels for the new mechanic. Nothing else in the tutorial system needs to change.
