using UnityEngine;

// 回避した際に敵へ物理攻撃力依存の反撃を行う
[CreateAssetMenu(menuName = "Data/Effect/Dodge/Counter")]
public class DodgeCounterEffect : OnDodgeEffect {
    [SerializeField, Tooltip("発動確率")] private float _chance = 100f;
    [SerializeField, Tooltip("物理攻撃によるダメージに対する倍率")] private float _powerMultiplier = 0.8f;

    protected override void Apply(DamageContext ctx) {
        Character dodger = ctx.Target;
        Character attacker = ctx.Attacker;
        if (!EffectUtility.IsAlive(dodger) || !EffectUtility.IsAlive(attacker)) return;
        if (!EffectUtility.Roll(_chance)) return;

        float damage = DamageCalculator.Calculate(dodger, attacker, StatusType.PhysicalAttack) * _powerMultiplier;
        var counter = AttackContextFactory.CreateCounterAttack(dodger, attacker, damage);
        counter.TriggerOnAttackEffects = false;
        AttackExecutor.Execute(counter);
    }
}
