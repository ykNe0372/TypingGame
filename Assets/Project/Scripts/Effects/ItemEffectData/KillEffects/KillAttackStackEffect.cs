using System.Collections.Generic;
using UnityEngine;

// 敵を倒した際に攻撃力が n% 上昇する（既定では永続・累積）
[CreateAssetMenu(menuName = "Data/Effect/Kill/Attack Stack")]
public class KillAttackStackEffect : Effect, IKillHook, IStatusModifierHook, IBattleStartHook {
    [SerializeField, Tooltip("上昇させるステータス")] private List<StatusType> _statusTypes = new() { StatusType.PhysicalAttack };
    [SerializeField, Tooltip("1体倒すごとの上昇率")] private float _percentPerKill = 3f;
    [SerializeField, Min(0), Tooltip("最大スタック数（0 で無制限）")] private int _maxStacks = 0;
    [SerializeField, Tooltip("スタックをいつリセットするか")] private EffectResetTiming _resetTiming = EffectResetTiming.Never;

    private class State { public int Stacks; }

    public void OnBattleStart(Character owner, BattleContext ctx) {
        if (_resetTiming == EffectResetTiming.EachBattle) owner.EffectStates.Get<State>(this).Stacks = 0;
    }

    public void OnKill(Character owner, DamageContext ctx) {
        var state = owner.EffectStates.Get<State>(this);
        if (_maxStacks > 0 && state.Stacks >= _maxStacks) return;

        ++state.Stacks;
        Debug.Log($"[KillAttackStack] Stacks: {state.Stacks}");
    }

    public void ModifyStatus(Character owner, StatusType type, ref float add, ref float multiplier) {
        if (!_statusTypes.Contains(type)) return;

        int stacks = owner.EffectStates.Get<State>(this).Stacks;
        multiplier *= 1f + _percentPerKill / 100f * stacks;
    }
}
