public abstract class OnDodgeEffect : Effect {
    public sealed override void OnDodge(DamageContext ctx) {
        Apply(ctx);
    }

    protected abstract void Apply(DamageContext ctx);
}