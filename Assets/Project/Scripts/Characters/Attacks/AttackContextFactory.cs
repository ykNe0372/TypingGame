using UnityEngine;
using System.Collections.Generic;

public static class AttackContextFactory {
    public static AttackContext CreateNormalAttack(Character attacker, List<Character> targets, StatusEffectData status) {
        var ctx = new AttackContext {
            Attacker = attacker,
            Targets = targets,
            Skill = attacker.CurrentSkill,
            Element = attacker.CurrentElement,
            StatusEffect = status
        };

        ctx.AttackInstances.Add(new AttackInstance {
            PowerMultiplier = attacker.CurrentSkill.powerMultiplier,
            CanCrit = true
        });

        return ctx;
    }

    public static AttackContext CreateBonusAttack(Character attacker, List<Character> targets, BonusAttackData data) {
        var ctx = new AttackContext {
            Attacker = attacker,
            Targets = targets,
            Skill = attacker.CurrentSkill
        };

        ctx.AttackInstances.Add(new AttackInstance {
            PowerMultiplier = data.multiplier,
            CanCrit = true
        });

        Debug.Log("[BONUS] Bonus Attack Executed");
        return ctx;
    }

    public static AttackContext CreateSpecialAttack(Character attacker, List<Character> targets, SpecialAttackData data) {
        var ctx = new AttackContext {
            Attacker = attacker,
            Targets = targets,
        };

        ctx.AttackInstances.Add(new AttackInstance {
            PowerMultiplier = data.multiplier,
            CanCrit = false
        });

        Debug.Log("[SPECIAL] Special Attack Executed");
        return ctx;
    }
}