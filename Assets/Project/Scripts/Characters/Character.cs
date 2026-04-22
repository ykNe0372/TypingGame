using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour {
    [SerializeField] private CharacterBaseStatus _baseStatus;
    [SerializeField] private List<GrowthItem> _items = new();

    private List<Effect> _passiveEffects = new();
    private List<OnAttackEffect> _attackEffects = new();

    private void Awake() {
        BuildEffectList();
    }

    private void BuildEffectList() {
        _passiveEffects.Clear();
        _attackEffects.Clear();

        foreach (var item in _items) {
            foreach (var effect in item.GetEffects()) {
                if (effect is OnAttackEffect attackEffect) _attackEffects.Add(attackEffect);
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
            int damage = CalculateDamage(target);
            target.TakeDamage(damage);
            Debug.Log($"{name} → {target.name} に {damage} ダメージ");
        }
    }

    // ダメージ計算（仮）
    private int CalculateDamage(Character target) {
        int atk = GetFinalStatus(StatusType.PhysicalAttack);
        int def = target.GetFinalStatus(StatusType.Defense);
        
        int damage = Mathf.Max(1, atk - def);
        return damage;
    }

    // ダメージ適応（仮）
    public void TakeDamage(int damage) {
        int _currentHP = GetFinalStatus(StatusType.MaxHP);
        _currentHP -= damage;
        Debug.Log($"{name} 残りHP: {_currentHP}");
    }
    
    public void TriggerAttack(AttackContext ctx) {
        foreach (var effect in _attackEffects) effect.OnAttack(ctx);
        for (int i=0; i<ctx.AttackCount; ++i) ExecuteAttack(ctx);
    }
}