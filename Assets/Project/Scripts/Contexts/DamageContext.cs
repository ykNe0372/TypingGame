using System.Collections.Generic;

public class DamageContext {
    public Character Attacker;
    public Character Target;
    public int BaseDamage;    // 計算前ダメージ
    public int FinalDamage;   // 計算後ダメージ
    public bool IsCritical;
}