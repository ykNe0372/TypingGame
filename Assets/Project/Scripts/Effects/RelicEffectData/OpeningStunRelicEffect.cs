using UnityEngine;

// 戦闘開始時、一定時間敵の行動が停止する（その間の敵の攻撃は発動しない）
[CreateAssetMenu(menuName = "Data/Relic/Opening Stun")]
public class OpeningStunRelicEffect : RelicEffect, IBattleStartHook, IBattleUpdateHook, IIncomingAttackCancelHook {
    [SerializeField, Min(0f), Tooltip("停止時間")] private float _duration = 5f;

    private class State { public float Elapsed; }

    public void OnBattleStart(Character owner, BattleContext ctx) {
        owner.EffectStates.Get<State>(this).Elapsed = 0f;
    }

    public void OnBattleUpdate(Character owner, float deltaTime) {
        owner.EffectStates.Get<State>(this).Elapsed += deltaTime;
    }

    public bool ShouldCancelIncomingAttack(Character owner, AttackContext ctx) {
        if (ctx.Attacker == owner) return false;
        return owner.EffectStates.Get<State>(this).Elapsed < _duration;
    }
}
