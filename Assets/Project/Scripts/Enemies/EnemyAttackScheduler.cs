using System.Collections.Generic;
using UnityEngine;

public class EnemyAttackScheduler {
    private Character _owner;
    private CombatSystem _combatSystem;
    private EnemyData _data;
    private readonly List<EnemySkillRuntime> _skills = new();

    public void Initialize(Character owner, CombatSystem combatSystem, EnemyData data) {
        _owner = owner;
        _combatSystem = combatSystem;
        _data = data;

        _skills.Clear();

        foreach(var skill in owner.Skills) {
            _skills.Add(new EnemySkillRuntime {
                Skill = skill
            });
        }
    }

    public void Tick(float deltaTime) {
        foreach (var runtime in _skills) {
            runtime.Timer += deltaTime;

            if (runtime.Timer >= GetInterval(runtime.Skill)) {
                runtime.Timer = 0f;
                _combatSystem.RequestEnemyAttack(_owner, runtime.Skill, _data);
            }
        }
    }

    private float GetInterval(SkillData skill) {
        float speed = _owner.GetFinalStatus(StatusType.Speed);

        return 50f / (1f + speed * 0.1f) * skill.speedMultiplier;
    }
}