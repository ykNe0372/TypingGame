using UnityEngine;

public static class DamageCalculator {
    public static float Calculate(Character attacker, Character target) {
        float atk = attacker.GetFinalStatus(StatusType.PhysicalAttack);
        float def = target.GetFinalStatus(StatusType.Defense);

        return Mathf.Max(1f, atk - def);
    }
}