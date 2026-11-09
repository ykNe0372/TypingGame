using System.Collections.Generic;
using UnityEngine;

public enum ResourceKind { HP, MP }

public enum ThresholdCondition {
    AtOrAbove,  // 割合以上
    AtOrBelow   // 割合以下
}

// HP / MP が一定割合以上（以下）の場合にステータスが上昇する
// ・「MPが一定割合以上 / 以下の場合、魔法攻撃力が上昇」: MP / MagicAttack
// ・「HPが一定割合以下の場合、回避率が上昇」          : HP / AtOrBelow / EvasionRate
[CreateAssetMenu(menuName = "Data/Effect/Status/Resource Threshold")]
public class ResourceThresholdStatusEffect : Effect, IStatusModifierHook {
    [SerializeField] private ResourceKind _resource = ResourceKind.MP;
    [SerializeField] private ThresholdCondition _condition = ThresholdCondition.AtOrAbove;
    [SerializeField, Range(0f, 100f), Tooltip("閾値")] private float _thresholdPercent = 70f;
    [SerializeField, Tooltip("条件を満たしている間の補正")]
    private List<BuffModifier> _modifiers = new() {
        EffectUtility.Modifier(StatusType.MagicAttack, BuffType.Multiplier, 1.25f),
    };

    public void ModifyStatus(Character owner, StatusType type, ref float add, ref float multiplier) {
        if (!EffectUtility.ContainsStatus(_modifiers, type)) return;

        float ratio = (_resource == ResourceKind.HP ? owner.GetCurrentHPRatio() : owner.GetCurrentMPRatio()) * 100f;
        bool satisfied = _condition == ThresholdCondition.AtOrAbove ? ratio >= _thresholdPercent : ratio <= _thresholdPercent;
        if (!satisfied) return;

        EffectUtility.ApplyModifiers(_modifiers, type, 1f, ref add, ref multiplier);
    }
}
