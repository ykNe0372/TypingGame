public abstract class OnAttackEffect : Effect {
    public sealed override void OnAttack(AttackContext ctx) {
        Apply(ctx);
    }

    protected abstract void Apply(AttackContext ctx);
}