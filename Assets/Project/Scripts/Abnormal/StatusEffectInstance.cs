using System.Collections.Generic;

public class StatusEffectInstance {
    public StatusEffectData Data;
    public Character Source;
    public float RemainingTime;
    public float InitialDuration;  // 付与時の時間

    private readonly Dictionary<StatusEffectParameter, float> _parameters;

    public StatusEffectInstance(StatusEffectData data, Character source, Dictionary<StatusEffectParameter, float> parameters) {
        Data = data;
        Source = source;
        _parameters = new Dictionary<StatusEffectParameter, float>(parameters);
        InitialDuration = GetPerameter(StatusEffectParameter.Duration);
        RemainingTime = InitialDuration;
    }

    public float GetPerameter(StatusEffectParameter parameter) {
        return _parameters.TryGetValue(parameter, out float value) ? value : 0f;
    }
}