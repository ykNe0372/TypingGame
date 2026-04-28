public class StatusEffectInstance {
    public StatusEffectData Data;
    public Character Source;
    public float RemainingTime;
    public float InitialDuration;  // 付与時の時間

    public StatusEffectInstance(StatusEffectData data, Character source) {
        Data = data;
        Source = source;
        RemainingTime = data.duration;
        InitialDuration = data.duration;
    }
}