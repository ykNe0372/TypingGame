using UnityEngine;

// 戦闘中 HP が持続的に回復する
[CreateAssetMenu(menuName = "Data/Relic/Battle Regen")]
public class BattleRegenRelicEffect : RelicEffect, IBattleStartHook, IBattleUpdateHook {
    [SerializeField, Tooltip("1秒あたりの回復量（最大HPの%）")] private float _percentPerSecond = 1f;

    private class State { public float Pending; }  // 端数を持ち越す

    public void OnBattleStart(Character owner, BattleContext ctx) {
        owner.EffectStates.Get<State>(this).Pending = 0f;
    }

    public void OnBattleUpdate(Character owner, float deltaTime) {
        if (!EffectUtility.IsAlive(owner)) return;

        var state = owner.EffectStates.Get<State>(this);
        state.Pending += owner.MaxHP * _percentPerSecond / 100f * deltaTime;

        int amount = Mathf.FloorToInt(state.Pending);
        if (amount <= 0) return;

        state.Pending -= amount;
        owner.RecoverHP(amount);
    }
}
