public class DamageContext {
    public Character Attacker;
    public Character Target;
    public float BaseDamage;    // 計算前ダメージ
    public float FinalDamage;   // 計算後ダメージ
    public int ActualDamage;    // 実際に与えたダメージ
    public bool IsCritical;
    public float CritMultiplier;
    public bool IsEnvironmentDamage;  // 環境ダメージか
    public bool ForceCritical;        // true なら確定で会心
    public float CritRateBonus;       // 会心率への加算（%）
}