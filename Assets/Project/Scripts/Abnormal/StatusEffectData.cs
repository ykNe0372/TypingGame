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
}