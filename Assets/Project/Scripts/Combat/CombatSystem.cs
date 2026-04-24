using System.Collections.Generic;
using UnityEngine;

public class CombatSystem : MonoBehaviour {
    [SerializeField] private Character _player;
    [SerializeField] private List<Character> _enemies;

    public void RequestAttack() {
        if (!CanAttack()) return;
        RequestPlayerAttack();
    }

    // スタン・クールタイムなどなど攻撃不可のタイミングが出てきたとき用
    private bool CanAttack() {
        return true;
    }

    private void RequestPlayerAttack() {
        var ctx = new AttackContext {
            Attacker = _player,
            Targets = GetTargets()
        };
        _player.TriggerAttack(ctx);
    }

    public void RequestEnemyAttack(Character enemy) {
        if (!CanAttack()) return;

        var ctx = new AttackContext {
            Attacker = enemy,
            Targets = new List<Character> { _player }   // 分身を用意した時に使うかも
        };
        enemy.TriggerAttack(ctx);
    }

    private List<Character> GetTargets() {
        // 取り敢えず今は敵全体が攻撃対象
        return _enemies;
    }
}