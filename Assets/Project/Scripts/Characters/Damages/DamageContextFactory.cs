public static class DamageContextFactory {
    // 通常攻撃用
    public static DamageContext CreateAttack(Character attacker, Character target) {
        return new DamageContext {
            Attacker = attacker,
            Target = target,
            BaseDamage = DamageCalculator.Calculate(attacker, target)
        };
    }

    // 固定ダメージ用（状態異常など）
    // isEnvironmentDamage → true:  環境ダメージ 被弾時処理が発動しない | false: 非環境ダメージ 被弾時処理が発動する
    public static DamageContext CreateFixed(Character attacker, Character target, float damage, bool isEnvironmentDamage) {
        return new DamageContext {
            Attacker = attacker,
            Target = target,
            BaseDamage = damage,
            FinalDamage = damage,   // 仮でそのまま出力
            IsEnvironmentDamage = isEnvironmentDamage
        };
    }
}