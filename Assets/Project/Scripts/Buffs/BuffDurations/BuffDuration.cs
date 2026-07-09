public abstract class BuffDuration {
    public abstract bool IsExpired { get; }
    public virtual void OnBattleEnd() {}
    public virtual void Tick(float deltaTime) {}
    public virtual void OnAttack() {}
    public virtual void OnDamaged() {}
}