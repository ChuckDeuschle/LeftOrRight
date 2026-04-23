public abstract class PrototypeRules
{
    public virtual void OnCardPlayed(Card card, DropZone.ZoneSide side, GameManager gm) { }
    public virtual bool ShouldInterruptPlayerTurn(GameManager gm) => false;
    public virtual int ModifyEnemyDamage(int baseDamage, GameManager gm) => baseDamage;
    public virtual void OnEnemyTurnEnd(GameManager gm) { }
    public virtual string GetStatusText(GameManager gm) => string.Empty;
    public virtual bool ShowEndTurnButton(GameManager gm) => false;

    public static PrototypeRules CreateForMode(MasterGameManager.PrototypeMode mode)
    {
        return mode switch
        {
            MasterGameManager.PrototypeMode.EnragedBoss => new EnragedBossRules(),
            _ => new CoreLoopRules()
        };
    }
}
