using UnityEngine;

// 1度だけ致命的なダメージを HP1 で持ちこたえ、一定時間無敵状態に入る
[CreateAssetMenu(menuName = "Data/Relic/Guts")]
public class GutsRelicEffect : RelicEffect, IIncomingDamageHook, IBattleStartHook, IBattleUpdateHook {
    [SerializeField, Min(0f), Tooltip("無敵時間")] private float _invincibleSeconds = 3f;
    [SerializeField, Tooltip("EachBattle: 戦闘ごとに1回 / Never: ラン中1回")] private EffectResetTiming _resetTiming = EffectResetTiming.EachBattle;

    private class State {
        public bool Used;
        public float InvincibleTimer;
    }

    public void OnBattleStart(Character owner, BattleContext ctx) {
        var state = owner.EffectStates.Get<State>(this);
        state.InvincibleTimer = 0f;
        if (_resetTiming == EffectResetTiming.EachBattle) state.Used = false;
    }

    public void OnBattleUpdate(Character owner, float deltaTime) {
        var state = owner.EffectStates.Get<State>(this);
        if (state.InvincibleTimer > 0f) state.InvincibleTimer -= deltaTime;
    }

    public void ModifyIncomingDamage(Character owner, DamageContext ctx) {
        var state = owner.EffectStates.Get<State>(this);
        if (state.InvincibleTimer > 0f) {
            ctx.FinalDamage = 0f;
            return;
        }
        if (state.Used || !EffectUtility.IsLethal(owner, ctx)) return;

        EffectUtility.ClampToSurvive(owner, ctx);
        state.Used = true;
        state.InvincibleTimer = _invincibleSeconds;
        Debug.Log("[Guts] 致命傷を耐えた");
    }
}
