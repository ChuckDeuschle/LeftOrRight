public abstract class PrototypeRules
{
    // One-time setup hook; called from GameManager.Start() after currentEnemy,
    // player, and deck have all been resolved. Use to configure compound intents
    // on the enemy (e.g. Infiltrator sets enemy.addsTrap = true).
    public virtual void Initialize(GameManager gm) { }

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
            MasterGameManager.PrototypeMode.RequeueEnemy => new RequeueEnemyRules(),
            MasterGameManager.PrototypeMode.Infiltrator => new InfiltratorRules(),
            MasterGameManager.PrototypeMode.Mirror => new MirrorRules(),
            MasterGameManager.PrototypeMode.EnragedBoss => new EnragedBossRules(),
            _ => new CoreLoopRules()
        };
    }
}
