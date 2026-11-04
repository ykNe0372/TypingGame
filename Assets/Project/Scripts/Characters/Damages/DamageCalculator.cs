using UnityEngine;

public static class DamageCalculator {
    public static float Calculate(Character attacker, Character target) {
        float DefenceConstant = GameConstants.Damage.DefenceConstant;

        float atk = attacker.GetFinalStatus(StatusType.PhysicalAttack);
        float pen = attacker.GetFinalStatus(StatusType.Penetration);
        float def = target.GetFinalStatus(StatusType.Defense);

        float effectiveDefence = Mathf.Max(0f, def * (1f - pen / 100f));
        float baseDamage = atk * (DefenceConstant / (DefenceConstant + effectiveDefence));

        return Mathf.Max(1f, baseDamage);
    }
}