using UnityEngine;

[CreateAssetMenu(fileName = "EnemyData", menuName = "Enemy/Enemy Data")]
public class EnemyData : ScriptableObject {
    public GameObject Prefab;
    public EnemyElementSettings ElementSettings;
    // public CharacterBaseStatus BaseStatus;
    // public List<SkillData> Skills;
    // public string Description;
    // public EnemyRank Rank;
}