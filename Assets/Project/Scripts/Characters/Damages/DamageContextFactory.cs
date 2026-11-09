public static class DamageContextFactory {
    // 通常攻撃用
    public static DamageContext CreateAttack(Character attacker, Character target) {
        return CreateAttack(attacker, target, StatusType.PhysicalAttack);
    }

    // 参照する攻撃力を指定する版（MP 消費攻撃は MagicAttack）
    public static DamageContext CreateAttack(Character attacker, Character target, StatusType attackStatus) {
        return new DamageContext {
            Attacker = attacker,
            Target = target,
            BaseDamage = DamageCalculator.Calculate(attacker, target, attackStatus)
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