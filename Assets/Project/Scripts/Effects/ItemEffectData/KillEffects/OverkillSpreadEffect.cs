using UnityEngine;

// 敵を倒した際、余剰ダメージを別の敵に与える
[CreateAssetMenu(menuName = "Data/Effect/Kill/Overkill Spread")]
public class OverkillSpreadEffect : Effect, IKillHook {
    [SerializeField, Tooltip("余剰ダメージの何% を移すか")] private float _transferPercent = 100f;

    public void OnKill(Character owner, DamageContext ctx) {
        float overflow = (ctx.FinalDamage - ctx.ActualDamage) * _transferPercent / 100f;
        if (overflow < 1f) return;

        var candidates = owner.GetCombatTargets(owner);  // 生存している敵のみ
        candidates.Remove(ctx.Target);
        if (candidates.Count == 0) return;

        Character next = candidates[Random.Range(0, candidates.Count)];
        Debug.Log($"[Overkill] {overflow:F0} -> {next.name}");

        // 攻撃ではないので回避・会心なしの固定ダメージ（さらに倒せば連鎖する）
        next.TakeDamage(DamageContextFactory.CreateFixed(owner, next, overflow, true));
    }
}
