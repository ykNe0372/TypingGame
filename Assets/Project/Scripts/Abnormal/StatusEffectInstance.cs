public class StatusEffectInstance {
    public StatusEffectData Data;
    public Character Source;
    public float RemainingTime;

    public StatusEffectInstance(StatusEffectData data, Character source) {
        Data = data;
        Source = source;
        RemainingTime = data.duration;
    }
}