using System;
using UnityEngine;

[Serializable]
public class TemporaryStatusBonus {
    public object Source { get; }
    public StatusType StatusType { get; }
    public float Value { get; private set; }

    public TemporaryStatusBonus(object source, StatusType statusType, float value) {
        Source = source;
        StatusType = statusType;
        Value = value;
    }

    public void Add(float amount) {
        Value += amount;
    }
}