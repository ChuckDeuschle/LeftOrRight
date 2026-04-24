# LeftOrRight

A deck-building roguelike where you only ever swipe left or right on the cards.

**[Play the alpha on itch.io →](https://cldeuschle.itch.io/left-or-right)** (WebGL, runs in-browser — it's an alpha, expect rough edges.)

## Core loop

- Draw a card. Swipe it left or right. Each direction resolves a different action; the action depends on the card and the current rule set.
- The enemy shows its next move and a card-countdown. Every swipe ticks the countdown down.
- When the countdown hits zero, the enemy acts. Whatever defenses you've built absorb the hit, then reset.
- Win the encounter → pick a reward card → deck grows → next fight hits harder.
- Lose → run ends.

Everything else — enemy archetypes, the prototype rule sets I'm experimenting with, tutorial flow — lives in the [design wiki](Design/INDEX.md).

## Under the hood

- **Engine:** Unity 6000.1.1f1
- **Target:** WebGL, 1280×720, shipped to itch.io
- **Studio:** MagicRedBeard (solo / personal)
- **Status:** Alpha

## The two wikis

The documentation lives in two wikis inside the repo.

- **[Dev wiki →](Dev/INDEX.md)** — architecture, scripts, the GameManager state machine, the prototype rules system, UI scaling, deployment. Start at [Dev/architecture.md](Dev/architecture.md) if you're poking around the code.
- **[Design wiki →](Design/INDEX.md)** — vision, core loop, enemy archetype designs, tutorial thinking.

## How Claude Code is used on this project

This is a side project I'm working on with Claude Code. I take a pair programming approach where I'm doing design and code work manually as well as with Claude.

**1. Claude is the wiki maintainer.** The rule, written into [CLAUDE.md](CLAUDE.md): every code change updates the relevant `Dev/` or `Design/` page in the same commit, not as a follow-up. In practice, most commits touch both `Assets/*.cs` and `Dev/*.md`. The wikis don't drift because updating them isn't optional.

**2. A pre-commit hook enforces it.** [.githooks/pre-commit](.githooks/pre-commit) scans staged files — if any `Assets/*.cs` is staged without at least one file from `Dev/`, the commit fails with a message telling me which scripts changed and that the wiki needs updating.

**3. Auto-build on C# edits.** [.claude/settings.json](.claude/settings.json) registers a `PostToolUse` hook: whenever Claude writes or edits a `.cs` file, `dotnet build` runs on the solution. Compile errors surface at edit time instead of at Unity Play time.

**4. Pair-programming, not autopilot.** I drive, Claude writes. The wiki-update requirement exists specifically because I want docs that stay in sync with the code. Claude updates it much more quickly than I could by hand.

Full conventions for Claude on this project: [CLAUDE.md](CLAUDE.md).

## Build & run

- Open the project in Unity Hub (6000.1.1f1).
- Scene play order is set in **File > Build Settings**. Run `SplashScreen` from the top, or open any scene directly for isolated testing.
- First time cloning? Activate the wiki-update hook once: `git config core.hooksPath .githooks`.

WebGL build + itch.io deploy instructions: [Dev/deployment.md](Dev/deployment.md).

## Repo layout

`Assets/` holds the Unity project, `Dev/` and `Design/` are the two wikis, `.claude/` + `CLAUDE.md` configure Claude Code, `.githooks/` holds the wiki-update guardrail, and `deploy.sh` pushes WebGL builds to itch.io.
