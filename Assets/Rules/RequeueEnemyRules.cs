using System.Collections.Generic;

// Requeue Enemy: when the enemy acts, the discard pile is slammed back into
// the deck, reordered by how each card was played — cards swiped left go to
// the TOP (you'll see them again soon), cards swiped right go to the BOTTOM
// (pushed away). The enemy disrupts the rhythm the player was building.
public class RequeueEnemyRules : CoreLoopRules
{
    public override void OnEnemyTurnEnd(GameManager gm)
    {
        base.OnEnemyTurnEnd(gm);

        if (gm.discardPile.Count == 0) { return; }

        List<Card> lefts = new List<Card>();
        List<Card> rights = new List<Card>();
        for (int i = 0; i < gm.discardPile.Count; i++)
        {
            if (i < gm.discardDirections.Count && gm.discardDirections[i] == DropZone.ZoneSide.Right)
            {
                rights.Add(gm.discardPile[i]);
            }
            else
            {
                lefts.Add(gm.discardPile[i]);
            }
        }

        List<Card> newDeck = new List<Card>();
        newDeck.AddRange(lefts);
        newDeck.AddRange(gm.deck);
        newDeck.AddRange(rights);
        gm.deck = newDeck;

        gm.discardPile.Clear();
        gm.discardDirections.Clear();
    }

    public override string GetStatusText(GameManager gm)
    {
        return "Requeue: left ↑  right ↓";
    }
}
