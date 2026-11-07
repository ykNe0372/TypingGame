using UnityEngine;
using System;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "EnemyDataBase", menuName = "Enemy/Enemy DataBase")]
public class EnemyDataBase : ScriptableObject {
    [Serializable]
    public class EnemyHierarchyTable {
        [Header("階層番号")] public int HierarchyNumber;
        [Header("通常敵")] public List<EnemySpawnEntry> NormalEnemies;
        [Header("ボス敵")] public List<EnemySpawnEntry> BossEnemies;
    }

    [SerializeField] private List<EnemyHierarchyTable> _hierarchyTables = new();

    public EnemyHierarchyTable GetHierarchyTable(int hierarchyNumber) {
        foreach (var table in _hierarchyTables) {
            if (table.HierarchyNumber == hierarchyNumber) return table;
        }

        return null;
    }
}