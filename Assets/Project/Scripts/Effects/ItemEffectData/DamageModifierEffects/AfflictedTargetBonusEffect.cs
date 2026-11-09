using UnityEngine;

// 状態異常の敵に対する攻撃力 / クリティカル発動率を上げる
[CreateAssetMenu(menuName = "Data/Effect/DamageModifier/Afflicted Target Bonus")]
public class AfflictedTargetBonusEffect : Effect, IOutgoingDamageHook {
    [SerializeField, Tooltip("ダメージ倍率")] private float _damageMultiplier = 1.2f;
    [SerializeField, Tooltip("会心率の加算")] private float _critRateBonus = 10f;
    [SerializeField, Tooltip("true: 自分が付与した状態異常のみ対象")] private bool _onlyOwnStatus = false;

    public void ModifyOutgoingDamage(Character owner, AttackContext attack, AttackInstance instance, DamageContext damage) {
        if (!EffectUtility.HasAnyStatus(damage.Target, _onlyOwnStatus ? owner : null)) return;

        damage.FinalDamage *= _damageMultiplier;
        damage.CritRateBonus += _critRateBonus;
    }
}
