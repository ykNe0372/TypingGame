using System;
using UnityEngine;

public static class DamageCalculator {
    public static int Calculate(Character attacker, Character target) {
        int atk = attacker.GetFinalStatus(StatusType.PhysicalAttack);
        int def = target.GetFinalStatus(StatusType.Defense);

        return Mathf.Max(1, atk - def);
    }
}