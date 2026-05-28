using UnityEngine;
using System;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "EnemyDataBase", menuName = "Enemy/Enemy DataBase")]
public class EnemyDataBase : ScriptableObject {
    [Serializable]
    public class EnemyFloorTable {
        [Header("階層番号")] public int FloorGroup;
        [Header("通常敵")] public List<EnemySpawnEntry> NormalEnemies;
        [Header("ボス敵")] public List<EnemySpawnEntry> BossEnemy;
    }

    [SerializeField] private List<EnemyFloorTable> _floorEnemySets = new();

    // 階層対応テーブルの取得
    public EnemyFloorTable GetFloorSet(int floorGroup) {
        Debug.Log($"Search FloorGroup: {floorGroup}");
        foreach (var set in _floorEnemySets) {
            Debug.Log($"Check: {set.FloorGroup}");
            if (set.FloorGroup == floorGroup) return set;
        }

        return null;
    }
}