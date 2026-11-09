using UnityEngine;

public enum TargetHPScalingMode {
    HigherHP,  // 敵の HP が高いほどダメージ上昇
    LowerHP    // 敵の HP が低いほどダメージ上昇
}

// 敵の HP が高い / 低いほど与えるダメージが上昇する
[CreateAssetMenu(menuName = "Data/Effect/DamageModifier/Target HP Scaling")]
public class TargetHPScalingEffect : Effect, IOutgoingDamageHook {
    [SerializeField] private TargetHPScalingMode _mode = TargetHPScalingMode.HigherHP;
    [SerializeField, Tooltip("最大で何% ダメージが上がるか")] private float _maxBonusPercent = 30f;

    public void ModifyOutgoingDamage(Character owner, AttackContext attack, AttackInstance instance, DamageContext damage) {
        if (!EffectUtility.IsAlive(damage.Target)) return;

        float hpRatio = Mathf.Clamp01(damage.Target.GetCurrentHPRatio());
        float t = _mode == TargetHPScalingMode.HigherHP ? hpRatio : 1f - hpRatio;
        damage.FinalDamage *= 1f + _maxBonusPercent / 100f * t;
    }
}
