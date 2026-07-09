public class BuffInstance {
    public BuffData Data { get; }
    public BuffDuration Duration { get; }

    public BuffInstance(BuffData data) {
        Data = data;
        Duration = data.DurationData.Create();
    }
}