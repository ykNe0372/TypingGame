using System.Collections.Generic;
using UnityEngine;

// 戦闘中に回避ができなくなるが、物理攻撃力と魔法攻撃力が大幅に上昇する
[CreateAssetMenu(menuName = "Data/Relic/No Evasion Power")]
public class NoEvasionPowerRelicEffect : RelicEffect, IEvasionBlockHook, IStatusModifierHook {
    [SerializeField, Tooltip("上昇量")]
    private List<BuffModifier> _modifiers = new() {
        EffectUtility.Modifier(StatusType.PhysicalAttack, BuffType.Multiplier, 1.5f),
        EffectUtility.Modifier(StatusType.MagicAttack, BuffType.Multiplier, 1.5f),
    };

    public bool CanEvade(Character owner) {
        return false;
    }

    public void ModifyStatus(Character owner, StatusType type, ref float add, ref float multiplier) {
        EffectUtility.ApplyModifiers(_modifiers, type, 1f, ref add, ref multiplier);
    }
}
