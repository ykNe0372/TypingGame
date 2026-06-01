using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "EnemySpawnTable", menuName = "Enemy/Spawn Table")]
public class EnemySpawnTable : ScriptableObject {
    [Header("通常敵")] public List<EnemySpawnEntry> NormalEnemies;
    [Header("ボス敵")] public List<EnemySpawnEntry> BossEnemies;
}