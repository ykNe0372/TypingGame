using UnityEngine;

// 敵を倒した際に HP / MP を回復する
// （「撃破時にHP/MPを回復」「撃破時にMPを回復」はこのクラスで値を変えたアセットを作る）
[CreateAssetMenu(menuName = "Data/Effect/Kill/Recovery")]
public class KillRecoveryEffect : Effect, IKillHook {
    [SerializeField, Tooltip("最大HPの何% 回復するか")] private float _hpPercent = 5f;
    [SerializeField, Tooltip("最大MPの何% 回復するか")] private float _mpPercent = 5f;

    public void OnKill(Character owner, DamageContext ctx) {
        float multiplier = owner.Hooks.GetKillRecoveryMultiplier();  // 討伐時回復量アップの効果
        EffectUtility.RecoverPercent(owner, _hpPercent, _mpPercent, multiplier);
    }
}
