using UnityEngine;

public abstract class StatusEffectBehaviour : ScriptableObject {
    // 付与時
    public virtual void OnApply(Character target, StatusEffectInstance instance) {}
    // 毎フレーム更新
    public abstract void OnUpdate(Character target, StatusEffectInstance instance, float deltaTime);
    // 解除時
    public virtual void OnRemove(Character target, StatusEffectInstance instance) {}
}