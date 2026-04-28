public static class StatusCalculator {
    public static float CalculateChance(Character attacker, Character target, StatusEffectData data, float bonus) {
        float infliction = attacker.GetFinalStatus(StatusType.StatusInfliction) / 100f;  // 付与確率を (StatusInfliction) % アップ
        float resistance = target.GetFinalStatus(StatusType.StatusResistance) / 100f;    // 付与確率を (StatusResistance) % ダウン
        return (data.baseChance + bonus) * (1 + infliction) * (1f - resistance);
    }
}