# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview

**LeftOrRight** is a deck-building roguelike card game built in Unity 6000.1.1f1 (company: MagicRedBeard). Players battle enemies by dragging cards left or right to activate different actions (Attack/Shield). Combat is turn-based with a roguelike encounter progression.

## Build & Run

This is a standard Unity project — open the project folder in Unity Hub or Unity Editor (6000.1.1f1). No build scripts or CI exist; use the Unity Editor Play button for testing. All testing is manual in-editor.

The scene play order is defined in File > Build Settings. Run `SplashScreen` to start from the beginning, or open any scene directly for isolated testing.

## Architecture

See [Dev/architecture.md](Dev/architecture.md) for scene flow, the GameManager state machine, and cross-scene data ownership. See the full [Dev wiki](Dev/INDEX.md) for all systems and scripts.

## Development Environment

**Primary IDE:** VS Code 1.113+ with the following extensions (all already installed):
- `visualstudiotoolsforunity.vstuc` — Unity debugger and project sync
- `ms-dotnettools.csdevkit` + `ms-dotnettools.csharp` — C# IntelliSense and build
- `anthropic.claude-code` — Claude Code sidebar (side-loaded by the CLI)

The `.vscode/` folder is committed to the repo and pre-configures file exclusions, YAML associations for Unity assets, and `dotnet.defaultSolution = LeftOrRight.sln`.

## Git & GitHub

- Remote: `https://github.com/ChuckDeuschle/LeftOrRight.git` (owner: ChuckDeuschle)
- Main branch: `master`
- Do **not** add `Co-Authored-By:` lines to commit messages.

**Git hooks:** A pre-commit hook in `.githooks/pre-commit` blocks commits that stage `.cs` changes without also staging a `Dev/` wiki update. After a fresh clone, activate it once with:
```
git config core.hooksPath .githooks
```

## Key Conventions

See [Dev/conventions.md](Dev/conventions.md) for the full list, including the intentional `Initalize()` typo, file layout rules, data model patterns, and TMPro usage.

## Wikis

Two wikis live in the repo root and must be kept current:

| Wiki | Path | Covers |
|---|---|---|
| Dev | `Dev/INDEX.md` | All C# scripts, systems, architecture, conventions |
| Design | `Design/INDEX.md` | Vision, gameplay prototypes, design decisions |

**Before making changes:** Read the relevant wiki page first — do not explore raw files if the wiki covers the area.

**When making changes:** Update the relevant wiki page in the same task — not as a follow-up. A wiki that drifts behind the code actively misleads future sessions.

- Code change (new/modified script, new scene, renamed field) → update the relevant `Dev/` page
- Design decision (new mechanic, prototype iteration) → update the relevant `Design/` page
- New script → add it to the appropriate `Dev/` topic page with a link to its `.cs` file

**Dev wiki link convention:** Script file references use relative links from the `Dev/` folder: `[Assets/FileName.cs](../Assets/FileName.cs)`. Scene `.unity` files are binary and not linked — leave scene names as plain text.
