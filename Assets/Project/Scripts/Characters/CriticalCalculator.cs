using UnityEngine;

public static class CriticalCalculator {
    public static void Apply(DamageContext ctx) {
        float critRate = ctx.Attacker.GetFinalStatus(StatusType.CritRate) / 100f;
        if (Random.value > critRate) return;
        ctx.IsCritical = true;

        float critDamage = ctx.Attacker.GetFinalStatus(StatusType.CritDamage);
        ctx.CritMultiplier = 1f + critDamage/100f; // 倍率 (100 +  critDamage) %
        ctx.FinalDamage *= ctx.CritMultiplier;
    }
}