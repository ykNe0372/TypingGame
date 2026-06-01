using System.Collections.Generic;
using UnityEngine;

public class EnemyFactory : MonoBehaviour {
    [SerializeField] private CombatSystem _combatSystem;
    [SerializeField] private Transform _enemyRoot;
    [SerializeField] private List<Transform> _spawnPoints;

    // 敵生成
    public Character CreateEnemy(EnemyData enemyData, int spawnIndex) {
        Transform spawnPoint = _spawnPoints[spawnIndex];
        GameObject obj = Instantiate(enemyData.Prefab, spawnPoint.position, Quaternion.identity, _enemyRoot);
        Character enemy = obj.GetComponent<Character>();
        if (obj.TryGetComponent<EnemyController>(out var controller)) controller.Initialize(_combatSystem);
        enemy.Initialize();

        return enemy;
    }
}