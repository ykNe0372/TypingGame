using UnityEngine;

[CreateAssetMenu(menuName = "Data/Effect/Damage/Absorption")]
public class AbsorptionEffect : OnDamageEffect {
    [SerializeField] private float _absorptionRate = 15f;

    // 攻撃時にHPの _absorptionRate% を吸収し回復する
    protected override void Apply(DamageContext ctx) {
        int healAount = Mathf.FloorToInt(ctx.ActualDamage * _absorptionRate / 100f);
        if (healAount <= 0) return;

        ctx.Attacker.RecoverHP(healAount);
    }
}