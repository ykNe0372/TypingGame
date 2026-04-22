using System.Collections.Generic;
using UnityEngine;

public class CombatSystem : MonoBehaviour {
    [SerializeField] private Character _player;
    [SerializeField] private List<Character> _enemies;

    public void RequestAttack() {
        if (!CanAttack()) return;
        ExecuteAttack();
    }

    private bool CanAttack() {
        // スタン・クールタイムなどなど攻撃不可のタイミングが出てきたとき用
        return true;
    }

    private void ExecuteAttack() {
        var ctx = new AttackContext {
            Attacker = _player,
            Targets = GetTargets()
        };
        _player.TriggerAttack(ctx);
    }

    private List<Character> GetTargets() {
        // 取り敢えず今は敵全体が攻撃対象
        return _enemies;
    }
}