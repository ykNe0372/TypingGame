using UnityEngine;

// 技の種類（仮）
public enum SkillTargetType {
    Single,
    All
}

[CreateAssetMenu(menuName = "Data/Skill/SkillData")]
public class SkillData : ScriptableObject {
    public string skillName;
    public float powerMultiplier;  // 攻撃力の補正用
    public float speedMultiplier;  // 攻撃速度の補正用
    public SkillTargetType targetType;
    public int MPCost;
}