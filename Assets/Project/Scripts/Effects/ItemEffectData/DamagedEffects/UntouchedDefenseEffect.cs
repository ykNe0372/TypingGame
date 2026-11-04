using System.Collections.Generic;
using UnityEngine;

// 一定期間攻撃を受けなかった場合、防御力が上昇する（被弾すると解除）
[CreateAssetMenu(menuName = "Data/Effect/Damaged/Untouched Defense")]
public class UntouchedDefenseEffect : Effect, IBattleStartHook, IBattleUpdateHook, IAfterDamagedHook, IStatusModifierHook {
    [SerializeField, Min(0f), Tooltip("何秒被弾しなければ発動するか")] private float _requiredSeconds = 5f;
    [SerializeField, Tooltip("発動中の補正")]
    private List<BuffModifier> _modifiers = new() {
        EffectUtility.Modifier(StatusType.Defense, BuffType.Multiplier, 1.3f),
    };

    private class State { public float Elapsed; }

    public void OnBattleStart(Character owner, BattleContext ctx) {
        owner.EffectStates.Get<State>(this).Elapsed = 0f;
    }

    public void OnBattleUpdate(Character owner, float deltaTime) {
        owner.EffectStates.Get<State>(this).Elapsed += deltaTime;
    }

    public void OnAfterDamaged(Character owner, DamageContext ctx) {
        if (ctx.IsEnvironmentDamage || ctx.ActualDamage <= 0) return;
        owner.EffectStates.Get<State>(this).Elapsed = 0f;
    }

    public void ModifyStatus(Character owner, StatusType type, ref float add, ref float multiplier) {
        if (!EffectUtility.ContainsStatus(_modifiers, type)) return;
        if (owner.EffectStates.Get<State>(this).Elapsed < _requiredSeconds) return;

        EffectUtility.ApplyModifiers(_modifiers, type, 1f, ref add, ref multiplier);
    }
}
