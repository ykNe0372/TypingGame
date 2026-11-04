using UnityEngine;

[CreateAssetMenu(menuName = "Data/Effect/Dodge/StatusUP")]
public class DodgeStatusUPEffect : OnDodgeEffect {
    [SerializeField] private BuffData _buffData;

    // 回避成立時に一定時間ステータスが上昇する
    protected override void Apply(DamageContext ctx) {
        ctx.Target.AddBuff(_buffData);  // 回避した側にバフだから Target 側にバフをかけるで合ってる
    }
}