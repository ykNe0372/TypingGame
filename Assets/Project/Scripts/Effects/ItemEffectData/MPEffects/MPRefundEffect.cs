using UnityEngine;

// MP消費時に低確率で使用分の n% を回復する
[CreateAssetMenu(menuName = "Data/Effect/MP/Refund")]
public class MPRefundEffect : Effect, IMPConsumedHook {
    [SerializeField, Tooltip("発動確率")] private float _chance = 15f;
    [SerializeField, Tooltip("消費した MP の何% を回復するか")] private float _refundPercent = 50f;

    public void OnMPConsumed(Character owner, int amount) {
        if (!EffectUtility.Roll(_chance)) return;

        int refund = Mathf.FloorToInt(amount * _refundPercent / 100f);
        if (refund <= 0) return;

        owner.RecoverMP(refund);
        Debug.Log($"[MPRefund] +{refund} MP");
    }
}
