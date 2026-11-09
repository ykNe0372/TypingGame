using UnityEngine;

[CreateAssetMenu(menuName = "Data/Effect/Attack/Extra Hit")]
public class ExtraAttackEffect : OnAttackEffect {
    [SerializeField] private float _extraAttackRate = 30f;
    [SerializeField] private float _powerMultiplier = 0.5f;
    [SerializeField] private int _extraCount = 1;

    // 攻撃回数を増やす
    protected override void Apply(AttackContext ctx) {
        if (Random.value > _extraAttackRate/100f) return;

        for (int i=0; i<_extraCount; ++i) {
            ctx.AttackInstances.Add(new AttackInstance {
                PowerMultiplier = _powerMultiplier
            });
        }
    }
}