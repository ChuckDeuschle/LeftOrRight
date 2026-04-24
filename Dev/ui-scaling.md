[← Index](INDEX.md)

# UI Scaling

How the game renders at different screen sizes — CanvasScaler settings, anchor ruleset, and the Editor migration script.

## Design target

- **Reference resolution:** 1280×720 (720p, 16:9)
- **Ship target:** WebGL embed at 1280×720 (set in `ProjectSettings.WebGL`)
- **Fullscreen / any 16:9 resolution:** UI scales uniformly — no letterbox, no distortion

## CanvasScaler settings (all Canvases)

| Field | Value |
|---|---|
| UI Scale Mode | Scale With Screen Size |
| Reference Resolution | 1280 × 720 |
| Screen Match Mode | Expand |
| Match | 0 (ignored under Expand) |
| Reference Pixels Per Unit | 100 |

With **Expand**, UI scales by `min(width / 1280, height / 720)` — whichever axis is more constrained. On any 16:9 screen both ratios are equal, so scaling is uniform. On taller/squatter aspects, UI fits inside the shorter axis and extra space appears on the longer axis; corner-anchored HUD elements stay pinned to the screen edges regardless.

## Anchor ruleset

Center-anchor + large pixel offset is fragile — it only works at one resolution. Assign anchors by the element's **role**:

| Role | Examples | Anchor min/max | Pivot |
|---|---|---|---|
| HUD top-left | Player Health | (0, 1) / (0, 1) | (0, 1) |
| HUD top-right | Enemy Health, Enemy Intent | (1, 1) / (1, 1) | (1, 1) |
| HUD bottom-left | Exit | (0, 0) / (0, 0) | (0, 0) |
| HUD bottom-right | Close, View Deck | (1, 0) / (1, 0) | (1, 0) |
| Top stretched | Title / tutorial header | (0, 1) / (1, 1) | (0.5, 1) |
| Center gameplay | Current card, cards row | (0.5, 0.5) / (0.5, 0.5) | (0.5, 0.5) |
| Full-screen overlay | Tutorial backdrop, modal blocker | (0, 0) / (1, 1) | (0.5, 0.5) |

For elements currently center-anchored with pixel offsets, recover the new `anchoredPosition` using the 1280×720 reference:

- center → top-left: `newX = oldX + 640`, `newY = oldY − 360`
- center → top-right: `newX = oldX − 640`, `newY = oldY − 360`
- center → bottom-left: `newX = oldX + 640`, `newY = oldY + 360`
- center → bottom-right: `newX = oldX − 640`, `newY = oldY + 360`

## Editor menu commands

File: [Assets/Editor/UiScalingMigration.cs](../Assets/Editor/UiScalingMigration.cs)

| Menu path | What it does |
|---|---|
| `Tools > UI > Apply CanvasScaler Settings` | Opens every enabled scene in Build Settings, rewrites every `CanvasScaler` to the values above, saves. Idempotent — safe to re-run after tweaking the constants at the top of the script. |
| `Tools > UI > Audit Anchors (Dry Run)` | Logs every `RectTransform` that is center-anchored with `|anchoredPosition.x| > 200` or `|y| > 150`. These are candidates for the anchor rework table above. Read-only. |

## TutorialCallout

Tutorial arrows and labels use [`TutorialCallout`](ui-scripts.md#tutorialcallout) which reads the target's live screen position every `LateUpdate` — so callouts stay aligned regardless of scaler state or screen size, without needing special handling during the migration.
