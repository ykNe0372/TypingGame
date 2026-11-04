using UnityEngine;

[CreateAssetMenu(menuName = "Data/Effect/Attack/Self Harm")]
public class SelfHarmAttackEffect : OnAttackEffect {
    [SerializeField] private float _selfHarmRate = 1f;
    [SerializeField] private float _powerMultiplier = 1.5f;
    [SerializeField] private float _critRateMultiplier = 1.5f;

    // 攻撃するごとに自傷ダメージを受けるが、攻撃力と会心率が上昇する
    protected override void Apply(AttackContext ctx) {
        ctx.Attacker.TakePercentDamage(_selfHarmRate);

        foreach (var attack in ctx.AttackInstances) {
            attack.PowerMultiplier *= _powerMultiplier;
            attack.CritRateMultiplier *= _critRateMultiplier;
        }
    }
}