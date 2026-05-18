
public abstract class OnDamageEffect : Effect {
    public sealed override void OnDamage(DamageContext ctx) {
        Apply(ctx);
    }

    protected abstract void Apply(DamageContext ctx);
}