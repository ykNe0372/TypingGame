using UnityEngine;

[CreateAssetMenu(menuName = "Data/Battle/SpecialAttack")]
public class SpecialAttackData : ScriptableObject {
    public float multiplier;
    public SkillTargetType targetType;
}