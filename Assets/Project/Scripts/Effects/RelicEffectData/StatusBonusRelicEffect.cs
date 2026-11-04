using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Data/Relic/Status Bonus")]
public class StatusBonusRelicEffect : RelicEffect {
    [Serializable]
    public class StatusEntry {
        public StatusType statusType;
        public float value;
    }

    [SerializeField] private List<StatusEntry> _status = new();

    public override float GetStatusBonus(StatusType type) {
        float total = 0f;

        foreach (var s in _status) {
            if (s.statusType == type) total += s.value;
        }
    
        return total;
    }
}