[← Index](../INDEX.md)

# Gameplay Loop Prototypes

## Problem

The current prototype plays the entire deck before the enemy acts, letting players perfectly calculate block vs. damage with no uncertainty. The goal is to introduce meaningful trade-offs while preserving the left/right swipe mechanic that defines the game.

---

## P1: The Countdown

Enemy acts on a visible counter, not at end-of-deck (e.g., every 4 cards played). A counter shows "Enemy attacks in 3." You may hit a string of attack cards right before the counter hits and have to choose between using them or eating the hit unblocked.

**Core trade-off:** You can't perfectly sequence because the counter and your deck position interact. Pressure to get block in before the interrupt fires.

---

## P2: Swipe Direction Sets Requeue Position

Left = card goes to top of discard (cycles back sooner). Right = card goes to bottom (cycles back later).

Left/right now has dual meaning: the immediate action (attack/shield) AND when you'll see the card again. A shield card swiped left means "block now, see it again soon." Swiped right means "use the shield now but it's going away for a while."

**Core trade-off:** Immediate effect vs. future availability. Strong cards become decisions — burn them now or cycle them for later?

---

## P3: Enemy Cards (The Trap Deck)

The deck contains hidden "enemy turn" cards shuffled in. Hitting one triggers an immediate enemy action. Players develop meta-knowledge across loops ("I got hit around card 6 last time"). Optional variant: make enemy cards visible 1–2 cards ahead as a warning window.

**Core trade-off:** Aggressive play is punished when an enemy card appears with no block up. Players learn to hedge block for the zones where they expect the interrupt. Memory and prediction become skills.

---

## P4: The Chain System

Consecutive same-direction swipes build a chain. Three lefts = attack chain bonus. Three rights = shield chain bonus. Breaking the chain gives the enemy a buff or triggers a weak enemy attack.

**Core trade-off:** Do you swipe a shield card left (breaking your attack chain to get the block) or right (continuing the chain at the cost of a weaker block)? Deck composition — how many attack vs. shield cards you have — becomes the constraint you fight against.

---

## P5: Rage Timer ★ Starting Point

Enemy has a "rage" meter that fills with each card you play. When it fills, the enemy acts and hits harder than if you'd ended your turn voluntarily. You can end your turn at any point and fight the enemy at lower rage.

**Core trade-off:** How many cards do you play before conceding the turn? Push for one more attack when the rage bar is nearly full? If you're low on health, end turns short and fight light. If you're winning, grind long turns. Deck order still matters — you want to see block before a greedy long turn ends.

Design variants to explore:
- Rage resets each enemy turn vs. carries over (escalating pressure)
- Some cards reduce rage (tension-relief mechanic)
- Rage threshold scales as the encounter progresses

---

## Feedback

*(To be filled in after playtesting each prototype)*

| Prototype | Feels Like | What Worked | What Didn't |
|-----------|-----------|-------------|-------------|
| P1: Countdown | | | |
| P2: Requeue | | | |
| P3: Enemy Cards | | | |
| P4: Chain | | | |
| P5: Rage Timer | | | |
