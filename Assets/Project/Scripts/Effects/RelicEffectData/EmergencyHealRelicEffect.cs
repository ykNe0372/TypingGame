using UnityEngine;

// HP が一定値まで減った際に1度だけ全回復する
[CreateAssetMenu(menuName = "Data/Relic/Emergency Heal")]
public class EmergencyHealRelicEffect : RelicEffect, IAfterDamagedHook, IBattleStartHook {
    [SerializeField, Range(0f, 100f), Tooltip("発動する HP 残量の割合")] private float _thresholdPercent = 30f;
    [SerializeField, Range(0f, 100f), Tooltip("回復量")] private float _healPercent = 100f;
    [SerializeField, Tooltip("EachBattle: 戦闘ごとに1回 / Never: ラン中1回")] private EffectResetTiming _resetTiming = EffectResetTiming.EachBattle;

    private class State { public bool Used; }

    public void OnBattleStart(Character owner, BattleContext ctx) {
        if (_resetTiming == EffectResetTiming.EachBattle) owner.EffectStates.Get<State>(this).Used = false;
    }

    public void OnAfterDamaged(Character owner, DamageContext ctx) {
        if (!EffectUtility.IsAlive(owner)) return;

        var state = owner.EffectStates.Get<State>(this);
        if (state.Used || owner.GetCurrentHPRatio() * 100f > _thresholdPercent) return;

        state.Used = true;
        EffectUtility.RecoverPercent(owner, _healPercent, 0f);
        Debug.Log("[EmergencyHeal] 回復");
    }
}
