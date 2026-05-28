using UnityEngine;

[System.Serializable]
public class EnemySpawnEntry {
    [Header("敵データ")] public EnemyData EnemyData;
    [Header("出現重み")] public int SpawnWeight = 1;
}