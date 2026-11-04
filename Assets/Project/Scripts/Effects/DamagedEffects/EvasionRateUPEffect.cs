using UnityEngine;

[CreateAssetMenu(menuName = "Data/Effect/Damaged/EvasionRateUP")]
public class EvasionRateUPEffect : OnDamagedEffect {
    [SerializeField] private float _evasionRateIncrease = 5f;

    protected override void Apply(DamageContext ctx) {
        if (ctx.Target == null) return;
        ctx.Target.AddTemporaryStatusBonus(this, StatusType.EvasionRate, _evasionRateIncrease);
    }
}