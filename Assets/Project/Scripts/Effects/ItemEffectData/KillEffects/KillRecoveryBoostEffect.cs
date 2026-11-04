using UnityEngine;

// 敵討伐時の回復量を増加させる（KillRecoveryEffect の回復量に倍率を掛ける）
[CreateAssetMenu(menuName = "Data/Effect/Kill/Recovery Boost")]
public class KillRecoveryBoostEffect : Effect, IKillRecoveryModifierHook {
    [SerializeField, Tooltip("回復量の倍率")] private float _multiplier = 1.5f;

    public float GetKillRecoveryMultiplier(Character owner) {
        return _multiplier;
    }
}
