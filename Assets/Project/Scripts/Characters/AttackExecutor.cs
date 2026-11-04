using UnityEngine;

public static class AttackExecutor {
    public static void Execute(AttackContext ctx) {
        // 対象側の効果で攻撃が無効化される場合は対象から外す（攻撃失敗・シールド・行動停止など）
        ctx.Targets.RemoveAll(target => target == null || target.Hooks.ShouldCancelIncomingAttack(ctx));
        if (ctx.Targets.Count == 0) return;

        ctx.Attacker.Hooks.BeforeAttack(ctx);
        if (ctx.TriggerOnAttackEffects) {
            foreach (var effect in ctx.Attacker.AttackEffects) effect.OnAttack(ctx);
        }
        foreach (var attack in ctx.AttackInstances) ExecuteAttack(ctx, attack);
    }

    private static void ExecuteAttack(AttackContext ctx, AttackInstance attack) {
        foreach (var target in ctx.Targets) {
            DamageContext dmgCtx;
            if (attack.IsUseFixedDamage) dmgCtx = DamageContextFactory.CreateFixed(ctx.Attacker, target, attack.FixedDamage, true);
            else {
                StatusType attackStatus = ctx.IsMagical ? StatusType.MagicAttack : StatusType.PhysicalAttack;  // MP 消費攻撃は魔法攻撃力を参照
                dmgCtx = DamageContextFactory.CreateAttack(ctx.Attacker, target, attackStatus);
                dmgCtx.FinalDamage = dmgCtx.BaseDamage * attack.PowerMultiplier;
            }
            ctx.Attacker.Hooks.ModifyOutgoingDamage(ctx, attack, dmgCtx);  // 与ダメージ補正（会心判定の前）
            if (attack.CanCrit) CriticalCalculator.Apply(dmgCtx, attack);

            if (!attack.IsUseFixedDamage && EvasionCalculator.Roll(target)) {  // 固定ダメージは回避不可
                target.TriggerDodge(dmgCtx);
                continue;
            }
            target.TakeDamage(dmgCtx);   // 被弾処理

            foreach (var effect in ctx.Attacker.DamageEffects) effect.OnDamage(dmgCtx);  // 与ダメ時に発動する効果の処理
            if (ctx.StatusEffect != null && target.TryApplyStatus(ctx.Attacker, ctx.StatusEffect)) {  // 状態異常付与
                ctx.Attacker.Hooks.StatusInflicted(target, ctx.StatusEffect);
            }
        }
    }
}