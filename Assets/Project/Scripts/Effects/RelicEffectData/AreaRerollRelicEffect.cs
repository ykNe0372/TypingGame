using UnityEngine;

// 次のエリア（行き先候補）を再抽選できる（回数制限あり）
// マップ選択中に R キーで発動（MapSelectInput）
[CreateAssetMenu(menuName = "Data/Relic/Area Reroll")]
public class AreaRerollRelicEffect : RelicEffect, IAreaRerollHook, IBattleEndHook {
    [SerializeField, Min(1), Tooltip("再抽選できる回数")] private int _maxRerolls = 3;
    [SerializeField, Tooltip("ステージ（ボス）突破時に回数を回復するか")] private bool _refillOnStageClear = false;

    private class State { public int Used; }

    public int GetRemainingRerolls(Character owner) {
        return Mathf.Max(0, _maxRerolls - owner.EffectStates.Get<State>(this).Used);
    }

    public bool TryConsumeReroll(Character owner) {
        if (GetRemainingRerolls(owner) <= 0) return false;

        ++owner.EffectStates.Get<State>(this).Used;
        return true;
    }

    public void OnBattleEnd(Character owner, BattleEndContext ctx) {
        if (_refillOnStageClear && ctx.IsVictory && ctx.BattleType == BattleType.Boss) owner.EffectStates.Get<State>(this).Used = 0;
    }
}
