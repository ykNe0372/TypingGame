using UnityEngine;

[CreateAssetMenu(menuName = "Data/Battle/BonusAttack")]
public class BonusAttackData : ScriptableObject {
    public float multiplier;
    public SkillTargetType targetType;
}