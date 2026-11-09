// 攻撃の種類（効果側で「通常攻撃だけ数える」などの判定に使う）
public enum AttackKind {
    Normal,   // タイピング完了による通常攻撃 / 敵の攻撃
    Bonus,    // ノーミスボーナス攻撃
    Special,  // 必殺技
    Counter,  // カウンター・反撃
    Auto      // オート攻撃
}
