using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Data/StatusEffect/Shock")]
public class ShockEffect : StatusEffectBehaviour {
    public override void OnApply(Character target, StatusEffectInstance instance) { }
    public override void OnUpdate(Character target, StatusEffectInstance instance, float deltaTime) { }
    public override void OnDamage(Character target, StatusEffectInstance instance, DamageContext ctx) { }

    public override float ModifyStat(StatusType type, float value, StatusEffectInstance instance) {
        // 感電による攻撃速度低下
        if (type == StatusType.Speed) {
            return value * instance.Data.shockSpeedMultiplier;
        }

        return value;
    }

    public override bool ShouldBlockAction(Character target, StatusEffectInstance instance) {
        float baseChance = instance.Data.shockFailChance;
        float rate = target.GetFinalStatus(StatusType.StatusInflictionRate) / 100f;
        float chance = baseChance * (1f + rate);

        if (UnityEngine.Random.value > chance) {
            Debug.Log("Shock: Action Failed");
            return true;
        }
        return false;
    }

    public override bool OnReapply(Character target, Character source, StatusEffectData data) {
        TriggerOverShock(target, source, data);
        return true;  // 新規付与しない
    }

    private void TriggerOverShock(Character target, Character source, StatusEffectData data) {
        var targets = target.GetCombatTargets(source);
        float magic = source.GetFinalStatus(StatusType.MagicAttack);

        foreach (var t in targets) {
            var shock = t.GetStatus(StatusEffectType.Shock);
            if (shock == null) continue;

            float multiplier = (t == target) ? data.overShockBaseMultiplier : data.overShockChainDecay;
            int damage = Mathf.FloorToInt(magic * multiplier);

            var ctx = DamageContextFactory.CreateFixed(source, t, damage);
            t.TakeDamage(ctx);

            t.RequestRemoveStatus(shock);  // 連鎖後は敵全体の感電を解除
            Debug.Log($"[OverShock] damage: {damage}");
        }
    }
}