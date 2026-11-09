using System.Collections.Generic;
using UnityEngine;

// HP が減った量に応じて攻撃力 & 防御力を上げる
[CreateAssetMenu(menuName = "Data/Effect/Status/Missing HP Bonus")]
public class MissingHPStatusEffect : Effect, IStatusModifierHook {
    [SerializeField, Tooltip("上昇させるステータス")] private List<StatusType> _statusTypes = new() { StatusType.PhysicalAttack, StatusType.Defense };
    [SerializeField, Tooltip("HP が 0 に近い時の最大上昇率。減った割合に比例して上昇")] private float _maxBonusPercent = 50f;

    public void ModifyStatus(Character owner, StatusType type, ref float add, ref float multiplier) {
        if (!_statusTypes.Contains(type)) return;

        float lostRatio = 1f - Mathf.Clamp01(owner.GetCurrentHPRatio());
        multiplier *= 1f + _maxBonusPercent / 100f * lostRatio;
    }
}
