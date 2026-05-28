using UnityEngine;

public class EnemyController : MonoBehaviour {
    [SerializeField] private Character _character;
    
    private CombatSystem _combatSystem;
    private float _attackTimer;

    private void Update() {
        if (_combatSystem.State != CombatState.Playing) return;

        _attackTimer += Time.deltaTime;     // 重かったら Coroutine に切り替えるかも
        float interval = GetAttackInterval();

        if (_attackTimer >= interval) {
            _attackTimer = 0f;
            RequestAttack();
        }
    }

    // 戦闘初期化
    public void Initialize(CombatSystem combatSystem) {
        _combatSystem = combatSystem;
        _attackTimer = 0f;
    }

    // 攻撃速度を決定（仮）
    private float GetAttackInterval() {
        float speed = _character.GetFinalStatus(StatusType.Speed);
        return 50.0f / (1f + speed * 0.1f);
    }

    private void RequestAttack() {
        _combatSystem.RequestEnemyAttack(_character);
    }
}