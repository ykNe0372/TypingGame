using UnityEngine;

[CreateAssetMenu(menuName = "Data/Effect/Damaged/Counter")]
public class CounterEffect : OnDamagedEffect {
    [SerializeField] private float _counterRate = 15f;
    [SerializeField] private float _powerMultiplier = 0.75f;

    protected override void Apply(DamageContext ctx) {
        if (Random.value > _counterRate/100f) return;

        float damage = ctx.FinalDamage * _powerMultiplier;
        var counter = AttackContextFactory.CreateCounterAttack(ctx.Target, ctx.Attacker, damage);
        counter.TriggerOnAttackEffects = false;  // 反撃で自傷・連撃などの攻撃時効果を発動させない
        AttackExecutor.Execute(counter);
    }
}