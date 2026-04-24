[← Index](INDEX.md)

# Core Combat Loop

Every encounter in LeftOrRight uses the same base loop: the enemy telegraphs their next action, a countdown ticks down with each card you play, and you swipe cards from your deck without knowing what's coming next. All enemy and encounter variants layer on top of this foundation.

## The Mechanic

### Enemy Intent
The enemy displays their next action and its value — for example, "Attack 7" or "Shield 5." Players can see exactly what's coming.

### The Countdown
Next to the intent, a card-count timer shows how many more cards you can play before the enemy acts. "Attack 7 in 4 cards" means you have 4 swipes before the hit lands.

### Deck Uncertainty
You do not see upcoming cards in your deck. Every swipe reveals a new card and forces an immediate left/right decision. You might hit a string of attack cards with no block in sight — or vice versa.

### Damage Escalation
Enemy damage scales up over the course of the encounter. Pure defense is not a viable strategy: if you only block and never attack, the enemy's damage will eventually exceed any block you can stack.

## Why This Creates Tension

The push-your-luck feel emerges from two forces pulling against each other:

- **Deck uncertainty** — you never know if the next card is the block you need
- **Escalating damage** — waiting to attack makes the enemy stronger

Every swipe becomes a bet: attack now and hope the next card is a block, or block now and hope enough attack cards come in time. You can't calculate the optimal play because you don't have complete information.

## Core Trade-offs

- Attack vs. block on any given card (the immediate swipe decision)
- Push for one more card to try for a block, or take the hit with what you have
- Commit damage early (cheaper enemies) vs. save for late (fewer turns, harder hits)
