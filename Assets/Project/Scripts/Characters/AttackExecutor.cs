using UnityEngine;

public static class AttackExecutor {
    public static void Execute(AttackContext ctx) {
        foreach (var effect in ctx.Attacker.AttackEffects) effect.OnAttack(ctx);
        foreach (var attack in ctx.AttackInstances) ExecuteAttack(ctx, attack);
    }

    private static void ExecuteAttack(AttackContext ctx, AttackInstance attack) {
        foreach (var target in ctx.Targets) {
            DamageContext dmgCtx;
            if (attack.IsUseFixedDamage) dmgCtx = DamageContextFactory.CreateFixed(ctx.Attacker, target, attack.FixedDamage, true);
            else {
                dmgCtx = DamageContextFactory.CreateAttack(ctx.Attacker, target);
                dmgCtx.FinalDamage = dmgCtx.BaseDamage * attack.PowerMultiplier;
            }

            if (attack.CanCrit) CriticalCalculator.Apply(dmgCtx);
            
            target.TakeDamage(dmgCtx);   // 被弾処理
            if (ctx.StatusEffect != null) {
                target.TryApplyStatus(ctx.Attacker, ctx.StatusEffect);  // 状態異常付与
            }
        }
    }
}