using UnityEngine;

// 回避した際に HP / MP を回復する
[CreateAssetMenu(menuName = "Data/Effect/Dodge/Recovery")]
public class DodgeRecoveryEffect : OnDodgeEffect {
    [SerializeField, Tooltip("最大HPの何% 回復するか")] private float _hpPercent = 3f;
    [SerializeField, Tooltip("最大MPの何% 回復するか")] private float _mpPercent = 3f;

    protected override void Apply(DamageContext ctx) {
        EffectUtility.RecoverPercent(ctx.Target, _hpPercent, _mpPercent);  // 回避した側 = Target
    }
}
