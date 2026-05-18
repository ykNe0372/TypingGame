public class DamageContext {
    public Character Attacker;
    public Character Target;
    public float BaseDamage;    // 計算前ダメージ
    public float FinalDamage;   // 計算後ダメージ
    public bool IsCritical;
    public float CritMultiplier;
}