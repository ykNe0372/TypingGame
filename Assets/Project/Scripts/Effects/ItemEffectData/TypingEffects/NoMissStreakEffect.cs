using System.Collections.Generic;
using UnityEngine;

// タイプミスをせずに連続で攻撃するたびに、攻撃力 / 会心率が上昇する（タイプミスでリセット）
[CreateAssetMenu(menuName = "Data/Effect/Typing/No Miss Streak")]
public class NoMissStreakEffect : Effect, IWordCompletedHook, ITypoHook, IStatusModifierHook, IBattleStartHook {
    [SerializeField, Tooltip("1スタックあたりの補正")]
    private List<BuffModifier> _modifiersPerStack = new() {
        EffectUtility.Modifier(StatusType.PhysicalAttack, BuffType.Multiplier, 1.02f),
        EffectUtility.Modifier(StatusType.CritRate, BuffType.Adding, 1f),
    };
    [SerializeField, Min(0), Tooltip("最大スタック数（0 で無制限）")] private int _maxStacks = 20;

    private class State { public int Stacks; }

    public void OnBattleStart(Character owner, BattleContext ctx) {
        owner.EffectStates.Get<State>(this).Stacks = 0;
    }

    public void OnWordCompleted(Character owner) {
        var state = owner.EffectStates.Get<State>(this);
        if (_maxStacks > 0 && state.Stacks >= _maxStacks) return;
        ++state.Stacks;
    }

    public void OnTypo(Character owner, TypoContext ctx) {
        if (ctx.IsNegated) return;
        owner.EffectStates.Get<State>(this).Stacks = 0;
    }

    public void ModifyStatus(Character owner, StatusType type, ref float add, ref float multiplier) {
        if (!EffectUtility.ContainsStatus(_modifiersPerStack, type)) return;

        int stacks = owner.EffectStates.Get<State>(this).Stacks;
        EffectUtility.ApplyModifiers(_modifiersPerStack, type, stacks, ref add, ref multiplier);
    }
}
