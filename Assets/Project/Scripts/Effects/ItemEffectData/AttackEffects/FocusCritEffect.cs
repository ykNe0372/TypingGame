using UnityEngine;

// 同じ敵を一定回数連続で攻撃すると、次の攻撃が確定クリティカルになる
// 通常攻撃・ボーナス攻撃の単体攻撃のみカウントする
[CreateAssetMenu(menuName = "Data/Effect/Attack/Focus Critical")]
public class FocusCritEffect : Effect, IBattleStartHook, IBeforeAttackHook, IOutgoingDamageHook {
    [SerializeField, Min(1), Tooltip("何回連続で攻撃すると次が確定会心になるか")] private int _requiredHits = 3;

    private class State {
        public Character LastTarget;
        public int Streak;
        public AttackContext CritAttack;  // 確定会心にする攻撃
    }

    public void OnBattleStart(Character owner, BattleContext ctx) {
        var state = owner.EffectStates.Get<State>(this);
        state.LastTarget = null;
        state.Streak = 0;
        state.CritAttack = null;
    }

    public void OnBeforeAttack(Character owner, AttackContext ctx) {
        if (ctx.Kind != AttackKind.Normal && ctx.Kind != AttackKind.Bonus) return;
        if (ctx.Targets.Count != 1) return;

        var state = owner.EffectStates.Get<State>(this);
        state.CritAttack = null;

        Character target = ctx.Targets[0];
        if (state.LastTarget != target) {
            state.LastTarget = target;
            state.Streak = 0;
        }

        if (state.Streak >= _requiredHits) {
            state.CritAttack = ctx;
            state.Streak = 0;
            Debug.Log("[FocusCrit] Guaranteed Critical");
            return;
        }

        ++state.Streak;
    }

    public void ModifyOutgoingDamage(Character owner, AttackContext attack, AttackInstance instance, DamageContext damage) {
        if (ReferenceEquals(owner.EffectStates.Get<State>(this).CritAttack, attack)) damage.ForceCritical = true;
    }
}
