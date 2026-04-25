using System;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour {
    [SerializeField] private CharacterBaseStatus _baseStatus;   // 基礎ステータス
    [SerializeField] private List<GrowthItem> _items = new();   // 強化アイテム
    [SerializeField] private List<SkillData> _skills;

    private List<Effect> _passiveEffects = new();           // ステータス変更系
    private List<OnAttackEffect> _attackEffects = new();    // 攻撃変更（連撃）系
    private List<OnDamageEffect> _damageEffects = new();    // ダメージ計算
    private int _currentHP;
    private int _currentSkillIndex = 0;

    public int MaxHP => GetFinalStatus(StatusType.MaxHP);
    public SkillData CurrentSkill => _skills[_currentSkillIndex];

    private void Awake() {
        BuildEffectList();
        InitializeHP();
    }

    public void InitializeHP() {
        _currentHP = MaxHP;
    }

    // 強化アイテム一覧
    private void BuildEffectList() {
        _passiveEffects.Clear();
        _attackEffects.Clear();

        foreach (var item in _items) {
            foreach (var effect in item.GetEffects()) {
                if (effect is OnAttackEffect attackEffect) _attackEffects.Add(attackEffect);
                else if (effect is OnDamageEffect damageEffect) _damageEffects.Add(damageEffect);
                else _passiveEffects.Add(effect);
            }
        }
    }

    public int GetFinalStatus(StatusType type) {
        int baseValue = _baseStatus.GetStatus(type);
        int bonus = 0;
        
        foreach (var effect in _passiveEffects) bonus += effect.GetStatusBonus(type);
        return baseValue + bonus;
    }

    private void ExecuteAttack(AttackContext ctx) {
        foreach (var target in ctx.Targets) {
            var dmgCtx = new DamageContext {
                Attacker = this,
                Target = target,
                BaseDamage = DamageCalculator.Calculate(this, target) // 基礎ダメージ計算
            };
            dmgCtx.FinalDamage = Mathf.RoundToInt(dmgCtx.BaseDamage * CurrentSkill.powerMultiplier);
            target.TakeDamage(dmgCtx);   // 被弾処理
        }
    }

    // ダメージ適応（仮）
    public void TakeDamage(DamageContext ctx) {
        foreach (var effect in _damageEffects) effect.OnDamage(ctx);
        int damage = Mathf.Max(0, ctx.FinalDamage);

        _currentHP -= damage;
        _currentHP = Mathf.Max(0, _currentHP);
    
        Debug.Log($"{name} HP: {_currentHP}/{MaxHP}");
    }
    
    public void TriggerAttack(AttackContext ctx) {
        foreach (var effect in _attackEffects) effect.OnAttack(ctx);
        for (int i=0; i<ctx.AttackCount; ++i) ExecuteAttack(ctx);
    }

    // 数字キーで技を変える
    public void ChangeSkill(int index) {
        if (index < 0 || index >= _skills.Count) return;
        _currentSkillIndex = index;

        Debug.Log($"Skill Changed: {CurrentSkill.skillName}");
    }
}