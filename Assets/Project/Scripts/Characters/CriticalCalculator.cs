using UnityEngine;

public static class CriticalCalculator {
    public static void Apply(DamageContext ctx, AttackInstance attack) {
        if (!ctx.ForceCritical) {
            float critRate = (ctx.Attacker.GetFinalStatus(StatusType.CritRate) + ctx.CritRateBonus) / 100f;
            float effectiveCritRate = Mathf.Min(1f, critRate * attack.CritRateMultiplier);
            if (Random.value > effectiveCritRate) return;
        }
        ctx.IsCritical = true;

        float critDamage = ctx.Attacker.GetFinalStatus(StatusType.CritDamage);
        ctx.CritMultiplier = 1f + critDamage/100f; // 倍率 (100 +  critDamage) %
        ctx.FinalDamage *= ctx.CritMultiplier;
    }
}