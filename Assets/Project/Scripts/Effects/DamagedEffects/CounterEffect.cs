using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Data/Effect/Damaged/Counter")]
public class CounterEffect : OnDamagedEffect {
    [SerializeField] private float _counterRate = 15f;
    [SerializeField] private float _powerMultiplier = 0.75f;

    protected override void Apply(DamageContext ctx) {
        if (Random.value > _counterRate/100f) return;

        float damage = ctx.FinalDamage * _powerMultiplier;
        var counter = AttackContextFactory.CreateCounterAttack(ctx.Target, ctx.Attacker, damage);
        AttackExecutor.Execute(counter);
    }
}