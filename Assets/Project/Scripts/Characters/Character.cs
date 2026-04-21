using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour {
    [SerializeField] private CharacterBaseStatus _baseStatus;
    [SerializeField] private List<GrowthItem> _items = new();

    private List<Effect> _effects = new();

    private void Awake() {
        BuildEffectList();
    }

    private void BuildEffectList() {
        _effects.Clear();

        foreach (var item in _items) _effects.AddRange(item.GetEffects());
    }

    public int GetFinalStatus(StatusType type) {
        int baseValue = _baseStatus.GetStatus(type);
        int bonus = 0;
        
        foreach (var effect in _effects) bonus += effect.GetStatusBonus(type);
        return baseValue + bonus;
    }

    // public void Attack() {
    //     var ctx = new CharacterContext { Owner = this };

    //     foreach (var effect in _effects) effect.OnAttack(ctx);
    // }
}