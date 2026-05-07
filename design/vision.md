[← Index](INDEX.md)

# Vision

## Core Premise

A deck-building roguelike where you battle by casting cards from your left or right hand. Each swipe sends the drawn card to the matching hand, which casts its spell or ability — attack, shield, or something more exotic. The interaction should feel fast, tactile, and instinctive — closer to Tinder than to Slay the Spire's hand management.

## What Makes This Unique

The left/right binary is the constraint that generates all interesting decisions. Unlike traditional card games where you choose *which* card to play, here you choose *how* to play every card you're given. The deck order and the directional choice together are the strategy.

Because each direction is a hand, the hands themselves become a visible character the player customizes over a run. Two hands, on screen, wearing what you've earned — this is the persistent, evolving avatar of your run, not a portrait in a corner.

## Thematic Framing

The player's two hands are the visual anchor of combat. When a card resolves left, the left hand casts. When a card resolves right, the right hand casts. This opens several design spaces:

- **Rings** are equipped on a specific hand (left or right) and only affect spells cast from that hand. A `+2 shield` ring on the right hand adds shield only when a card is swiped right; the same ring on the left hand would do nothing on a right-swipe. Asymmetric builds are the natural endpoint — a right hand stacked for defense and a left hand stacked for damage tells the story of the run at a glance.
- **Held items** — weapons, tools, focuses — alter what that hand does. A sword in the right hand might turn right-cast shield cards into counter-attacks; a staff in the left might amplify damage but cost HP.
- **Visual state** — each hand's condition (crit charge, chain, buffs, rage of a tracked enemy) can live on or around the hand itself, keeping the combat HUD spatial and legible.

Equipment as a full system is not yet designed — this section is a sketch of the design space the framing opens up.

## Target Feel

*(To be filled in — what should a good run feel like? What's the emotional arc of a battle?)*

## Design Constraints

- Every mechanic must be expressible through left/right swipes (or minimal supplementary UI)
- Trade-offs should be legible quickly — players are swiping, not deliberating
- Roguelike progression should reward deck knowledge built over multiple runs
