using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Data/StatusEffect/Freeze")]
public class FreezeEffect : StatusEffectBehaviour {
    [SerializeField] private float minMultiplier = 0.75f;
    [SerializeField] private float maxMultiplier = 2.5f;

    private readonly Dictionary<StatusEffectInstance, float> _breakBonus = new();

    public override void OnApply(Character target, StatusEffectInstance instance) {
        _breakBonus[instance] = 0f;

        float magic = instance.Source.GetFinalStatus(StatusType.MagicAttack);
        int damage = Mathf.FloorToInt(magic);    // 仮で MagicAttack = ATK
        
        var ctx = new DamageContext {
            Attacker = instance.Source,
            Target = target,
            FinalDamage = damage
        };
        target.TakeDamage(ctx);
        target.SetFrozen(true);
    }

    public override void OnUpdate(Character target, StatusEffectInstance instance, float deltaTime) {
        // 凍結中は継続処理はなし
    }

    public override void OnDamage(Character target, StatusEffectInstance instance, DamageContext ctx) {
        float bonus = _breakBonus.GetValueOrDefault(instance, 0f);
        float baseChance = instance.Data.freezeBreakBaseChance;

        float rate = ctx.Attacker.GetFinalStatus(StatusType.StatusInflictionRate) / 100f;
        float chance = (bonus + baseChance) * (1f + rate);

        if (UnityEngine.Random.value < chance) ForceRemove(target, instance);
        else {
            float gain = instance.Data.freezeBreakAccumulation * (1f + rate);
            bonus += gain;
            bonus = Mathf.Min(bonus, instance.Data.freezeBreakMaxBonus);
            _breakBonus[instance] = bonus;
        }
    }

    public override void OnRemove(Character target, StatusEffectInstance instance) {
        float magic = instance.Source.GetFinalStatus(StatusType.MagicAttack);
        float ratio = instance.RemainingTime / instance.InitialDuration;     // 残り時間割合
        float multipiler = Mathf.Lerp(minMultiplier, maxMultiplier, ratio);  // 倍率補間
        int damage = Mathf.FloorToInt(magic * multipiler);

        var ctx = new DamageContext {
            Attacker = instance.Source,
            Target = target,
            FinalDamage = damage
        };
        target.TakeDamage(ctx);
        
        target.SetFrozen(false);
        _breakBonus.Remove(instance);
        Debug.Log($"[FreezeBreak] ratio: {ratio:F2}, mul: {multipiler: F2}, dmg: {damage}");
    }

    private void ForceRemove(Character target, StatusEffectInstance instance) {
        target.RequestRemoveStatus(instance);
    }
}