using UnityEngine;

// 被弾時に MP を消費し、被ダメージを抑える（MP が足りない時は発動しない）
[CreateAssetMenu(menuName = "Data/Relic/Mana Guard")]
public class ManaGuardRelicEffect : RelicEffect, IIncomingDamageHook {
    [SerializeField, Tooltip("消費する MP")] private float _mpCostPercent = 5f;
    [SerializeField, Range(0f, 100f), Tooltip("ダメージ軽減率")] private float _damageReductionPercent = 40f;

    public void ModifyIncomingDamage(Character owner, DamageContext ctx) {
        if (ctx.IsEnvironmentDamage || ctx.FinalDamage <= 0f) return;
        if (ctx.Attacker == null || ctx.Attacker == owner) return;

        int cost = Mathf.Max(1, Mathf.CeilToInt(owner.MaxMP * _mpCostPercent / 100f));
        if (!owner.TryConsumeMP(cost)) return;

        ctx.FinalDamage *= 1f - _damageReductionPercent / 100f;
    }
}
