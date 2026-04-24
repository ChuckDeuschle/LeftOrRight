// Enraged Boss: uses the core loop (intent + countdown) AND a rage meter on top.
// Rage fills with each card played; when it fills the boss interrupts the scheduled
// intent and hits harder than the countdown would have allowed. The player can
// voluntarily end the turn early (End Turn button) to fight the boss at lower rage.
public class EnragedBossRules : CoreLoopRules
{
    private int cardsPlayedThisTurn = 0;
    private const int rageThreshold = 5;
    private const int bossCountdownLength = 8;

    public override void Initialize(GameManager gm)
    {
        // Give the boss a longer scheduled cycle than the rage threshold so rage
        // can actually fire as a mid-cycle interrupt. With the default countdown
        // of 4 and rage threshold 5, the scheduled attack always beats rage to
        // the punch and the rage meter is a dead mechanic.
        gm.currentEnemy.countdownLength = bossCountdownLength;
        gm.currentEnemy.intentCountdown = bossCountdownLength;
    }

    public override void OnCardPlayed(Card card, DropZone.ZoneSide side, GameManager gm)
    {
        base.OnCardPlayed(card, side, gm);
        cardsPlayedThisTurn++;
    }

    public override bool ShouldInterruptPlayerTurn(GameManager gm)
    {
        return base.ShouldInterruptPlayerTurn(gm) || cardsPlayedThisTurn >= rageThreshold;
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
        base.OnEnemyTurnEnd(gm);
        cardsPlayedThisTurn = 0;
    }

    public override string GetStatusText(GameManager gm)
    {
        return $"Rage: {cardsPlayedThisTurn} / {rageThreshold}";
    }

    public override bool ShowEndTurnButton(GameManager gm) => true;
}
