using UnityEngine;

// 敵がかかっている状態異常と同じ属性の攻撃の攻撃力を上げる（燃焼中の敵に炎属性など）
[CreateAssetMenu(menuName = "Data/Effect/DamageModifier/Element Match Bonus")]
public class ElementMatchBonusEffect : Effect, IOutgoingDamageHook {
    [SerializeField, Tooltip("ダメージ倍率")] private float _damageMultiplier = 1.3f;

    public void ModifyOutgoingDamage(Character owner, AttackContext attack, AttackInstance instance, DamageContext damage) {
        if (!EffectUtility.TryGetStatusOfElement(attack.Element, out var statusType)) return;
        if (damage.Target == null || damage.Target.GetStatus(statusType) == null) return;

        damage.FinalDamage *= _damageMultiplier;
    }
}
