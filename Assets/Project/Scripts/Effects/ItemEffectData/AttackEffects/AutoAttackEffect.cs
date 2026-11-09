using System.Collections.Generic;
using UnityEngine;

// 物理攻撃力に依存するオート攻撃を一定間隔で行う
[CreateAssetMenu(menuName = "Data/Effect/Attack/Auto Attack")]
public class AutoAttackEffect : Effect, IBattleStartHook, IBattleUpdateHook {
    [SerializeField, Min(0.1f), Tooltip("攻撃間隔（秒）")] private float _interval = 3f;
    [SerializeField, Tooltip("物理攻撃力に対する倍率")] private float _powerMultiplier = 0.3f;
    [SerializeField, Tooltip("会心するか")] private bool _canCrit = false;
    [SerializeField, Tooltip("Single: 生存している敵からランダムに1体 / All: 敵全体")] private SkillTargetType _targetType = SkillTargetType.Single;

    private class State { public float Timer; }

    public void OnBattleStart(Character owner, BattleContext ctx) {
        owner.EffectStates.Get<State>(this).Timer = 0f;
    }

    public void OnBattleUpdate(Character owner, float deltaTime) {
        var state = owner.EffectStates.Get<State>(this);
        state.Timer += deltaTime;
        if (state.Timer < _interval) return;
        state.Timer -= _interval;

        if (!EffectUtility.IsAlive(owner) || !owner.CanAct()) return;

        var enemies = owner.GetCombatTargets(owner);
        if (enemies.Count == 0) return;

        var targets = _targetType == SkillTargetType.All
            ? enemies
            : new List<Character> { enemies[Random.Range(0, enemies.Count)] };

        var ctx = new AttackContext {
            Attacker = owner,
            Targets = targets,
            Kind = AttackKind.Auto,
            TriggerOnAttackEffects = false  // 自傷・連撃などの攻撃時効果は発動しない
        };
        ctx.AttackInstances.Add(new AttackInstance {
            PowerMultiplier = _powerMultiplier,
            CanCrit = _canCrit
        });

        AttackExecutor.Execute(ctx);
    }
}
