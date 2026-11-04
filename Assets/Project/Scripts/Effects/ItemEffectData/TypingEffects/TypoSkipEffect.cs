using UnityEngine;

// タイプミスをすると即座に攻撃失敗となり次の文章に切り替わる
// 「代わりに全ステータスが上昇」は、同じ強化アイテムに StatusBonusEffect を併せて設定する
[CreateAssetMenu(menuName = "Data/Effect/Typing/Typo Skip")]
public class TypoSkipEffect : Effect, ITypoHook {
    public void OnTypo(Character owner, TypoContext ctx) {
        if (ctx.IsNegated) return;
        ctx.SkipQuestion = true;
    }
}
