using UnityEngine;

// ノーミスボーナスが発動した際、敵からの攻撃を1回無効化する盾を生成する
[CreateAssetMenu(menuName = "Data/Relic/Bonus Shield")]
public class BonusShieldRelicEffect : RelicEffect, IBonusAttackHook, IIncomingAttackCancelHook, IBattleStartHook {
    [SerializeField, Min(1), Tooltip("ボーナス1回で生成する盾の数")] private int _shieldsPerBonus = 1;
    [SerializeField, Min(1), Tooltip("盾の最大所持数")] private int _maxShields = 3;
    [SerializeField, Tooltip("戦闘開始時に盾をなくすか")] private bool _resetOnBattleStart = true;

    private class State { public int Shields; }

    public void OnBattleStart(Character owner, BattleContext ctx) {
        if (_resetOnBattleStart) owner.EffectStates.Get<State>(this).Shields = 0;
    }

    public void OnBonusAttack(Character owner) {
        var state = owner.EffectStates.Get<State>(this);
        state.Shields = Mathf.Min(state.Shields + _shieldsPerBonus, _maxShields);
        Debug.Log($"[BonusShield] 盾: {state.Shields}");
    }

    public bool ShouldCancelIncomingAttack(Character owner, AttackContext ctx) {
        if (ctx.Attacker == owner) return false;

        var state = owner.EffectStates.Get<State>(this);
        if (state.Shields <= 0) return false;

        --state.Shields;
        Debug.Log($"[BonusShield] 攻撃を無効化（残り {state.Shields}）");
        return true;
    }
}
