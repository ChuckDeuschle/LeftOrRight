// Infiltrator: the enemy's intent is compound — each time it acts, it performs
// its scheduled attack AND shuffles a fresh trap card into the deck. Trap
// spawning lives on Enemy (enemy.addsTrap + Enemy.SpawnTrap) so it shows up
// inside the intent display. This rule set flips the flag on, counts the
// visible trap population, and applies the 5 damage when a trap is played.
public class InfiltratorRules : CoreLoopRules
{
    private const int trapDamage = 5;

    public override void Initialize(GameManager gm)
    {
        gm.currentEnemy.addsTrap = true;
    }

    public override void OnCardPlayed(Card card, DropZone.ZoneSide side, GameManager gm)
    {
        base.OnCardPlayed(card, side, gm);

        if (card.isTrap)
        {
            int absorbed = trapDamage < gm.player.shield ? trapDamage : gm.player.shield;
            gm.player.currentHealth -= trapDamage - absorbed;
            gm.player.shield -= absorbed;
        }
    }

    public override string GetStatusText(GameManager gm)
    {
        int count = CountTraps(gm);
        return count > 0 ? $"Traps in play: {count}" : "";
    }

    private int CountTraps(GameManager gm)
    {
        int count = 0;
        foreach (Card c in gm.deck) { if (c != null && c.isTrap) { count++; } }
        foreach (Card c in gm.discardPile) { if (c != null && c.isTrap) { count++; } }
        return count;
    }
}
