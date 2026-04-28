using System.Collections.Generic;
using UnityEngine;

public class CombatSystem : MonoBehaviour {
    [SerializeField] private Character _player;
    [SerializeField] private List<Character> _enemies;
    [SerializeField] private StatusEffectResolver _resolver;
    public void RequestAttack() {
        if (!CanAttack(_player)) return;
        RequestPlayerAttack();
    }

    private bool CanAttack(Character actor) {
        return actor.CanAct();
    }

    private AttackContext CreateContext(Character attacker) {
        return new AttackContext {
            Attacker = attacker,
            Targets = GetTargets(attacker, attacker.CurrentSkill),
            Element = attacker.CurrentElement,
            Skill = attacker.CurrentSkill,
            StatusEffect = _resolver.Get(attacker.CurrentElement)
        };
    }

    private void RequestPlayerAttack() {
        var ctx = CreateContext(_player);

        if (ctx.Element != ElementType.None) {
            if (!_player.TryConsumeMP(ctx.Skill.MPCost)) {
                Debug.Log("Change to None Element due to lacking MP");
                ctx.Element = ElementType.None;
            }
        }
        _player.TriggerAttack(ctx);
    }

    public void RequestEnemyAttack(Character enemy) {
        if (!CanAttack(enemy)) return;

        var ctx = CreateContext(enemy);
        enemy.TriggerAttack(ctx);
    }

    public List<Character> GetEnemies(Character requester) {
        if (requester == _player) return _enemies;
        else return new List<Character> { _player };  // 仮でプレイヤーだけ、分身など味方 NPC が出てきた時は変更
    }

    private List<Character> GetTargets(Character attacker, SkillData skill) {
        var enemies = GetEnemies(attacker);

        return skill.targetType switch {
            SkillTargetType.Single => new List<Character> { enemies[0] },  // 仮で先頭の敵に飛ぶようにする
            SkillTargetType.All => enemies,
            _ => enemies,
        };
    }
}