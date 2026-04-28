using UnityEngine;

public abstract class StatusEffectBehaviour : ScriptableObject {
    // 付与時
    public virtual void OnApply(Character target, StatusEffectInstance instance) {}
    // 毎フレーム更新
    public abstract void OnUpdate(Character target, StatusEffectInstance instance, float deltaTime);
    // 解除時
    public virtual void OnRemove(Character target, StatusEffectInstance instance) {}
    // 被弾フック
    public virtual void OnDamage(Character target, StatusEffectInstance instance, DamageContext ctx) {}

    // ステータス補正
    public virtual float ModifyStat(StatusType type, float value, StatusEffectInstance instance) => value;
    // 行動可否
    public virtual bool ShouldBlockAction(Character target, StatusEffectInstance instance) => false;
    // 再付与時のハンドリング
    public virtual bool OnReapply(Character target, Character source, StatusEffectData data) => false;
 }