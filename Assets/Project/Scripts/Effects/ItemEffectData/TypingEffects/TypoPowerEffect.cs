using System.Collections.Generic;
using UnityEngine;

// タイプミスをした際に HP が減る代わりに、攻撃力が上がる
// （HP が足りずに減らせなかった場合は上昇しない）
[CreateAssetMenu(menuName = "Data/Effect/Typing/Typo Power")]
public class TypoPowerEffect : Effect, ITypoHook, IStatusModifierHook, IBattleStartHook {
    [SerializeField, Tooltip("タイプミス1回で失う HP")] private float _hpCostPercent = 2f;
    [SerializeField, Tooltip("1スタックあたりの補正")]
    private List<BuffModifier> _modifiersPerStack = new() {
        EffectUtility.Modifier(StatusType.PhysicalAttack, BuffType.Multiplier, 1.03f),
    };
    [SerializeField, Min(0), Tooltip("最大スタック数（0 で無制限）")] private int _maxStacks = 10;
    [SerializeField, Tooltip("スタックをいつリセットするか")] private EffectResetTiming _resetTiming = EffectResetTiming.EachBattle;

    private class State { public int Stacks; }

    public void OnBattleStart(Character owner, BattleContext ctx) {
        if (_resetTiming == EffectResetTiming.EachBattle) owner.EffectStates.Get<State>(this).Stacks = 0;
    }

    public void OnTypo(Character owner, TypoContext ctx) {
        if (ctx.IsNegated) return;

        float before = owner.CurrentHP;
        owner.TakePercentDamage(_hpCostPercent);  // HP が足りない場合は減らない（死なない）
        if (owner.CurrentHP >= before) return;

        var state = owner.EffectStates.Get<State>(this);
        if (_maxStacks > 0 && state.Stacks >= _maxStacks) return;
        ++state.Stacks;
    }

    public void ModifyStatus(Character owner, StatusType type, ref float add, ref float multiplier) {
        if (!EffectUtility.ContainsStatus(_modifiersPerStack, type)) return;

        int stacks = owner.EffectStates.Get<State>(this).Stacks;
        EffectUtility.ApplyModifiers(_modifiersPerStack, type, stacks, ref add, ref multiplier);
    }
}
