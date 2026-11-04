using System.Collections.Generic;

public class AttackContext {
    public Character Attacker;       // 攻撃者
    public List<Character> Targets;  // 対象（単体/複数）
    public List<AttackInstance> AttackInstances = new(); // 攻撃倍率などの情報
    public ElementType Element;      // 属性
    public SkillData Skill;
    public StatusEffectData StatusEffect;
    public AttackKind Kind = AttackKind.Normal;
    public bool IsMagical;                      // MP を消費した攻撃か（true なら魔法攻撃力を参照）
    public bool TriggerOnAttackEffects = true;  // false: 攻撃時効果（自傷・連撃など）を発動しない
}