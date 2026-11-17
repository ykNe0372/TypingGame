using UnityEngine;

public class TutorialEnemyAttackController : MonoBehaviour {
    [SerializeField] private Character _character;
    [SerializeField] private EnemyData _data;

    private CombatSystem _combatSystem;

    public Character Character => _character;

    private void Awake() {
        EnemyController timedController = GetComponent<EnemyController>();
        if (timedController != null) timedController.enabled = false;
    }

    public bool Initialize(CombatSystem combatSystem) {
        if (combatSystem == null || _character == null || _data == null) {
            Debug.LogError("Tutorial enemy character and data must be assigned.", this);
            return false;
        }

        _combatSystem = combatSystem;
        return true;
    }

    public bool TryAttack(int skillIndex) {
        if (_combatSystem == null || _character == null || _data == null) {
            Debug.LogError("Tutorial enemy is not initialized.", this);
            return false;
        }
        if (_combatSystem.State != CombatState.Playing || _character.IsDead || !_character.CanAct()) return false;
        if (skillIndex < 0 || skillIndex >= _character.Skills.Count) {
            Debug.LogError($"Tutorial enemy skill index {skillIndex} is out of range.", this);
            return false;
        }

        _combatSystem.RequestEnemyAttack(_character, _character.Skills[skillIndex], _data);
        return true;
    }
}
