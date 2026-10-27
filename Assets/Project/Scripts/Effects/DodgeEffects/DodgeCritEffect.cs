using UnityEngine;

[CreateAssetMenu(menuName = "Data/Effect/Dodge/PowDefUP")]
public class DodgeCritEffect : OnDodgeEffect {
    [SerializeField] private float _absorptionRate = 15f;

    // 回避成立時に攻撃力と防御力が上昇する
    protected override void Apply(DamageContext ctx) {
        // TODO: 実装
    }
}