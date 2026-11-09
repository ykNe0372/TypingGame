using UnityEngine;

// HP が 0 になっても一定時間生き延びる。その間に敵を倒しきれば HP を回復して復活する
// 生存中は HP1 で踏みとどまり、追加のダメージは受けない。時間切れで戦闘不能になる
[CreateAssetMenu(menuName = "Data/Relic/Last Stand")]
public class LastStandRelicEffect : RelicEffect, IIncomingDamageHook, IBattleStartHook, IBattleUpdateHook, IBattleEndHook {
    [SerializeField, Min(0f), Tooltip("生き延びる時間")] private float _surviveSeconds = 10f;
    [SerializeField, Range(0f, 100f), Tooltip("復活時の HP")] private float _reviveHPPercent = 30f;

    private class State {
        public bool Used;
        public bool Active;
        public bool Expiring;  // 時間切れで戦闘不能にする処理中
        public float Timer;
    }

    public void OnBattleStart(Character owner, BattleContext ctx) {
        var state = owner.EffectStates.Get<State>(this);
        state.Used = false;
        state.Active = false;
        state.Expiring = false;
        state.Timer = 0f;
    }

    public void ModifyIncomingDamage(Character owner, DamageContext ctx) {
        var state = owner.EffectStates.Get<State>(this);
        if (state.Expiring) return;
        if (state.Active) {
            ctx.FinalDamage = 0f;
            return;
        }
        if (state.Used || !EffectUtility.IsLethal(owner, ctx)) return;

        EffectUtility.ClampToSurvive(owner, ctx);
        state.Used = true;
        state.Active = true;
        state.Timer = _surviveSeconds;
        Debug.Log($"[LastStand] {_surviveSeconds} 秒以内に敵を倒しきれ！");
    }

    public void OnBattleUpdate(Character owner, float deltaTime) {
        var state = owner.EffectStates.Get<State>(this);
        if (!state.Active) return;

        state.Timer -= deltaTime;
        if (state.Timer > 0f) return;

        state.Active = false;
        state.Expiring = true;
        Debug.Log("[LastStand] 時間切れ");
        owner.TakeDamage(DamageContextFactory.CreateFixed(null, owner, Mathf.Ceil(owner.CurrentHP), true));
        state.Expiring = false;
    }

    public void OnBattleEnd(Character owner, BattleEndContext ctx) {
        var state = owner.EffectStates.Get<State>(this);
        if (!state.Active) return;

        state.Active = false;
        if (!ctx.IsVictory) return;

        float targetHP = owner.MaxHP * _reviveHPPercent / 100f;
        int heal = Mathf.CeilToInt(targetHP - owner.CurrentHP);
        owner.RecoverHP(heal);
        Debug.Log("[LastStand] 復活");
    }
}
