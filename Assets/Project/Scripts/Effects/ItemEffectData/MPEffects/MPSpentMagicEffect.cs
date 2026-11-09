using UnityEngine;

// MPの消費量に応じて魔法攻撃力が上昇する
[CreateAssetMenu(menuName = "Data/Effect/MP/Spent Magic Up")]
public class MPSpentMagicEffect : Effect, IMPConsumedHook, IStatusModifierHook, IBattleStartHook {
    [SerializeField, Tooltip("MP1 消費あたりの魔法攻撃力上昇量")] private float _magicAttackPerMP = 0.5f;
    [SerializeField, Tooltip("上昇量の上限（0 で無制限）")] private float _maxBonus = 50f;
    [SerializeField, Tooltip("累積した消費量をいつリセットするか")] private EffectResetTiming _resetTiming = EffectResetTiming.EachBattle;

    private class State { public int SpentMP; }

    public void OnBattleStart(Character owner, BattleContext ctx) {
        if (_resetTiming == EffectResetTiming.EachBattle) owner.EffectStates.Get<State>(this).SpentMP = 0;
    }

    public void OnMPConsumed(Character owner, int amount) {
        owner.EffectStates.Get<State>(this).SpentMP += amount;
    }

    public void ModifyStatus(Character owner, StatusType type, ref float add, ref float multiplier) {
        if (type != StatusType.MagicAttack) return;

        float bonus = owner.EffectStates.Get<State>(this).SpentMP * _magicAttackPerMP;
        if (_maxBonus > 0f) bonus = Mathf.Min(bonus, _maxBonus);
        add += bonus;
    }
}
