using UnityEngine;

// クリア報酬を受け取る際、中確率で再度クリア報酬を受け取れる（回数制限なし）
// ボス報酬では強化アイテム報酬のみ繰り返す（レリックは1枠のため）
[CreateAssetMenu(menuName = "Data/Relic/Reward Repeat")]
public class RewardRepeatRelicEffect : RelicEffect, IRewardRepeatHook {
    [SerializeField, Tooltip("再度受け取れる確率")] private float _chance = 40f;

    public bool ShouldRepeatReward(Character owner) {
        return EffectUtility.Roll(_chance);
    }
}
