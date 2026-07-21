using UnityEngine;

public class EnemyController : MonoBehaviour {
    [SerializeField] private Character _character;
    [SerializeField] private EnemyData _data;
    
    private EnemyAttackScheduler _scheduler;
    private CombatSystem _combatSystem;

    private void Update() {
        if (_combatSystem.State != CombatState.Playing) return;

        _scheduler.Tick(Time.deltaTime);
    }

    // 戦闘初期化
    public void Initialize(CombatSystem combatSystem) {
        _combatSystem = combatSystem;
        _scheduler = new EnemyAttackScheduler();
        _scheduler.Initialize(_character, combatSystem, _data);
    }
}