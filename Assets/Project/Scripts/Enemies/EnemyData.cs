using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "EnemyData", menuName = "Enemy/Enemy Data")]
public class EnemyData : ScriptableObject {
    public CharacterBaseStatus BaseStatus;
    public List<SkillData> Skills;
    public GameObject Prefab;
    // public string Description;
    // public EnemyRank Rank;
}