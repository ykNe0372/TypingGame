using UnityEngine;

public enum StatusEffectType {
    Burn,
    Freeze,
    Shock
}

[CreateAssetMenu(menuName = "Data/StatusEffect")]
public class StatusEffectData : ScriptableObject {
    public StatusEffectType type;
    public float baseChance;  // 基礎付与率
    public float duration;    // 持続時間
    public  float accumulationPerFail = 0.05f;  // 付与失敗時の上昇率
    public float maxBonus = 0.99f;  // 付与確率上昇の上限
    public StatusEffectBehaviour behaviour;

    public float freezeBreakBaseChance = 0.5f;    // 初期解除確率
    public float freezeBreakAccumulation = 0.1f;  // 非解除時の上昇率
    public float freezeBreakMaxBonus = 0.95f;     // 解除確率の上限

    public float shockSpeedMultiplier = 0.8f;     // 攻撃速度を (1f - shockSpeedMultiplier) % 低下
    public float shockFailChance = 0.1f;          // 攻撃不発確率
    public float overShockBaseMultiplier = 1.2f;  // 迅雷直撃時の攻撃倍率
    public float overShockChainDecay = 0.5f;      // 迅雷伝播時の攻撃倍率
}