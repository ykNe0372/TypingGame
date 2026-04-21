using UnityEngine;

public abstract class Effect : ScriptableObject {
    public virtual int GetStatusBonus(StatusType type) {
        return 0;
    }
    
    // public virtual void OnAttack(CharacterContext context) { }
}