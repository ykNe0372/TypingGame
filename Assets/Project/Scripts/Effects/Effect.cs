using UnityEngine;

public abstract class Effect : ScriptableObject {
    public virtual int GetStatusBonus(StatusType type) {
        return 0;
    }

    // override 前提の設計
    public virtual void OnAttack(AttackContext ctx) {}
}