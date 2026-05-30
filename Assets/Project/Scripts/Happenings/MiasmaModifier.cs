using UnityEngine;

[CreateAssetMenu(menuName = "BattleModifier/Curse")]
public class MiasmaModifier : BattleModifierBase {
    [SerializeField] private float _interval = 20f;
    [SerializeField] private float _damage = 20f;

    private float _timer;

    public override void OnBattleStart(BattleContext ctx) {
        _timer = 0f;
    }

    public override void OnUpdate(BattleContext ctx, float deltaTime) {
        _timer += deltaTime;
        if (_timer < _interval) return;

        _timer = 0f;
        ApplyCurseDamage(ctx.Player, _damage);
        foreach (var enemy in ctx.Enemies) {
            if (enemy == null || enemy.IsDead) continue;
            ApplyCurseDamage(enemy, _damage);
        }

        Debug.Log("[MiasmaModifier] 瘴気");
    }

    private void ApplyCurseDamage(Character target, float damage) {
        DamageContext ctx = DamageContextFactory.CreateFixed(null, target, damage, true);
        target.TakeDamage(ctx);
    }
}