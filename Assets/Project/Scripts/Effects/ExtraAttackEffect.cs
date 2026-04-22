using UnityEngine;

[CreateAssetMenu(menuName = "Data/Effect/Attack/Extra Hit")]
public class ExtraAttackEffect : OnAttackEffect {
    [SerializeField] private int _extraCount = 1;

    // 攻撃回数を増やす
    protected override void Apply(AttackContext ctx) {
        ctx.AttackCount += _extraCount;
    }
}