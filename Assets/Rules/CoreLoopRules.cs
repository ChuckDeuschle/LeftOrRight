// Core loop: the always-on foundation for every encounter.
// Tracks the enemy's intent countdown: each card played decrements it; when it
// reaches zero the enemy acts, and after the action the next intent is scheduled
// with escalation applied.
public class CoreLoopRules : PrototypeRules
{
    public override void OnCardPlayed(Card card, DropZone.ZoneSide side, GameManager gm)
    {
        gm.currentEnemy.intentCountdown--;
    }

    public override bool ShouldInterruptPlayerTurn(GameManager gm)
    {
        return gm.currentEnemy.intentCountdown <= 0;
    }

    public override void OnEnemyTurnEnd(GameManager gm)
    {
        gm.currentEnemy.ScheduleNextIntent(gm);
    }
}
