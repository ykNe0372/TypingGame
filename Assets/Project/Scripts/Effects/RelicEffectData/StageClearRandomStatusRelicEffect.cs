using System.Collections.Generic;
using UnityEngine;

// ステージ（区画）突破時に、プレイヤーのランダムなステータスが上昇する
// ボス戦勝利をステージ突破として扱う
[CreateAssetMenu(menuName = "Data/Relic/Stage Clear Random Status")]
public class StageClearRandomStatusRelicEffect : RelicEffect, IBattleEndHook, IStatusModifierHook {
    [SerializeField, Tooltip("上昇候補")]
    private List<BuffModifier> _candidates = new() {
        EffectUtility.Modifier(StatusType.MaxHP, BuffType.Adding, 20f),
        EffectUtility.Modifier(StatusType.MaxMP, BuffType.Adding, 10f),
        EffectUtility.Modifier(StatusType.PhysicalAttack, BuffType.Adding, 5f),
        EffectUtility.Modifier(StatusType.MagicAttack, BuffType.Adding, 5f),
        EffectUtility.Modifier(StatusType.Defense, BuffType.Adding, 5f),
        EffectUtility.Modifier(StatusType.Speed, BuffType.Adding, 3f),
        EffectUtility.Modifier(StatusType.CritRate, BuffType.Adding, 3f),
    };

    private class State { public readonly List<BuffModifier> Gained = new(); }

    public void OnBattleEnd(Character owner, BattleEndContext ctx) {
        if (!ctx.IsVictory || ctx.BattleType != BattleType.Boss) return;
        if (_candidates.Count == 0) return;

        var picked = _candidates[Random.Range(0, _candidates.Count)];
        owner.EffectStates.Get<State>(this).Gained.Add(picked);
        Debug.Log($"[StageClear] {picked.StatusType} {picked.BuffType} {picked.Value}");
    }

    public void ModifyStatus(Character owner, StatusType type, ref float add, ref float multiplier) {
        var gained = owner.EffectStates.Get<State>(this).Gained;
        EffectUtility.ApplyModifiers(gained, type, 1f, ref add, ref multiplier);
    }
}
