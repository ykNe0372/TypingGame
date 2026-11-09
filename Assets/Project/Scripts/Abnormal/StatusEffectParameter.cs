// 各項目に対して補正値を指定できるようにするための enum
public enum StatusEffectParameter {
    BaseChance,
    Duration,
    AccumulationPerFail,
    MaxBonus,

    FreezeBreakBaseChance,
    FreezeBreakAccumulation,
    FreezeBreakMaxBonus,

    ShockSpeedMultiplier,
    ShockFailChance,
    OverShockBaseMultiplier,
    OverShockChainDecay,

    // 末尾に追加（既存アセットのシリアライズ値をずらさないため）
    BurnDamageMultiplier,    // 燃焼ダメージ倍率（基礎値 1）
    FreezeDamageMultiplier   // 凍結・凍結破砕ダメージ倍率（基礎値 1）
}