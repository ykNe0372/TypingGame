public abstract class OnDamagedEffect : Effect {
    public sealed override void OnDamaged(DamageContext ctx) {
        Apply(ctx);
    }

    protected abstract void Apply(DamageContext ctx);
}