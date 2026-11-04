using UnityEngine;

[CreateAssetMenu(menuName = "Data/Effect/Dodge/ResetTemporaryStatusBonus")]
public class ResetTempStatusEffect : OnDodgeEffect {
    // 回避成立時に、回避成功で解除する系のバフを解除する
    protected override void Apply(DamageContext ctx) {
        ctx.Target.ResetTemporaryStatusBonuses(this);
    }
}