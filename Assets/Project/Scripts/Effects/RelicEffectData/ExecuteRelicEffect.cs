using UnityEngine;

// 敵の HP を一定割合以下にした際に即死させる
[CreateAssetMenu(menuName = "Data/Relic/Execute")]
public class ExecuteRelicEffect : RelicEffect, IDealtDamageHook {
    [SerializeField, Range(0f, 100f), Tooltip("即死させる HP 割合")] private float _thresholdPercent = 5f;

    public void OnDealtDamage(Character owner, DamageContext ctx) {
        Character target = ctx.Target;
        if (!EffectUtility.IsAlive(target) || target == owner) return;
        if (target.GetCurrentHPRatio() * 100f > _thresholdPercent) return;

        Debug.Log($"[Execute] {target.name}");
        target.TakeDamage(DamageContextFactory.CreateFixed(owner, target, Mathf.Ceil(target.CurrentHP), true));
    }
}
