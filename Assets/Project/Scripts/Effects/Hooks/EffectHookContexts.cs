// フックに渡す情報

// タイプミス時の情報（各フックが書き換えて TypingManager に結果を返す）
public class TypoContext {
    public bool IsNegated;     // true: タイプミスを無効化（連続正解数をリセットしない）
    public bool SkipQuestion;  // true: 攻撃失敗として次の文章に切り替える
}

// 戦闘終了時の情報
public class BattleEndContext {
    public BattleType BattleType;
    public bool IsVictory;

    public BattleEndContext(BattleType battleType, bool isVictory) {
        BattleType = battleType;
        IsVictory = isVictory;
    }
}

// 回数制限や累積値をいつリセットするか
public enum EffectResetTiming {
    Never,       // リセットしない（ラン中ずっと持続）
    EachBattle   // 戦闘開始ごとにリセット
}
