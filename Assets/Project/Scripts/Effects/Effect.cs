using UnityEngine;

public abstract class Effect : ScriptableObject {
    public virtual int GetStatusBonus(StatusType type) {
        return 0;
    }
    
    public virtual void OnAttack(AttackContext ctx) {
        Debug.Log("OnAttack called");
        Debug.Log($"Target Count: {ctx.Targets.Count}");
    }
}