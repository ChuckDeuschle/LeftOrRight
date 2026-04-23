// Mirror: consecutive same-direction swipes build a chain. Reaching the
// threshold grants a one-time bonus (+5 shield). Breaking a chain that had
// reached the threshold empowers the enemy's next intent (+3 damage).
// Rewards rhythm; punishes forced variety.
public class MirrorRules : CoreLoopRules
{
    private bool hasSwiped = false;
    private DropZone.ZoneSide lastSide;
    private int chainLength = 0;

    private const int chainThreshold = 3;
    private const int chainBonusShield = 5;
    private const int breakPenalty = 3;

    public override void OnCardPlayed(Card card, DropZone.ZoneSide side, GameManager gm)
    {
        base.OnCardPlayed(card, side, gm);

        if (hasSwiped && side == lastSide)
        {
            chainLength++;
            if (chainLength == chainThreshold)
            {
                gm.player.shield += chainBonusShield;
            }
        }
        else
        {
            if (chainLength >= chainThreshold)
            {
                gm.currentEnemy.intentValue += breakPenalty;
            }
            chainLength = 1;
            lastSide = side;
            hasSwiped = true;
        }
    }

    public override string GetStatusText(GameManager gm)
    {
        if (!hasSwiped) { return "Chain: —"; }
        string arrow = lastSide == DropZone.ZoneSide.Left ? "←" : "→";
        return $"Chain {arrow} {chainLength}";
    }
}
