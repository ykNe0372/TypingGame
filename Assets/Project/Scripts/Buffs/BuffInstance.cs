public class BuffInstance {
    public BuffData Data { get; private set; }
    public int RemainingBattleCount { get; set; }

    public BuffInstance(BuffData data) {
        Data = data;
        RemainingBattleCount = data.DurationBattleCount;
    }
}