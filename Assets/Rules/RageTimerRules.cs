// P5 Rage Timer: each card played fills a rage meter. When the meter fills the
// enemy attacks immediately (mid-turn interrupt) with bonus damage. The player
// can voluntarily end their turn early via the End Turn button to avoid the bonus.
public class RageTimerRules : PrototypeRules
{
    private int cardsPlayedThisTurn = 0;
    private const int rageThreshold = 5;

    public override void OnCardPlayed(Card card, DropZone.ZoneSide side, GameManager gm)
    {
        cardsPlayedThisTurn++;
    }

    public override bool ShouldInterruptPlayerTurn(GameManager gm)
    {
        return cardsPlayedThisTurn >= rageThreshold;
    }

    public override int ModifyEnemyDamage(int baseDamage, GameManager gm)
    {
        if (cardsPlayedThisTurn >= rageThreshold)
        {
            return baseDamage + (cardsPlayedThisTurn - rageThreshold + 1) * 5;
        }
        return baseDamage;
    }

    public override void OnEnemyTurnEnd(GameManager gm)
    {
        cardsPlayedThisTurn = 0;
    }

    public override string GetStatusText(GameManager gm)
    {
        return $"Rage: {cardsPlayedThisTurn} / {rageThreshold}";
    }

    public override bool ShowEndTurnButton(GameManager gm) => true;
}
