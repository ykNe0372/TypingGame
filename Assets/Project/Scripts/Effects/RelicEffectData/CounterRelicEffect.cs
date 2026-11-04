using UnityEngine;

// 被弾時に中確率でカウンターを発動する（既存の CounterEffect と同じく受けたダメージ基準）
[CreateAssetMenu(menuName = "Data/Relic/Counter")]
public class CounterRelicEffect : RelicEffect, IAfterDamagedHook {
    [SerializeField, Tooltip("発動確率")] private float _chance = 50f;
    [SerializeField, Tooltip("受けたダメージに対する倍率")] private float _powerMultiplier = 0.75f;

    public void OnAfterDamaged(Character owner, DamageContext ctx) {
        if (ctx.IsEnvironmentDamage || ctx.ActualDamage <= 0) return;
        if (!EffectUtility.IsAlive(owner) || !EffectUtility.IsAlive(ctx.Attacker) || ctx.Attacker == owner) return;
        if (!EffectUtility.Roll(_chance)) return;

        var counter = AttackContextFactory.CreateCounterAttack(owner, ctx.Attacker, ctx.FinalDamage * _powerMultiplier);
        counter.TriggerOnAttackEffects = false;
        AttackExecutor.Execute(counter);
    }
}
