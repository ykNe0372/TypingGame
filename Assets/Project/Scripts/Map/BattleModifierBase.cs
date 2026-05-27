using UnityEngine;

public abstract class BattleModifierBase : ScriptableObject {
    public virtual void OnBattleStart(BattleContext ctx) {}
    public virtual void OnUpdate(BattleContext ctx, float deltaTime) {}
    public virtual void OnAttack(BattleContext ctx, AttackContext attackContext) {}
    public virtual void OnTakeDamage(BattleContext ctx, DamageContext damageContext) {}
    public virtual void OnBattleEnd(BattleContext ctx) {}
}