using UnityEngine;

public abstract class Effect : ScriptableObject {
    public virtual float GetStatusBonus(StatusType type) {
        return 0;
    }

    // 中間層でこの処理を override して設計する
    public virtual void OnAttack(AttackContext ctx) {}
    public virtual void OnDamage(DamageContext ctx) {}
}