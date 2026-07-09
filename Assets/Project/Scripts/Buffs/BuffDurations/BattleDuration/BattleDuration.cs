public class BattleDuration : BuffDuration {
    private int _remainingBattleCount;

    public BattleDuration(int battleCount) {
        _remainingBattleCount = battleCount;
    }

    public override bool IsExpired => _remainingBattleCount <= 0;

    public override void OnBattleEnd() {
        --_remainingBattleCount;
    }
}