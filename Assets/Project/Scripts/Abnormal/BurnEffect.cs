using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Data/StatusEffect/Burn")]
public class BurnEffect: StatusEffectBehaviour {
    [SerializeField] private float _damageInterval = 1.0f;

    private readonly Dictionary<StatusEffectInstance, float> _timers = new(); // 燃焼ごとにタイマーを持つ

    public override void OnApply(Character target, StatusEffectInstance instance) {
        _timers[instance] = 0f;
    }

    public override void OnUpdate(Character target, StatusEffectInstance instance, float deltaTime) {
        _timers[instance] += deltaTime;
        if (_timers[instance] >= _damageInterval) {
            _timers[instance] -= _damageInterval;

            float magic = instance.Source.GetFinalStatus(StatusType.MagicAttack);
            int damage = Mathf.FloorToInt(magic);    // 仮で MagicAttack = ATK とする
            
            var ctx = DamageContextFactory.CreateFixed(instance.Source, target, damage);
            target.TakeDamage(ctx);

            Debug.Log($"[Burn] {target.name} に {damage} ダメージ");
        }
    }

    public override void OnRemove(Character target, StatusEffectInstance instance) {
        _timers.Remove(instance);
    }
}