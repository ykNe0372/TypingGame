using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class StatusModifier {
    public StatusType statusType;
    public int value;
}

[CreateAssetMenu(menuName = "Data/Growth Item")]
public class GrowthItem : ScriptableObject {
    [SerializeField] private List<StatusModifier> _modifiers = new();

    public int GetStatusBonus(StatusType type) {
        int total = 0;
        foreach (var mod in _modifiers) {
            if (mod.statusType == type) total += mod.value;
        }
        return total;
    }
}