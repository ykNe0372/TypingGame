using UnityEngine;

// タイプミスを一定回数無効化する（連続正解数がリセットされない）
[CreateAssetMenu(menuName = "Data/Relic/Typo Guard")]
public class TypoGuardRelicEffect : RelicEffect, ITypoHook, IBattleStartHook {
    [SerializeField, Min(1), Tooltip("無効化できる回数")] private int _guardCount = 3;
    [SerializeField, Tooltip("EachBattle: 戦闘ごとに回数を回復 / Never: ラン中の合計")] private EffectResetTiming _resetTiming = EffectResetTiming.EachBattle;

    private class State {
        public bool Initialized;
        public int Remaining;
    }

    private State GetState(Character owner) {
        var state = owner.EffectStates.Get<State>(this);
        if (!state.Initialized) {
            state.Initialized = true;
            state.Remaining = _guardCount;
        }
        return state;
    }

    public void OnBattleStart(Character owner, BattleContext ctx) {
        var state = GetState(owner);
        if (_resetTiming == EffectResetTiming.EachBattle) state.Remaining = _guardCount;
    }

    public void OnTypo(Character owner, TypoContext ctx) {
        if (ctx.IsNegated) return;

        var state = GetState(owner);
        if (state.Remaining <= 0) return;

        --state.Remaining;
        ctx.IsNegated = true;
        Debug.Log($"[TypoGuard] 残り {state.Remaining} 回");
    }
}
