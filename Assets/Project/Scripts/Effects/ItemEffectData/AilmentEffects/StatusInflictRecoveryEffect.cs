using UnityEngine;

// 状態異常を付与した際に HP / MP を回復する
[CreateAssetMenu(menuName = "Data/Effect/Ailment/Inflict Recovery")]
public class StatusInflictRecoveryEffect : Effect, IStatusInflictedHook {
    [SerializeField, Tooltip("最大HPの何% 回復するか")] private float _hpPercent = 2f;
    [SerializeField, Tooltip("最大MPの何% 回復するか")] private float _mpPercent = 3f;

    public void OnStatusInflicted(Character owner, Character target, StatusEffectData data) {
        EffectUtility.RecoverPercent(owner, _hpPercent, _mpPercent);
    }
}
