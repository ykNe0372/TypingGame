using System.Collections.Generic;

public class AttackContext {
    public Character Attacker;       // 攻撃者
    public List<Character> Targets;  // 対象（単体/複数）
    public int AttackCount = 1;
    public ElementType Element;      // 属性
    public SkillData Skill;
    public StatusEffectData StatusEffect;
}