using UnityEngine;

public abstract class RelicEffect : ScriptableObject {
    public virtual float GetStatusBonus(StatusType type) {
        return 0f;
    }

    public virtual void OnEquip(Character owner) {}
    public virtual void OnUnequip(Character owner) {}

    public virtual void OnAttack(AttackContext ctx) {}
    public virtual void OnDamage(DamageContext ctx) {}
}