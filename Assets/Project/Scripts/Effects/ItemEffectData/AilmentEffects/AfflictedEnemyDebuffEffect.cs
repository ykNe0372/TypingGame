using System.Collections.Generic;
using UnityEngine;

// 自分が付与した状態異常にかかっている敵のステータスを下げる
// ・「状態異常の敵の攻撃力と攻撃速度を下げる」: PhysicalAttack / Speed を Multiplier 0.85 など
// ・「状態異常の敵の防御力を下げる」        : Defense を Multiplier 0.8 など
// 複数の状態異常にかかっていても補正は1回だけ
[CreateAssetMenu(menuName = "Data/Effect/Ailment/Afflicted Enemy Debuff")]
public class AfflictedEnemyDebuffEffect : Effect, IAfflictedTargetStatusHook {
    [SerializeField, Tooltip("Adding: 加算 / Multiplier: 乗算（0.85 = 15% 低下）")]
    private List<BuffModifier> _modifiers = new() {
        EffectUtility.Modifier(StatusType.PhysicalAttack, BuffType.Multiplier, 0.85f),
        EffectUtility.Modifier(StatusType.Speed, BuffType.Multiplier, 0.85f),
    };

    public float ModifyAfflictedTargetStatus(Character owner, Character target, StatusEffectInstance status, StatusType type, float value) {
        if (!EffectUtility.ContainsStatus(_modifiers, type)) return value;
        if (EffectUtility.GetFirstStatusFrom(target, owner) != status) return value;  // 重複適用を防ぐ

        float add = 0f;
        float multiplier = 1f;
        EffectUtility.ApplyModifiers(_modifiers, type, 1f, ref add, ref multiplier);
        return (value + add) * multiplier;
    }
}
