using System.Collections.Generic;
using UnityEngine;

// 被弾するたびに防御力が上昇する。回避に成功すると上昇分はリセットされる
[CreateAssetMenu(menuName = "Data/Effect/Damaged/Defense Stack")]
public class DamagedDefenseStackEffect : Effect, IAfterDamagedHook, IDodgeHook, IStatusModifierHook, IBattleStartHook {
    [SerializeField, Tooltip("1スタックあたりの補正（Multiplier は 1.05 で +5%/スタック）")]
    private List<BuffModifier> _modifiersPerStack = new() {
        EffectUtility.Modifier(StatusType.Defense, BuffType.Adding, 3f),
    };
    [SerializeField, Min(0), Tooltip("最大スタック数（0 で無制限）")] private int _maxStacks = 10;
    [SerializeField, Tooltip("スタックをいつリセットするか（回避成功時は常にリセット）")] private EffectResetTiming _resetTiming = EffectResetTiming.EachBattle;

    private class State { public int Stacks; }

    public void OnBattleStart(Character owner, BattleContext ctx) {
        if (_resetTiming == EffectResetTiming.EachBattle) owner.EffectStates.Get<State>(this).Stacks = 0;
    }

    public void OnAfterDamaged(Character owner, DamageContext ctx) {
        if (ctx.IsEnvironmentDamage || ctx.ActualDamage <= 0) return;

        var state = owner.EffectStates.Get<State>(this);
        if (_maxStacks > 0 && state.Stacks >= _maxStacks) return;
        ++state.Stacks;
    }

    public void OnDodge(Character owner, DamageContext ctx) {
        owner.EffectStates.Get<State>(this).Stacks = 0;
    }

    public void ModifyStatus(Character owner, StatusType type, ref float add, ref float multiplier) {
        if (!EffectUtility.ContainsStatus(_modifiersPerStack, type)) return;

        int stacks = owner.EffectStates.Get<State>(this).Stacks;
        EffectUtility.ApplyModifiers(_modifiersPerStack, type, stacks, ref add, ref multiplier);
    }
}
